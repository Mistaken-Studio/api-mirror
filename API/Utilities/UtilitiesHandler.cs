// -----------------------------------------------------------------------
// <copyright file="UtilitiesHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Mistaken.API.Diagnostics;

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
            Exiled.Events.Handlers.Server.RespawningTeam += this.Handle<Exiled.Events.EventArgs.RespawningTeamEventArgs>((ev) => this.Server_RespawningTeam(ev));
            Exiled.Events.Handlers.Scp079.InteractingTesla += this.Handle<Exiled.Events.EventArgs.InteractingTeslaEventArgs>((ev) => this.Scp079_InteractingTesla(ev));
            Exiled.Events.Handlers.Player.TriggeringTesla += this.Handle<Exiled.Events.EventArgs.TriggeringTeslaEventArgs>((ev) => this.Player_TriggeringTesla(ev));
            Exiled.Events.Handlers.Server.RestartingRound += this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= this.Handle<Exiled.Events.EventArgs.RespawningTeamEventArgs>((ev) => this.Server_RespawningTeam(ev));
            Exiled.Events.Handlers.Scp079.InteractingTesla -= this.Handle<Exiled.Events.EventArgs.InteractingTeslaEventArgs>((ev) => this.Scp079_InteractingTesla(ev));
            Exiled.Events.Handlers.Player.TriggeringTesla -= this.Handle<Exiled.Events.EventArgs.TriggeringTeslaEventArgs>((ev) => this.Player_TriggeringTesla(ev));
            Exiled.Events.Handlers.Server.RestartingRound -= this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
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
        }
    }
}
