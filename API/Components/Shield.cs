using JetBrains.Annotations;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator
namespace Mistaken.API.Shield;

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
    public static T Initialize<T>(Player player) where T : Shield, new()
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
    protected Player Player { get; private set; }

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
    /// Gets the AHP Process associated with that instance of the Shield.
    /// </summary>
    protected AhpStat.AhpProcess Process { get; private set; }

    /// <summary>
    /// Unity's Start.
    /// </summary>
    protected virtual void Start()
    {
        Log.Debug("Created " + GetType().Name, Plugin.Instance.Config.Debug);
        Process = ((AhpStat)Player.ReferenceHub.playerStats.StatModules[1]).ServerAddProcess(0f, MaxShield, -ShieldRechargeRate, ShieldEffectivnes, 0, true);

        EventManager.RegisterEvents(this);
    }

    /// <summary>
    /// Unity's OnDestroy.
    /// </summary>
    protected virtual void OnDestroy()
    {
        ((AhpStat)Player.ReferenceHub.playerStats.StatModules[1]).ServerKillProcess(Process.KillCode);

        EventManager.UnregisterEvents(this);
        Log.Debug("Destroyed " + GetType().Name, Plugin.Instance.Config.Debug);
    }

    /// <summary>
    /// Unity's FixedUpdate.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        Process.Limit = MaxShield;
        Process.Efficacy = ShieldEffectivnes;

        if (TimeUntilShieldRecharge != 0f)
        {
            InternalTimeUntilShieldRecharge -= Time.fixedDeltaTime;
            CanRegen = InternalTimeUntilShieldRecharge <= 0f;
        }
        else
            CanRegen = true;

        if (Player.ArtificialHealth > MaxShield)
        {
            if (ShieldDropRateOnOverflow != 0)
            {
                if (Player.ArtificialHealth - 1 < MaxShield)
                {
                    Process.DecayRate = 0;
                    CurrentShieldRechargeRate = 0;
                    Player.ArtificialHealth = MaxShield;
                    return;
                }
            }

            Process.DecayRate = ShieldDropRateOnOverflow;
            CurrentShieldRechargeRate = -ShieldDropRateOnOverflow;
            return;
        }
        else if (Player.ArtificialHealth == MaxShield)
        {
            Process.DecayRate = 0;
            CurrentShieldRechargeRate = 0;
            return;
        }

        if (CanRegen)
        {
            Process.DecayRate = -ShieldRechargeRate;
            CurrentShieldRechargeRate = ShieldRechargeRate;
        }
        else
        {
            Process.DecayRate = 0f;
            CurrentShieldRechargeRate = 0;
        }
    }

    [PluginEvent(ServerEventType.PlayerChangeRole)]
    private void OnPlayerChangeRole(MPlayer player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
    {
        if (player != Player)
            return;

        Destroy(this);
    }

    [PluginEvent(ServerEventType.PlayerDamage)]
    private void OnPlayerDamage(MPlayer player, MPlayer attacker, DamageHandlerBase damageHandler)
    {
        if (player != Player)
            return;

        InternalTimeUntilShieldRecharge = TimeUntilShieldRecharge;
    }
}
