// -----------------------------------------------------------------------
// <copyright file="SyncVar.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Mirror;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// SyncVar Extensions.
    /// </summary>
    public static class SyncVar
    {
        /// <summary>
        /// Zmienia rangę <paramref name="player"/> dla <paramref name="target"/>.
        /// </summary>
        /// <param name="player">Osob.</param>
        /// <param name="target">Osoba która ma widzieć inną rangę.</param>
        /// <param name="name">Nazwa.</param>
        /// <param name="color">Kolor.</param>
        public static void TargetSetBadge(this Player player, Player target, string name, string color)
        {
            MirrorExtensions.SendFakeSyncVar(target, player.ReferenceHub.networkIdentity, typeof(ServerRoles), nameof(ServerRoles.NetworkMyText), name);
            MirrorExtensions.SendFakeSyncVar(target, player.ReferenceHub.networkIdentity, typeof(ServerRoles), nameof(ServerRoles.NetworkMyColor), color);
        }

        /// <summary>
        /// Changes Nickname.
        /// </summary>
        /// <param name="player">Player to change nickname for.</param>
        /// <param name="target">Player that will see change.</param>
        /// <param name="nickname">Nickname.</param>
        public static void TargetSetNickname(this Player player, Player target, string nickname)
        {
            MirrorExtensions.SendFakeSyncVar(target, player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), nickname);
        }

        /// <summary>
        /// Changes Appeareance.
        /// </summary>
        /// <param name="player">Player to change role for.</param>
        /// <param name="target">Player that will see change.</param>
        /// <param name="type">Role.</param>
        public static void ChangeAppearance(this Player player, Player target, RoleType type)
        {
            MirrorExtensions.SendFakeSyncVar(target, player.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte)type);
        }
    }
}
