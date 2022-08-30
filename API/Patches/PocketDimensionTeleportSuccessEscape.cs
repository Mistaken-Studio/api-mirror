// -----------------------------------------------------------------------
// <copyright file="PocketDimensionTeleportSuccessEscape.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.API.Features;
using HarmonyLib;

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.SuccessEscape), typeof(ReferenceHub))]
    internal static class PocketDimensionTeleportSuccessEscape
    {
        private static int instructionCounter = 0;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var fld = AccessTools.Field(typeof(PocketDimensionTeleportSuccessEscape), nameof(PocketDimensionTeleportSuccessEscape.instructionCounter));

            yield return new CodeInstruction(OpCodes.Ldc_I4_M1);
            yield return new CodeInstruction(OpCodes.Stsfld, fld);

            int x = 0;
            foreach (var item in instructions)
            {
                yield return item;
                yield return new CodeInstruction(OpCodes.Ldc_I4, x++);
                yield return new CodeInstruction(OpCodes.Stsfld, fld);
            }

            yield break;
        }

        [HarmonyFinalizer]
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        private static Exception Finalizer(Exception __exception)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            if (__exception != null)
            {
                // An exception was thrown by the method!
                Log.Error($"Exception on instruction {instructionCounter}");
                Log.Error(__exception.Message);
                Log.Error(__exception.StackTrace);
            }

            // return null so that no Exception is thrown. You could re-throw as a different Exception as well.
            return null;
        }
    }
}
