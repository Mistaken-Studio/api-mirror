/*using Mistaken.API.Extensions;

namespace Mistaken.API.Handlers
{
    internal sealed class DoorPermissionsHandler
    {
        public DoorPermissionsHandler()
        {
        }

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += this.Player_InteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker += this.Player_InteractingLocker;
            Exiled.Events.Handlers.Player.UnlockingGenerator += this.Player_UnlockingGenerator;
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= this.Player_InteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker -= this.Player_InteractingLocker;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= this.Player_UnlockingGenerator;
        }

        private void Player_UnlockingGenerator(Exiled.Events.EventArgs.UnlockingGeneratorEventArgs ev)
        {
            if (ev.IsAllowed)
                return;

            if (ev.Player.IsCuffed)
                return;

            if (!ev.Player.TryGetSessionVariable<KeycardPermissions>(SessionVarType.BUILTIN_DOOR_ACCESS, out var value))
                return;

            var basePermissions = (ev.Player.CurrentItem as Keycard)?.Permissions ?? KeycardPermissions.None;

            if ((basePermissions & ev.Generator.KeycardPermissions) != 0)
                return; // Keycard gives access, but still ev.IsAllowed = false, so maybe plugin denied it?

            if (((value | basePermissions) & ev.Generator.KeycardPermissions) != 0)
                ev.IsAllowed = true;
        }

        private void Player_InteractingLocker(Exiled.Events.EventArgs.InteractingLockerEventArgs ev)
        {
            if (ev.IsAllowed)
                return;

            if (ev.Player.IsCuffed)
                return;

            if (!ev.Player.TryGetSessionVariable<KeycardPermissions>(SessionVarType.BUILTIN_DOOR_ACCESS, out var value))
                return;

            var basePermissions = (ev.Player.CurrentItem as Keycard)?.Permissions ?? KeycardPermissions.None;

            if ((basePermissions & (KeycardPermissions)ev.Chamber.RequiredPermissions) != 0)
                return; // Keycard gives access, but still ev.IsAllowed = false, so maybe plugin denied it?

            if (((value | basePermissions) & (KeycardPermissions)ev.Chamber.RequiredPermissions) != 0)
                ev.IsAllowed = true;
        }

        private void Player_InteractingDoor(Exiled.Events.EventArgs.InteractingDoorEventArgs ev)
        {
            if (ev.IsAllowed)
                return;

            if (ev.Player.IsCuffed)
                return;

            if (ev.Door.IsLocked)
                return;

            if (!ev.Player.TryGetSessionVariable<KeycardPermissions>(SessionVarType.BUILTIN_DOOR_ACCESS, out var value))
                return;

            var basePermissions = (ev.Player.CurrentItem as Keycard)?.Permissions ?? KeycardPermissions.None;

            if ((basePermissions & (KeycardPermissions)ev.Door.RequiredPermissions.RequiredPermissions) != 0)
                return; // Keycard gives access, but still ev.IsAllowed = false, so maybe plugin denied it?

            if (((value | basePermissions) & (KeycardPermissions)ev.Door.RequiredPermissions.RequiredPermissions) != 0)
                ev.IsAllowed = true;
        }
    }
}*/
