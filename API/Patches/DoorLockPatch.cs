// -----------------------------------------------------------------------
// <copyright file="DoorLockPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HarmonyLib;
using Interactables.Interobjects.DoorUtils;

namespace Mistaken.API.Patches
{
    /// <summary>
    /// Patch used to allow for custom door locks.
    /// </summary>
    /*[HarmonyPatch(typeof(DoorLockUtils), "GetMode")]
    public static class DoorLockPatch
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        private static bool Prefix(DoorLockReason reason, ref DoorLockMode __result)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            if ((ushort)reason > 256)
            {
                __result = DoorLockMode.FullLock;
                return false;
            }

            return true;
        }
    }*/
}
