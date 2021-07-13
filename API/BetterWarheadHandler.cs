// -----------------------------------------------------------------------
// <copyright file="BetterWarheadHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Mistaken.API.Diagnostics;

namespace Mistaken.API
{
    /// <inheritdoc/>
    public class BetterWarheadHandler : Module
    {
        /// <summary>
        /// Gets <see cref="WarheadStatus"/> instance.
        /// </summary>
        public static WarheadStatus Warhead { get; } = new WarheadStatus();

        /// <inheritdoc cref="Module.Module(Exiled.API.Interfaces.IPlugin{Exiled.API.Interfaces.IConfig})"/>
        public BetterWarheadHandler(PluginHandler p)
            : base(p)
        {
        }

        /// <inheritdoc/>
        public override string Name => "BetterWarhead";

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Warhead.Starting += this.Handle<Exiled.Events.EventArgs.StartingEventArgs>((ev) => this.Warhead_Starting(ev));
            Exiled.Events.Handlers.Warhead.Stopping += this.Handle<Exiled.Events.EventArgs.StoppingEventArgs>((ev) => this.Warhead_Stopping(ev));
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += this.Handle<Exiled.Events.EventArgs.ActivatingWarheadPanelEventArgs>((ev) => this.Player_ActivatingWarheadPanel(ev));
            Exiled.Events.Handlers.Server.RestartingRound += this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Handle(() => this.Server_WaitingForPlayers(), "WaitingForPlayers");
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Warhead.Starting -= this.Handle<Exiled.Events.EventArgs.StartingEventArgs>((ev) => this.Warhead_Starting(ev));
            Exiled.Events.Handlers.Warhead.Stopping -= this.Handle<Exiled.Events.EventArgs.StoppingEventArgs>((ev) => this.Warhead_Stopping(ev));
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= this.Handle<Exiled.Events.EventArgs.ActivatingWarheadPanelEventArgs>((ev) => this.Player_ActivatingWarheadPanel(ev));
            Exiled.Events.Handlers.Server.RestartingRound -= this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.Handle(() => this.Server_WaitingForPlayers(), "WaitingForPlayers");
        }

        /// <summary>
        /// Warhead status.
        /// </summary>
        public class WarheadStatus
        {
            /// <inheritdoc cref="Exiled.API.Features.Warhead.IsInProgress"/>
            public bool CountingDown
            {
                get => Exiled.API.Features.Warhead.IsInProgress;
                set
                {
                    if (value)
                        Exiled.API.Features.Warhead.Start();
                    else
                        Exiled.API.Features.Warhead.Stop();
                }
            }

            /// <inheritdoc cref="Exiled.API.Features.Warhead.DetonationTimer"/>
            public float TimeLeft => Exiled.API.Features.Warhead.DetonationTimer;

            /// <summary>
            /// Gets or sets a value indicating whether lever should be locked or not.
            /// </summary>
            public bool LeverLock { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether warhead can be stopped using button or not.
            /// </summary>
            public bool StopLock { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether warhead can be started using button or not.
            /// </summary>
            public bool StartLock { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether surface button can be unlocked or not.
            /// </summary>
            public bool ButtonLock { get; set; } = false;

            /// <inheritdoc cref="Exiled.API.Features.Warhead.IsKeycardActivated"/>
            public bool ButtonOpen
            {
                get => Exiled.API.Features.Warhead.IsKeycardActivated;
                set => Exiled.API.Features.Warhead.IsKeycardActivated = value;
            }

            /// <inheritdoc cref="Exiled.API.Features.Warhead.IsDetonated"/>
            public bool Detonated => Exiled.API.Features.Warhead.IsDetonated;

            /// <inheritdoc cref="Exiled.API.Features.Warhead.LeverStatus"/>
            public bool Enabled
            {
                get => Exiled.API.Features.Warhead.LeverStatus;
                set => Exiled.API.Features.Warhead.LeverStatus = value;
            }

            /// <summary>
            /// Gets last user that used started warhead.
            /// </summary>
            public Player LastStartUser { get; internal set; } = null;

            /// <summary>
            /// Gets last user that used stoped warhead.
            /// </summary>
            public Player LastStopUser { get; internal set; } = null;
        }

        private void Server_WaitingForPlayers()
        {
            _ = Exiled.API.Features.Warhead.Controller;
            _ = Exiled.API.Features.Warhead.SitePanel;
            _ = Exiled.API.Features.Warhead.OutsitePanel;
        }

        private void Server_RestartingRound()
        {
            Warhead.ButtonLock = false;
            Warhead.LeverLock = false;
            Warhead.StartLock = false;
            Warhead.StopLock = false;
        }

        private void Player_ActivatingWarheadPanel(Exiled.Events.EventArgs.ActivatingWarheadPanelEventArgs ev)
        {
            if (Warhead.ButtonLock)
                ev.IsAllowed = false;
            if (ev.Player.IsBypassModeEnabled)
                ev.IsAllowed = true;
        }

        private void Warhead_Stopping(Exiled.Events.EventArgs.StoppingEventArgs ev)
        {
            if (ev.Player?.IsHost ?? true)
                return;
            if (Warhead.StopLock)
                ev.IsAllowed = false;
            if (ev.Player.IsBypassModeEnabled)
                ev.IsAllowed = true;

            if (ev.IsAllowed)
            {
                if (ev.Player != null)
                    Warhead.LastStopUser = ev.Player;
            }
        }

        private void Warhead_Starting(Exiled.Events.EventArgs.StartingEventArgs ev)
        {
            if (ev.Player?.IsHost ?? true)
                return;
            if (Warhead.StartLock)
                ev.IsAllowed = false;
            if (ev.Player.IsBypassModeEnabled)
                ev.IsAllowed = true;

            if (ev.IsAllowed)
            {
                if (ev.Player != null)
                    Warhead.LastStartUser = ev.Player;
            }
        }

        private void Server_RoundStarted()
        {
            Warhead.LastStartUser = null;
            Warhead.LastStopUser = null;
        }
    }
}
