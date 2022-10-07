// -----------------------------------------------------------------------
// <copyright file="SyncVarExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Extensions;
using Exiled.API.Features;

namespace Mistaken.API.Extensions
{
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
            MirrorExtensions.SendFakeSyncVar(target, player.ReferenceHub.networkIdentity, typeof(ServerRoles), nameof(ServerRoles.Network_myText), name);
            MirrorExtensions.SendFakeSyncVar(target, player.ReferenceHub.networkIdentity, typeof(ServerRoles), nameof(ServerRoles.Network_myColor), color);
        }

        /// <summary>
        /// Changes Nickname.
        /// </summary>
        /// <param name="player">Player to change nickname for.</param>
        /// <param name="target">Player that will see change.</param>
        /// <param name="nickname">Nickname.</param>
        public static void TargetSetNickname(this Player player, Player target, string nickname)
            => MirrorExtensions.SendFakeSyncVar(target, player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), nickname);

        /// <summary>
        /// Changes Appeareance.
        /// </summary>
        /// <param name="player">Player to change role for.</param>
        /// <param name="target">Player that will see change.</param>
        /// <param name="type">Role.</param>
        public static void ChangeAppearance(this Player player, Player target, RoleType type)
            => MirrorExtensions.SendFakeSyncVar(target, player.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
    }
}
