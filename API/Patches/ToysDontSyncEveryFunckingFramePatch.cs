// -----------------------------------------------------------------------
// <copyright file="ToysDontSyncEveryFunckingFramePatch.cs" company="Mistaken">
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
    [HarmonyPatch(typeof(AdminToys.AdminToyBase), nameof(AdminToys.AdminToyBase.LateUpdate))]
    internal static class ToysDontSyncEveryFunckingFramePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var label = generator.DefineLabel();

            List<CodeInstruction> newInstructions = NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions[0].WithLabels(label);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Extensions.Extensions), nameof(Extensions.Extensions.SyncToyPosition))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(HashSet<AdminToys.AdminToyBase>), nameof(HashSet<AdminToys.AdminToyBase>.Contains), parameters: new Type[] { typeof(AdminToys.AdminToyBase) })),
                new CodeInstruction(OpCodes.Brtrue_S, label),
                new CodeInstruction(OpCodes.Ret),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Return(newInstructions);

            yield break;
        }
    }
}
