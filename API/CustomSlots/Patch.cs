// -----------------------------------------------------------------------
// <copyright file="Patch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable

using HarmonyLib;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Mistaken.API.CustomSlots
{
    // [HarmonyPatch(typeof(ReservedSlot), "HasReservedSlot", new Type[] { typeof(string) })]
    internal static class ReservedSlotPatch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Ret);

            yield break;
        }
    }
}
