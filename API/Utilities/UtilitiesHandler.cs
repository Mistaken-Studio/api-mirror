// -----------------------------------------------------------------------
// <copyright file="UtilitiesHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Mistaken.API.Extensions;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using Respawning;

namespace Mistaken.API.Utilities
{
    internal sealed class UtilitiesHandler
    {
        public UtilitiesHandler()
        {
            EventManager.RegisterEvents(this);
        }

        ~UtilitiesHandler()
        {
            EventManager.UnregisterEvents(this);
        }

        [PluginEvent(ServerEventType.PlayerDamage)]
        private bool OnPlayerDamage(Player player, Player attacker, DamageHandlerBase damageHandler)
        {
            if (damageHandler is UniversalDamageHandler universalDamageHandler && universalDamageHandler.TranslationId == 10)
            {
                if (player.GetSessionVariable<bool>(SessionVarType.IGNORE_SCP207_DAMAGE))
                    return false;
            }

            return true;
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        private void OnRoundRestart()
            => Map.Restart();

        [PluginEvent(ServerEventType.Scp079UseTesla)]
        private bool OnScp079UseTesla(Player player, TeslaGate tesla)
        {
            if (Map.TeslaMode == TeslaMode.DISABLED_FOR_079 || Map.TeslaMode == TeslaMode.DISABLED_FOR_ALL)
                return false;

            return true;
        }

        [PluginEvent(ServerEventType.TeamRespawn)]
        private bool OnTeamRespawn(SpawnableTeamType type)
            => !Map.RespawnLock;

        /*private void Server_RespawningTeam(Exiled.Events.EventArgs.RespawningTeamEventArgs ev)
        {
            if (Map.RespawnLock)
                ev.Players.Clear();

            foreach (var player in ev.Players.ToArray())
            {
                if (player.GetSessionVariable<bool>(SessionVarType.RESPAWN_BLOCK))
                    ev.Players.Remove(player);
            }
        }

        private void Player_TriggeringTesla(Exiled.Events.EventArgs.TriggeringTeslaEventArgs ev)
        {
            if (Map.TeslaMode == TeslaMode.DISABLED || Map.TeslaMode == TeslaMode.DISABLED_FOR_ALL)
                ev.IsTriggerable = false;
        }*/
    }
}
