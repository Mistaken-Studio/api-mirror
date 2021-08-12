// -----------------------------------------------------------------------
// <copyright file="EnableVarPatchs.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

namespace Mistaken.API.Patches.Vars
{
    internal static class EnableVarPatchs
    {
        public static void Patch()
        {
            PluginHandler.Instance.Harmony.Patch(typeof(Player).GetProperty(nameof(Player.IsGodModeEnabled)).GetSetMethod(), null, new HarmonyLib.HarmonyMethod(typeof(GodModePatch).GetMethod(nameof(GodModePatch.Postfix))));
            PluginHandler.Instance.Harmony.Patch(typeof(Player).GetProperty(nameof(Player.IsBypassModeEnabled)).GetSetMethod(), null, new HarmonyLib.HarmonyMethod(typeof(BypassPatch).GetMethod(nameof(BypassPatch.Postfix))));
            PluginHandler.Instance.Harmony.Patch(typeof(ServerRoles).GetMethod(nameof(ServerRoles.SetOverwatchStatus), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance), null, new HarmonyLib.HarmonyMethod(typeof(OverwatchPatch).GetMethod(nameof(OverwatchPatch.Postfix))));
            PluginHandler.Instance.Harmony.Patch(typeof(NicknameSync).GetProperty(nameof(NicknameSync.DisplayName)).GetSetMethod(), null, new HarmonyLib.HarmonyMethod(typeof(NicknamePatch).GetMethod(nameof(NicknamePatch.Postfix))));
            PluginHandler.Instance.Harmony.Patch(typeof(ServerRoles).GetMethod(nameof(ServerRoles.CallCmdSetNoclipStatus)), null, new HarmonyLib.HarmonyMethod(typeof(NoClipPatch).GetMethod(nameof(NoClipPatch.Postfix))));
        }
    }
}
