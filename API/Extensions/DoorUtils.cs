// -----------------------------------------------------------------------
// <copyright file="DoorUtils.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using Mirror;
using UnityEngine;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Door Utils.
    /// </summary>
    public static class DoorUtils
    {
        /// <summary>
        /// Initiates <see cref="DoorUtils"/>.
        /// </summary>
        public static void Ini()
        {
            if (initiated)
                return;

            Exiled.Events.Handlers.Map.Generated += Map_Generated;
            initiated = true;
        }

        /// <summary>
        /// Deinitiates <see cref="DoorUtils"/>.
        /// </summary>
        public static void DeIni()
        {
            Exiled.Events.Handlers.Map.Generated -= Map_Generated;
            initiated = false;
        }

        /// <summary>
        /// Returns door prefab.
        /// </summary>
        /// <param name="type">Door Type.</param>
        /// <returns>Prefab.</returns>
        public static DoorVariant GetPrefab(DoorType type)
        {
            switch (type)
            {
                case DoorType.EZ_BREAKABLE:
                case DoorType.HCZ_BREAKABLE:
                case DoorType.LCZ_BREAKABLE:
                    return Prefabs[type];
                default:
                    return null;
            }
        }

        /// <summary>
        /// Spawns Door.
        /// </summary>
        /// <param name="type">Door Type.</param>
        /// <param name="position">Door Position, if <see cref="Vector3.y"/> is smaller than 900 then door are automaticly locked to prevent crash.</param>
        /// <param name="rotation">Door Rotation.</param>
        /// <param name="size">Door Size.</param>
        /// <param name="shouldSpawn">Should door be spawned on clients.</param>
        /// <param name="name">Door name or <see langword="null"/> if there should be no name.</param>
        /// <returns>Rerurns spawned <see cref="DoorVariant"/>.</returns>
        public static DoorVariant SpawnDoor(DoorType type, Vector3 position, Vector3 rotation, Vector3 size, bool shouldSpawn = true, string name = null)
        {
            DoorVariant doorVariant = UnityEngine.Object.Instantiate(GetPrefab(type), position, Quaternion.Euler(rotation));
            UnityEngine.Object.Destroy(doorVariant.GetComponent<DoorEventOpenerExtension>());
            if (doorVariant.TryGetComponent<Scp079Interactable>(out var scp079Interactable))
                UnityEngine.Object.Destroy(scp079Interactable);
            doorVariant.transform.localScale = size;
            if (!string.IsNullOrEmpty(name))
                doorVariant.gameObject.AddComponent<DoorNametagExtension>().UpdateName(name);
            if (shouldSpawn)
                NetworkServer.Spawn(doorVariant.gameObject);
            return doorVariant;
        }

        /// <inheritdoc cref="SpawnDoor(DoorType, Vector3, Vector3, Vector3, bool, string)"/>
        public static DoorVariant SpawnDoor(DoorType type, Room room, Vector3 offset, Vector3 rotationOffset, Vector3 size, bool shouldSpawn = true, string name = null)
        {
            offset = (room.transform.forward * -offset.x) + (room.transform.right * -offset.z) + (Vector3.up * offset.y);
            return SpawnDoor(type, room.Position + offset, room.transform.eulerAngles + rotationOffset, size, shouldSpawn, name);
        }

        /// <summary>
        /// Changes lock setting for plugin door lock.
        /// </summary>
        /// <param name="door">Door to update.</param>
        /// <param name="type">Lock.</param>
        /// <param name="active">Should lock be enabled or disabled.</param>
        public static void ServerChangeLock(this DoorVariant door, PluginDoorLockReason type, bool active)
        {
            door?.ServerChangeLock((DoorLockReason)type, active);
        }

        /// <summary>
        /// Door Type.
        /// </summary>
        public enum DoorType
        {
#pragma warning disable CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
            EZ_BREAKABLE,
            HCZ_BREAKABLE,
            LCZ_BREAKABLE,
#pragma warning restore CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
        }

        /// <summary>
        /// Door lock reason.
        /// </summary>
        [Flags]
        public enum PluginDoorLockReason : ushort
        {
#pragma warning disable CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
            COOLDOWN = 512,
            BLOCKED_BY_SOMETHING = 1024,
            REQUIREMENTS_NOT_MET = 2048,
#pragma warning restore CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
        }

        private static readonly Dictionary<DoorType, DoorVariant> Prefabs = new Dictionary<DoorType, DoorVariant>();
        private static bool initiated = false;

        private static void Map_Generated()
        {
            Prefabs.Clear();
            foreach (var spawnpoint in UnityEngine.Object.FindObjectsOfType<DoorSpawnpoint>())
            {
                switch (spawnpoint.TargetPrefab.name.ToUpper())
                {
                    case "EZ BREAKABLEDOOR":
                        Prefabs[DoorType.EZ_BREAKABLE] = spawnpoint.TargetPrefab;
                        break;
                    case "HCZ BREAKABLEDOOR":
                        Prefabs[DoorType.HCZ_BREAKABLE] = spawnpoint.TargetPrefab;
                        break;
                    case "LCZ BREAKABLEDOOR":
                        Prefabs[DoorType.LCZ_BREAKABLE] = spawnpoint.TargetPrefab;
                        break;
                }
            }
        }
    }
}
