// -----------------------------------------------------------------------
// <copyright file="ProjectileExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Projectile Extensions.
    /// </summary>
    public static class ProjectileExtensions
    {
        /// <summary>
        /// Throws <paramref name="grenade"/> from <paramref name="position"/> with <paramref name="direction"/> and <paramref name="force"/>.
        /// </summary>
        /// <param name="grenade">Item to throw.</param>
        /// <param name="position">Position to throw from.</param>
        /// <param name="direction">Direction of throw.</param>
        /// <param name="force">Force of throw.</param>
        /// <param name="upWardFactor">Up force of throw.</param>
        /// <returns>Thrown projectile.</returns>
        public static ThrownProjectile Throw(this ThrowableItem grenade, Vector3 position, Vector3 direction, float force, float upWardFactor = 1f)
        {
            grenade._destroyTime = Time.timeSinceLevelLoad + grenade._postThrownAnimationTime;
            grenade._alreadyFired = true;

            var thrownProjectile = Object.Instantiate(grenade.Projectile, position, Quaternion.Euler(direction));
            var pickupSyncInfo = default(PickupSyncInfo);
            pickupSyncInfo.ItemId = grenade.ItemTypeId;
            pickupSyncInfo.Locked = !grenade._repickupable;
            pickupSyncInfo.Serial = grenade.ItemSerial;
            pickupSyncInfo.Weight = grenade.Weight;
            pickupSyncInfo.Position = thrownProjectile.transform.position;
            pickupSyncInfo.Rotation = new(thrownProjectile.transform.rotation);
            var newInfo = thrownProjectile.NetworkInfo = pickupSyncInfo;
            thrownProjectile.PreviousOwner = new(Server.Host.ReferenceHub);
            NetworkServer.Spawn(thrownProjectile.gameObject);
            pickupSyncInfo = default;
            thrownProjectile.InfoReceived(pickupSyncInfo, newInfo);
            if (thrownProjectile.TryGetComponent(out Rigidbody component))
                grenade.PropelBody(component, direction, Vector3.zero, force, upWardFactor);

            thrownProjectile.ServerActivate();

            return thrownProjectile;
        }

        /// <summary>
        /// Throws <paramref name="grenade"/> from <paramref name="position"/> with <paramref name="direction"/> and default force.
        /// </summary>
        /// <param name="grenade">Item to throw.</param>
        /// <param name="position">Position to throw from.</param>
        /// <param name="direction">Direction of throw.</param>
        /// <returns>Thrown projectile.</returns>
        public static ThrownProjectile Throw(this ThrowableItem grenade, Vector3 position, Vector3 direction)
            => Throw(grenade, position, direction, grenade.WeakThrowSettings.StartVelocity, grenade.WeakThrowSettings.UpwardsFactor);
    }
}
