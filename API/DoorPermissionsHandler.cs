// -----------------------------------------------------------------------
// <copyright file="DoorPermissionsHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Enums;
using Exiled.API.Interfaces;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;

namespace Mistaken.API
{
    internal class DoorPermissionsHandler : Module
    {
        public DoorPermissionsHandler(IPlugin<IConfig> plugin)
            : base(plugin)
        {
        }

        public override string Name => nameof(DoorPermissionsHandler);

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += this.Handle<Exiled.Events.EventArgs.InteractingDoorEventArgs>((ev) => this.Player_InteractingDoor(ev));
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= this.Handle<Exiled.Events.EventArgs.InteractingDoorEventArgs>((ev) => this.Player_InteractingDoor(ev));
        }

        private void Player_InteractingDoor(Exiled.Events.EventArgs.InteractingDoorEventArgs ev)
        {
            if (ev.IsAllowed) return;
            if (ev.Door.IsLocked) return;
            var value = ev.Player.GetSessionVar<KeycardPermissions>(SessionVarType.BUILTIN_DOOR_ACCESS);
            if ((value & (KeycardPermissions)ev.Door.RequiredPermissions.RequiredPermissions) != 0)
                ev.IsAllowed = true;
        }
    }
}
