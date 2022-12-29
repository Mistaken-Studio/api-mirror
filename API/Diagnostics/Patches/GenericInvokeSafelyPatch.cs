// -----------------------------------------------------------------------
// <copyright file="GenericInvokeSafelyPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

/*using System;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using HarmonyLib;
using JetBrains.Annotations;

// ReSharper disable UnusedMember.Local
#pragma warning disable CS1591

namespace Mistaken.API.Diagnostics.Patches
{
    // [HarmonyPatch(typeof(Exiled.Events.Extensions.Event), nameof(Exiled.Events.Extensions.Event.InvokeSafely), typeof(Exiled.Events.Events.CustomEventHandler<>))]
    // [HarmonyPatch]
    [PublicAPI]
    public static class GenericInvokeSafelyPatch
    {
        public static void PatchEvents(Harmony harmony, Type eventList)
        {
            var baseType = eventList
                .GetMethods()
                .First(x => x.Name == nameof(Exiled.Events.Extensions.Event.InvokeSafely) && x.IsGenericMethod);
            var types = Assembly.GetAssembly(eventList).GetTypes().Where(x => x.IsSubclassOf(typeof(EventArgs))).ToArray();
            foreach (var item in types)
            {
                try
                {
                    Log.Debug(item.FullName, PluginHandler.VerboseOutput);
                    var genericPatch = typeof(Patch<>).MakeGenericType(item)
                        .GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static);
                    harmony.Patch(baseType.MakeGenericMethod(item), prefix: new HarmonyMethod(genericPatch));
                }
                catch (Exception ex)
                {
                    Log.Error("Exception when patching GenericInvokeSafelyPatch: " + item.FullName);
                    Log.Error(ex);
                }
            }
        }

        public static void UnpatchEvents(Harmony harmony, Type eventList)
        {
            var baseType = eventList.GetMethods().First(x => x.Name == nameof(Exiled.Events.Extensions.Event.InvokeSafely) && x.IsGenericMethod);
            foreach (var item in Assembly.GetAssembly(eventList).GetTypes().Where(x => x.IsSubclassOf(typeof(EventArgs))))
                harmony.Unpatch(baseType.MakeGenericMethod(item), HarmonyPatchType.Prefix);
        }

        private static class Patch<T>
            where T : EventArgs
        {
            internal static bool Prefix(Exiled.Events.Events.CustomEventHandler<T> ev, T arg)
            {
                if (ev == null)
                    return false;
                try
                {
                    var fullStartTime = DateTime.UtcNow;
                    double time;
                    var fullName = ev.GetType().FullName;
                    var invocationList = ev.GetInvocationList();
                    foreach (var customEventHandler in invocationList)
                    {
                        try
                        {
                            // MasterHandler.LogJunk(customEventHandler.Method.FullDescription());
                            // Log.Debug($"[DIAGNOSTICS] [{typeof(T).FullName}] Handling by {fullName}");
                            var startTime = DateTime.UtcNow;
                            customEventHandler.DynamicInvoke(arg);
                            time = (DateTime.UtcNow - startTime).TotalMilliseconds;

                            // Log.Debug($"[DIAGNOSTICS] [{typeof(T).FullName}] Handled by {fullName}");
                            Utilities.LogTime(customEventHandler.Method, time);
                        }
                        catch (TargetInvocationException ex)
                        {
                            Utilities.LogException(ex.InnerException, customEventHandler.Method, fullName);
                        }
                        catch (Exception ex)
                        {
                            Utilities.LogException(ex, customEventHandler.Method, fullName);
                        }
                    }

                    time = (DateTime.UtcNow - fullStartTime).TotalMilliseconds;
                    MasterHandler.LogTime("Summary." + arg.GetType().Name, time);
                }
                catch (Exception ex)
                {
                    MasterHandler.LogError(ex, "GenericInvokeSafelyPatch.Prefix<T>");
                    Log.Error("Error when handling GenericInvokeSafelyPatch.Prefix<T>:");
                    Log.Error(ex);
                }

                return false;
            }
        }
    }
}*/
