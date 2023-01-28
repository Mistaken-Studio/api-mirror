using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Doors;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;


namespace Mistaken.API.Patches;

internal static class DoorPatch
{
    /// <summary>
    /// HashSet of door to be ignored by RA.
    /// </summary>
    public static readonly HashSet<DoorVariant> IgnoredDoor = new();

    // [HarmonyPatch(typeof(CloseDoorCommand), nameof(CloseDoorCommand.Execute))]
    private static class CloseDoor
    {
        internal static bool Prefix(CloseDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
            {
                __result = false;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array![0] + " " + __instance.DisplayCommandUsage();
                __result = false;
                return false;
            }

            if (string.IsNullOrEmpty(arguments.At(0)))
            {
                response = "Please select door first.";
                __result = false;
                return false;
            }

            var flag = false;
            var text = arguments.At(0).ToUpper();
            DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
            foreach (var doorVariant in array)
            {
                if (IgnoredDoor.Contains(doorVariant))
                    continue;

                if (text != "**")
                {
                    if (doorVariant.TryGetComponent(out DoorNametagExtension component))
                    {
                        if (text != "*" && !string.Equals(component.GetName, text, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                    else if (text != "!*")
                    {
                        continue;
                    }
                }

                IDamageableDoor damageableDoor;
                if ((damageableDoor = (doorVariant as IDamageableDoor)) != null && damageableDoor.IsDestroyed)
                {
                    sender.Respond("ERROR: Cannot close door " + text + " because it is destroyed!");
                    continue;
                }

                doorVariant.NetworkTargetState = false;
                flag = true;
            }

            var flag2 = arguments.Array![0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
            if (flag)
            {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + arguments.Array[0].ToLower() + (flag2 ? "d" : "ed") + " door " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
            }

            response = (flag ? ("Door " + text + " " + arguments.Array[0].ToLower() + (flag2 ? "d." : "ed.")) : ("Can't find door " + text + "."));
            __result = flag;
            return false;
        }
    }

    // [HarmonyPatch(typeof(OpenDoorCommand), nameof(OpenDoorCommand.Execute))]
    private static class OpenDoor
    {
        internal static bool Prefix(OpenDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
            {
                __result = false;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array![0] + " " + __instance.DisplayCommandUsage();
                __result = false;
                return false;
            }

            if (string.IsNullOrEmpty(arguments.At(0)))
            {
                response = "Please select door first.";
                __result = false;
                return false;
            }

            var flag = false;
            var text = arguments.At(0).ToUpper();
            DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
            foreach (var doorVariant in array)
            {
                if (IgnoredDoor.Contains(doorVariant))
                    continue;

                if (text != "**")
                {
                    if (doorVariant.TryGetComponent(out DoorNametagExtension component))
                    {
                        if (text != "*" && !string.Equals(component.GetName, text, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                    else if (text != "!*")
                    {
                        continue;
                    }
                }

                IDamageableDoor damageableDoor;
                if ((damageableDoor = (doorVariant as IDamageableDoor)) != null && damageableDoor.IsDestroyed)
                {
                    sender.Respond("ERROR: Cannot open door " + text + " because it is destroyed!");
                    continue;
                }

                doorVariant.NetworkTargetState = true;
                flag = true;
            }

            var flag2 = arguments.Array![0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
            if (flag)
            {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + arguments.Array[0].ToLower() + (flag2 ? "d" : "ed") + " door " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
            }

            response = (flag ? ("Door " + text + " " + arguments.Array[0].ToLower() + (flag2 ? "d." : "ed.")) : ("Can't find door " + text + "."));

            __result = flag;
            return false;
        }
    }

    // [HarmonyPatch(typeof(DestroyDoorCommand), nameof(DestroyDoorCommand.Execute))]
    private static class DestroyDoor
    {
        internal static bool Prefix(DestroyDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
            {
                __result = false;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array![0] + " " + __instance.DisplayCommandUsage();
                __result = false;
                return false;
            }

            if (string.IsNullOrEmpty(arguments.At(0)))
            {
                response = "Please select door first.";
                __result = false;
                return false;
            }

            var flag = false;
            var text = arguments.At(0).ToUpper();
            DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
            foreach (var doorVariant in array)
            {
                if (IgnoredDoor.Contains(doorVariant))
                    continue;

                if (text != "**")
                {
                    if (doorVariant.TryGetComponent(out DoorNametagExtension component))
                    {
                        if (text != "*" && !string.Equals(component.GetName, text, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                    else if (text != "!*")
                    {
                        continue;
                    }
                }

                IDamageableDoor damageableDoor;
                if ((damageableDoor = (doorVariant as IDamageableDoor)) != null && !damageableDoor.IsDestroyed)
                {
                    damageableDoor.ServerDamage(65535f, DoorDamageType.ServerCommand);
                }

                flag = true;
            }

            var flag2 = arguments.Array![0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
            if (flag)
            {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + arguments.Array[0].ToLower() + (flag2 ? "d" : "ed") + " door " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
            }

            response = (flag ? ("Door " + text + " " + arguments.Array[0].ToLower() + (flag2 ? "d." : "ed.")) : ("Can't find door " + text + "."));
            __result = flag;
            return false;
        }
    }

    // [HarmonyPatch(typeof(LockDoorCommand), nameof(LockDoorCommand.Execute))]
    private static class LockDoor
    {
        internal static bool Prefix(LockDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
            {
                __result = false;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array![0] + " " + __instance.DisplayCommandUsage();
                __result = false;
                return false;
            }

            if (string.IsNullOrEmpty(arguments.At(0)))
            {
                response = "Please select door first.";
                __result = false;
                return false;
            }

            var flag = false;
            var text = arguments.At(0).ToUpper();
            DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
            foreach (var doorVariant in array)
            {
                if (IgnoredDoor.Contains(doorVariant))
                    continue;

                if (text != "**")
                {
                    if (doorVariant.TryGetComponent(out DoorNametagExtension component))
                    {
                        if (text != "*" && !string.Equals(component.GetName, text, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                    else if (text != "!*")
                    {
                        continue;
                    }
                }

                IDamageableDoor damageableDoor;
                if ((damageableDoor = (doorVariant as IDamageableDoor)) != null && damageableDoor.IsDestroyed)
                {
                    sender.Respond("ERROR: Cannot lock door " + text + " because it is destroyed!");
                    continue;
                }

                doorVariant.ServerChangeLock(DoorLockReason.AdminCommand, newState: true);
                flag = true;
            }

            var flag2 = arguments.Array![0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
            if (flag)
            {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + arguments.Array[0].ToLower() + (flag2 ? "d" : "ed") + " door " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
            }

            response = (flag ? ("Door " + text + " " + arguments.Array[0].ToLower() + (flag2 ? "d." : "ed.")) : ("Can't find door " + text + "."));
            __result = flag;
            return false;
        }
    }

    // [HarmonyPatch(typeof(UnlockDoorCommand), nameof(UnlockDoorCommand.Execute))]
    private static class UnlockDoor
    {
        internal static bool Prefix(UnlockDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
            {
                __result = false;
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array![0] + " " + __instance.DisplayCommandUsage();
                __result = false;
                return false;
            }

            if (string.IsNullOrEmpty(arguments.At(0)))
            {
                response = "Please select door first.";
                __result = false;
                return false;
            }

            var flag = false;
            var text = arguments.At(0).ToUpper();
            DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
            foreach (var doorVariant in array)
            {
                if (text != "**")
                {
                    if (doorVariant.TryGetComponent(out DoorNametagExtension component))
                    {
                        if (text != "*" && !string.Equals(component.GetName, text, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                    else if (text != "!*")
                    {
                        continue;
                    }
                }

                IDamageableDoor damageableDoor;
                if ((damageableDoor = (doorVariant as IDamageableDoor)) != null && damageableDoor.IsDestroyed)
                {
                    sender.Respond("ERROR: Cannot unlock door " + text + " because it is destroyed!");
                    continue;
                }

                doorVariant.ServerChangeLock(DoorLockReason.AdminCommand, newState: false);
                flag = true;
            }

            var flag2 = arguments.Array![0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
            if (flag)
            {
                ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + arguments.Array[0].ToLower() + (flag2 ? "d" : "ed") + " door " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
            }

            response = (flag ? ("Door " + text + " " + arguments.Array[0].ToLower() + (flag2 ? "d." : "ed.")) : ("Can't find door " + text + "."));
            __result = flag;
            return false;
        }
    }
}
