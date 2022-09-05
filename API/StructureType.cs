// -----------------------------------------------------------------------
// <copyright file="StructureType.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej...

namespace Mistaken.API
{
    /// <summary>
    /// Structure types.
    /// </summary>
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
