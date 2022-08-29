// -----------------------------------------------------------------------
// <copyright file="NetworkConnectionIdentityPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HarmonyLib;
using InventorySystem.Disarming;
using InventorySystem.Items.Firearms.BasicMessages;
using Mirror;

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerShotReceived))]
    [HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
    internal static class NetworkConnectionIdentityPatch
    {
        private static bool Prefix(NetworkConnection conn)
        {
            return !conn.identity;
        }
    }
}