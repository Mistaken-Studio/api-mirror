// -----------------------------------------------------------------------
// <copyright file="GrenadeFixPatch3.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable

using Exiled.API.Features;
using Grenades;
using HarmonyLib;
using Mirror;
using Mistaken.API.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(Grenades.FragGrenade), nameof(Grenades.FragGrenade.ServersideExplosion))]
    static class GrenadeFixPatch3
    {
        internal static readonly List<GameObject> disabledGoList = new List<GameObject>();
        public static bool Prefix(FragGrenade __instance)
        {
            foreach (var item in GameObject.FindObjectsOfType(typeof(GameObject)))
            {
                var go = item as GameObject;
                if (item.name == "LookingTarget" || item.name.Contains("InRange"))
                {
                    if (!go.activeSelf)
                        continue;
                    disabledGoList.Add(go);
                    go.SetActive(false);
                }
            }
            try
            {
                Vector3 position = __instance.transform.position;
                foreach (var keyValuePair in ReferenceHub.GetAllHubs())
                {
                    var player = Player.Get(keyValuePair.Value);
                    try
                    {
                        if (!player.IsDev())
                            continue;
                    }
                    catch { continue; }
                    if (ServerConsole.FriendlyFire || !(keyValuePair.Key != __instance.thrower.gameObject) || (keyValuePair.Value.weaponManager.GetShootPermission(__instance.throwerTeam, false) && keyValuePair.Value.weaponManager.GetShootPermission(__instance.TeamWhenThrown, false)))
                    {
                        PlayerStats playerStats = keyValuePair.Value.playerStats;
                        if (playerStats != null && playerStats.ccm.InWorld)
                        {
                            float num2 = __instance.damageOverDistance.Evaluate(Vector3.Distance(position, playerStats.transform.position)) * (playerStats.ccm.IsHuman() ? GameCore.ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f) : GameCore.ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f));
                            if (num2 > __instance.absoluteDamageFalloff)
                            {
                                foreach (Transform transform in playerStats.grenadePoints)
                                {
                                    if (Physics.Linecast(position, transform.position, out var _hit, __instance.hurtLayerMask))
                                    {
                                        if (!(_hit.collider.name == "PlyCenter"))
                                            player.SendConsoleMessage($"[GRENADE] Blocked by {_hit.collider.name} ({_hit.collider.gameObject.layer})", "red");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
            return true;
        }

        public static void Postfix(FragGrenade __instance)
        {
            try
            {
                foreach (var item in disabledGoList)
                {
                    try
                    {
                        item.SetActive(true);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error("Got them inside");
                        Log.Error(ex.Message);
                        Log.Error(ex.StackTrace);
                    }
                }
            }
            catch(System.Exception ex)
            {
                Log.Error("Got them outside");
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
            disabledGoList.Clear();
        }
    }
}
