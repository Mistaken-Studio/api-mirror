// -----------------------------------------------------------------------
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

            DateTime fullStartTime = DateTime.UtcNow;
            DateTime startTime;
            double time;
            string fullName = ev.GetType().FullName;
            string lastName = "ERROR";
            foreach (Exiled.Events.Events.CustomEventHandler customEventHandler in ev.GetInvocationList())
            {
                try
                {
                    startTime = DateTime.UtcNow;
                    customEventHandler();
                    time = (DateTime.UtcNow - startTime).TotalMilliseconds;

                    Extensions.Utilities.LogTime(customEventHandler.Method, time);
                    if (customEventHandler.Method.Name != "Invoke")
                        lastName = customEventHandler.Method.Name;
                }
                catch (Exception ex)
                {
                    Extensions.Utilities.LogException(ex, customEventHandler.Method, fullName);
                }
            }

            time = (DateTime.UtcNow - fullStartTime).TotalMilliseconds;
            MasterHandler.LogTime("Summary." + lastName, time);

            return false;
        }
    }
}
