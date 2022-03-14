using SmartDataApartment.Search.Api.Configurations;
using SmartDataApartment.Search.Api.Middlewares;
using SmartDataApartment.Search.Api.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomServicesAndConfigurations(builder.Configuration);
builder.Services.InitializeElasticSearch(new ElasticSearchConfig
{
    BaseUrl = builder.Configuration["ElasticSearchConfig:BaseUrl"]
});
builder.Services.InitializeActors();
builder.Services.AddControllers();
builder.Services.InitializeSwagger(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation(app.Configuration);
}

app.ConfigureGlobalHandler(app.Logger);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();