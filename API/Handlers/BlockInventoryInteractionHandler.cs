using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables;
using Mistaken.API.Extensions;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

namespace Mistaken.API.Handlers
{
    internal sealed class BlockInventoryInteractionHandler
    {
        public BlockInventoryInteractionHandler()
        {
            EventManager.RegisterEvents(this);
        }

        ~BlockInventoryInteractionHandler()
        {
            EventManager.UnregisterEvents(this);
        }

        [PluginEvent(ServerEventType.PlayerSearchPickup)]
        private bool OnPlayerSearchPickup(Player player, ItemPickupBase pickup)
            => !player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION);

        [PluginEvent(ServerEventType.PlayerUseItem)]
        private bool OnPlayerUseItem(Player player, UsableItem item)
            => !player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION);

        [PluginEvent(ServerEventType.PlayerChangeItem)]
        private bool OnPlayerChangeItem(Player player, ushort oldItem, ushort newItem)
            => !player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION);

        [PluginEvent(ServerEventType.PlayerDropItem)]
        private bool OnPlayerDropItem(Player player, ItemBase item)
            => !player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION);

        [PluginEvent(ServerEventType.PlayerDropAmmo)]
        private bool OnPlayerDropAmmo(Player player, ItemType type, int amount)
            => !player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION);

        [PluginEvent(ServerEventType.PlayerThrowItem)]
        private bool OnPlayerThorwItem(Player player, ItemBase item, Rigidbody rigidbody)
            => !player.GetSessionVariable<bool>(SessionVarType.BLOCK_INVENTORY_INTERACTION);
    }
}
