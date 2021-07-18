// -----------------------------------------------------------------------
// <copyright file="IPermissionLocked.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Mistaken.API.Commands
{
    /// <summary>
    /// Interface used to mark that command requires permissions.
    /// </summary>
    public interface IPermissionLocked
    {
        /// <summary>
        /// Gets permission.
        /// </summary>
        string Permission { get; }

        /// <summary>
        /// Gets plugin name, used as prefix.
        /// </summary>
        string PluginName { get; }
    }
}
