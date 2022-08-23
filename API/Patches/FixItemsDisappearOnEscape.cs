// -----------------------------------------------------------------------
// <copyright file="FixItemsDisappearOnEscape.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using UnityEngine;

namespace Mistaken.API.Patches
{
    internal static class FixItemsDisappearOnEscape
    {
        public static void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            List<ItemType> itemsToDrop = new List<ItemType>();
            foreach (var item in ev.Player.Inventory.UserInventory.Items.ToArray())
            {
                if (item.Value.ItemTypeId == ItemType.SCP2176 || item.Value.ItemTypeId == ItemType.SCP018)
                {
                    ev.Player.Inventory.UserInventory.Items.Remove(item.Key);
                    itemsToDrop.Add(item.Value.ItemTypeId);
                }
            }

            MEC.Timing.RunCoroutine(DropItems(itemsToDrop, ev.Player));
        }

        private static IEnumerator<float> DropItems(List<ItemType> itemsToDrop, Player player)
        {
            yield return MEC.Timing.WaitForSeconds(0.5f);
            var position = player.Position - Vector3.up;
            foreach (var item in itemsToDrop)
            {
                Item.Create(item, player).Spawn(position);
                yield return MEC.Timing.WaitForSeconds(0.2f);
            }
        }
    }
}
