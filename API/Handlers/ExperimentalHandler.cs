// -----------------------------------------------------------------------
// <copyright file="ExperimentalHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Module = Mistaken.API.Diagnostics.Module;

namespace Mistaken.API.Handlers
{
    /// <inheritdoc/>
    [PublicAPI]
    public class ExperimentalHandler : Module
    {
        /// <summary>
        /// Gets plugin versions list.
        /// </summary>
        /// <returns>List of plugin versions.</returns>
        public static string[] GetPluginVersionsList(bool showEnableStatus = false)
        {
            List<string> tor = new();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var item in Exiled.Loader.Loader.PluginAssemblies)
            {
                var att = item.Key.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                if (att is null)
                    continue;

                if (item.Value.Author != "Mistaken Devs")
                    continue;

                tor.Add($"[{item.Value.Author}.{item.Value.Name}] {att.InformationalVersion}{(att.InformationalVersion.EndsWith("0") ? " <color=red>DEVELOPMENT BUILD</color>" : string.Empty)} {(showEnableStatus ? $"[{(item.Value.Config.IsEnabled ? "<color=green>ENABLED</color>" : "<color=red>DISABLED</color>")}]" : string.Empty)}");
            }

            return tor.ToArray();
        }

        /// <inheritdoc cref="Diagnostics.Module(Exiled.API.Interfaces.IPlugin{Exiled.API.Interfaces.IConfig})"/>
        public ExperimentalHandler(PluginHandler p)
            : base(p)
        {
        }

        /// <inheritdoc/>
        public override bool IsBasic => true;

        /// <inheritdoc/>
        public override string Name => "ExperimentalHandler";

        /// <inheritdoc/>
        public override bool Enabled => PluginHandler.VerboseOutput;

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.Server_WaitingForPlayers;
        }

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Server_WaitingForPlayers;
        }

        private bool first;

        private void Server_WaitingForPlayers()
        {
            if (this.first)
                return;

            this.first = true;

            this.Log.Debug($"Mistaken Studio's plugin versions:", PluginHandler.VerboseOutput);
            foreach (var item in GetPluginVersionsList())
                this.Log.Debug(item, PluginHandler.VerboseOutput);
        }
    }
}
