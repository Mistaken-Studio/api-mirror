// -----------------------------------------------------------------------
// <copyright file="CustomInfoHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;

// ReSharper disable once CheckNamespace
namespace Mistaken.API
{
    /// <summary>
    /// Obsolete.
    /// </summary>
    [Obsolete("Moved to Mistaken.API.API", true)]
    public class CustomInfoHandler
    {
        /// <summary>
        /// Obsolete.
        /// </summary>
        public static void Set(Player player, string key, string value)
        {
            Handlers.CustomInfoHandler.Set(player, key, value);
        }

        /// <summary>
        /// Obsolete.
        /// </summary>
        public static void SetTargets(Player player, string key, string value, Func<Player, bool> selector)
        {
            Handlers.CustomInfoHandler.SetTargets(player, key, value, selector);
        }

        /// <summary>
        /// Obsolete.
        /// </summary>
        public static void SetTarget(Player player, string key, string value, Player target)
        {
            Handlers.CustomInfoHandler.SetTarget(player, key, value, target);
        }
    }
}
