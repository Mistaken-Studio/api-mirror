﻿// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Footprinting;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PlayerStatsSystem;
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

        /// <inheritdoc cref="MapPlus.Broadcast(string, ushort, string, Broadcast.BroadcastFlags)"/>
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
            => (player.Id != player.ReferenceHub.spectatorManager.CurrentSpectatedPlayer.playerId) ? Player.Get(player.ReferenceHub.spectatorManager.CurrentSpectatedPlayer) : null;

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
        public static T GetSessionVariable<T>(this Player me, SessionVarType type, T defaultValue = default)
            => me.GetSessionVariable(type.ToString(), defaultValue);

        /// <summary>
        /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="me">Player.</param>
        /// <param name="name">Session Var.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Value.</returns>
        public static T GetSessionVariable<T>(this Player me, string name, T defaultValue = default)
        {
            if (me.TryGetSessionVariable(name, out T value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// If SessionVar was found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="me">Player.</param>
        /// <param name="type">Session Var.</param>
        /// <param name="value">Value.</param>
        /// <returns>If session var was found.</returns>
        public static bool TryGetSessionVariable<T>(this Player me, SessionVarType type, out T value)
            => me.TryGetSessionVariable(type.ToString(), out value);

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="type">Session Var.</param>
        /// <param name="value">Value.</param>
        public static void SetSessionVariable(this Player me, SessionVarType type, object value)
            => me.SetSessionVariable(type.ToString(), value);

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="name">Session Var.</param>
        /// <param name="value">Value.</param>
        public static void SetSessionVariable(this Player me, string name, object value)
            => me.SessionVariables[name] = value;

        /// <summary>
        /// Removes SessionVar.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="type">Session Var.</param>
        public static void RemoveSessionVariable(this Player me, SessionVarType type)
            => me.RemoveSessionVariable(type.ToString());

        /// <summary>
        /// Removes SessionVar.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="name">Session Var.</param>
        public static void RemoveSessionVariable(this Player me, string name)
            => me.SessionVariables.Remove(name);

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
            {
                try
                {
                    var perms = new List<string>();
                    string group = player.GroupName;

                    while (!string.IsNullOrWhiteSpace(group))
                    {
                        var groupObj = Exiled.Permissions.Extensions.Permissions.Groups[group];
                        perms.AddRange(groupObj.Permissions);
                        group = groupObj.Inheritance.FirstOrDefault();
                    }

                    if (perms.Contains(".*") || perms.Contains(permission) || perms.Contains(permission.Split('.')[0] + ".*"))
                        return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                return Exiled.Permissions.Extensions.Permissions.CheckPermission(player, permission);
            }
        }

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
        /// <param name="me">Playet to check.</param>
        /// <returns>If player is ready, real player.</returns>
        public static bool IsReadyPlayer(this Player me)
            => me.IsConnected && me.IsVerified && !me.GetSessionVariable<bool>("IsNPC") && me.UserId != null;

        /// <summary>
        /// Spawns BoxCollider.
        /// </summary>
        /// <param name="pos">Position.</param>
        /// <param name="scale">Scale.</param>
        /// <returns>GameObject with BoxCollider.</returns>
        public static GameObject SpawnBoxCollider(Vector3 pos, Vector3 scale)
        {
            var obj = new GameObject();
            obj.AddComponent<BoxCollider>();
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = scale;
            return obj;
        }

        /// <summary>
        /// Throws <paramref name="nade"/> from <paramref name="position"/> with <paramref name="direction"/> and <paramref name="force"/>.
        /// </summary>
        /// <param name="nade">Item to throw.</param>
        /// <param name="position">Position to throw from.</param>
        /// <param name="direction">Direction of throw.</param>
        /// <param name="force">Force of throw.</param>
        /// <param name="upWardFactor">Up force of throw.</param>
        /// <returns>Thrown projectile.</returns>
        public static ThrownProjectile Throw(this ThrowableItem nade, Vector3 position, Vector3 direction, float force, float upWardFactor = 1f)
        {
            nade._destroyTime = Time.timeSinceLevelLoad + nade._postThrownAnimationTime;
            nade._alreadyFired = true;

            ThrownProjectile thrownProjectile = UnityEngine.Object.Instantiate(nade.Projectile, position, Quaternion.Euler(direction));
            PickupSyncInfo pickupSyncInfo = default(PickupSyncInfo);
            pickupSyncInfo.ItemId = nade.ItemTypeId;
            pickupSyncInfo.Locked = !nade._repickupable;
            pickupSyncInfo.Serial = nade.ItemSerial;
            pickupSyncInfo.Weight = nade.Weight;
            pickupSyncInfo.Position = thrownProjectile.transform.position;
            pickupSyncInfo.Rotation = new LowPrecisionQuaternion(thrownProjectile.transform.rotation);
            PickupSyncInfo newInfo = thrownProjectile.NetworkInfo = pickupSyncInfo;
            thrownProjectile.PreviousOwner = new Footprint(Server.Host.ReferenceHub);
            NetworkServer.Spawn(thrownProjectile.gameObject);
            pickupSyncInfo = default(PickupSyncInfo);
            thrownProjectile.InfoReceived(pickupSyncInfo, newInfo);
            if (thrownProjectile.TryGetComponent(out Rigidbody component))
                nade.PropelBody(component, direction, Vector3.zero, force, upWardFactor);

            thrownProjectile.ServerActivate();

            return thrownProjectile;
        }

        /// <summary>
        /// Throws <paramref name="nade"/> from <paramref name="position"/> with <paramref name="direction"/> and default force.
        /// </summary>
        /// <param name="nade">Item to throw.</param>
        /// <param name="position">Position to throw from.</param>
        /// <param name="direction">Direction of throw.</param>
        /// <returns>Thrown projectile.</returns>
        public static ThrownProjectile Throw(this ThrowableItem nade, Vector3 position, Vector3 direction)
            => Throw(nade, position, direction, nade.WeakThrowSettings.StartVelocity, nade.WeakThrowSettings.UpwardsFactor);

        /// <summary>
        /// Returns if player will die because of damage caused by <paramref name="handler"/>.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="handler">Damage Cause.</param>
        /// <returns>If player will die because of this damage.</returns>
        public static bool WillDie(this Player player, DamageHandlerBase handler)
        {
            var tmp = player.ActiveArtificialHealthProcesses.Select(x => new { Process = x, x.CurrentAmount });
            var hp = player.Health;
            var death = handler.ApplyDamage(player.ReferenceHub) == DamageHandlerBase.HandlerOutput.Death;

            player.Health = hp;
            foreach (var item in tmp)
                item.Process.CurrentAmount = item.CurrentAmount;

            return death;
        }
    }
}
