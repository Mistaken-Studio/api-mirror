// -----------------------------------------------------------------------
// <copyright file="KeycardProcessCollisionPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

namespace Mistaken.API.Patches
{
    /// <summary>
    /// Patch for making thrown keycard trigger InteractingDoor event and so if the keycard hasn't got enough permission to open the door it will be denied on the button.
    /// </summary>
    [HarmonyPatch(typeof(KeycardPickup), nameof(KeycardPickup.ProcessCollision), new Type[] { typeof(Collision) })]
    public static class KeycardProcessCollisionPatch
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        private static bool Prefix(KeycardPickup __instance, Collision collision)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            float sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
            if (NetworkServer.active)
            {
                float num = __instance.Info.Weight * sqrMagnitude / 2f;
                if (num > 30f)
                {
                    float damage = num * 0.25f;
                    BreakableWindow breakableWindow;
                    if (collision.collider.TryGetComponent<BreakableWindow>(out breakableWindow))
                    {
                        breakableWindow.Damage(damage, null, __instance.PreviousOwner, Vector3.zero);
                    }
                }
            }

            __instance.MakeCollisionSound(sqrMagnitude);
            RegularDoorButton regularDoorButton;
            if (!collision.collider.TryGetComponent<RegularDoorButton>(out regularDoorButton))
            {
                return false;
            }

            DoorVariant doorVariant;
            if ((doorVariant = (DoorVariant)regularDoorButton.Target) == null || doorVariant.ActiveLocks != 0 || doorVariant.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
            {
                return false;
            }

            ItemBase item;
            if (InventoryItemLoader.AvailableItems.TryGetValue(__instance.Info.ItemId, out item))
            {
                if (!doorVariant.RequiredPermissions.CheckPermissions(item, null))
                {
                    doorVariant.PermissionsDenied(null, regularDoorButton.ColliderId);
                    Exiled.Events.Handlers.Player.OnInteractingDoor(new Exiled.Events.EventArgs.InteractingDoorEventArgs(Player.Get(__instance.PreviousOwner.Hub), doorVariant, false));
                }
                else
                {
                    if (doorVariant.AllowInteracting(null, regularDoorButton.ColliderId))
                    {
                        doorVariant.NetworkTargetState = !doorVariant.TargetState;
                        Exiled.Events.Handlers.Player.OnInteractingDoor(new Exiled.Events.EventArgs.InteractingDoorEventArgs(Player.Get(__instance.PreviousOwner.Hub), doorVariant, true));
                        return false;
                    }
                }
            }

            return false;
        }
    }
}
