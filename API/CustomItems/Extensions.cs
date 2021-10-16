// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Footprinting;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;

namespace Mistaken.API.CustomItems
{
    /// <summary>
    /// Custom Items Extensions.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Checks if <paramref name="item"/> is custom item made by Mistaken Devs and has it <paramref name="itemType"/>.
        /// </summary>
        /// <param name="item">Item to check.</param>
        /// <param name="itemType">Item Id.</param>
        /// <returns>Result.</returns>
        public static bool IsCustomItem(this Item item, MistakenCustomItems itemType)
        {
            if (!CustomItem.TryGet(item, out var customItem))
                return false;
            if (!(customItem is IMistakenCustomItem mistakenItem))
                return false;
            if (mistakenItem.CustomItem == itemType)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if <paramref name="pickup"/> is custom item made by Mistaken Devs and has it <paramref name="itemType"/>.
        /// </summary>
        /// <param name="pickup">Item to check.</param>
        /// <param name="itemType">Item Id.</param>
        /// <returns>Result.</returns>
        public static bool IsCustomItem(this Pickup pickup, MistakenCustomItems itemType)
        {
            if (!CustomItem.TryGet(pickup, out var customItem))
                return false;
            if (!(customItem is IMistakenCustomItem mistakenItem))
                return false;
            if (mistakenItem.CustomItem == itemType)
                return true;

            return false;
        }

        /// <inheritdoc cref="MistakenCustomItem.Get(MistakenCustomItems)"/>
        public static CustomItem Get(this MistakenCustomItems customItem)
            => MistakenCustomItem.Get(customItem);

        /// <inheritdoc cref="MistakenCustomItem.TryGet(MistakenCustomItems, out MistakenCustomItem)"/>
        public static bool TryGet(MistakenCustomItems id, out MistakenCustomItem customItem)
            => MistakenCustomItem.TryGet(id, out customItem);

        /// <inheritdoc cref="MistakenCustomItem.TrySpawn(MistakenCustomItems, Vector3, out Pickup)"/>
        public static bool TrySpawn(MistakenCustomItems id, Vector3 position, out Pickup spawned)
            => MistakenCustomItem.TrySpawn(id, position, out spawned);

        /// <inheritdoc cref="MistakenCustomItem.TryGive(Player, MistakenCustomItems, bool)"/>
        public static bool TryGive(Player player, MistakenCustomItems id, bool displayMessage = true)
            => MistakenCustomItem.TryGive(player, id, displayMessage);
    }
}
