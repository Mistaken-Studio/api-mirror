// -----------------------------------------------------------------------
// <copyright file="FixNWDumbPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading;
using Exiled.API.Features;
using HarmonyLib;

#pragma warning disable SA1118 // Parameter should not span multiple lines

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(PlayerStatsSystem.CustomReasonDamageHandler), MethodType.Constructor, typeof(string), typeof(float), typeof(string))]
    internal static class FixNWDumbPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Rent(instructions);
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(PlayerStatsSystem.CustomReasonDamageHandler.CassieAnnouncement))),
                new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(PlayerStatsSystem.CustomReasonDamageHandler), nameof(PlayerStatsSystem.CustomReasonDamageHandler._cassieAnnouncement))),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Return(newInstructions);

            yield break;
        }
    }
}
