// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Mirror;
using UnityEngine;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Main Utils.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Gets or sets list of staff that allowed to ignore DNT.
        /// </summary>
        public static string[] IgnoredUIDs { get; set; } = new string[] { };

        /// <summary>
        /// Returns room offseted position.
        /// </summary>
        /// <param name="me">Room.</param>
        /// <param name="offset">Offset.</param>
        /// <returns>Position.</returns>
        public static Vector3 GetByRoomOffset(this Room me, Vector3 offset)
        {
            var basePos = me.Position;
            offset = (me.transform.forward * -offset.x) + (me.transform.right * -offset.z) + (Vector3.up * offset.y);
            basePos += offset;
            return basePos;
        }

        /// <inheritdoc cref="MapPlus.Broadcast(string, ushort, string, global::Broadcast.BroadcastFlags)"/>
        public static void Broadcast(this Player me, string tag, ushort duration, string message, Broadcast.BroadcastFlags flags = global::Broadcast.BroadcastFlags.Normal)
        {
            me.Broadcast(duration, $"<color=orange>[<color=green>{tag}</color>]</color> {message}", flags);
        }

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
            if (me == null)
                return false;
            return me.UserId.IsDevUserId();
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
                // Hyper
                case "76561198215262787":
                case "365499281215586326":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns player.
        /// </summary>
        /// <param name="me">Potentialy player.</param>
        /// <returns>Player.</returns>
        public static Player GetPlayer(this CommandSender me) => Player.Get(me.SenderId);

        /// <summary>
        /// Returns player.
        /// </summary>
        /// <param name="me">Potentialy player.</param>
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
        /// If player has DNT and if it should be effective.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <returns>if has DNT.</returns>
        public static bool IsDNT(this Player me)
        {
            if (IgnoredUIDs.Contains(me.UserId))
                return false;
            return me.DoNotTrack;
        }

        /// <summary>
        /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="me">Player.</param>
        /// <param name="type">Session Var.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Value.</returns>
        public static T GetSessionVar<T>(this Player me, SessionVarType type, T defaultValue = default) => me.GetSessionVar<T>(type.ToString(), defaultValue);

        /// <summary>
        /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="me">Player.</param>
        /// <param name="name">Session Var.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Value.</returns>
        public static T GetSessionVar<T>(this Player me, string name, T defaultValue = default)
        {
            if (me.TryGetSessionVariable(name, out T value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="type">Session Var.</param>
        /// <param name="value">Value.</param>
        public static void SetSessionVar(this Player me, SessionVarType type, object value) => me.SetSessionVar(type.ToString(), value);

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="name">Session Var.</param>
        /// <param name="value">Value.</param>
        public static void SetSessionVar(this Player me, string name, object value) => me.SessionVariables[name] = value;

        /// <summary>
        /// Returns if player has permission.
        /// </summary>
        /// <param name="cs">Player.</param>
        /// <param name="permission">Permission.</param>
        /// <returns>If has permisison.</returns>
        public static bool CheckPermission(this CommandSender cs, string permission) => CheckPermission(cs.GetPlayer(), permission);

        /// <summary>
        /// Returns if player has permission.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="permission">Permission.</param>
        /// <returns>If has permisison.</returns>
        public static bool CheckPermission(this Player player, string permission)
        {
            if (player.IsDev())
                return true;
            else
                Exiled.Permissions.Extensions.Permissions.CheckPermission(player, permission);
            return false;
        }

        /// <summary>
        /// Returns <see cref="Player.DisplayNickname"/> or <see cref="Player.Nickname"/> if first is null or "NULL" if player is null.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <returns>Name.</returns>
        public static string GetDisplayName(this Player player) => player == null ? "NULL" : player.DisplayNickname ?? player.Nickname;

        /// <summary>
        /// Drops greneade under player.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="grenadeType">Grenade type.</param>
        /// <param name="amount">Amount.</param>
        public static void DropGrenadeUnder(this Player me, int grenadeType, int amount = 1)
        {
            var grenadeManager = me.GameObject.GetComponent<Grenades.GrenadeManager>();
            Grenades.GrenadeSettings settings = grenadeManager.availableGrenades[grenadeType];
            for (int i = 0; i < amount; i++)
            {
                Grenades.Grenade component = UnityEngine.Object.Instantiate(settings.grenadeInstance).GetComponent<Grenades.Grenade>();
                component.InitData(grenadeManager, Vector3.zero, Vector3.down);
                NetworkServer.Spawn(component.gameObject);
            }
        }

        /// <summary>
        /// Kills player with message.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="reason">Kill reason.</param>
        public static void Kill(this Player me, string reason)
        {
            me.ReferenceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(float.MaxValue, $"*{reason}", DamageTypes.None, -1), me.GameObject);
        }

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
        /// <param name="me">Playet to check.</param>
        /// <returns>If player is ready, real player.</returns>
        public static bool IsReadyPlayer(this Player me) => me.IsConnected && me.IsVerified && !me.GetSessionVar<bool>("IsNPC") && me.UserId != null;

        /// <summary>
        /// If player is sprinting.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns><see langword="true"/> if player is sprinting else <see langword="false"/>.</returns>
        public static bool IsPressingShift(this Player player) => player.ReferenceHub.animationController.curAnim == 1;
    }
}
