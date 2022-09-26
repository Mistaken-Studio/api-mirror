// -----------------------------------------------------------------------
// <copyright file="ToyHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using JetBrains.Annotations;
using MEC;
using Mirror;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using UnityEngine;

namespace Mistaken.API.Toys
{
    /// <inheritdoc/>
    public class ToyHandler : Module
    {
        /// <summary>
        /// Spawns primitive object admin toy.
        /// </summary>
        /// <param name="type">Toy type.</param>
        /// <param name="parent">Toy's parent.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="hasCollision">If toy should have collision.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <param name="meshRenderer">Color source, if defined color will be synced.</param>
        /// <returns>Spawned toy.</returns>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Transform parent, Color color, bool hasCollision, bool syncPosition, byte? movementSmoothing, [CanBeNull] MeshRenderer meshRenderer)
        {
            var toy = SpawnBase(PrimitiveBaseObject, parent, movementSmoothing);

            var primitiveObjectToy = InitializePrimitive(toy, type, color, hasCollision, syncPosition, meshRenderer);

            FinishSpawningToy(toy);

            return primitiveObjectToy;
        }

        /// <summary>
        /// Spawns primitive object admin toy.
        /// </summary>
        /// <param name="type">Toy type.</param>
        /// <param name="position">Toy's position.</param>
        /// <param name="rotation">Toy's rotation.</param>
        /// <param name="scale">Toy's scale.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <param name="meshRenderer">Color source, if defined color will be synced.</param>
        /// <returns>Spawned toy.</returns>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool syncPosition, byte? movementSmoothing, [CanBeNull] MeshRenderer meshRenderer)
        {
            var toy = SpawnBase(PrimitiveBaseObject, position, rotation, scale, movementSmoothing);
            var primitiveObjectToy = InitializePrimitive(toy, type, color, null, syncPosition, meshRenderer);

            FinishSpawningToy(toy);

            return primitiveObjectToy;
        }

        /// <summary>
        /// Spawns light source admin toy.
        /// </summary>
        /// <param name="parent">Toy's parent.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="intensity">Toy's light intensity.</param>
        /// <param name="range">Toy's light range.</param>
        /// <param name="shadows">Should toy's light cause shadows.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static LightSourceToy SpawnLight(Transform parent, Color color, float intensity, float range, bool shadows, bool syncPosition, byte? movementSmoothing = null)
        {
            var toy = SpawnBase(PrimitiveBaseLight, parent, movementSmoothing);

            var lightSourceToy = InitializeLightSource(toy, color, intensity, range, shadows, syncPosition);

            FinishSpawningToy(toy);

            return lightSourceToy;
        }

        /// <summary>
        /// Spawns light source admin toy.
        /// </summary>
        /// <param name="position">Toy's position.</param>
        /// <param name="rotation">Toy's rotation.</param>
        /// <param name="scale">Toy's scale.</param>
        /// <param name="color">Toy's color.</param>
        /// <param name="intensity">Toy's light intensity.</param>
        /// <param name="range">Toy's light range.</param>
        /// <param name="shadows">Should toy's light cause shadows.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static LightSourceToy SpawnLight(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float intensity, float range, bool shadows, bool syncPosition, byte? movementSmoothing = null)
        {
            var toy = SpawnBase(PrimitiveBaseLight, position, rotation, scale, movementSmoothing);

            var lightSourceToy = InitializeLightSource(toy, color, intensity, range, shadows, syncPosition);

            FinishSpawningToy(toy);

            return lightSourceToy;
        }

        /// <summary>
        /// Spawns light source admin toy.
        /// </summary>
        /// <param name="original">Light to mimic.</param>
        /// <param name="syncPosition">Should toy's position be sync once every frame.</param>
        /// <param name="movementSmoothing">Toy's movementSmoothing.</param>
        /// <returns>Spawned toy.</returns>
        public static LightSourceToy SpawnLight(Light original, bool syncPosition, byte? movementSmoothing = null)
        {
            var toy = SpawnBase(PrimitiveBaseLight, original.transform, movementSmoothing);

            var lightSourceToy = InitializeLightSource(toy, original, syncPosition);

            FinishSpawningToy(toy);

            return lightSourceToy;
        }

