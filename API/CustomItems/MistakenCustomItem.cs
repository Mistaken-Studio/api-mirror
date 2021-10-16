﻿// -----------------------------------------------------------------------
// <copyright file="MistakenCustomItem.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using UnityEngine;

namespace Mistaken.API.CustomItems
{
    /// <inheritdoc/>
    public abstract class MistakenCustomItem : CustomItem, IMistakenCustomItem
    {
        /// <inheritdoc cref="CustomItem.Get(uint)"/>
        public static CustomItem Get(MistakenCustomItems customItem)
            => Get((uint)customItem) as MistakenCustomItem;

        /// <inheritdoc cref="CustomItem.TryGet(uint, out Exiled.CustomItems.API.Features.CustomItem)"/>
        public static bool TryGet(MistakenCustomItems id, out MistakenCustomItem customItem)
        {
            customItem = null;
            if (!TryGet((uint)id, out CustomItem customItem1))
                return false;
            customItem = customItem1 as MistakenCustomItem;
            return true;
        }

        /// <inheritdoc cref="CustomItem.TrySpawn(uint, Vector3, out Pickup)"/>
        public static bool TrySpawn(MistakenCustomItems id, Vector3 position, out Pickup spawned)
            => TrySpawn((uint)id, position, out spawned);

        /// <inheritdoc cref="CustomItem.TryGive(Exiled.API.Features.Player, uint, bool)"/>
        public static bool TryGive(Exiled.API.Features.Player player, MistakenCustomItems id, bool displayMessage = true)
            => TryGive(player, (uint)id, displayMessage);

        /// <inheritdoc/>
        public abstract MistakenCustomItems CustomItem { get; }

        /// <inheritdoc/>
        public override uint Id => (uint)this.CustomItem;
    }
}
