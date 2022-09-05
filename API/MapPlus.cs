// -----------------------------------------------------------------------
// <copyright file="MapPlus.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AdminToys;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapGeneration;
using MapGeneration.Distributors;
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
        /// Gets a value indicating whether SCP079's recontainment is in proggres.
        /// </summary>
        public static bool IsSCP079ReadyForRecontainment => SCP079Recontainer._prevEngaged == 3;

        /// <summary>
        /// Gets a value indicating whether SCP079 recontainment has finished.
        /// </summary>
        public static bool IsSCP079Recontained => SCP079Recontainer._alreadyRecontained;

        /// <summary>
        /// Gets <see cref="Recontainer079"/> instance.
        /// </summary>
        public static Recontainer079 SCP079Recontainer
        {
            get
            {
                if (recontainer == null)
                    recontainer = GameObject.FindObjectOfType<Recontainer079>();

                return recontainer;
            }
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

        /// <summary>
        /// Spawns primitive object admin toy.
        /// </summary>
        /// <param name="type">Toy type.</param>
        /// <param name="parent">Toy's parent.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="hasCollision">If toy should have collision.</param>
        /// <param name="syncPosition">Should toy's position be synce once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Transform parent, Color color, bool hasCollision, bool syncPosition, byte? movementSmoothing = null)
        {
            AdminToyBase toy = UnityEngine.Object.Instantiate(PrimitiveBaseObject, parent);
            PrimitiveObjectToy ptoy = toy.GetComponent<PrimitiveObjectToy>();
            ptoy.NetworkPrimitiveType = type;
            ptoy.NetworkMaterialColor = color;
            if (!(movementSmoothing is null))
                ptoy.MovementSmoothing = movementSmoothing ?? 0;
            ptoy.transform.localPosition = Vector3.zero;
            ptoy.transform.localRotation = Quaternion.identity;
            ptoy.transform.localScale = Vector3.one;
            ptoy.NetworkScale = hasCollision ?
                new Vector3(Math.Abs(ptoy.transform.lossyScale.x), Math.Abs(ptoy.transform.lossyScale.y), Math.Abs(ptoy.transform.lossyScale.z)) :
                new Vector3(-Math.Abs(ptoy.transform.lossyScale.x), -Math.Abs(ptoy.transform.lossyScale.y), -Math.Abs(ptoy.transform.lossyScale.z));
            NetworkServer.Spawn(toy.gameObject);

            if (syncPosition)
                SyncToyPosition.Add(ptoy);
            else
                ptoy.UpdatePositionServer();

            return ptoy;
        }

        /// <inheritdoc cref="SpawnPrimitive(PrimitiveType, Transform, Color, bool, bool, byte?)"/>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Transform parent, Color color, bool syncPosition, byte? movementSmoothing = null)
        {
            return SpawnPrimitive(type, parent, color, true, syncPosition, movementSmoothing);
        }

        /// <summary>
        /// Spawns primitive object admin toy.
        /// </summary>
        /// <param name="type">Toy type.</param>
        /// <param name="position">Toy's position.</param>
        /// <param name="rotation">Toy's rotation.</param>
        /// <param name="scale">Toy's scale.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="syncPosition">Should toy's position be synce once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool syncPosition, byte? movementSmoothing = null)
        {
            AdminToyBase toy = UnityEngine.Object.Instantiate(PrimitiveBaseObject);
            PrimitiveObjectToy ptoy = toy.GetComponent<PrimitiveObjectToy>();
            ptoy.NetworkPrimitiveType = type;
            ptoy.NetworkMaterialColor = color;
            if (!(movementSmoothing is null))
                ptoy.MovementSmoothing = movementSmoothing ?? 0;
            ptoy.transform.position = position;
            ptoy.transform.rotation = rotation;
            ptoy.transform.localScale = scale;
            ptoy.NetworkScale = ptoy.transform.lossyScale;
            NetworkServer.Spawn(toy.gameObject);

            if (syncPosition)
                SyncToyPosition.Add(ptoy);
            else
                ptoy.UpdatePositionServer();

            return ptoy;
        }

        /// <summary>
        /// Spawns light srource admin toy.
        /// </summary>
        /// <param name="parent">Toy's parent.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="intensity">Toy's light intensity.</param>
        /// <param name="range">Toy's ligh range.</param>
        /// <param name="shadows">Should toy's light cause shadows.</param>
        /// <param name="syncPosition">Should toy's position be synce once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static LightSourceToy SpawnLight(Transform parent, Color color, float intensity, float range, bool shadows, bool syncPosition, byte? movementSmoothing = null)
        {
            AdminToyBase toy = UnityEngine.Object.Instantiate(PrimitiveBaseLight, parent);
            LightSourceToy ptoy = toy.GetComponent<LightSourceToy>();
            ptoy.NetworkLightColor = color;
            ptoy.NetworkLightIntensity = intensity;
            ptoy.NetworkLightRange = range;
            ptoy.NetworkLightShadows = shadows;
            if (!(movementSmoothing is null))
                ptoy.MovementSmoothing = movementSmoothing ?? 0;
            ptoy.transform.localPosition = Vector3.zero;
            ptoy.transform.localRotation = Quaternion.identity;
            ptoy.transform.localScale = Vector3.one;
            ptoy.NetworkScale = ptoy.transform.localScale;
            NetworkServer.Spawn(toy.gameObject);

            if (syncPosition)
                SyncToyPosition.Add(ptoy);
            else
                ptoy.UpdatePositionServer();
            return ptoy;
        }

        /// <summary>
        /// Spawns light srource admin toy.
        /// </summary>
        /// <param name="position">Toy's position.</param>
        /// <param name="rotation">Toy's rotation.</param>
        /// <param name="scale">Toy's scale.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="intensity">Toy's light intensity.</param>
        /// <param name="range">Toy's ligh range.</param>
        /// <param name="shadows">Should toy's light cause shadows.</param>
        /// <param name="syncPosition">Should toy's position be synce once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static LightSourceToy SpawnLight(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float intensity, float range, bool shadows, bool syncPosition, byte? movementSmoothing = null)
        {
            AdminToyBase toy = UnityEngine.Object.Instantiate(PrimitiveBaseLight);
            LightSourceToy ptoy = toy.GetComponent<LightSourceToy>();
            ptoy.NetworkLightColor = color;
            ptoy.NetworkLightIntensity = intensity;
            ptoy.NetworkLightRange = range;
            ptoy.NetworkLightShadows = shadows;
            if (!(movementSmoothing is null))
                ptoy.MovementSmoothing = movementSmoothing ?? 0;
            ptoy.transform.position = position;
            ptoy.transform.rotation = rotation;
            ptoy.transform.localScale = scale;
            ptoy.NetworkScale = ptoy.transform.localScale;
            NetworkServer.Spawn(toy.gameObject);

            if (syncPosition)
                SyncToyPosition.Add(ptoy);
            else
                ptoy.UpdatePositionServer();
            return ptoy;
        }

        /// <summary>
        /// Spawn's structures.
        /// </summary>
        /// <param name="type">Structure type.</param>
        /// <param name="position">Structure's position.</param>
        /// <param name="rotation">Structure's rotation.</param>
        /// <returns>Spawned structure.</returns>
        public static GameObject SpawnStructure(StructureType type, Vector3 position, Quaternion rotation)
        {
            if (!NetworkClient.prefabs.TryGetValue(StructurePrefabs[type], out GameObject prefab))
                return null;

            GameObject obj = UnityEngine.Object.Instantiate(prefab, position, rotation);
            NetworkServer.Spawn(obj);
            return obj;
        }

        /// <summary>
        /// Spawn's structures.
        /// </summary>
        /// <param name="type">Structure type.</param>
        /// <param name="parent">Structure's parent.</param>
        /// <returns>Spawned structure.</returns>
        public static GameObject SpawnStructure(StructureType type, Transform parent)
        {
            if (!NetworkClient.prefabs.TryGetValue(StructurePrefabs[type], out GameObject prefab))
                return null;

            GameObject obj = UnityEngine.Object.Instantiate(prefab, parent.position, parent.rotation);
            obj.transform.SetParent(parent);
            NetworkServer.Spawn(obj);
            return obj;
        }

        internal static readonly HashSet<AdminToyBase> SyncToyPosition = new HashSet<AdminToyBase>();

        internal static void PostRoundCleanup()
        {
            container = null;
            recontainer = null;
            SyncToyPosition.Clear();
        }

        private static readonly Dictionary<StructureType, Guid> StructurePrefabs = new Dictionary<StructureType, Guid>()
        {
            { StructureType.Scp018Pedestal, new Guid("a149d3eb-11bd-de24-f9dd-57187f5771ef") },
            { StructureType.Scp207Pedestal, new Guid("17054030-9461-d104-5b92-9456c9eb0ab7") },
            { StructureType.Scp244Pedestal, new Guid("fa602fdc-724c-d2a4-8b8c-1fb314b82746") },
            { StructureType.Scp268Pedestal, new Guid("68f13209-e652-6024-2b89-0f75fb88a998") },
            { StructureType.Scp500Pedestal, new Guid("f4149b66-c503-87a4-0b93-aabfe7c352da") },
            { StructureType.Scp1853Pedestal, new Guid("4f36c701-ea0c-9064-2a58-2c89240e51ba") },
            { StructureType.Scp2176Pedestal, new Guid("fff1c10c-a719-bea4-d95c-3e262ed03ab2") },
            { StructureType.RegularMedkitLocker, new Guid("5b227bd2-1ed2-8fc4-2aa1-4856d7cb7472") },
            { StructureType.AdrenalineMedkitLocker, new Guid("db602577-8d4f-97b4-890b-8c893bfcd553") },
            { StructureType.RifleRackLocker, new Guid("850f84ad-e273-1824-8885-11ae5e01e2f4") },
            { StructureType.MiscLocker, new Guid("d54bead1-286f-3004-facd-74482a872ad8") },
            { StructureType.LargeGunLocker, new Guid("5ad5dc6d-7bc5-3154-8b1a-3598b96e0d5b") },
            { StructureType.Generator, new Guid("daf3ccde-4392-c0e4-882d-b7002185c6b8") },
            { StructureType.Workstation, new Guid("ad8a455f-062d-dea4-5b47-ac9217d4c58b") },
        };

        private static Recontainer079 recontainer;
        private static LureSubjectContainer container;

        private static LightSourceToy primitiveBaseLight = null;
        private static PrimitiveObjectToy primitiveBaseObject = null;

        private static PrimitiveObjectToy PrimitiveBaseObject
        {
            get
            {
                if (primitiveBaseObject == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<PrimitiveObjectToy>(out var component))
                            primitiveBaseObject = component;
                    }
                }

                return primitiveBaseObject;
            }
        }

        private static LightSourceToy PrimitiveBaseLight
        {
            get
            {
                if (primitiveBaseLight == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<LightSourceToy>(out var component))
                            primitiveBaseLight = component;
                    }
                }

                return primitiveBaseLight;
            }
        }
    }
}
