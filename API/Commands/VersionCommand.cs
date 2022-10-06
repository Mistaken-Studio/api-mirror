// -----------------------------------------------------------------------
// <copyright file="VersionCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using CommandSystem;
using Mistaken.API;
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

            tor.AddRange(ExperimentalHandler.GetPluginVersionsList(true));

            success = true;
            return tor.ToArray();
        }
    }
}
