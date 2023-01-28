using Interactables.Interobjects.DoorUtils;

namespace Mistaken.API.Patches;

//[HarmonyPatch(typeof(DoorLockUtils), nameof(DoorLockUtils.GetMode))]
internal static class DoorLockPatch
{
    internal static bool Prefix(DoorLockReason reason, ref DoorLockMode __result)
    {
        if (reason <= DoorLockReason.Lockdown2176)
            return true;

        __result = DoorLockMode.FullLock;
        return false;
    }
}
