// -----------------------------------------------------------------------
// <copyright file="ToyHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using AdminToys;
using Exiled.API.Interfaces;
using Mirror;
using Mistaken.API.Diagnostics;
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
        /// <returns>Spawned toy.</returns>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Transform parent, Color color, bool hasCollision, bool syncPosition, byte? movementSmoothing = null)
        {
            var toy = SpawnBase(PrimitiveBaseObject, parent, movementSmoothing);

            var primitiveObjectToy = InitializePrimitiveSource(toy, type, color, hasCollision);

            FinishSpawningToy(toy, syncPosition);

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
        /// <returns>Spawned toy.</returns>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool syncPosition, byte? movementSmoothing = null)
        {
            var toy = SpawnBase(PrimitiveBaseObject, position, rotation, scale, movementSmoothing);
            var primitiveObjectToy = InitializePrimitiveSource(toy, type, color, null);

            FinishSpawningToy(toy, syncPosition);

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

            var lightSourceToy = InitializeLightSource(toy, color, intensity, range, shadows);

            FinishSpawningToy(toy, syncPosition);

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

            var lightSourceToy = InitializeLightSource(toy, color, intensity, range, shadows);

            FinishSpawningToy(toy, syncPosition);

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
            Exiled.Events.Handlers.Server.RestartingRound += PostRoundCleanup;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= PostRoundCleanup;
        }

        internal static readonly HashSet<AdminToyBase> SyncToyPosition = new HashSet<AdminToyBase>();

        private static LightSourceToy primitiveBaseLight;
        private static PrimitiveObjectToy primitiveBaseObject;

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

        private static void FinishSpawningToy(AdminToyBase toy, bool syncPosition)
        {
            NetworkServer.Spawn(toy.gameObject);

            if (syncPosition)
                SyncToyPosition.Add(toy);
            else
                toy.UpdatePositionServer();
        }

        private static LightSourceToy InitializeLightSource(AdminToyBase toy, Color color, float intensity, float range, bool shadows)
        {
            var lightSourceToy = toy.GetComponent<LightSourceToy>();
            lightSourceToy.NetworkLightColor = color;
            lightSourceToy.NetworkLightIntensity = intensity;
            lightSourceToy.NetworkLightRange = range;
            lightSourceToy.NetworkLightShadows = shadows;

            return lightSourceToy;
        }

        private static PrimitiveObjectToy InitializePrimitiveSource(AdminToyBase toy, PrimitiveType type, Color color, bool? hasCollision)
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
            }

            return primitiveObjectToy;
        }

        private static void PostRoundCleanup()
        {
            SyncToyPosition.Clear();
        }
    }
}
