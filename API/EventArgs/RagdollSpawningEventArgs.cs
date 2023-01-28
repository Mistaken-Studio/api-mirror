using PlayerStatsSystem;
using PluginAPI.Core;

namespace Mistaken.API.EventArgs;

/// <summary>
/// Event Args for spawning ragdoll.
/// </summary>
public sealed class RagdollSpawningEventArgs : System.EventArgs
{
    public RagdollSpawningEventArgs(ReferenceHub owner, DamageHandlerBase handler, bool isAllowed = true)
    {
        Owner = Player.Get(owner);
        DamageHandler = handler;
        IsAllowed = isAllowed;
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
    /// Is spawning of ragdoll allowed.
    /// </summary>
    public bool IsAllowed { get; set; }
}
