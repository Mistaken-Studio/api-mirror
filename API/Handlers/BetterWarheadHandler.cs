// -----------------------------------------------------------------------
// <copyright file="BetterWarheadHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using JetBrains.Annotations;
using Mistaken.API.Diagnostics;

namespace Mistaken.API.Handlers
{
    /// <inheritdoc/>
    [PublicAPI]
    public class BetterWarheadHandler : Module
    {
        /// <summary>
        /// Gets <see cref="WarheadStatus"/> instance.
        /// </summary>
        public static WarheadStatus Warhead { get; } = new();

        /// <inheritdoc cref="Module(Exiled.API.Interfaces.IPlugin{Exiled.API.Interfaces.IConfig})"/>
        public BetterWarheadHandler(PluginHandler p)
            : base(p)
        {
        }

        /// <inheritdoc/>
        public override string Name => "BetterWarhead";

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Warhead.Starting += this.Warhead_Starting;
            Exiled.Events.Handlers.Warhead.Stopping += this.Warhead_Stopping;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += this.Player_ActivatingWarheadPanel;
            Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound;
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Server_WaitingForPlayers;
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Warhead.Starting -= this.Warhead_Starting;
            Exiled.Events.Handlers.Warhead.Stopping -= this.Warhead_Stopping;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= this.Player_ActivatingWarheadPanel;
            Exiled.Events.Handlers.Server.RestartingRound -= this.Server_RestartingRound;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.Server_WaitingForPlayers;
        }

        /// <summary>
        /// Warhead status.
        /// </summary>
        [PublicAPI]
        public class WarheadStatus
        {
            /// <inheritdoc cref="Warhead.IsInProgress"/>
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

            /// <inheritdoc cref="Warhead.DetonationTimer"/>
            public float TimeLeft => Exiled.API.Features.Warhead.DetonationTimer;

            /// <summary>
            /// Gets or sets a value indicating whether lever should be locked or not.
            /// </summary>
            public bool LeverLock { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether warhead can be stopped using button or not.
            /// </summary>
            public bool StopLock { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether warhead can be started using button or not.
            /// </summary>
            public bool StartLock { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether surface button can be unlocked or not.
            /// </summary>
            public bool ButtonLock { get; set; }

            /// <inheritdoc cref="Warhead.IsKeycardActivated"/>
            public bool ButtonOpen
            {
                get => Exiled.API.Features.Warhead.IsKeycardActivated;
                set => Exiled.API.Features.Warhead.IsKeycardActivated = value;
            }

            /// <inheritdoc cref="Warhead.IsDetonated"/>
            public bool Detonated => Exiled.API.Features.Warhead.IsDetonated;

            /// <inheritdoc cref="Warhead.LeverStatus"/>
            public bool Enabled
            {
                get => Exiled.API.Features.Warhead.LeverStatus;
                set => Exiled.API.Features.Warhead.LeverStatus = value;
            }

            /// <summary>
            /// Gets last user that used started warhead.
            /// </summary>
            public Player LastStartUser { get; internal set; }

            /// <summary>
            /// Gets last user that used stopped warhead.
            /// </summary>
            public Player LastStopUser { get; internal set; }
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

        private void Player_ActivatingWarheadPanel(Exiled.Events.EventArgs.Player.ActivatingWarheadPanelEventArgs ev)
        {
            if (Warhead.ButtonLock)
                ev.IsAllowed = false;
            if (ev.Player.IsBypassModeEnabled)
                ev.IsAllowed = true;
        }

        private void Warhead_Stopping(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
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

        private void Warhead_Starting(Exiled.Events.EventArgs.Warhead.StartingEventArgs ev)
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
