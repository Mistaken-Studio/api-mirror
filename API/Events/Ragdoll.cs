using Mistaken.API.EventArgs;

namespace Mistaken.API.Events;

/// <summary>
/// Mistaken ragdoll events.
/// </summary>
public static class Ragdoll
{
    /// <summary>
    /// Invoked when Ragdoll was spawned.
    /// </summary>
    public static event CustomEventHandler<RagdollSpawnedEventArgs> Spawned;

    /// <summary>
    /// Invoked before spawning Ragdoll.
    /// </summary>
    public static event CustomEventHandler<RagdollSpawningEventArgs> Spawning;

    internal static void RagdollSpawn(RagdollSpawnedEventArgs ev)
        => Spawned?.Invoke(ev);

    internal static void RagdollSpawning(RagdollSpawningEventArgs ev)
        => Spawning?.Invoke(ev);
}
