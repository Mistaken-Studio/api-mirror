// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace Mistaken.API
{
    /// <inheritdoc/>
    public class PluginHandler : Plugin<Config>
    {
        /// <inheritdoc/>
        public override string Author => "Mistaken Devs";

        /// <inheritdoc/>
        public override string Name => "Mistaken API";

        /// <inheritdoc/>
        public override string Prefix => "MAPI";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Higher;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(3, 0, 3);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Server_WaitingForPlayers;
            MEC.Timing.CallDelayed(1, () => Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound);

            Patches.TestFixPatch.MainThread = Thread.CurrentThread;

            this.Harmony = new HarmonyLib.Harmony("com.mistaken.api");
            this.Harmony.PatchAll();
            Patches.Vars.EnableVarPatchs.Patch();

            new BetterWarheadHandler(this);
            new CustomInfoHandler(this);
            new VanishHandler(this);
            new DoorPermissionsHandler(this);
            new CustomSlots.CustomSlotsHandler(this);

            new Utilities.UtilitiesHandler(this);

            Extensions.DoorUtils.Ini();

            Diagnostics.Module.OnEnable(this);

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.RestartingRound -= this.Server_RestartingRound;

            this.Harmony.UnpatchAll();

            Diagnostics.Module.OnDisable(this);

            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }

        internal HarmonyLib.Harmony Harmony { get; private set; }

        private void Server_WaitingForPlayers()
        {
            GUI.PseudoGUIHandler.Ini();
            RoundPlus.IncRoundId();
        }

        private void Server_RestartingRound()
        {
            MapPlus.PostRoundCleanup();

            if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.Restart)
            {
                Server.Host.ReferenceHub.playerStats.RpcRoundrestart((float)GameCore.ConfigFile.ServerConfig.GetInt("full_restart_rejoin_time", 25), true);
                IdleMode.PauseIdleMode = true;
                MEC.Timing.CallDelayed(1, () => Server.Restart());
            }
        }
    }
}
