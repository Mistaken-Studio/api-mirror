// -----------------------------------------------------------------------
// <copyright file="Patch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable

using HarmonyLib;
using System;

namespace Mistaken.API.CustomSlots
{
    [HarmonyPatch(typeof(ReservedSlot), "HasReservedSlot", new Type[] { typeof(string) })]
    internal static class ReservedSlotPatch
    {
        internal static bool Prefix(ref bool __result, string userId)
        {
            __result = true;
            return false;
        }
    }
}
