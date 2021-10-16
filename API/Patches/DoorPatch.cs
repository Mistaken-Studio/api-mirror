// -----------------------------------------------------------------------
// <copyright file="DoorPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1119 // Statement should not use unnecessary parenthesis
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using System;
using System.Collections.Generic;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Doors;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;

namespace Mistaken.API.Patches
{
    /// <summary>
    /// Patch used to block some door from beeing opened by RA.
    /// </summary>
    public static class DoorPatch
    {
        /// <summary>
        /// HashSet of door to be ignored by RA.
        /// </summary>
        public static readonly HashSet<DoorVariant> IgnoredDoor = new HashSet<DoorVariant>();

        [HarmonyPatch(typeof(CloseDoorCommand), nameof(CloseDoorCommand.Execute))]
        private static class CloseDoor
        {
            private static bool Prefix(CloseDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
            {
                if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
                {
                    __result = false;
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + __instance.DisplayCommandUsage();
                    __result = false;
                    return false;
                }

                if (string.IsNullOrEmpty(arguments.At(0)))
                {
                    response = "Please select door first.";
                    __result = false;
                    return false;
                }

                bool flag = false;
                string text = arguments.At(0).ToUpper();
                DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
                foreach (DoorVariant doorVariant in array)
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

                bool flag2 = arguments.Array[0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
                if (flag)
                {
                    ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + arguments.Array[0].ToLower() + (flag2 ? "d" : "ed") + " door " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                }

                response = (flag ? ("Door " + text + " " + arguments.Array[0].ToLower() + (flag2 ? "d." : "ed.")) : ("Can't find door " + text + "."));
                __result = flag;
                return false;
            }
        }

        [HarmonyPatch(typeof(OpenDoorCommand), nameof(OpenDoorCommand.Execute))]
        private static class OpenDoor
        {
            private static bool Prefix(OpenDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
            {
                if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
                {
                    __result = false;
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + __instance.DisplayCommandUsage();
                    __result = false;
                    return false;
                }

                if (string.IsNullOrEmpty(arguments.At(0)))
                {
                    response = "Please select door first.";
                    __result = false;
                    return false;
                }

                bool flag = false;
                string text = arguments.At(0).ToUpper();
                DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
                foreach (DoorVariant doorVariant in array)
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

                bool flag2 = arguments.Array[0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
                if (flag)
                {
                    ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + arguments.Array[0].ToLower() + (flag2 ? "d" : "ed") + " door " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                }

                response = (flag ? ("Door " + text + " " + arguments.Array[0].ToLower() + (flag2 ? "d." : "ed.")) : ("Can't find door " + text + "."));

                __result = flag;
                return false;
            }
        }

        [HarmonyPatch(typeof(DestroyDoorCommand), nameof(DestroyDoorCommand.Execute))]
        private static class DestroyDoor
        {
            private static bool Prefix(DestroyDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
            {
                if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
                {
                    __result = false;
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + __instance.DisplayCommandUsage();
                    __result = false;
                    return false;
                }

                if (string.IsNullOrEmpty(arguments.At(0)))
                {
                    response = "Please select door first.";
                    __result = false;
                    return false;
                }

                bool flag = false;
                string text = arguments.At(0).ToUpper();
                DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
                foreach (DoorVariant doorVariant in array)
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

                bool flag2 = arguments.Array[0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
                if (flag)
                {
                    ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + arguments.Array[0].ToLower() + (flag2 ? "d" : "ed") + " door " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                }

                response = (flag ? ("Door " + text + " " + arguments.Array[0].ToLower() + (flag2 ? "d." : "ed.")) : ("Can't find door " + text + "."));
                __result = flag;
                return false;
            }
        }

        [HarmonyPatch(typeof(LockDoorCommand), nameof(LockDoorCommand.Execute))]
        private static class LockDoor
        {
            private static bool Prefix(LockDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
            {
                if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
                {
                    __result = false;
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + __instance.DisplayCommandUsage();
                    __result = false;
                    return false;
                }

                if (string.IsNullOrEmpty(arguments.At(0)))
                {
                    response = "Please select door first.";
                    __result = false;
                    return false;
                }

                bool flag = false;
                string text = arguments.At(0).ToUpper();
                DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
                foreach (DoorVariant doorVariant in array)
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

                bool flag2 = arguments.Array[0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
                if (flag)
                {
                    ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + arguments.Array[0].ToLower() + (flag2 ? "d" : "ed") + " door " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
                }

                response = (flag ? ("Door " + text + " " + arguments.Array[0].ToLower() + (flag2 ? "d." : "ed.")) : ("Can't find door " + text + "."));
                __result = flag;
                return false;
            }
        }

        [HarmonyPatch(typeof(UnlockDoorCommand), nameof(UnlockDoorCommand.Execute))]
        private static class UnlockDoor
        {
            private static bool Prefix(UnlockDoorCommand __instance, ref bool __result, ArraySegment<string> arguments, ICommandSender sender, ref string response)
            {
                if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
                {
                    __result = false;
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + __instance.DisplayCommandUsage();
                    __result = false;
                    return false;
                }

                if (string.IsNullOrEmpty(arguments.At(0)))
                {
                    response = "Please select door first.";
                    __result = false;
                    return false;
                }

                bool flag = false;
                string text = arguments.At(0).ToUpper();
                DoorVariant[] array = UnityEngine.Object.FindObjectsOfType<DoorVariant>();
                foreach (DoorVariant doorVariant in array)
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

                bool flag2 = arguments.Array[0].EndsWith("e", StringComparison.OrdinalIgnoreCase);
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
}
