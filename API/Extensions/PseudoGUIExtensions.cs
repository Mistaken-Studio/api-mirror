// -----------------------------------------------------------------------
// <copyright file="PseudoGUIExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Mistaken.API.GUI;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Extensions.
    /// </summary>
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
        public static void SetGUI(this Player player, string key, PseudoGUIPosition type, string content, float duration) =>
            PseudoGUIHandlerComponent.Set(player, key, type, content, duration);

        /// <summary>
        /// Sets GUI Element.
        /// </summary>
        /// <param name="player">target.</param>
        /// <param name="key">key.</param>
        /// <param name="type">position.</param>
        /// <param name="content">content.</param>
        public static void SetGUI(this Player player, string key, PseudoGUIPosition type, string content) =>
            PseudoGUIHandlerComponent.Set(player, key, type, content);
    }
}
