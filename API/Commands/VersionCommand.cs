// -----------------------------------------------------------------------
// <copyright file="VersionCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using CommandSystem;

namespace Mistaken.API.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal class VersionCommand : IBetterCommand
    {
        public override string Command => "version";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            List<string> tor = new()
            {
                "Mistaken Studio's plugin versions list:",
            };

            tor.AddRange(Handlers.ExperimentalHandler.GetPluginVersionsList(true));

            success = true;
            return tor.ToArray();
        }
    }
}
