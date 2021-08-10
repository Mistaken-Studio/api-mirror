// -----------------------------------------------------------------------
// <copyright file="GrenadeFixOnLeavePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable

using Exiled.API.Features;
using Grenades;
using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(Grenades.Grenade), nameof(Grenades.Grenade.FullInitData))]
    static class GrenadeFixOnLeavePatch
    {
        public static bool Prefix(ref GrenadeManager player, Vector3 position, Quaternion rotation, Vector3 linearVelocity, Vector3 angularVelocity, global::Team originalTeam)
        {
            if (player == null)
                player = ReferenceHub.HostHub.GetComponent<GrenadeManager>();
            return true;
        }
    }
}
