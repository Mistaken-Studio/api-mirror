// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using InventorySystem.Items.ThrowableProjectiles;
using PlayerStatsSystem;
using UnityEngine;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Spawns BoxCollider.
        /// </summary>
        /// <param name="pos">Position.</param>
        /// <param name="scale">Scale.</param>
        /// <returns>GameObject with BoxCollider.</returns>
        [System.Obsolete("Will be removed", true)]
        public static GameObject SpawnBoxCollider(Vector3 pos, Vector3 scale)
        {
            var obj = new GameObject();
            obj.AddComponent<BoxCollider>();
            obj.transform.position = pos;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = scale;
            return obj;
        }

        #region Backwards Compatibility
        private static void Shuffle<T>(this IList<T> list) => CollectionExtensions.Shuffle(list);

        private static void Shuffle<T>(this IList<T> list, int seed) => CollectionExtensions.Shuffle<T>(list, seed);

        private static T[] Shuffle<T>(this T[] list) => CollectionExtensions.Shuffle(list);

        private static T[] Shuffle<T>(this T[] list, int seed) => CollectionExtensions.Shuffle(list, seed);

        [System.Obsolete("Use overload with StandardDamageHandler!", true)]
        private static bool WillDie(this Player player, DamageHandlerBase handler) => DamageExtensions.WillDie(player, handler);

        private static bool WillDie(this Player player, StandardDamageHandler handler) => DamageExtensions.WillDie(player, handler);

        private static float GetRealDamageAmount(this Player player, StandardDamageHandler handler, out float dealtHealthDamage, out float absorbedAhpDamage) => DamageExtensions.GetRealDamageAmount(player, handler, out dealtHealthDamage, out absorbedAhpDamage);

        private static float GetRealDamageAmount(this Player player, StandardDamageHandler handler) => DamageExtensions.GetRealDamageAmount(player, handler);

        [System.Obsolete("DONT", true)]
        private static bool IsDNT(this Player me) => DntExtensions.IsDNT(me);

        [System.Obsolete("Use Exiled.Permissions.Extensions.Permissions.CheckPermission", true)]
        private static bool CheckPermission(this CommandSender cs, string permission) => PermissionsExtensions.CheckPermission(cs, permission);

        [System.Obsolete("Use Exiled.Permissions.Extensions.Permissions.CheckPermission", true)]
        private static bool CheckPermission(this Player player, string permission) => PermissionsExtensions.CheckPermission(player, permission);

        private static T GetSessionVariable<T>(this Player me, SessionVarType type, T defaultValue = default) => SessionVariableExtensions.GetSessionVariable<T>(me, type, defaultValue);

        private static T GetSessionVariable<T>(this Player me, string name, T defaultValue = default) => SessionVariableExtensions.GetSessionVariable(me, name, defaultValue);

        private static bool TryGetSessionVariable<T>(this Player me, SessionVarType type, out T value) => SessionVariableExtensions.TryGetSessionVariable<T>(me, type, out value);

        private static void SetSessionVariable(this Player me, SessionVarType type, object value) => SessionVariableExtensions.SetSessionVariable(me, type, value);

        private static void SetSessionVariable(this Player me, string name, object value) => SessionVariableExtensions.SetSessionVariable(me, name, value);

        private static void RemoveSessionVariable(this Player me, SessionVarType type) => SessionVariableExtensions.RemoveSessionVariable(me, type);

        private static void RemoveSessionVariable(this Player me, string name) => SessionVariableExtensions.RemoveSessionVariable(me, name);

        private static void Broadcast(this Player me, string tag, ushort duration, string message, Broadcast.BroadcastFlags flags = global::Broadcast.BroadcastFlags.Normal) => PlayerExtensions.Broadcast(me, tag, duration, message, flags);

        private static Player GetSpectatedPlayer(this Player player) => PlayerExtensions.GetSpectatedPlayer(player);

        private static bool CheckPermissions(this Player me, PlayerPermissions perms) => PlayerExtensions.CheckPermissions(me, perms);

        private static bool IsDev(this Player me) => PlayerExtensions.IsDev(me);

        private static bool IsDevUserId(this string me) => PlayerExtensions.IsDevUserId(me);

        private static Player GetPlayer(this CommandSender me) => PlayerExtensions.GetPlayer(me);

        private static Player GetPlayer(this ICommandSender me) => PlayerExtensions.GetPlayer(me);

        private static bool IsPlayer(this CommandSender me) => PlayerExtensions.IsPlayer(me);

        private static bool IsPlayer(this ICommandSender me) => PlayerExtensions.IsPlayer(me);

        private static string GetDisplayName(this Player player) => PlayerExtensions.GetDisplayName(player);

        private static string ToString(this Player me, bool userId) => PlayerExtensions.ToString(me, userId);

        private static bool IsReadyPlayer(this Player me) => PlayerExtensions.IsReadyPlayer(me);

        private static bool IsConnected(this Player player) => PlayerExtensions.IsConnected(player);

        private static ThrownProjectile Throw(this ThrowableItem nade, Vector3 position, Vector3 direction, float force, float upWardFactor = 1f) => ProjectileExtensions.Throw(nade, position, direction, force, upWardFactor);

        private static ThrownProjectile Throw(this ThrowableItem nade, Vector3 position, Vector3 direction) => ProjectileExtensions.Throw(nade, position, direction);

        private static Vector3 GetByRoomOffset(this Room me, Vector3 offset) => RoomExtensions.GetByRoomOffset(me, offset);
        #endregion
    }
}
