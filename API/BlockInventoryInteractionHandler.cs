// -----------------------------------------------------------------------
// <copyright file="BlockInventoryInteractionHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;

namespace Mistaken.API
{
    internal class BlockInventoryInteractionHandler : Module
    {
        public BlockInventoryInteractionHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
        }

        public override string Name => nameof(BlockInventoryInteractionHandler);

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.ThrowingItem += this.Player_ThrowingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += this.Player_DroppingAmmo;
            Exiled.Events.Handlers.Player.DroppingItem += this.Player_DroppingItem;
            Exiled.Events.Handlers.Player.ChangingItem += this.Player_ChangingItem;
            Exiled.Events.Handlers.Player.UsingItem += this.Player_UsingItem;
            Exiled.Events.Handlers.Player.SearchingPickup += this.Player_SearchingPickup;
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.ThrowingItem -= this.Player_ThrowingItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= this.Player_DroppingAmmo;
            Exiled.Events.Handlers.Player.DroppingItem -= this.Player_DroppingItem;
            Exiled.Events.Handlers.Player.ChangingItem -= this.Player_ChangingItem;
            Exiled.Events.Handlers.Player.UsingItem -= this.Player_UsingItem;
            Exiled.Events.Handlers.Player.SearchingPickup -= this.Player_SearchingPickup;
        }

        private void Player_SearchingPickup(Exiled.Events.EventArgs.SearchingPickupEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private void Player_PickingUpItem(Exiled.Events.EventArgs.PickingUpItemEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private void Player_UsingItem(Exiled.Events.EventArgs.UsingItemEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private void Player_ChangingItem(Exiled.Events.EventArgs.ChangingItemEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private void Player_DroppingItem(Exiled.Events.EventArgs.DroppingItemEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private void Player_DroppingAmmo(Exiled.Events.EventArgs.DroppingAmmoEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private void Player_ThrowingItem(Exiled.Events.EventArgs.ThrowingItemEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }
    }
}
