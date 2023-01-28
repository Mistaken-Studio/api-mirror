﻿using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Mistaken.API.Patches
{
    // [HarmonyPatch(typeof(AdminToys.ShootingTarget), nameof(AdminToys.ShootingTarget.ServerInteract))]
    internal static class ShootingTargetBlockSyncModePatch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var label = generator.DefineLabel();

            List<CodeInstruction> newInstructions = NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions[0].WithLabels(label);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldc_I4_5),
                new(OpCodes.Beq_S, label),
                new(OpCodes.Ret),
            });

            foreach (var t in newInstructions)
                yield return t;

            NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