        /// <inheritdoc cref="Module"/>
        public ToyHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
        }

        /// <inheritdoc/>
        public override string Name => nameof(ToyHandler);

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += PostRoundCleanup;
            Exiled.Events.Handlers.Player.Verified += Player_Verified;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= PostRoundCleanup;
            Exiled.Events.Handlers.Player.Verified -= Player_Verified;
        }

        internal static readonly HashSet<AdminToyBase> SyncToyPosition = new HashSet<AdminToyBase>();

        private static readonly Dictionary<Room, SynchronizerControllerScript> Controllers =
            new Dictionary<Room, SynchronizerControllerScript>();

        private static LightSourceToy primitiveBaseLight;
        private static PrimitiveObjectToy primitiveBaseObject;

        private static GlobalSynchronizerControllerScript globalController;

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

        private static AdminToyBase SpawnBase(AdminToyBase prefab, Transform parent, byte? movementSmoothing = null)
        {
            var toy = UnityEngine.Object.Instantiate(prefab, parent);

            if (!(movementSmoothing is null))
                toy.MovementSmoothing = (byte)movementSmoothing;
            toy.transform.localPosition = Vector3.zero;
            toy.transform.localRotation = Quaternion.identity;
            toy.transform.localScale = Vector3.one;
            toy.NetworkScale = toy.transform.localScale;

            return toy;
        }

        private static AdminToyBase SpawnBase(AdminToyBase prefab, Vector3 position, Quaternion rotation, Vector3 scale, byte? movementSmoothing = null)
        {
            var toy = UnityEngine.Object.Instantiate(prefab);

            if (!(movementSmoothing is null))
                toy.MovementSmoothing = (byte)movementSmoothing;
            toy.transform.position = position;
            toy.transform.rotation = rotation;
            toy.transform.localScale = scale;
            toy.NetworkScale = toy.transform.localScale;

            return toy;
        }

        private static void FinishSpawningToy(AdminToyBase toy)
        {
            NetworkServer.Spawn(toy.gameObject);

            toy.UpdatePositionServer();
        }

        private static LightSourceToy InitializeLightSource(AdminToyBase toy, Color color, float intensity, float range, bool shadows, bool syncPosition)
        {
            var lightSourceToy = toy.GetComponent<LightSourceToy>();
            lightSourceToy._light.color = color;
            lightSourceToy._light.intensity = intensity;
            lightSourceToy._light.range = range;
            lightSourceToy._light.shadows = shadows ? LightShadows.Soft : LightShadows.None;

            lightSourceToy.NetworkLightColor = color;
            lightSourceToy.NetworkLightIntensity = intensity;
            lightSourceToy.NetworkLightRange = range;
            lightSourceToy.NetworkLightShadows = shadows;

            var syncScript = lightSourceToy.gameObject.AddComponent<LightSynchronizerScript>();
            syncScript.Toy = lightSourceToy;
            syncScript.SyncPosition = syncScript.SyncRotation = syncScript.SyncScale = syncPosition;
            (toy.GetComponentInParent<SynchronizerControllerScript>() ?? globalController).AddScript(syncScript);

            return lightSourceToy;
        }

        private static LightSourceToy InitializeLightSource(AdminToyBase toy, Light original, bool syncPosition)
        {
            var lightSourceToy = toy.GetComponent<LightSourceToy>();

            lightSourceToy.NetworkLightColor = original.color;
            lightSourceToy.NetworkLightIntensity = original.intensity;
            lightSourceToy.NetworkLightRange = original.range;
            lightSourceToy.NetworkLightShadows = original.shadows != LightShadows.None;

            var syncScript = lightSourceToy.gameObject.AddComponent<LightSynchronizerScript>();
            syncScript.Toy = lightSourceToy;

            (toy.GetComponentInParent<SynchronizerControllerScript>() ?? globalController).AddScript(syncScript);

            syncScript.ClonedLight = original;
            syncScript.SyncPosition = syncScript.SyncRotation = syncScript.SyncScale = syncPosition;

            return lightSourceToy;
        }

        private static PrimitiveObjectToy InitializePrimitive(AdminToyBase toy, PrimitiveType type, Color color, bool? hasCollision, bool syncPosition, [CanBeNull] MeshRenderer syncColor)
        {
            var primitiveObjectToy = toy.GetComponent<PrimitiveObjectToy>();
            primitiveObjectToy.NetworkPrimitiveType = type;
            primitiveObjectToy.NetworkMaterialColor = color;

            if (hasCollision.HasValue)
            {
                toy.NetworkScale = new Vector3(
                    Math.Abs(toy.transform.lossyScale.x),
                    Math.Abs(toy.transform.lossyScale.y),
                    Math.Abs(toy.transform.lossyScale.z));
                toy.NetworkScale *= hasCollision.Value ? 1 : -1;

                if (!hasCollision.Value && toy.transform.localScale.x > 0 && (type == PrimitiveType.Plane || type == PrimitiveType.Quad))
                {
                    toy.transform.eulerAngles += Vector3.right * 180;
                    Exiled.API.Features.Log.Info("Rotated 180° X to compensate for negative scale");
                }

                toy.transform.localScale = new Vector3(
                    Math.Abs(toy.transform.localScale.x),
                    Math.Abs(toy.transform.localScale.y),
                    Math.Abs(toy.transform.localScale.z));
                toy.transform.localScale *= hasCollision.Value ? 1 : -1;
            }

            var syncScript = primitiveObjectToy.gameObject.AddComponent<PrimitiveSynchronizerScript>();
            syncScript.Toy = primitiveObjectToy;
            syncScript.SyncPosition = syncScript.SyncRotation = syncScript.SyncScale = syncPosition;
            syncScript.MeshRenderer = syncColor;

            (toy.GetComponentInParent<SynchronizerControllerScript>() ?? globalController).AddScript(syncScript);

            return primitiveObjectToy;
        }

        private static void PostRoundCleanup()
        {
            globalController = Server.Host.GameObject.AddComponent<GlobalSynchronizerControllerScript>();
            SyncToyPosition.Clear();

            foreach (var room in Room.List)
                Controllers[room] = room.gameObject.AddComponent<SynchronizerControllerScript>();

            Module.RunSafeCoroutine(SynchronizationHandler(), nameof(SynchronizationHandler));
        }

        private static IEnumerator<float> SynchronizationHandler()
        {
            Dictionary<Player, Room> lastRooms = new Dictionary<Player, Room>();

            while (!Round.IsStarted)
                yield return Timing.WaitForSeconds(1f);

            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(1f);

                foreach (var player in RealPlayers.List)
                {
                    var curRoom = player.GetCurrentRoom();

                    if (lastRooms.TryGetValue(player, out var lastRoom) && lastRoom == curRoom)
                        continue; // Skip, room didn't change since last update
                    lastRooms[player] = curRoom;

                    Exiled.API.Features.Log.Debug($"Room changed, {lastRoom?.Type.ToString() ?? "NONE"} -> {curRoom?.Type.ToString() ?? "NONE"}");

                    var room = Utilities.Room.Get(curRoom);

                    if (room == null)
                    {
                        foreach (var item in Controllers.Values)
                            item.RemoveSubscriber(player);

                        continue;
                    }

                    HashSet<SynchronizerControllerScript> toSync = NorthwoodLib.Pools.HashSetPool<SynchronizerControllerScript>.Shared.Rent();

                    if (Controllers.TryGetValue(room.ExiledRoom, out var script))
                        toSync.Add(script);

                    foreach (var item in room.FarNeighbors.Select(x => x.ExiledRoom))
                    {
                        if (Controllers.TryGetValue(item, out script))
                            toSync.Add(script);
                    }

                    foreach (var item in Controllers.Values.Where(x => !toSync.Contains(x)))
                        item.RemoveSubscriber(player);

                    foreach (var item in toSync)
                        item.AddSubscriber(player);

                    NorthwoodLib.Pools.HashSetPool<SynchronizerControllerScript>.Shared.Return(toSync);
                }
            }
        }

        private static void Player_Verified(Exiled.Events.EventArgs.VerifiedEventArgs ev)
        {
            globalController.SyncFor(ev.Player);
        }
    }
}
