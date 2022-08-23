// -----------------------------------------------------------------------
// <copyright file="VersionCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using CommandSystem;
using Mistaken.API.Commands;

namespace Mistaken.CommandsExtender.Commands
{
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    internal class VersionCommand : IBetterCommand
    {
        public override string Command => "version";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            List<string> tor = new List<string>()
            {
                "Mistaken Studio's plugin versions list:",
            };

            foreach (var item in Exiled.Loader.Loader.PluginAssemblies)
            {
                var att = item.Key.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                if (att is null)
                    continue;

                if (item.Value.Author != "Mistaken Devs")
                    continue;

                tor.Add($"[{item.Value.Author}.{item.Value.Name}] {att.InformationalVersion}{(att.InformationalVersion.EndsWith("0") ? " <color=red>DEVELOPMENT BUILD</color>" : string.Empty)}");
            }

            success = true;
            return tor.ToArray();
        }
    }
}
