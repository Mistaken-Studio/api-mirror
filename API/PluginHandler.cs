// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Enums;

namespace Mistaken.API
{
    /// <inheritdoc/>
    public class PluginHandler : AutoUpdatablePlugin<Config>
    {
        /// <inheritdoc/>
        public override string Author => "Mistaken Devs";

        /// <inheritdoc/>
        public override string Name => "MistakenAPI";

        /// <inheritdoc/>
        public override string Prefix => "MAPI";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Higher;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(2, 11, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Server_WaitingForPlayers;

            this.harmony = new HarmonyLib.Harmony("com.mistaken.api");
            this.harmony.PatchAll();

            new BetterWarheadHandler(this);
            new CustomInfoHandler(this);

            API.Diagnostics.Module.OnEnable(this);

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.Server_WaitingForPlayers;

            this.harmony.UnpatchAll();

            API.Diagnostics.Module.OnDisable(this);

            base.OnDisabled();
        }

        private HarmonyLib.Harmony harmony;

        private void Server_WaitingForPlayers()
        {
            GUI.PseudoGUIHandlerComponent.Ini();
        }
    }
}
