// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

namespace Mistaken.API
{
    internal sealed class Config
    {
        public bool IsEnabled { get; set; } = true;

        [Description("If true then debug will be displayed")]
        public bool VerboseOutput { get; set; }

        /*[Description("If true then diagnostics will generate run result file (If you don't know what this is, just leave it disabled)")]
        public bool GenerateRunResultFile { get; set; } = false;

        [Description("Auto Update Settings")]
        public System.Collections.Generic.Dictionary<string, string> AutoUpdateConfig { get; set; } = new()
        {
            { "Url", "https://git.mistaken.pl/api/v4/projects/9" },
            { "Token", string.Empty },
            { "Type", "GITLAB" },
            { "VerbouseOutput", "false" },
        };*/
    }
}
