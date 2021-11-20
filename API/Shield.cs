// -----------------------------------------------------------------------
// <copyright file="Shield.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Timers;
using Exiled.API.Features;
using UnityEngine;

namespace Mistaken.API.Shield
{
    /// <summary>
    /// Scritp used to handle shield for players.
    /// </summary>
    public abstract class Shield : MonoBehaviour
    {
        /// <summary>
        /// Creates new <typeparamref name="T"/> instance.
        /// </summary>
        /// <typeparam name="T">Shield.</typeparam>
        /// <param name="player">Player to affect.</param>
        /// <returns>New shield.</returns>
        public static T Ini<T>(Player player)
            where T : Shield, new()
        {
            T instance = player.GameObject.AddComponent<T>();
            instance.Player = player;

            return instance;
        }

        /// <summary>
        /// Gets a value indicating whether shield can regenerate.
        /// </summary>
        protected bool CanRegen { get; private set; }

        /// <summary>
        /// Gets player with shield.
        /// </summary>
        protected Player Player { get; private set; }

        /// <summary>
        /// Gets max value of shield.
        /// </summary>
        protected abstract int MaxShield { get; }

        /// <summary>
        /// Gets shield's recharage rate per second.
        /// </summary>
        protected abstract float ShieldRechargeRate { get; }

        /// <summary>
        /// Gets shield effectivnes.
        /// </summary>
        protected abstract float ShieldEffectivnes { get; }

        /// <summary>
        /// Gets time that has to pass since last damage to start regerenrating shield.
        /// </summary>
        protected abstract float TimeUntilShieldRecharge { get; }

        /// <summary>
        /// Gets speed at which shield will drop when it overflows over <see cref="MaxShield"/> (set to 0 to disable).
        /// </summary>
        protected virtual float ShieldDropRateOnOverflow { get; } = 5f;

        /// <summary>
        /// Gets or sets time left untill shield recharge is possible.
        /// </summary>
        protected float InternalTimeUntilShieldRecharge { get; set; }

        public float CurrentShieldRechargeRate { get; private set; }

        /// <summary>
        /// Unity's Start.
        /// </summary>
        protected virtual void Start()
        {
            Log.Debug("Created " + this.GetType().Name);

            this.prevArtificialHpDelay = this.Player.ArtificialHealthDecay;
            this.prevArtificialHpRatio = this.Player.ReferenceHub.playerStats.ArtificialNormalRatio;
            this.prevMaxArtificialHp = this.Player.MaxArtificialHealth;

            this.InternalTimeUntilShieldRecharge = 5f;

            Exiled.Events.Handlers.Player.Hurting += this.Player_Hurting;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;

            this.Player.MaxArtificialHealth = 5000;
            this.Player.ArtificialHealthDecay = 0;
            this.CurrentShieldRechargeRate = 0;
            this.Player.ReferenceHub.playerStats.ArtificialNormalRatio = this.ShieldEffectivnes;
        }

        /// <summary>
        /// Unity's OnDestroy.
        /// </summary>
        protected virtual void OnDestroy()
        {
            this.Player.ArtificialHealthDecay = this.prevArtificialHpDelay;
            this.Player.ReferenceHub.playerStats.ArtificialNormalRatio = this.prevArtificialHpRatio;
            this.Player.MaxArtificialHealth = this.prevMaxArtificialHp;

            Exiled.Events.Handlers.Player.Hurting -= this.Player_Hurting;
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;

            Log.Debug("Destoryed " + this.GetType().Name);
        }

        /// <summary>
        /// Unity's FixedUpdate.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (this.TimeUntilShieldRecharge != 0f)
            {
                this.InternalTimeUntilShieldRecharge -= Time.fixedDeltaTime;
                this.CanRegen = this.InternalTimeUntilShieldRecharge <= 0f;
            }
            else
                this.CanRegen = true;

            if (this.Player.ArtificialHealth > this.MaxShield)
            {
                if (this.ShieldDropRateOnOverflow != 0)
                {
                    if (this.Player.ArtificialHealth - 1 < this.MaxShield)
                    {
                        this.Player.ArtificialHealthDecay = 0;
                        this.CurrentShieldRechargeRate = 0;
                        this.Player.ArtificialHealth = this.MaxShield;
                        return;
                    }
                }

                this.Player.ArtificialHealthDecay = this.ShieldDropRateOnOverflow;
                this.CurrentShieldRechargeRate = -this.ShieldDropRateOnOverflow;
                return;
            }
            else if (this.Player.ArtificialHealth == this.MaxShield)
            {
                this.Player.ArtificialHealthDecay = 0;
                this.CurrentShieldRechargeRate = 0;
                return;
            }

            if (this.CanRegen)
            {
                this.Player.ArtificialHealthDecay = -this.ShieldRechargeRate;
                this.CurrentShieldRechargeRate = this.ShieldRechargeRate;
            }
            else
            {
                this.Player.ArtificialHealthDecay = 0f;
                this.CurrentShieldRechargeRate = 0;
            }
        }

        private float prevArtificialHpDelay;
        private float prevArtificialHpRatio;
        private int prevMaxArtificialHp;

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Player != this.Player)
                return;

            if (!ev.IsAllowed)
                return;

            Destroy(this);
        }

        private void Player_Hurting(Exiled.Events.EventArgs.HurtingEventArgs ev)
        {
            if (ev.Target != this.Player)
                return;

            if (!ev.IsAllowed)
                return;

            this.InternalTimeUntilShieldRecharge = this.TimeUntilShieldRecharge;
        }
    }
}
