// -----------------------------------------------------------------------
// <copyright file="InRangePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Scp914;
using UnityEngine;

#pragma warning disable IDE0060

// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedParameter.Global
namespace Mistaken.API.Patches
{
    [UsedImplicitly]
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.Upgrade))]
    internal static class InRangePatch
    {
        internal static void Prefix(ref Collider[] intake, Vector3 moveVector, Scp914Mode mode, Scp914KnobSetting setting)
        {
            intake = intake.Where(x => x.GetComponent<Components.InRange>() == null && x.GetComponent<Components.InRangeBall>() == null).ToArray();
        }
    }
}
