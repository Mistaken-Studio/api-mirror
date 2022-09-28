// -----------------------------------------------------------------------
// <copyright file="PlayerExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using UnityEngine;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Player Extensions.
    /// </summary>
    public static class PlayerExtensions
    {
        /// <inheritdoc cref="MapPlus.Broadcast(string, ushort, string, global::Broadcast.BroadcastFlags)"/>
        public static void Broadcast(this Player me, string tag, ushort duration, string message, Broadcast.BroadcastFlags flags = global::Broadcast.BroadcastFlags.Normal)
        {
            me.Broadcast(duration, $"<color=orange>[<color=green>{tag}</color>]</color> {message}", flags);
        }

        /// <summary>
        /// Get's spectated player.
        /// </summary>
        /// <param name="player">Spectator.</param>
        /// <returns>Spectated player or null if not spectating anyone.</returns>
        public static Player GetSpectatedPlayer(this Player player)
            => player.Role is SpectatorRole role ? role.SpectatedPlayer : null;

        /// <summary>
        /// Checks if player has base game permission.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="perms">Permission.</param>
        /// <returns>If has permission.</returns>
        public static bool CheckPermissions(this Player me, PlayerPermissions perms)
        {
            return PermissionsHandler.IsPermitted(me.ReferenceHub.serverRoles.Permissions, perms);
        }

        /// <summary>
        /// If player is Dev.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <returns>Is Dev.</returns>
        public static bool IsDev(this Player me)
        {
            return me?.UserId.IsDevUserId() ?? false;
        }

        /// <summary>
        /// Returns if UserId is Dev's userId.
        /// </summary>
        /// <param name="me">UserId.</param>
        /// <returns>If belongs to dev.</returns>
        public static bool IsDevUserId(this string me)
        {
            if (me == null)
                return false;
            switch (me.Split('@')[0])
            {
                // WW
                case "76561198134629649":
                case "356174382655209483":
                // Barwa
                case "76561198035545880":
                case "373551302292013069":
                case "barwa":
                // Xname
                case "76561198123437513":
                case "373911388575236096":
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns player.
        /// </summary>
        /// <param name="me">Potently player.</param>
        /// <returns>Player.</returns>
        public static Player GetPlayer(this CommandSender me) => Player.Get(me.SenderId);

        /// <summary>
        /// Returns player.
        /// </summary>
        /// <param name="me">Potently player.</param>
        /// <returns>Player.</returns>
        public static Player GetPlayer(this ICommandSender me) => Player.Get(((CommandSender)me).SenderId);

        /// <summary>
        /// Returns if <paramref name="me"/> is Player or Server.
        /// </summary>
        /// <param name="me">To Check.</param>
        /// <returns>Result.</returns>
        public static bool IsPlayer(this CommandSender me) => GetPlayer(me) != null;

        /// <summary>
        /// Returns if <paramref name="me"/> is Player or Server.
        /// </summary>
        /// <param name="me">To Check.</param>
        /// <returns>Result.</returns>
        public static bool IsPlayer(this ICommandSender me) => GetPlayer(me) != null;

        /// <summary>
        /// Returns <see cref="Player.DisplayNickname"/> or <see cref="Player.Nickname"/> if first is null or "NULL" if player is null.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <returns>Name.</returns>
        public static string GetDisplayName(this Player player) => player == null ? "NULL" : player.DisplayNickname ?? player.Nickname;

        /// <summary>
        /// Converts player to string.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="userId">If userId should be shown.</param>
        /// <returns>String version of player.</returns>
        public static string ToString(this Player me, bool userId)
        {
            if (!userId)
                return $"({me.Id}) {me.GetDisplayName()}";
            return $"({me.Id}) {me.GetDisplayName()} | {me.UserId}";
        }

        /// <summary>
        /// Returns if player is real, ready player.
        /// </summary>
        /// <param name="me">Player to check.</param>
        /// <returns>If player is ready, real player.</returns>
        public static bool IsReadyPlayer(this Player me)
            => me.IsConnected() && me.IsVerified && me.UserId != null && me.ReferenceHub.Ready;

        /// <summary>
        /// Checks if player is really connected to the server.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <returns>True if player is connected. Otherwise false.</returns>
        public static bool IsConnected(this Player player)
            => (player?.IsConnected ?? false) && !(player.Connection is null);

        /// <summary>
        /// Gets player's current room.
        /// </summary>
        /// <param name="player">Player to get room from.</param>
        /// <returns>Player's current room.</returns>
        public static Room GetCurrentRoom(this Player player)
        {
            switch (player.Role)
            {
                case Scp079Role role079:
                    return Map.FindParentRoom(role079.Camera.GameObject);
                case SpectatorRole roleSpec:
                    return roleSpec.SpectatedPlayer?.GetCurrentRoom();
            }

            return Physics.RaycastNonAlloc(
                       new Ray(
                           player.GameObject.transform.position,
                           Vector3.down),
                       CachedFindParentRoomRaycast,
                       25f,
                       1,
                       QueryTriggerInteraction.Ignore) == 1
                ? CachedFindParentRoomRaycast[0]
                    .collider.gameObject.GetComponentInParent<Room>()
                : null;
        }

        private static readonly RaycastHit[] CachedFindParentRoomRaycast = new RaycastHit[1];
    }
}
