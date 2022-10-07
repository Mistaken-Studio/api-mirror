// -----------------------------------------------------------------------
// <copyright file="VanishHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;

// ReSharper disable once CheckNamespace
namespace Mistaken.API
{
    /// <summary>
    /// Obsolete.
    /// </summary>
    [System.Obsolete("Moved to Mistaken.API.API", true)]
    public class VanishHandler
    {
        /// <summary>
        /// Gets obsolete.
        /// </summary>
        public static Dictionary<int, int> Vanished => Handlers.VanishHandler.Vanished;

        /// <summary>
        /// Obsolete.
        /// </summary>
        public static void SetGhost(Player player, bool value, byte level = 1, bool silent = false)
        {
            Handlers.VanishHandler.SetGhost(player, value, level, silent);
        }
    }
}
