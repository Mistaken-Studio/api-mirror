// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HarmonyLib;
using JetBrains.Annotations;
using Mirror;
using Mistaken.API.Handlers;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using RoundRestarting;

namespace Mistaken.API
{
    internal sealed class Plugin
    {
        public static Plugin Instance { get; private set; }

        [PluginConfig]
        public Config Config;

        private static readonly Harmony _harmony = new("com.mistaken.api");

        [UsedImplicitly]
        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("Mistaken API", "1.0.0", "Mistaken API", "Mistaken Devs")]
        private void Load()
        {
            Instance = this;
            FactoryManager.RegisterPlayerFactory(this, new MPlayerFactory());
            EventManager.RegisterEvents(this);

            // Exiled.Events.Handlers.Server.WaitingForPlayers += Module.TerminateAllCoroutines;
            MEC.Timing.CallDelayed(1, () => Exiled.Events.Handlers.Server.RestartingRound += Server_RestartingRound);

            _harmony.PatchAll();
            Patches.Vars.EnableVarPatch.Patch();

            // Diagnostics.Patches.GenericInvokeSafelyPatch.PatchEvents(this.Harmony, typeof(Exiled.Events.Extensions.Event));
            Module.RegisterHandler<Handlers.BetterWarheadHandler>(this);
            Module.RegisterHandler<Handlers.CustomInfoHandler>(this);
            Module.RegisterHandler<Handlers.VanishHandler>(this);

            Module.RegisterHandler<DoorPermissionsHandler>(this);
            Module.RegisterHandler<InfiniteAmmoHandler>(this);
            Module.RegisterHandler<BlockInventoryInteractionHandler>(this);

            Module.RegisterHandler<Utilities.UtilitiesHandler>(this);

            Module.RegisterHandler<Handlers.ExperimentalHandler>(this);
        }

        [UsedImplicitly]
        [PluginUnload]
        private void Unload()
        {
            Instance = null;

            // Exiled.Events.Handlers.Server.WaitingForPlayers -= Module.TerminateAllCoroutines;
            EventManager.UnregisterEvents(this);
            _harmony.UnpatchAll();

            // Diagnostics.Patches.GenericInvokeSafelyPatch.UnpatchEvents(this.Harmony, typeof(Exiled.Events.Extensions.Event));
        }

        [UsedImplicitly]
        [PluginEvent(ServerEventType.WaitingForPlayers)]
        private void OnWaitingForPlayers()
        {
            GUI.PseudoGUIHandler.Ini();
            RoundPlus.IncRoundId();
            Utilities.Room.Reload();
        }

        [UsedImplicitly]
        [PluginEvent(ServerEventType.RoundRestart)]
        private void OnRoundRestart()
        {
            MapPlus.PostRoundCleanup();

            if (ServerStatic.StopNextRound != ServerStatic.NextRoundAction.Restart)
                return;

            NetworkServer.SendToAll(new RoundRestartMessage(RoundRestartType.FullRestart, GameCore.ConfigFile.ServerConfig.GetInt("full_restart_rejoin_time", 25), 0, true, true));
            IdleMode.PauseIdleMode = true;
            MEC.Timing.CallDelayed(1, Server.Restart);
        }

        [UsedImplicitly]
        [PluginEvent(ServerEventType.MapGenerated)]
        private void OnMapGenerated() => Extensions.DoorUtils.OnMapGenerated();
    }
}
