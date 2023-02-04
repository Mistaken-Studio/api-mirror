using PlayerRoles;
using PluginAPI.Core;

namespace Mistaken.API.Extensions;

/// <summary>
/// SyncVar Extensions.
/// </summary>
public static class SyncVarExtensions
{
    /// <summary>
    /// Changes <paramref name="player"/> role for <paramref name="target"/>.
    /// </summary>
    /// <param name="player">Player to change rank for.</param>
    /// <param name="target">Target that will see change.</param>
    /// <param name="name">Name.</param>
    /// <param name="color">Color.</param>
    public static void TargetSetBadge(this Player player, Player target, string name, string color)
    {
        target.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(ServerRoles), nameof(ServerRoles.Network_myText), name);
        target.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(ServerRoles), nameof(ServerRoles.Network_myColor), color);
    }

    /// <summary>
    /// Changes Nickname.
    /// </summary>
    /// <param name="player">Player to change nickname for.</param>
    /// <param name="target">Player that will see change.</param>
    /// <param name="nickname">Nickname.</param>
    public static void TargetSetNickname(this Player player, Player target, string nickname)
        => target.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), nickname);

    /// <summary>
    /// Changes Appeareance.
    /// </summary>
    /// <param name="player">Player to change role for.</param>
    /// <param name="target">Player that will see change.</param>
    /// <param name="type">Role.</param>
    public static void ChangeAppearance(this Player player, Player target, RoleTypeId type)
        => target.Connection.Send(new RoleSyncInfo(player.ReferenceHub, type, target.ReferenceHub));
}
