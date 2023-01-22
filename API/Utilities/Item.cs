using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

namespace Mistaken.API.Utilities;

public static class Item
{
    public static ItemBase GetItem(this ItemType type)
        => ReferenceHub.HostHub.inventory.CreateItemInstance(new ItemIdentifier(type, ItemSerialGenerator.GenerateNext()), false);

    public static T GetItem<T>(this ItemType type) where T : ItemBase
        => type.GetItem() as T;

    public static ItemPickupBase CreatePickup(this ItemType type, Vector3 position = default, Quaternion rotation = default, bool spawn = true)
    {
        var item = type.GetItem();
        PickupSyncInfo psi = new(type, position, rotation, item.Weight);
        ItemPickupBase itemPickupBase = Object.Instantiate(item.PickupDropModel, position, rotation);
        if (itemPickupBase is FirearmPickup pickup)
        {
            var firearm = item as Firearm;
            pickup.NetworkStatus = new(2, FirearmStatusFlags.MagazineInserted, AttachmentsUtils.GetRandomAttachmentsCode(type));
            item.OnAdded(itemPickupBase);
            pickup.NetworkStatus = new(firearm.AmmoManagerModule.MaxAmmo, firearm._status.Flags, firearm._status.Attachments);
        }

        itemPickupBase.NetworkInfo = psi;

        if (spawn)
            NetworkServer.Spawn(itemPickupBase.gameObject);

        itemPickupBase.InfoReceived(default, psi);
        return itemPickupBase;
    }

    public static T CreatePickup<T>(this ItemType type, Vector3 position = default, Quaternion rotation = default, bool spawn = true) where T : ItemPickupBase
        => type.CreatePickup(position, rotation, spawn) as T;
}
