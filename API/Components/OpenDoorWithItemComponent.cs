// -----------------------------------------------------------------------
// <copyright file="OpenDoorWithItemComponent.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine;

namespace Mistaken.API.Components
{
    internal class OpenDoorWithItemComponent : MonoBehaviour
    {
        private ItemBase item;

        private void Awake()
        {
            this.item = this.GetComponent<ItemBase>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            RegularDoorButton regularDoorButton;
            if (!collision.collider.TryGetComponent<RegularDoorButton>(out regularDoorButton))
                return;

            DoorVariant doorVariant;
            if ((doorVariant = (DoorVariant)regularDoorButton.Target) is null || doorVariant.ActiveLocks != 0)
                return;

            if (doorVariant.RequiredPermissions.RequiredPermissions != KeycardPermissions.None)
                return;

            if (doorVariant.AllowInteracting(null, regularDoorButton.ColliderId))
            {
                doorVariant.NetworkTargetState = !doorVariant.TargetState;
                Exiled.Events.Handlers.Player.OnInteractingDoor(new Exiled.Events.EventArgs.InteractingDoorEventArgs(Player.Get(this.item.Owner), doorVariant, true));
            }
        }
    }
}
