// -----------------------------------------------------------------------
// <copyright file="PlayerPreferences.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace Mistaken.API
{
    /// <summary>
    /// Player preferences enum.
    /// </summary>
    [System.Obsolete("Moved to Mistaken.API.API", true)]
    public enum PlayerPreferences : ulong
    {
#pragma warning disable CS1591
        NONE = 0,
        DISABLE_COLORFUL_EZ_SPECTATOR_079 = 2,
        DISABLE_TRANSCRYPT = 4,
        DISABLE_FAST_ROUND_RESTART = 8,
#pragma warning restore CS1591
    }
}
