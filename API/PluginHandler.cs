// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Enums;
using Exiled.API.Features;
using Mirror;
using RoundRestarting;

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
        public override System.Version RequiredExiledVersion => new System.Version(5, 0, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Diagnostics.Module.TerminateAllCoroutines;
            MEC.Timing.CallDelayed(1, () => Exiled.Events.Handlers.Server.RestartingRound += Server_RestartingRound);

            this.Harmony = new HarmonyLib.Harmony("com.mistaken.api");
            this.Harmony.PatchAll();
            Patches.Vars.EnableVarPatchs.Patch();
            Diagnostics.Patches.GenericInvokeSafelyPatch.PatchEvents(this.Harmony, typeof(Exiled.Events.Extensions.Event));

            new BetterWarheadHandler(this);
            new CustomInfoHandler(this);
            new VanishHandler(this);
            new DoorPermissionsHandler(this);
            new InfiniteAmmoHandler(this);
            new BlockInventoryInteractionHandler(this);

            new Utilities.UtilitiesHandler(this);

            new ExperimentalHandler(this);

            new Toys.ToyHandler(this);

            Extensions.DoorUtils.Ini();

            Diagnostics.Module.OnEnable(this);

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Diagnostics.Module.TerminateAllCoroutines;
            Exiled.Events.Handlers.Server.RestartingRound -= Server_RestartingRound;

            this.Harmony.UnpatchAll();
            Diagnostics.Patches.GenericInvokeSafelyPatch.UnpatchEvents(this.Harmony, typeof(Exiled.Events.Extensions.Event));

            Extensions.DoorUtils.DeIni();

            Diagnostics.Module.OnDisable(this);

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
            MapPlus.PostRoundCleanup();

            if (ServerStatic.StopNextRound != ServerStatic.NextRoundAction.Restart)
                return;

            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.FullRestart, GameCore.ConfigFile.ServerConfig.GetInt("full_restart_rejoin_time", 25), 0, true, true));
            IdleMode.PauseIdleMode = true;
            MEC.Timing.CallDelayed(1, Server.Restart);
        }
    }
}
