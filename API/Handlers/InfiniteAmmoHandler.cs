// -----------------------------------------------------------------------
// <copyright file="InfiniteAmmoHandler.cs" company="Mistaken">
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
    internal class InfiniteAmmoHandler : Module
    {
        public InfiniteAmmoHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
        }

        public override string Name => nameof(InfiniteAmmoHandler);

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon += this.Player_ReloadingWeapon;
            Exiled.Events.Handlers.Player.DroppingAmmo += this.Player_DroppingAmmo;
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon -= this.Player_ReloadingWeapon;
            Exiled.Events.Handlers.Player.DroppingAmmo -= this.Player_DroppingAmmo;
        }

        private void Player_DroppingAmmo(Exiled.Events.EventArgs.DroppingAmmoEventArgs ev)
        {
            if (!ev.Player.TryGetSessionVariable(SessionVarType.INFINITE_AMMO, out bool hasInfiniteAmmo) || !hasInfiniteAmmo)
                return;

            ev.IsAllowed = false;
        }

        private void Player_ReloadingWeapon(Exiled.Events.EventArgs.ReloadingWeaponEventArgs ev)
        {
            if (!ev.Player.TryGetSessionVariable(SessionVarType.INFINITE_AMMO, out bool hasInfiniteAmmo) || !hasInfiniteAmmo)
                return;

            ev.Player.SetAmmo(ev.Firearm.AmmoType, (ushort)(ev.Firearm.MaxAmmo - ev.Firearm.Ammo + 1));
        }
    }
}
