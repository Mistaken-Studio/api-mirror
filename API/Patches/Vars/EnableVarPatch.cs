/*using HarmonyLib;

namespace Mistaken.API.Patches.Vars
{
    internal static class EnableVarPatch
    {
        public static void Patch()
        {
            // GodMode
            var patch = new HarmonyMethod(AccessTools.Method(typeof(GodModePatch), nameof(GodModePatch.Postfix)));
            var method = AccessTools.PropertySetter(typeof(Player), nameof(Player.IsGodModeEnabled));
            PluginHandler.Instance.Harmony.Patch(method, null, patch);

            // Bypass
            patch = new HarmonyMethod(AccessTools.Method(typeof(BypassPatch), nameof(BypassPatch.Postfix)));
            method = AccessTools.PropertySetter(typeof(Player), nameof(Player.IsBypassModeEnabled));
            PluginHandler.Instance.Harmony.Patch(method, null, patch);

            // Overwatch
            patch = new HarmonyMethod(AccessTools.Method(typeof(OverwatchPatch), nameof(OverwatchPatch.Postfix)));
            method = AccessTools.Method(
                typeof(ServerRoles),
                nameof(ServerRoles.SetOverwatchStatus),
                new[] { typeof(bool) });
            PluginHandler.Instance.Harmony.Patch(method, null, patch);

            // Nickname
            patch = new HarmonyMethod(AccessTools.Method(typeof(NicknamePatch), nameof(NicknamePatch.Postfix)));
            method = AccessTools.PropertySetter(typeof(NicknameSync), nameof(NicknameSync.DisplayName));
            PluginHandler.Instance.Harmony.Patch(method, null, patch);

            // NoClip
            patch = new HarmonyMethod(AccessTools.Method(typeof(NoClipPatch), nameof(NoClipPatch.Postfix)));
            method = AccessTools.Method(typeof(ServerRoles), nameof(ServerRoles.UserCode_CmdSetNoclipStatus));
            PluginHandler.Instance.Harmony.Patch(method, null, patch);
        }
    }
}
*/