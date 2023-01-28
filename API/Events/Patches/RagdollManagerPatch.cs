using HarmonyLib;
using PlayerRoles.Ragdolls;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Mistaken.API.Events.Patches;

[HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.OnSpawnedRagdoll))]
internal static class OnSpawnedRagdollPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Rent(instructions);

        newInstructions.InsertRange(0, new[]
        {
            new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Utilities.Map), nameof(Utilities.Map.ragdolls))),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(List<BasicRagdoll>), nameof(List<BasicRagdoll>.Add))),
        });

        foreach (var instruction in newInstructions)
            yield return instruction;

        NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}

[HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.OnRemovedRagdoll))]
internal static class OnRemovedRagdollPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Rent(instructions);

        newInstructions.InsertRange(0, new[]
        {
            new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Utilities.Map), nameof(Utilities.Map.ragdolls))),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(List<BasicRagdoll>), nameof(List<BasicRagdoll>.Remove))),
            new CodeInstruction(OpCodes.Pop),
        });

        foreach (var instruction in newInstructions)
            yield return instruction;

        NorthwoodLib.Pools.ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}
