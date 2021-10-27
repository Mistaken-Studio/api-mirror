// -----------------------------------------------------------------------
// <copyright file="TestFixPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using Exiled.API.Features;
using HarmonyLib;

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.FindObjectOfType), typeof(Type))]
    internal static class TestFixPatch
    {
        internal static Thread MainThread { get; set; }

        private static bool Prefix(Type type)
        {
            if (MainThread != Thread.CurrentThread)
            {
                Log.Error("Called FindObjectOfType not from main thread, denied");
                
                return false;
            }

            return true;
        }
    }
}
