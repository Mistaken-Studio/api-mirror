// -----------------------------------------------------------------------
// <copyright file="GrenadeFixChainingPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable

using Exiled.API.Features;
using InventorySystem.Items.ThrowableProjectiles;
using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mistaken.API.Patches
{
    /*[HarmonyPatch(typeof(Grenade), nameof(Grenades.Grenade.InitData), typeof(FragGrenade), typeof(Pickup))]
    static class GrenadeFixChainingPatch
    {
        public static bool Prefix(Grenade __instance, FragGrenade original, global::Pickup item)
        {
            if (!NetworkServer.active)
                return true;
            Transform transform = item.transform;
            Vector3 position = transform.position;
            Vector3 velocity = item.Rb.velocity;
            Vector3 linear;
            if (velocity.normalized == Vector3.zero)
                linear = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(0.1f, 0.6f), UnityEngine.Random.Range(-5f, 5f));
            else
                linear = velocity + original.chainSpeed * (position + Quaternion.LookRotation(velocity.normalized, Vector3.up) * original.chainMovement);
            __instance.FullInitData(original.thrower, position, transform.rotation, linear, item.Rb.angularVelocity, original.TeamWhenThrown);
            __instance.currentChainLength = original.currentChainLength + 1;
            return false;
        }
    }*/
}
