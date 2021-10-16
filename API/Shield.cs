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
            var instance = new T
            {
                Player = player,
            };

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
        protected abstract float ShielEffectivnes { get; }

        /// <summary>
        /// Gets time that has to pass since last damage to start regerenrating shield.
        /// </summary>
        protected abstract float TimeUntilShieldRecharge { get; }

        private float prevArtificialHpDelay;
        private float prevArtificialHpRatio;
        private int prevMaxArtificialHp;
        private float timeUntilShieldRecharge;

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

            this.timeUntilShieldRecharge = this.TimeUntilShieldRecharge;
        }

        private void OnStart()
        {
            this.prevArtificialHpDelay = this.Player.ArtificialHealthDecay;
            this.prevArtificialHpRatio = this.Player.ReferenceHub.playerStats.ArtificialNormalRatio;
            this.prevMaxArtificialHp = this.Player.MaxArtificialHealth;

            Exiled.Events.Handlers.Player.Hurting += this.Player_Hurting;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;

            this.Player.MaxArtificialHealth += this.MaxShield;
            this.Player.ArtificialHealthDecay = 0;
            this.Player.ReferenceHub.playerStats.ArtificialNormalRatio = this.ShielEffectivnes;
        }

        private void OnDestroy()
        {
            this.Player.ArtificialHealthDecay = this.prevArtificialHpDelay;
            this.Player.ReferenceHub.playerStats.ArtificialNormalRatio = this.prevArtificialHpRatio;
            this.Player.MaxArtificialHealth = this.prevMaxArtificialHp;

            Exiled.Events.Handlers.Player.Hurting -= this.Player_Hurting;
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;
        }

        private void FixedUpdate()
        {
            this.timeUntilShieldRecharge -= Time.fixedDeltaTime;

            this.CanRegen = this.timeUntilShieldRecharge <= 0f;
            if (this.CanRegen)
                this.Player.ArtificialHealthDecay = -(this.ShieldRechargeRate * Time.fixedDeltaTime);
            else
                this.Player.ArtificialHealthDecay = 0f;
        }
    }

#pragma warning disable

    [System.Obsolete("Use Shield script", true)]
    public class Shielded
    {
        public ushort MaxShield
        {
            get => throw new System.Exception("Use Shield MonoBehaviour script");
            set => throw new System.Exception("Use Shield MonoBehaviour script");
        }

        public ushort Regeneration { get; set; }

        public float SafeTime { get; }

        public float ShieldEffectivnes { get; }

        public float ShieldDecay { get; set; }

        public bool IsSafe
        {
            get => throw new System.Exception("Use Shield MonoBehaviour script");
        }

        public Shielded(Player p, ushort maxShield, ushort regeneration, float safeTime = -1, float shieldDecay = -1, float shieldEffectivnes = -1)
        {
            throw new System.Exception("Use Shield MonoBehaviour script");
        }
    }
}
