using Mistaken.API.EventArgs;

namespace Mistaken.API.Events;

/// <summary>
/// Mistaken Ragdoll events.
/// </summary>
public static class Ragdoll
{
    public delegate void OnSpawned(RagdollSpawnedEventArgs ev);

    public delegate void OnSpawning(RagdollSpawningEventArgs ev);

    /// <summary>
    /// Invoked when Ragdoll was spawned.
    /// </summary>
    public static event OnSpawned Spawned;

    /// <summary>
    /// Invoked before spawning Ragdoll.
    /// </summary>
    public static event OnSpawning Spawning;

    internal static void RagdollSpawn(RagdollSpawnedEventArgs ev)
        => Spawned?.Invoke(ev);

    internal static void RagdollSpawning(RagdollSpawningEventArgs ev)
        => Spawning?.Invoke(ev);
}
