// -----------------------------------------------------------------------
// <copyright file="ToyHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using AdminToys;
using Exiled.API.Enums;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Mistaken.API.Toys
{
    /// <summary>
    /// Obsolete.
    /// </summary>
    [Obsolete("Use Mistaken.Toy.API.ToyHandler")]
    public class ToyHandler
    {
        /// <inheritdoc cref="Toy.API.ToyHandler.SpawnPrimitive(PrimitiveType, Transform, Color, bool, bool, byte?, MeshRenderer)"/>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Transform parent, Color color, bool hasCollision, bool syncPosition, byte? movementSmoothing, [CanBeNull] MeshRenderer meshRenderer)
        {
            return Toy.API.ToyHandler.SpawnPrimitive(
                type,
                parent,
                color,
                hasCollision,
                syncPosition,
                movementSmoothing,
                meshRenderer);
        }

        /// <inheritdoc cref="Toy.API.ToyHandler.SpawnPrimitive(PrimitiveType, Vector3, Quaternion, Vector3, Color, bool, byte?, MeshRenderer)"/>
        public static PrimitiveObjectToy SpawnPrimitive(PrimitiveType type, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool syncPosition, byte? movementSmoothing, [CanBeNull] MeshRenderer meshRenderer)
        {
            return Toy.API.ToyHandler.SpawnPrimitive(
                type,
                position,
                rotation,
                scale,
                color,
                syncPosition,
                movementSmoothing,
                meshRenderer);
        }

        /// <inheritdoc cref="Toy.API.ToyHandler.SpawnLight(Transform, Color, float, float, bool, bool, byte?)"/>
        public static LightSourceToy SpawnLight(Transform parent, Color color, float intensity, float range, bool shadows, bool syncPosition, byte? movementSmoothing = null)
        {
            return Toy.API.ToyHandler.SpawnLight(
                parent,
                color,
                intensity,
                range,
                shadows,
                syncPosition,
                movementSmoothing);
        }

        /// <inheritdoc cref="Toy.API.ToyHandler.SpawnLight(Vector3, Quaternion, Vector3, Color, float, float, bool, bool, byte?)"/>
        public static LightSourceToy SpawnLight(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float intensity, float range, bool shadows, bool syncPosition, byte? movementSmoothing = null)
        {
            return Toy.API.ToyHandler.SpawnLight(
                position,
                rotation,
                scale,
                color,
                intensity,
                range,
                shadows,
                syncPosition,
                movementSmoothing);
        }

        /// <inheritdoc cref="Toy.API.ToyHandler.SpawnLight(Light, bool, byte?)"/>
        public static LightSourceToy SpawnLight(Light original, bool syncPosition, byte? movementSmoothing = null)
        {
            return Toy.API.ToyHandler.SpawnLight(
                original,
                syncPosition,
                movementSmoothing);
        }

        /// <inheritdoc cref="Toy.API.ToyHandler.GetPrimitiveType(MeshFilter)"/>
        public static PrimitiveType GetPrimitiveType(MeshFilter filter)
        {
            return Toy.API.ToyHandler.GetPrimitiveType(filter);
        }

        /// <inheritdoc cref="Toy.API.ToyHandler.SpawnShootingTarget(ShootingTargetType, Vector3, Quaternion, Vector3)"/>
        public static ShootingTarget SpawnShootingTarget(ShootingTargetType type, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return Toy.API.ToyHandler.SpawnShootingTarget(
                type,
                position,
                rotation,
                scale);
        }
    }
}
