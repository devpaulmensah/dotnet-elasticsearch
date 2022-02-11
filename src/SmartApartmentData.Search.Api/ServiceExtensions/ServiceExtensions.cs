using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Elasticsearch.Net;
using Microsoft.OpenApi.Models;
using Nest;
using SmartDataApartment.Search.Api.Actors;
using SmartDataApartment.Search.Api.Actors.ElasticSearchActors;
using SmartDataApartment.Search.Api.Configurations;
using SmartDataApartment.Search.Api.Helpers;
using SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;
using SmartDataApartment.Search.Api.Services.Interfaces;
using SmartDataApartment.Search.Api.Services.Providers;

namespace SmartDataApartment.Search.Api.ServiceExtensions;

public static class ServiceExtensions
{
    public static void InitializeSwagger(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var version = configuration["SwaggerConfig:Version"];
        var title = configuration["SwaggerConfig:Title"];

        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(version, new OpenApiInfo
            {
                Contact = new OpenApiContact
                {
                    Email = "paulmensah1409@gmail.com",
                    Name = "Paul Mensah",
                    Url = new Uri("https://paulmensah.dev")
                },
                Version = version,
                Title = title
            });
            c.ResolveConflictingActions(resolver => resolver.First());
        });
    }

    public static void UseSwaggerDocumentation(this IApplicationBuilder applicationBuilder, IConfiguration configuration)
    {
        var title = configuration["SwaggerConfig:Title"];
        
        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", title);
        });
    }

    public static void InitializeActors(this IServiceCollection serviceCollection)
    {
        var actorSystem = ActorSystem.Create("DataUploadActors");
        serviceCollection.AddSingleton(typeof(ActorSystem), s => actorSystem);

        var builder = new ContainerBuilder();
        builder.Populate(serviceCollection);

        builder.RegisterType<MainActor>();
        builder.RegisterType<PersistManagementCompaniesToEsActor>();
        builder.RegisterType<PersistPropertiesToEsActor>();

        var buildContainer = builder.Build();
        var resolver = new AutoFacDependencyResolver(buildContainer, actorSystem);

        TopLevelActors.ActorSystem = actorSystem;
        TopLevelActors.MainActor = actorSystem.ActorOf(actorSystem.DI()
                .Props<MainActor>()
                .WithSupervisorStrategy(TopLevelActors.GetDefaultSupervisorStrategy)
            , nameof(MainActor));

        TopLevelActors.PersistManagementCompaniesToEsActor = TopLevelActors.ActorSystem.ActorOf(TopLevelActors.ActorSystem.DI()
                .Props<PersistManagementCompaniesToEsActor>()
                .WithSupervisorStrategy(TopLevelActors.GetDefaultSupervisorStrategy),
            nameof(PersistManagementCompaniesToEsActor));
        
        TopLevelActors.PersistPropertiesToEsActor = TopLevelActors.ActorSystem.ActorOf(TopLevelActors.ActorSystem.DI()
                .Props<PersistPropertiesToEsActor>()
                .WithSupervisorStrategy(TopLevelActors.GetDefaultSupervisorStrategy),
            nameof(PersistPropertiesToEsActor));
    }

    public static void AddCustomServicesAndConfigurations(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddScoped<IPropertyService, PropertyService>();
        serviceCollection.AddScoped<IManagementCompanyService, ManagementCompanyService>();

        serviceCollection.Configure<BulkUploadOptions>(configuration.GetSection(nameof(BulkUploadOptions)));
        serviceCollection.Configure<ElasticSearchConfig>(configuration.GetSection(nameof(ElasticSearchConfig)));
    }
    
    public static async void InitializeElasticSearch(this IServiceCollection services, ElasticSearchConfig elasticSearchConfig)
    {
        services.Configure<ElasticSearchConfig>(config =>
        {
            config.BaseUrl = elasticSearchConfig.BaseUrl;
        });

        var pool = new SingleNodeConnectionPool(new Uri(elasticSearchConfig.BaseUrl));
        var connectionSettings = new ConnectionSettings(pool)
            .DefaultIndex(CommonConstants.PropertiesIndex);
        connectionSettings.PrettyJson();
        connectionSettings.DisableDirectStreaming();
        
        var elasticClient = new ElasticClient(connectionSettings);
        var elasticLowLevelClient = new ElasticLowLevelClient(connectionSettings);

        services.AddSingleton<IElasticClient>(elasticClient);
        services.AddSingleton<IElasticLowLevelClient>(elasticLowLevelClient);
        services.AddSingleton<IElasticSearchService, ElasticSearchService>();
        
        await CreateIndexAsync<PropertyElasticSearchDto>(elasticClient, CommonConstants.PropertiesIndex); 
        await CreateIndexAsync<ManagementCompanyElasticSearchDto>(elasticClient, CommonConstants.ManagementsCompaniesIndex);
    }
    
    private static async Task CreateIndexAsync<T>(IElasticClient elasticClient, string indexName) where T: class
    {
        var indexExistsResponse = await elasticClient.Indices.ExistsAsync(indexName);

        if (!indexExistsResponse.Exists)
        {
            var createIndexResponse =
                await elasticClient.Indices.CreateAsync(indexName, descriptor => descriptor
                    .Settings(s => s
                        .Analysis(a => a
                            .Analyzers(analyzer => analyzer
                                .Custom(CommonConstants.CustomStandardAnalyzer, customAnalyzer => customAnalyzer
                                    .Filters(CommonConstants.EnglishStopWords, "lowercase", "trim")
                                    .Tokenizer("standard"))
                                .Custom(CommonConstants.KeywordAnalyzer, keywordAnalyzer => keywordAnalyzer
                                    .Filters("lowercase", "trim")
                                    .Tokenizer("keyword"))
                                .Custom(CommonConstants.AutoCompleteAnalyzer, autoCompleteAnalyzer => autoCompleteAnalyzer
                                    .Filters(CommonConstants.EnglishStopWords, "lowercase", "trim")
                                    .Tokenizer(CommonConstants.AutoCompleteTokenizer)))
                        
                            .Tokenizers(tokenizer => tokenizer
                                .NGram(CommonConstants.AutoCompleteTokenizer, nGramDescriptor => nGramDescriptor
                                    .MinGram(3)
                                    .MaxGram(4)
                                    .TokenChars(new TokenChar[] {TokenChar.Digit, TokenChar.Letter})))
                        
                            .TokenFilters(filter => filter
                                .Stop(CommonConstants.EnglishStopWords, stopToken => stopToken
                                    .StopWords("_English_")))))
                    .Map<T>(m => 
                        m.AutoMap<T>()));

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Error occured creating index:{indexName},\nDebugInformation => {createIndexResponse.DebugInformation}");
            }
        }
    }
}