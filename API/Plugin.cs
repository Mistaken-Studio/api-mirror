// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JetBrains.Annotations;
using Mirror;
using Mistaken.API.Handlers;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using RoundRestarting;

namespace Mistaken.API
{
    internal sealed class Plugin
    {
        internal static Plugin Instance { get; private set; }

        internal static bool VerboseOutput => Instance.Config.VerbouseOutput;

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

        [UsedImplicitly]
        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("Mistaken API", "1.0.0", "Mistaken API", "Mistaken Devs")]
        private void Load()
        {
            Instance = this;
            FactoryManager.RegisterPlayerFactory(this, new MPlayerFactory());

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
        }

        [UsedImplicitly]
        [PluginUnload]
        private void Unload()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Server_WaitingForPlayers;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Module.TerminateAllCoroutines;
            Exiled.Events.Handlers.Server.RestartingRound -= Server_RestartingRound;

            this.Harmony.UnpatchAll();
            Diagnostics.Patches.GenericInvokeSafelyPatch.UnpatchEvents(this.Harmony, typeof(Exiled.Events.Extensions.Event));

            Extensions.DoorUtils.DeIni();

            Module.OnDisable(this);
        }
    }
}
