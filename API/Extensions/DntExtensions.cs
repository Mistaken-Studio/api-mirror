// -----------------------------------------------------------------------
// <copyright file="DntExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// DNT Extensions.
    /// </summary>
    [System.Obsolete("DONT", true)]
    public static class DntExtensions
    {
        /// <summary>
        /// If player has DNT and if it should be effective.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <returns>if has DNT.</returns>
        [Obsolete("DONT", true)]
        public static bool IsDNT(this Player me)
        {
            Log.Warn("Do not use this method, use getter");
            Log.Warn(Environment.StackTrace);
            return me.DoNotTrack;
        }
    }
}
