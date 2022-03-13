// -----------------------------------------------------------------------
// <copyright file="FixUnloadingEvent.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs;
using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using NorthwoodLib.Pools;

#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1117 // Parameters should be on same line or separate lines

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerRequestReceived))]
    internal static class FixUnloadingEvent
    {
        // Token: 0x060000A1 RID: 161 RVA: 0x00004112 File Offset: 0x00002312
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            LocalBuilder localBuilder = generator.DeclareLocal(typeof(TogglingWeaponFlashlightEventArgs));
            int num = -2;
            int index = newInstructions.FindIndex((CodeInstruction instruction) => instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == AccessTools.Method(typeof(IAmmoManagerModule), "ServerTryReload", null, null)) + num;
            Label label = generator.DefineLabel();
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_0, null).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.API.Features.Player), "Get", new Type[]
                {
                    typeof(ReferenceHub),
                }, null)),
                new CodeInstruction(OpCodes.Ldc_I4_1, null),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(ReloadingWeaponEventArgs), null)[0]),
                new CodeInstruction(OpCodes.Dup, null),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.Events.Handlers.Player), "OnReloadingWeapon", null, null)),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ReloadingWeaponEventArgs), "IsAllowed")),
                new CodeInstruction(OpCodes.Brfalse, label),
            });
            num = -2; // 2; Changed to prevent ServerTryUnload from beeing called when IsAllowed is false
            index = newInstructions.FindIndex((CodeInstruction instruction) => instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == AccessTools.Method(typeof(IAmmoManagerModule), "ServerTryUnload", null, null)) + num;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_0, null).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.API.Features.Player), "Get", new Type[]
                {
                    typeof(ReferenceHub),
                }, null)),
                new CodeInstruction(OpCodes.Ldc_I4_1, null),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(UnloadingWeaponEventArgs), null)[0]),
                new CodeInstruction(OpCodes.Dup, null),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.Events.Handlers.Player), "OnUnloadingWeapon", null, null)),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnloadingWeaponEventArgs), "IsAllowed")),
                new CodeInstruction(OpCodes.Brfalse, label),
            });
            num = -2;
            index = newInstructions.FindIndex((CodeInstruction instruction) => instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand == AccessTools.Method(typeof(IActionModule), "ServerAuthorizeDryFire", null, null)) + num;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_0, null).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.API.Features.Player), "Get", new Type[]
                {
                    typeof(ReferenceHub),
                }, null)),
                new CodeInstruction(OpCodes.Ldc_I4_1, null),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(DryfiringWeaponEventArgs), null)[0]),
                new CodeInstruction(OpCodes.Dup, null),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.Events.Handlers.Player), "OnDryfiringWeapon", null, null)),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(DryfiringWeaponEventArgs), "IsAllowed")),
                new CodeInstruction(OpCodes.Brfalse, label),
            });
            num = 2;
            index = newInstructions.FindIndex((CodeInstruction instruction) => instruction.opcode == OpCodes.Pop) + num;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_0, null).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.API.Features.Player), "Get", new Type[]
                {
                    typeof(ReferenceHub),
                }, null)),
                new CodeInstruction(OpCodes.Ldc_I4_1, null),
                new CodeInstruction(OpCodes.Ldc_I4_0, null),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(AimingDownSightEventArgs), null)[0]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.Events.Handlers.Player), "OnAimingDownSight", null, null)),
            });
            num = -7;
            index = newInstructions.FindIndex((CodeInstruction instruction) => instruction.opcode == OpCodes.Ldfld && (FieldInfo)instruction.operand == AccessTools.Field(typeof(FirearmStatus), "Flags")) + num;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_0, null).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.API.Features.Player), "Get", new Type[]
                {
                    typeof(ReferenceHub),
                }, null)),
                new CodeInstruction(OpCodes.Ldc_I4_0, null),
                new CodeInstruction(OpCodes.Ldc_I4_1, null),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(AimingDownSightEventArgs), null)[0]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.Events.Handlers.Player), "OnAimingDownSight", null, null)),
            });
            num = -6;
            index = newInstructions.FindLastIndex((CodeInstruction instruction) => instruction.opcode == OpCodes.Call) + num;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_0, null).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.API.Features.Player), "Get", new Type[]
                {
                    typeof(ReferenceHub),
                }, null)),
                new CodeInstruction(OpCodes.Ldloc_S, 7),
                new CodeInstruction(OpCodes.Ldc_I4_1, null),
                new CodeInstruction(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(TogglingWeaponFlashlightEventArgs), null)[0]),
                new CodeInstruction(OpCodes.Dup, null),
                new CodeInstruction(OpCodes.Dup, null),
                new CodeInstruction(OpCodes.Stloc_S, localBuilder.LocalIndex),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.Events.Handlers.Player), "OnTogglingWeaponFlashlight", null, null)),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(TogglingWeaponFlashlightEventArgs), "IsAllowed")),
                new CodeInstruction(OpCodes.Brfalse_S, label),
                new CodeInstruction(OpCodes.Ldloc_S, localBuilder.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(TogglingWeaponFlashlightEventArgs), "NewState")),
                new CodeInstruction(OpCodes.Stloc_S, 7),
            });
            newInstructions[newInstructions.Count - 1].WithLabels(new Label[]
            {
                label,
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
            yield break;
        }
    }
}
