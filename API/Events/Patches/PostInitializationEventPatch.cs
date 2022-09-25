// -----------------------------------------------------------------------
// <copyright file="PostInitializationEventPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using NorthwoodLib.Pools;

#pragma warning disable SA1118
#pragma warning disable SA1115

namespace Mistaken.API.Events.Patches
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.CheckRoot))]
    internal static class PostInitializationEventPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new CodeInstruction(
                        OpCodes.Call,
                        AccessTools.Method(typeof(Events), nameof(Events.OnPostInitialization))),
                });

            foreach (var t in newInstructions)
                yield return t;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
