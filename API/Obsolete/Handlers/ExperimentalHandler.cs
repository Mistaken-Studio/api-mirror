// -----------------------------------------------------------------------
// <copyright file="ExperimentalHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace Mistaken.API
{
    /// <summary>
    /// Obsolete.
    /// </summary>
    [System.Obsolete("Moved to Mistaken.API.API", true)]
    public class ExperimentalHandler
    {
        /// <summary>
        /// Obsolete.
        /// </summary>
        public static string[] GetPluginVersionsList()
        {
            return Handlers.ExperimentalHandler.GetPluginVersionsList();
        }
    }
}
