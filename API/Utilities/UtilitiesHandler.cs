// -----------------------------------------------------------------------
// <copyright file="UtilitiesHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;

namespace Mistaken.API.Utilities
{
    internal class UtilitiesHandler : Module
    {
        public UtilitiesHandler(PluginHandler p)
            : base(p)
        {
        }

        public override bool IsBasic => true;

        public override string Name => "Utilities";

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += this.Server_RespawningTeam;
            Exiled.Events.Handlers.Scp079.InteractingTesla += this.Scp079_InteractingTesla;
            Exiled.Events.Handlers.Player.TriggeringTesla += this.Player_TriggeringTesla;
            Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound;
            Exiled.Events.Handlers.Player.Hurting += this.Player_Hurting;
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Server_RespawningTeam;
            Exiled.Events.Handlers.Scp079.InteractingTesla -= this.Scp079_InteractingTesla;
            Exiled.Events.Handlers.Player.TriggeringTesla -= this.Player_TriggeringTesla;
            Exiled.Events.Handlers.Server.RestartingRound -= this.Server_RestartingRound;
            Exiled.Events.Handlers.Player.Hurting -= this.Player_Hurting;
        }

        private void Player_Hurting(Exiled.Events.EventArgs.HurtingEventArgs ev)
        {
            if (ev.Handler.Type == Exiled.API.Enums.DamageType.Scp207)
            {
                if (ev.Target.GetSessionVariable<bool>(SessionVarType.IGNORE_SCP207_DAMAGE))
                    ev.IsAllowed = false;
            }
        }

        private void Server_RestartingRound()
        {
            Map.Restart();
        }

        private void Player_TriggeringTesla(Exiled.Events.EventArgs.TriggeringTeslaEventArgs ev)
        {
            if (Map.TeslaMode == TeslaMode.DISABLED || Map.TeslaMode == TeslaMode.DISABLED_FOR_ALL)
                ev.IsTriggerable = false;
        }

        private void Scp079_InteractingTesla(Exiled.Events.EventArgs.InteractingTeslaEventArgs ev)
        {
            if (Map.TeslaMode == TeslaMode.DISABLED_FOR_079 || Map.TeslaMode == TeslaMode.DISABLED_FOR_ALL)
                ev.IsAllowed = false;
        }

        private void Server_RespawningTeam(Exiled.Events.EventArgs.RespawningTeamEventArgs ev)
        {
            if (Map.RespawnLock)
                ev.Players.Clear();
            foreach (var player in ev.Players.ToArray())
            {
                if (player.GetSessionVariable<bool>(SessionVarType.RESPAWN_BLOCK))
                    ev.Players.Remove(player);
            }
        }
    }
}
