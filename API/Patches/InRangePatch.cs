using HarmonyLib;
using Scp914;
using System.Linq;
using UnityEngine;


namespace Mistaken.API.Patches;

// [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.Upgrade))]
internal static class InRangePatch
{
    internal static void Prefix(ref Collider[] intake, Vector3 moveVector, Scp914Mode mode, Scp914KnobSetting setting)
    {
        intake = intake.Where(x => x.GetComponent<Components.InRange>() == null && x.GetComponent<Components.InRangeBall>() == null).ToArray();
    }
}
