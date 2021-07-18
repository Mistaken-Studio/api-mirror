// -----------------------------------------------------------------------
// <copyright file="RoundPlus.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Mistaken.API
{
    /// <summary>
    /// Used for RoundId.
    /// </summary>
    public static class RoundPlus
    {
        /// <summary>
        /// Gets round Id.
        /// </summary>
        public static int RoundId { get; private set; } = 0;

        /// <summary>
        /// Increments Round Id.
        /// </summary>
        public static void IncRoundId() => RoundId++;
    }
}
