// -----------------------------------------------------------------------
// <copyright file="MapPlus.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Mirror;
using NorthwoodLib.Pools;
using RemoteAdmin;
using UnityEngine;

namespace Mistaken.API
{
    /// <summary>
    /// Map Extensions but not as extensionb because <see cref="Map"/> is static.
    /// </summary>
    public static class MapPlus
    {
        /// <summary>
        /// Gets or sets a value indicating whether if SCP 106 someone lured.
        /// </summary>
        public static bool Lured
        {
            get => LureSubjectContainer.NetworkallowContain;
            set => LureSubjectContainer.SetState(value, value);
        }

        /// <summary>
        /// Gets instance of <see cref="global::LureSubjectContainer"/>.
        /// </summary>
        public static LureSubjectContainer LureSubjectContainer
        {
            get
            {
                if (container == null)
                    container = UnityEngine.Object.FindObjectOfType<LureSubjectContainer>();

                return container;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if SCP 106 Containment was used.
        /// </summary>
        public static bool FemurBreaked
        {
            get => OneOhSixContainer.used;
            set => OneOhSixContainer.used = value;
        }

        /// <summary>
        /// Gets time to decontamination end.
        /// </summary>
        public static float DecontaminationEndTime
        {
            get => LightContainmentZoneDecontamination.DecontaminationController.Singleton.DecontaminationPhases
                .First(i => i.Function == LightContainmentZoneDecontamination.DecontaminationController.DecontaminationPhase.PhaseFunction.Final)
                .TimeTrigger;
        }

        /// <summary>
        /// Send Broadcast.
        /// </summary>
        /// <param name="tag">Tag.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="message">Message.</param>
        /// <param name="flags">Flags.</param>
        public static void Broadcast(string tag, ushort duration, string message, Broadcast.BroadcastFlags flags = global::Broadcast.BroadcastFlags.Normal)
        {
            if (flags == global::Broadcast.BroadcastFlags.AdminChat)
            {
                string fullMessage = $"<color=orange>[<color=green>{tag}</color>]</color> {message}";
                foreach (var item in RealPlayers.List?.Where(p => p.Connection != null && PermissionsHandler.IsPermitted(p.ReferenceHub.serverRoles.Permissions, PlayerPermissions.AdminChat)) ?? new List<Player>())
                    item.ReferenceHub.queryProcessor.TargetReply(item.Connection, "@" + fullMessage, true, false, string.Empty);
            }
            else
                Map.Broadcast(duration, $"<color=orange>[<color=green>{tag}</color>]</color> {message}", flags);
        }

        /// <summary>
        /// Spawns items with args.
        /// </summary>
        /// <param name="item">Item info.</param>
        /// <param name="position">Spawn position.</param>
        /// <param name="rotation">Spawn rotation.</param>
        /// <param name="size">Pickup size.</param>
        /// <returns>Spawned object.</returns>
        [System.Obsolete("Use new Exiled.API.Features.Items.Item(item).Spawn(position, rotation)", true)]
        public static ItemPickupBase Spawn(ItemType item, Vector3 position, Quaternion rotation, Vector3 size)
            => new Item(item)
            {
                Scale = size,
            }.Spawn(position, rotation).Base;

        /// <summary>
        /// Spawns dummy.
        /// </summary>
        /// <param name="role">Role.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="size">Size.</param>
        /// <param name="name">Name.</param>
        /// <returns>Dummy.</returns>
        public static GameObject SpawnDummy(RoleType role, Vector3 position, Quaternion rotation, Vector3 size, string name)
        {
            GameObject obj = UnityEngine.Object.Instantiate(NetworkManager.singleton.spawnPrefabs.FirstOrDefault(p => p.gameObject.name == "Player"));
            CharacterClassManager ccm = obj.GetComponent<CharacterClassManager>();
            if (ccm == null)
                Log.Error("[SPAWN DUMMY] CCM is null");
            ccm.CurClass = role;

            // ccm.RefreshPlyModel(role);
            obj.GetComponent<NicknameSync>().Network_myNickSync = name;
            obj.GetComponent<QueryProcessor>().PlayerId = 99999;
            obj.GetComponent<QueryProcessor>().NetworkPlayerId = 99999;
            obj.transform.localScale = size;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            NetworkServer.Spawn(obj);

            return obj;
        }

        /// <summary>
        /// Is LCZ Decontaminated.
        /// </summary>
        /// <param name="minTimeLeft">Offset.</param>
        /// <returns>If LCZ is Decontaminated.</returns>
        public static bool IsLCZDecontaminated(float minTimeLeft = 0)
            => IsLCZDecontaminated(out _, minTimeLeft);

        /// <summary>
        /// If LCZ was decontaminated with out param.
        /// </summary>
        /// <param name="lczTime">Time to decontamination.</param>
        /// <param name="minTimeLeft">Offset.</param>
        /// <returns>If LCZ is decontaminated.</returns>
        public static bool IsLCZDecontaminated(out float lczTime, float minTimeLeft = 0)
        {
            lczTime = DecontaminationEndTime - (float)LightContainmentZoneDecontamination.DecontaminationController.GetServerTime;
            return lczTime < minTimeLeft;
        }

        internal static void PostRoundCleanup()
        {
            container = null;
        }

        private static LureSubjectContainer container;
    }
}
