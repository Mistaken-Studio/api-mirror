// -----------------------------------------------------------------------
// <copyright file="InvokeSafelyPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

/*using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace Mistaken.API.Diagnostics.Patches
{
    [PublicAPI]
    [HarmonyPatch(typeof(Exiled.Events.Extensions.Event), nameof(Exiled.Events.Extensions.Event.InvokeSafely), typeof(Exiled.Events.Events.CustomEventHandler))]
    internal static class InvokeSafelyPatch
    {
        internal static bool Prefix(Exiled.Events.Events.CustomEventHandler ev)
        {
            if (ev == null)
                return false;

            var fullStartTime = DateTime.UtcNow;
            double time;
            var fullName = ev.GetType().FullName;
            var lastName = "ERROR";

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (Exiled.Events.Events.CustomEventHandler customEventHandler in ev.GetInvocationList())
            {
                try
                {
                    var startTime = DateTime.UtcNow;
                    customEventHandler();
                    time = (DateTime.UtcNow - startTime).TotalMilliseconds;

                    Utilities.LogTime(customEventHandler.Method, time);
                    if (customEventHandler.Method.Name != "Invoke")
                        lastName = customEventHandler.Method.Name;
                }
                catch (Exception ex)
                {
                    Utilities.LogException(ex, customEventHandler.Method, fullName);
                }
            }

            time = (DateTime.UtcNow - fullStartTime).TotalMilliseconds;
            MasterHandler.LogTime("Summary." + lastName, time);

            return false;
        }
    }
}
*/