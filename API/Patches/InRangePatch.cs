// -----------------------------------------------------------------------
// <copyright file="InRangePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Doors;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Scp914;
using UnityEngine;

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.Upgrade))]
    internal static class InRangePatch
    {
        private static void Prefix(ref Collider[] intake, Vector3 moveVector, Scp914Mode mode, Scp914KnobSetting setting)
        {
            intake = intake.Where(x => x.GetComponent<Components.InRange>() == null && x.GetComponent<Components.InRangeBall>() == null).ToArray();
        }
    }
}
