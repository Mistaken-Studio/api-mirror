﻿// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;
using System.Threading;
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

            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Server_WaitingForPlayers;
            Patches.GenerateCachePatch.GeneratedCache += this.GenerateCachePatch_GeneratedCache;
            MEC.Timing.CallDelayed(1, () => Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound);

            Patches.TestFixPatch.MainThread = Thread.CurrentThread;

            this.Harmony = new HarmonyLib.Harmony("com.mistaken.api");
            this.Harmony.Patch(
                typeof(Exiled.Events.Handlers.Warhead).Assembly.GetType("Exiled.Events.Handlers.Internal.MapGenerated").GetMethod("GenerateCache", BindingFlags.NonPublic | BindingFlags.Static),
                postfix: new HarmonyLib.HarmonyMethod(typeof(Patches.GenerateCachePatch), nameof(Patches.GenerateCachePatch.Postfix)));
            this.Harmony.PatchAll();
            Patches.Vars.EnableVarPatchs.Patch();
            Diagnostics.Patches.GenericInvokeSafelyPatch.PatchEvents(this.Harmony);

            Exiled.Events.Events.DisabledPatchesHashSet
                .Add(typeof(InventorySystem.Items.Firearms.BasicMessages.FirearmBasicMessagesHandler)
                .GetMethod(nameof(InventorySystem.Items.Firearms.BasicMessages.FirearmBasicMessagesHandler.ServerRequestReceived), BindingFlags.Static | BindingFlags.NonPublic));

            Exiled.Events.Events.Instance.ReloadDisabledPatches();

            new BetterWarheadHandler(this);
            new CustomInfoHandler(this);
            new VanishHandler(this);
            new DoorPermissionsHandler(this);
            new InfiniteAmmoHandler(this);
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
            Patches.GenerateCachePatch.GeneratedCache -= this.GenerateCachePatch_GeneratedCache;

            this.Harmony.UnpatchAll();
            Diagnostics.Patches.GenericInvokeSafelyPatch.UnpatchEvents(this.Harmony);

            Diagnostics.Module.OnDisable(this);

            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }

        internal HarmonyLib.Harmony Harmony { get; private set; }

        private void Server_WaitingForPlayers()
        {
            Mistaken.API.Patches.RoundStartedPatch.AlreadyStarted = false;
            GUI.PseudoGUIHandler.Ini();
            RoundPlus.IncRoundId();
            MEC.Timing.RunCoroutine(Patches.YeetConsolePatch.UpdateConsolePrint());
        }

        private void GenerateCachePatch_GeneratedCache()
            => Utilities.Room.Reload();

        private void Server_RestartingRound()
        {
            MapPlus.PostRoundCleanup();

            if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.Restart)
            {
                NetworkServer.SendToAll<RoundRestartMessage>(new RoundRestartMessage(RoundRestartType.FullRestart, (float)GameCore.ConfigFile.ServerConfig.GetInt("full_restart_rejoin_time", 25), 0, true, true));
                IdleMode.PauseIdleMode = true;
                MEC.Timing.CallDelayed(1, () => Server.Restart());
            }
        }
    }
}
