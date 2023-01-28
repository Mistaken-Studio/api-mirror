using HarmonyLib;
using Mistaken.API.EventArgs;
using PlayerRoles.Ragdolls;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Mistaken.API.Events.Patches;

[HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
internal static class RagdollSpawningPatches
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Rent(instructions);

        int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + 1;

        var label = newInstructions[index].ExtractLabels()[0];
        var continueLabel = generator.DefineLabel();
        newInstructions[index].WithLabels(continueLabel);

        newInstructions.InsertRange(index, new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0).WithLabels(label),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldc_I4_1),
            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(RagdollSpawningEventArgs))[0]),
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Ragdoll), nameof(Ragdoll.RagdollSpawning))),
            new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(RagdollSpawningEventArgs), nameof(RagdollSpawningEventArgs.IsAllowed))),
            new CodeInstruction(OpCodes.Brtrue_S, continueLabel),
            new CodeInstruction(OpCodes.Ret),
        });

        index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ret) - 1;

        newInstructions.InsertRange(index, new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(RagdollSpawnedEventArgs))[0]),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Ragdoll), nameof(Ragdoll.RagdollSpawn))),
        });

        foreach (var instruction in newInstructions)
            yield return instruction;

        NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}
