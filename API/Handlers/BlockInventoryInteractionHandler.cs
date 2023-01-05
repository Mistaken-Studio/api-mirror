// -----------------------------------------------------------------------
// <copyright file="BlockInventoryInteractionHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;
using JetBrains.Annotations;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;

namespace Mistaken.API.Handlers
{
    [UsedImplicitly]
    internal sealed class BlockInventoryInteractionHandler : Module
    {
        public BlockInventoryInteractionHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
        }

        public override string Name => nameof(BlockInventoryInteractionHandler);

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.ThrowingRequest += Player_ThrowingRequest;
            Exiled.Events.Handlers.Player.DroppingAmmo += Player_DroppingAmmo;
            Exiled.Events.Handlers.Player.DroppingItem += Player_DroppingItem;
            Exiled.Events.Handlers.Player.ChangingItem += Player_ChangingItem;
            Exiled.Events.Handlers.Player.UsingItem += Player_UsingItem;
            Exiled.Events.Handlers.Player.SearchingPickup += Player_SearchingPickup;
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.ThrowingRequest -= Player_ThrowingRequest;
            Exiled.Events.Handlers.Player.DroppingAmmo -= Player_DroppingAmmo;
            Exiled.Events.Handlers.Player.DroppingItem -= Player_DroppingItem;
            Exiled.Events.Handlers.Player.ChangingItem -= Player_ChangingItem;
            Exiled.Events.Handlers.Player.UsingItem -= Player_UsingItem;
            Exiled.Events.Handlers.Player.SearchingPickup -= Player_SearchingPickup;
        }

        private static void Player_SearchingPickup(Exiled.Events.EventArgs.Player.SearchingPickupEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private static void Player_UsingItem(Exiled.Events.EventArgs.Player.UsingItemEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private static void Player_ChangingItem(Exiled.Events.EventArgs.Player.ChangingItemEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private static void Player_DroppingItem(Exiled.Events.EventArgs.Player.DroppingItemEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private static void Player_DroppingAmmo(Exiled.Events.EventArgs.Player.DroppingAmmoEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }

        private void Player_ThrowingRequest(Exiled.Events.EventArgs.Player.ThrowingRequestEventArgs ev)
        {
            if (ev.Player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION))
                ev.IsAllowed = false;
        }
    }
}
