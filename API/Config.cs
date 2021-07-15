// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Mistaken.API
{
    /// <inheritdoc/>
    public class Config : IAutoUpdatableConfig
    {
        /// <inheritdoc/>
        public bool AutoUpdateVerbouseOutput { get; set; }

        /// <inheritdoc/>
        public string AutoUpdateUrl { get; set; }

        /// <inheritdoc/>
        public AutoUpdateType AutoUpdateType { get; set; }

        /// <inheritdoc/>
        public string AutoUpdateLogin { get; set; }

        /// <inheritdoc/>
        public string AutoUpdateToken { get; set; }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;
    }
}
