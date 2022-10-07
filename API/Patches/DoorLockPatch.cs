// -----------------------------------------------------------------------
// <copyright file="DoorLockPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mistaken.API.Patches
{
    /// <summary>
    /// Patch used to allow for custom door locks.
    /// </summary>
    [UsedImplicitly]
    [HarmonyPatch(typeof(DoorLockUtils), nameof(DoorLockUtils.GetMode))]
    public static class DoorLockPatch
    {
        internal static bool Prefix(DoorLockReason reason, ref DoorLockMode __result)
        {
            if (reason <= DoorLockReason.Lockdown2176)
                return true;

            __result = DoorLockMode.FullLock;
            return false;
        }
    }
}
