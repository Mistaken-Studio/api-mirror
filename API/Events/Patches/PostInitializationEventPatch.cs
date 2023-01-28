using HarmonyLib;
using NorthwoodLib.Pools;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Mistaken.API.Events.Patches;

// [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.CheckRoot))]
internal static class PostInitializationEventPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
        newInstructions.InsertRange(0, new CodeInstruction[]
        {
            new(
                OpCodes.Call,
                AccessTools.Method(typeof(Events), nameof(Events.OnPostInitialization))),
        });

        foreach (var instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}
