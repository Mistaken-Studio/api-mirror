// -----------------------------------------------------------------------
// <copyright file="PlayerExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using NorthwoodLib.Pools;
using PlayerRoles.Spectating;
using PluginAPI.Core;
using RemoteAdmin;
using Respawning;
using System.Text;
using UnityEngine;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Player Extensions.
    /// </summary>
    public static class PlayerExtensions
    {
        /*/// <inheritdoc cref="MapPlus.Broadcast(string, ushort, string, global::Broadcast.BroadcastFlags)"/>
        public static void Broadcast(this Player player, string tag, ushort duration, string message, Broadcast.BroadcastFlags flags = global::Broadcast.BroadcastFlags.Normal)
        {
            player.Broadcast(duration, $"<color=orange>[<color=green>{tag}</color>]</color> {message}", flags);
        }*/

        /// <summary>
        /// Get's spectated player.
        /// </summary>
        /// <param name="player">Spectator.</param>
        /// <returns>Spectated player or null if not spectating anyone.</returns>
        public static Player GetSpectatedPlayer(this Player player)
            => player.ReferenceHub.roleManager.CurrentRole is SpectatorRole spectator ? Player.Get(spectator.SyncedSpectatedNetId) : null;

        /// <summary>
        /// Checks if player has base game permission.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="perms">Permission.</param>
        /// <returns>If has permission.</returns>
        public static bool CheckPermissions(this Player player, PlayerPermissions perms)
        {
            return PermissionsHandler.IsPermitted(player.ReferenceHub.serverRoles.Permissions, perms);
        }

        /// <summary>
        /// If player is Dev.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <returns>Is Dev.</returns>
        public static bool IsDev(this Player player)
        {
            return player?.UserId.IsDevUserId() ?? false;
        }

        /// <summary>
        /// Returns if UserId is Dev's userId.
        /// </summary>
        /// <param name="player">UserId.</param>
        /// <returns>If belongs to dev.</returns>
        public static bool IsDevUserId(this string player)
        {
            if (player == null)
                return false;
            return player.Split('@')[0] switch
            {
                // WW
                "76561198134629649" => true,
                "356174382655209483" => true,

                // Barwa
                "76561198035545880" => true,
                "373551302292013069" => true,
                "barwa" => true,

                // Xname
                "76561198123437513" => true,
                "373911388575236096" => true,
                _ => false
            };
        }

        /// <summary>
        /// Returns player.
        /// </summary>
        /// <param name="sender">Potently player.</param>
        /// <returns>Player.</returns>
        public static Player GetPlayer(this CommandSender sender) => sender is PlayerCommandSender player ? Player.Get(player.ReferenceHub) : null;

        /// <summary>
        /// Returns player.
        /// </summary>
        /// <param name="sender">Potently player.</param>
        /// <returns>Player.</returns>
        public static Player GetPlayer(this ICommandSender sender) => sender is PlayerCommandSender player ? Player.Get(player.ReferenceHub) : null;

        /// <summary>
        /// Returns true if <paramref name="sender"/> is Player.
        /// </summary>
        /// <param name="sender">To Check.</param>
        /// <returns>Result.</returns>
        public static bool IsPlayer(this CommandSender sender) => sender is PlayerCommandSender;

        /// <summary>
        /// Returns true if <paramref name="sender"/> is Player.
        /// </summary>
        /// <param name="sender">To Check.</param>
        /// <returns>Result.</returns>
        public static bool IsPlayer(this ICommandSender sender) => sender is PlayerCommandSender;

        /// <summary>
        /// Returns <see cref="Player.DisplayNickname"/> or <see cref="Player.Nickname"/> if first is null or "NULL" if player is null.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <returns>Name.</returns>
        public static string GetDisplayName(this Player player) => player == null ? "NULL" : player.DisplayNickname ?? player.Nickname;

        /// <summary>
        /// Converts player to string.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="userId">If userId should be shown.</param>
        /// <returns>String version of player.</returns>
        public static string ToString(this Player player, bool userId)
        {
            return userId ?
                $"({player.PlayerId}) {player.GetDisplayName()} | {player.UserId}"
                :
                $"({player.PlayerId}) {player.GetDisplayName()}";
        }

        /// <summary>
        /// Returns if player is real, ready player.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns>If player is ready, real player.</returns>
        public static bool IsReadyPlayer(this Player player)
            => player.IsConnected() && player.IsVerified && player.UserId != null && player.ReferenceHub.Ready;

        /// <summary>
        /// Checks if player is really connected to the server.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <returns>True if player is connected. Otherwise false.</returns>
        public static bool IsConnected(this Player player)
            => player?.GameObject != null && player.Connection is not null;

        /*/// <summary>
        /// Gets player's current room.
        /// </summary>
        /// <param name="player">Player to get room from.</param>
        /// <returns>Player's current room.</returns>
        public static Room GetCurrentRoom(this Player player)
        {
            return player.Role switch
            {
                Scp079Role role079 => Map.FindParentRoom(role079.Camera.GameObject),
                SpectatorRole roleSpec => roleSpec.SpectatedPlayer?.GetCurrentRoom(),
                _ => Physics.RaycastNonAlloc(
                    new Ray(player.GameObject.transform.position, Vector3.down),
                    CachedFindParentRoomRaycast,
                    25f,
                    1,
                    QueryTriggerInteraction.Ignore) == 1
                    ? CachedFindParentRoomRaycast[0].collider.gameObject.GetComponentInParent<Room>()
                    : null
            };
        }

        private static readonly RaycastHit[] CachedFindParentRoomRaycast = new RaycastHit[1];*/
    }
}
