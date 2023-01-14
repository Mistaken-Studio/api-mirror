using InventorySystem.Items.Firearms;
using Mistaken.API.Extensions;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace Mistaken.API.Handlers
{
    internal sealed class InfiniteAmmoHandler
    {
        public InfiniteAmmoHandler()
        {
            EventManager.RegisterEvents(this);
        }

        ~InfiniteAmmoHandler()
        {
            EventManager.UnregisterEvents(this);
        }

        [PluginEvent(ServerEventType.PlayerDropAmmo)]
        private bool Player_DroppingAmmo(Player player, ItemType type, int amount)
        {
            if (!player.TryGetSessionVariable(SessionVarType.INFINITE_AMMO, out bool hasInfiniteAmmo))
                return true;

            return !hasInfiniteAmmo;
        }

        [PluginEvent(ServerEventType.PlayerReloadWeapon)]
        private void Player_ReloadingWeapon(Player player, Firearm firearm)
        {
            if (!player.TryGetSessionVariable(SessionVarType.INFINITE_AMMO, out bool hasInfiniteAmmo) || !hasInfiniteAmmo)
                return;

            player.SetAmmo(firearm.AmmoType, (ushort)(firearm.AmmoManagerModule.MaxAmmo - firearm.Status.Ammo + 1));
        }
    }
}
