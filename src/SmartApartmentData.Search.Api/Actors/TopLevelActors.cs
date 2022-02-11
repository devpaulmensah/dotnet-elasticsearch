using Akka.Actor;

namespace SmartDataApartment.Search.Api.Actors;

public static class TopLevelActors
{
    public static IActorRef MainActor = ActorRefs.Nobody;
    public static IActorRef PersistPropertiesToEsActor = ActorRefs.Nobody;
    public static IActorRef PersistManagementCompaniesToEsActor = ActorRefs.Nobody;
    public static ActorSystem ActorSystem;

    public static SupervisorStrategy GetDefaultSupervisorStrategy => new OneForOneStrategy(3,
        TimeSpan.FromSeconds(3), ex =>
        {
            if (ex is not ActorInitializationException) return Directive.Resume;
            Stop();
            return Directive.Stop;
        });

    private static void Stop()
    {
        ActorSystem.Terminate().Wait(1000);
    }
}