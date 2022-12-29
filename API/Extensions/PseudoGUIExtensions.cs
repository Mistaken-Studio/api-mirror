// -----------------------------------------------------------------------
// <copyright file="PseudoGUIExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JetBrains.Annotations;
using Mistaken.API.GUI;

// ReSharper disable InconsistentNaming
namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Extensions.
    /// </summary>
    [PublicAPI]
    public static class PseudoGUIExtensions
    {
        /// <summary>
        /// Sets GUI Element.
        /// </summary>
        /// <param name="player">target.</param>
        /// <param name="key">key.</param>
        /// <param name="type">position.</param>
        /// <param name="content">content.</param>
        /// <param name="duration">duration.</param>
        public static void SetGUI(this MPlayer player, string key, PseudoGUIPosition type, string content, float duration) =>
            PseudoGUIHandler.Set(player, key, type, content, duration);

        /// <summary>
        /// Sets GUI Element.
        /// </summary>
        /// <param name="player">target.</param>
        /// <param name="key">key.</param>
        /// <param name="type">position.</param>
        /// <param name="content">content.</param>
        public static void SetGUI(this MPlayer player, string key, PseudoGUIPosition type, string content) =>
            PseudoGUIHandler.Set(player, key, type, content);
    }
}
