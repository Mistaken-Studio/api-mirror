// -----------------------------------------------------------------------
// <copyright file="ShieldedManager.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Exiled.API.Features;
using MEC;
using Mistaken.API.Diagnostics;

namespace Mistaken.API.Shield
{
#pragma warning disable
    /// <inheritdoc/>
    [System.Obsolete("Use script", true)]
    public class ShieldedManager
    {
        /// <summary>
        /// Adds shield.
        /// </summary>
        /// <param name="shielded">Shield to add.</param>
        public static void Add(Shielded shielded)
        {
            throw new System.Exception("Use Shield MonoBehaviour script");
        }

        /// <summary>
        /// Removes shield for player.
        /// </summary>
        /// <param name="player">Player to remove shield for.</param>
        public static void Remove(Player player)
        {
            throw new System.Exception("Use Shield MonoBehaviour script");
        }

        /// <summary>
        /// Checks if <paramref name="player"/> has shield.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns>If player has shield.</returns>
        public static bool Has(Player player)
            => throw new System.Exception("Use Shield MonoBehaviour script");

        /// <summary>
        /// Gets <paramref name="player"/>'s shield or <see langword="null"/> has none.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns>Shield.</returns>
        public static Shielded Get(Player player)
            => throw new System.Exception("Use Shield MonoBehaviour script");

        /// <summary>
        /// Tries to get <paramref name="player"/>'s shield.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <param name="result">Shield.</param>
        /// <returns>If player has shield.</returns>
        public static bool TryGet(Player player, out Shielded result)
            => throw new System.Exception("Use Shield MonoBehaviour script");
    }
}
