// -----------------------------------------------------------------------
// <copyright file="Shield.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JetBrains.Annotations;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator
namespace Mistaken.API.Shield
{
    /// <summary>
    /// Script used to handle shield for players.
    /// </summary>
    [PublicAPI]
    public abstract class Shield : MonoBehaviour
    {
        /// <summary>
        /// Creates new <typeparamref name="T"/> instance.
        /// </summary>
        /// <typeparam name="T">Shield.</typeparam>
        /// <param name="player">Player to affect.</param>
        /// <returns>New shield.</returns>
        public static T Ini<T>(MPlayer player)
            where T : Shield, new()
        {
            var instance = player.GameObject.AddComponent<T>();
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
        protected MPlayer Player { get; private set; }

        /// <summary>
        /// Gets max value of shield.
        /// </summary>
        protected abstract float MaxShield { get; }

        /// <summary>
        /// Gets shield's recharge rate per second.
        /// </summary>
        protected abstract float ShieldRechargeRate { get; }

        /// <summary>
        /// Gets shield effectivnes.
        /// </summary>
        protected abstract float ShieldEffectivnes { get; }

        /// <summary>
        /// Gets time that has to pass since last damage to start regenerating shield.
        /// </summary>
        protected abstract float TimeUntilShieldRecharge { get; }

        /// <summary>
        /// Gets speed at which shield will drop when it overflows over <see cref="MaxShield"/> (set to 0 to disable).
        /// </summary>
        protected virtual float ShieldDropRateOnOverflow { get; } = 5f;

        /// <summary>
        /// Gets or sets time left until shield recharge is possible.
        /// </summary>
        protected float InternalTimeUntilShieldRecharge { get; set; }

        /// <summary>
        /// Gets current shield recharge rate.
        /// </summary>
        protected float CurrentShieldRechargeRate { get; private set; }

        /// <summary>
        /// Gets ... YES.
        /// </summary>
        protected PlayerStatsSystem.AhpStat.AhpProcess Process { get; private set; }

        /// <summary>
        /// Unity's Start.
        /// </summary>
        protected virtual void Start()
        {
            Log.Debug("Created " + this.GetType().Name, Plugin.Instance.Config.Debug);
            this.Process = ((PlayerStatsSystem.AhpStat)this.Player.ReferenceHub.playerStats.StatModules[1]).ServerAddProcess(0f, this.MaxShield, -this.ShieldRechargeRate, this.ShieldEffectivnes, 0, true);

            EventManager.RegisterEvents(this);
        }

        /// <summary>
        /// Unity's OnDestroy.
        /// </summary>
        protected virtual void OnDestroy()
        {
            ((PlayerStatsSystem.AhpStat)this.Player.ReferenceHub.playerStats.StatModules[1]).ServerKillProcess(this.Process.KillCode);

            EventManager.UnregisterEvents(this);
            Log.Debug("Destroyed " + this.GetType().Name, Plugin.Instance.Config.Debug);
        }

        /// <summary>
        /// Unity's FixedUpdate.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            this.Process.Limit = this.MaxShield;
            this.Process.Efficacy = this.ShieldEffectivnes;

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
                        this.Process.DecayRate = 0;
                        this.CurrentShieldRechargeRate = 0;
                        this.Player.ArtificialHealth = this.MaxShield;
                        return;
                    }
                }

                this.Process.DecayRate = this.ShieldDropRateOnOverflow;
                this.CurrentShieldRechargeRate = -this.ShieldDropRateOnOverflow;
                return;
            }
            else if (this.Player.ArtificialHealth == this.MaxShield)
            {
                this.Process.DecayRate = 0;
                this.CurrentShieldRechargeRate = 0;
                return;
            }

            if (this.CanRegen)
            {
                this.Process.DecayRate = -this.ShieldRechargeRate;
                this.CurrentShieldRechargeRate = this.ShieldRechargeRate;
            }
            else
            {
                this.Process.DecayRate = 0f;
                this.CurrentShieldRechargeRate = 0;
            }
        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        private void OnPlayerChangeRole(MPlayer player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            if (player != this.Player)
                return;

            // if (!ev.IsAllowed)
                // return;

            Destroy(this);
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        private void OnPlayerDamage(MPlayer player, MPlayer attacker, DamageHandlerBase damageHandler)
        {
            if (player != this.Player)
                return;

            // if (!ev.IsAllowed)
                // return;

            this.InternalTimeUntilShieldRecharge = this.TimeUntilShieldRecharge;
        }
    }
}
