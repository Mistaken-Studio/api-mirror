// -----------------------------------------------------------------------
// <copyright file="PlayerHasHintPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using HarmonyLib;
using Hints;
using MEC;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
    internal static class PlayerHasHintPatch
    {
        private static readonly Dictionary<Player, CoroutineHandle> PlayerHasHintCoroutines = new Dictionary<Player, CoroutineHandle>();

        private static void Postfix(HintDisplay __instance, Hint hint)
        {
            if (__instance == null || __instance.gameObject is null || !(Player.Get(__instance.gameObject) is Player player))
                return;

            if (PlayerHasHintCoroutines.TryGetValue(player, out CoroutineHandle oldcoroutine))
                Timing.KillCoroutines(oldcoroutine);

            PlayerHasHintCoroutines[player] = Timing.RunCoroutine(HasHintToFalse(player, hint.DurationScalar));

            if (!player.HasHint)
                player.HasHint = true;
        }

        private static IEnumerator<float> HasHintToFalse(Player player, float duration)
        {
            yield return Timing.WaitForSeconds(duration);

            if (player.GameObject is null)
                yield break;

            player.HasHint = false;
            PlayerHasHintCoroutines.Remove(player);
        }
    }
}
