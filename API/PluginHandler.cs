// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Enums;
using Exiled.API.Features;
using Mirror;
using Mistaken.API.Diagnostics;
using Mistaken.API.Handlers;
using Mistaken.Updater.API.Config;
using RoundRestarting;

namespace Mistaken.API
{
    /// <inheritdoc/>
    public sealed class PluginHandler : Plugin<Config>, IAutoUpdateablePlugin
    {
        /// <inheritdoc/>
        public override string Author => "Mistaken Devs";

        /// <inheritdoc/>
        public override string Name => "Mistaken API";

        /// <inheritdoc/>
        public override string Prefix => "MAPI";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Highest - 1;

        /// <inheritdoc/>
        public override System.Version RequiredExiledVersion => new(5, 0, 0);

        public AutoUpdateConfig AutoUpdateConfig => new()
        {
            Url = "https://git.mistaken.pl/api/v4/projects/9",
            Type = SourceType.GITLAB,
        };

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Module.TerminateAllCoroutines;
            MEC.Timing.CallDelayed(1, () => Exiled.Events.Handlers.Server.RestartingRound += Server_RestartingRound);

            this.Harmony = new("com.mistaken.api");
            this.Harmony.PatchAll();
            Patches.Vars.EnableVarPatch.Patch();
            Diagnostics.Patches.GenericInvokeSafelyPatch.PatchEvents(this.Harmony, typeof(Exiled.Events.Extensions.Event));

            Module.RegisterHandler<Handlers.BetterWarheadHandler>(this);
            Module.RegisterHandler<Handlers.CustomInfoHandler>(this);
            Module.RegisterHandler<Handlers.VanishHandler>(this);

            Module.RegisterHandler<DoorPermissionsHandler>(this);
            Module.RegisterHandler<InfiniteAmmoHandler>(this);
            Module.RegisterHandler<BlockInventoryInteractionHandler>(this);

            Module.RegisterHandler<Utilities.UtilitiesHandler>(this);

            Module.RegisterHandler<Handlers.ExperimentalHandler>(this);

            Extensions.DoorUtils.Ini();

            Module.OnEnable(this);

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Module.TerminateAllCoroutines;
            Exiled.Events.Handlers.Server.RestartingRound -= Server_RestartingRound;

            this.Harmony.UnpatchAll();
            Diagnostics.Patches.GenericInvokeSafelyPatch.UnpatchEvents(this.Harmony, typeof(Exiled.Events.Extensions.Event));

            Extensions.DoorUtils.DeIni();

            Module.OnDisable(this);

            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }

        internal HarmonyLib.Harmony Harmony { get; private set; }

        private static void Server_WaitingForPlayers()
        {
            GUI.PseudoGUIHandler.Ini();
            RoundPlus.IncRoundId();
            Utilities.Room.Reload();
        }

        private static void Server_RestartingRound()
        {
            // MapPlus.PostRoundCleanup();

            if (ServerStatic.StopNextRound != ServerStatic.NextRoundAction.Restart)
                return;

            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.FullRestart, GameCore.ConfigFile.ServerConfig.GetInt("full_restart_rejoin_time", 25), 0, true, true));
            IdleMode.PauseIdleMode = true;
            MEC.Timing.CallDelayed(1, Server.Restart);
        }
    }
}
