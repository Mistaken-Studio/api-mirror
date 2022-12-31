// -----------------------------------------------------------------------
// <copyright file="PreWaitingForPlayersPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HarmonyLib;

namespace Mistaken.API.Events.Patches
{
    [HarmonyPatch(typeof(Exiled.Events.Handlers.Server), nameof(Exiled.Events.Handlers.Server.OnWaitingForPlayers))]
    internal static class PreWaitingForPlayersPatch
    {
        internal static bool Prefix()
        {
            Events.OnPreWaitingForPlayers();
            return true;
        }
    }
}