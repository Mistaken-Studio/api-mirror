// -----------------------------------------------------------------------
// <copyright file="NetworkConnectionIdentityPatches.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using HarmonyLib;
using InventorySystem.Disarming;
using InventorySystem.Items.Firearms.BasicMessages;
using Mirror;

#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1402 // File may only contain a single type

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerShotReceived))]
    internal static class NetworkConnectionIdentityPatches0
    {
        private static bool Prefix(NetworkConnection conn, ShotMessage msg)
        {
            return conn.identity != null;
        }
    }

    [HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
    internal static class NetworkConnectionIdentityPatches1
    {
        private static bool Prefix(NetworkConnection conn, DisarmMessage msg)
        {
            return conn.identity != null;
        }
    }
}