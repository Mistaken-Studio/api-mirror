// -----------------------------------------------------------------------
// <copyright file="Shield.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Exiled.API.Features;
using MEC;
using Mistaken.API.Diagnostics;
using PlayableScps.Interfaces;
using UnityEngine;

namespace Mistaken.API.Shield
{
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

#pragma warning disable CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
#pragma warning disable SA1401 // Fields should be private
        protected bool canRegen;
        protected float prevArtificialHpDelay;
        protected float prevArtificialHpRatio;
        protected int prevMaxArtificialHp;
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej

        protected Player Player { get; set; }

        protected abstract int MaxShield { get; }

        protected abstract float ShieldRechargeRate { get; }

        protected abstract float ShielEffectivnes { get; }

        protected abstract float TimeUntilShieldRecharge { get; }

        private float timeUntilShieldRecharge;

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Player != this.Player)
                return;

            if (!ev.IsAllowed)
                return;

            GameObject.Destroy(this);
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

            this.Player.MaxArtificialHealth = this.MaxShield;
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

            this.canRegen = this.timeUntilShieldRecharge <= 0f;
            if (this.canRegen)
            {
                this.Player.ArtificialHealthDecay = -this.ShieldRechargeRate;
            }
            else
            {
                this.Player.ArtificialHealthDecay = 0f;
            }
        }
    }

#pragma warning disable

    [System.Obsolete("Use Shield script", true)]
    public class Shielded
    {
        internal Player player { get; }

        public ushort MaxShield
        {
            get
            {
                return this.maxShield;
            }

            set
            {
                if (this.player != null)
                    this.player.MaxArtificialHealth += value - this.maxShield;
                this.maxShield = value;
            }
        }

        private ushort maxShield = 0;

        public ushort Regeneration { get; set; }

        public float SafeTime { get; }

        public float ShieldEffectivnes { get; }

        private readonly float originalShieldEffectivnes;

        public float ShieldDecay { get; set; }

        private readonly float originalShieldDecay;

        private readonly Timer safeTimer;
        private bool isSafe = true;

        public bool IsSafe
        {
            get
            {
                return this.isSafe;
            }

            private set
            {
                if (value)
                    this.isSafe = true;
                else
                {
                    this.safeTimer.Start();
                    this.isSafe = false;
                }
            }
        }

        public Shielded(Player p, ushort maxShield, ushort regeneration, float safeTime = -1, float shieldDecay = -1, float shieldEffectivnes = -1)
        {
            Log.Debug($"Enabling shield for {p.Nickname}");
            this.MaxShield = maxShield;
            this.Regeneration = regeneration;
            this.SafeTime = safeTime;
            this.ShieldDecay = shieldDecay;
            this.ShieldEffectivnes = shieldEffectivnes;
            this.player = p;

            this.safeTimer = new Timer(this.SafeTime * 1000);
            this.safeTimer.Elapsed += (_, __) =>
            {
                this.IsSafe = true;
                this.safeTimer.Stop();
            };

            Exiled.Events.Handlers.Player.Left += this.Player_Left;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.Hurting += this.Player_Hurting;

            var ps = p.ReferenceHub.playerStats;
            this.originalShieldDecay = ps.ArtificialHpDecay;
            this.originalShieldEffectivnes = ps.ArtificialNormalRatio;
            if (this.ShieldDecay != -1)
                ps.ArtificialHpDecay = this.ShieldDecay;
            if (this.ShieldEffectivnes != -1)
                ps.ArtificialNormalRatio = this.ShieldEffectivnes;
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Player.Id == this.player.Id)
                this.Disable();
        }

        private void Player_Left(Exiled.Events.EventArgs.LeftEventArgs ev)
        {
            if (ev.Player.Id == this.player.Id)
                this.Disable();
        }

        private void Player_Hurting(Exiled.Events.EventArgs.HurtingEventArgs ev)
        {
            if (ev.Target.Id == this.player.Id)
                this.IsSafe = false;
        }

        internal void DoRegenerationCicle()
        {
            if (this.player.ArtificialHealth > this.MaxShield)
            {
                if (this.ShieldDecay != -1)
                    this.player.ReferenceHub.playerStats.ArtificialHpDecay = this.originalShieldDecay;
                return;
            }
            else
            {
                if (this.ShieldDecay != -1)
                    this.player.ReferenceHub.playerStats.ArtificialHpDecay = this.ShieldDecay;
            }

            if (this.player.ArtificialHealth == this.MaxShield)
                return;

            if (this.SafeTime == -1 || !this.IsSafe)
                return;
            this.player.ArtificialHealth += this.Regeneration;
            if (this.player.ArtificialHealth > this.MaxShield)
                this.player.ArtificialHealth = this.MaxShield;
        }

        internal void Disable()
        {
            Log.Debug($"Disabling shield for {this.player.Nickname}");
            Exiled.Events.Handlers.Player.Left -= this.Player_Left;
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.Hurting -= this.Player_Hurting;

            this.player.MaxArtificialHealth = 75;
            this.player.ArtificialHealth = 0;
            var ps = this.player.ReferenceHub.playerStats;
            if (this.ShieldEffectivnes != -1)
                ps.ArtificialNormalRatio = this.originalShieldEffectivnes;
            if (this.ShieldDecay != -1)
                ps.ArtificialHpDecay = this.originalShieldDecay;

            ShieldedManager.Shieldeds.Remove(this.player);
        }
    }
}
