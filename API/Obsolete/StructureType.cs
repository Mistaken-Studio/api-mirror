// -----------------------------------------------------------------------
// <copyright file="StructureType.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
#pragma warning disable

// ReSharper disable once CheckNamespace
namespace Mistaken.API
{
    /// <summary>
    /// Structure types.
    /// </summary>
    [System.Obsolete("Moved to Mistaken.API.API", true)]
    public enum StructureType : byte
    {
        Scp018Pedestal,
        Scp207Pedestal,
        Scp244Pedestal,
        Scp268Pedestal,
        Scp500Pedestal,
        Scp1853Pedestal,
        Scp2176Pedestal,
        RegularMedkitLocker,
        AdrenalineMedkitLocker,
        RifleRackLocker,
        MiscLocker,
        LargeGunLocker,
        Generator,
        Workstation,
    }
}
