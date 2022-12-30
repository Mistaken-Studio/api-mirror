// -----------------------------------------------------------------------
// <copyright file="PlayerExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using PluginAPI.Core;
using RemoteAdmin;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Player Extensions.
    /// </summary>
    public static class PlayerExtensions
    {
        /// <summary>
        /// Returns player.
        /// </summary>
        /// <param name="sender">Potently player.</param>
        /// <returns>Player.</returns>
        public static MPlayer GetPlayer(this CommandSender sender) => sender is PlayerCommandSender player ? Player.Get<MPlayer>(player.ReferenceHub) : null;

        /// <summary>
        /// Returns player.
        /// </summary>
        /// <param name="sender">Potently player.</param>
        /// <returns>Player.</returns>
        public static MPlayer GetPlayer(this ICommandSender sender) => sender is PlayerCommandSender player ? Player.Get<MPlayer>(player.ReferenceHub) : null;

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
        /// Returns if UserId is Dev's userId.
        /// </summary>
        /// <param name="userId">UserId.</param>
        /// <returns>If belongs to dev.</returns>
        public static bool IsDevUserId(this string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            return userId.Split('@')[0] switch
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

        /*/// <summary>
        /// Returns if player is real, ready player.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns>If player is ready, real player.</returns>
        public static bool IsReadyPlayer(this Player player)
            => player.IsConnected() && player.IsVerified && player.UserId != null && player.ReferenceHub.Ready;*/

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
