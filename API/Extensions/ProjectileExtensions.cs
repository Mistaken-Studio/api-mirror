// -----------------------------------------------------------------------
// <copyright file="ProjectileExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Footprinting;
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
    }
}
