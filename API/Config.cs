// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Mistaken.Updater.Config;

namespace Mistaken.API
{
    /// <inheritdoc/>
    public class Config : IAutoUpdatableConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug should be displayed.
        /// </summary>
        [Description("If true then debug will be displayed")]
        public bool VerbouseOutput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether custom slots module is enabled.
        /// </summary>
        [Description("If true then custom slots module will be enabled")]
        public bool CustomSlotsEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether run result file will be generated.
        /// </summary>
        [Description("If true then diagnostics will generate run result file (If you don't know what this is, just leave it disabled)")]
        public bool GenerateRunResultFile { get; set; } = false;

        /// <inheritdoc/>
        [Description("Auto Update Settings")]
        public System.Collections.Generic.Dictionary<string, string> AutoUpdateConfig { get; set; } = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Url", "https://git.mistaken.pl/api/v4/projects/9" },
            { "Token", string.Empty },
            { "Type", "GITLAB" },
            { "VerbouseOutput", "false" },
        };
    }
}
