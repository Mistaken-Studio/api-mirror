// -----------------------------------------------------------------------
// <copyright file="ShootingTargetBlockSyncModePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

#pragma warning disable SA1118 // Parameter should not span multiple lines

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(AdminToys.ShootingTarget), nameof(AdminToys.ShootingTarget.ServerInteract))]
    internal static class ShootingTargetBlockSyncModePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var label = generator.DefineLabel();

            List<CodeInstruction> newInstructions = NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions[0].WithLabels(label);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_5),
                new CodeInstruction(OpCodes.Beq_S, label),
                new CodeInstruction(OpCodes.Ret),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Return(newInstructions);

            yield break;
        }
    }
}
