// -----------------------------------------------------------------------
// <copyright file="PlayerPreferences.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JetBrains.Annotations;

#pragma warning disable CS1591

namespace Mistaken.API
{
    /// <summary>
    /// Player preferences enum.
    /// </summary>
    [PublicAPI]
    public enum PlayerPreferences : ulong
    {
        NONE = 0,
        DISABLE_COLORFUL_EZ_SPECTATOR_079 = 2,
        DISABLE_TRANSCRYPT = 4,
        DISABLE_FAST_ROUND_RESTART = 8,
    }
}
