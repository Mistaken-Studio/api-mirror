// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Exiled.API.Interfaces;

namespace Mistaken.API
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        [Description("If true then debug will be displayed")]
        public bool Debug { get; set; }

        [Description("If true then diagnostics will generate run result file (If you don't know what this is, just leave it disabled)")]
        public bool GenerateRunResultFile { get; set; } = false;
    }
}
