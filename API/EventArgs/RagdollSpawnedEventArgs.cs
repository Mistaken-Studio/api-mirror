using PlayerStatsSystem;
using PluginAPI.Core;

namespace Mistaken.API.EventArgs;

/// <summary>
/// Event Args for spawning ragdoll.
/// </summary>
public sealed class RagdollSpawnedEventArgs : System.EventArgs
{
    public RagdollSpawnedEventArgs(ReferenceHub owner, DamageHandlerBase handler, BasicRagdoll ragdoll)
    {
        Owner = Player.Get(owner);
        DamageHandler = handler;
        Ragdoll = ragdoll;
    }

    /// <summary>
    /// Ragdoll's owner.
    /// </summary>
    public Player Owner { get; set; }

    /// <summary>
    /// Owner's damagehandler.
    /// </summary>
    public DamageHandlerBase DamageHandler { get; set; }

    /// <summary>
    /// Spawned ragdoll.
    /// </summary>
    public BasicRagdoll Ragdoll { get; set; }
}
