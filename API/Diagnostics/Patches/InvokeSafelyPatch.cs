﻿// -----------------------------------------------------------------------
// <copyright file="InvokeSafelyPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using HarmonyLib;

namespace Mistaken.API.Diagnostics.Patches
{
    [HarmonyPatch(typeof(Exiled.Events.Extensions.Event), nameof(Exiled.Events.Extensions.Event.InvokeSafely), typeof(Exiled.Events.Events.CustomEventHandler))]
    internal static class InvokeSafelyPatch
    {
        private static bool Prefix(Exiled.Events.Events.CustomEventHandler ev)
        {
            if (ev == null)
                return false;

            DateTime startTime;
            double time;
            string fullName = ev.GetType().FullName;
            foreach (Exiled.Events.Events.CustomEventHandler customEventHandler in ev.GetInvocationList())
            {
                try
                {
                    startTime = DateTime.UtcNow;
                    customEventHandler();
                    time = (DateTime.UtcNow - startTime).TotalMilliseconds;

                    Extensions.Utilities.LogTime(customEventHandler.Method, time);
                }
                catch (Exception ex)
                {
                    Extensions.Utilities.LogException(ex, customEventHandler.Method, fullName);
                }
            }

            return false;
        }
    }
}
