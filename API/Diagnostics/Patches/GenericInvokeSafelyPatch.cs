// -----------------------------------------------------------------------
// <copyright file="GenericInvokeSafelyPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using HarmonyLib;

#pragma warning disable CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej

namespace Mistaken.API.Diagnostics.Patches
{
    // [HarmonyPatch(typeof(Exiled.Events.Extensions.Event), nameof(Exiled.Events.Extensions.Event.InvokeSafely), typeof(Exiled.Events.Events.CustomEventHandler<>))]
    // [HarmonyPatch]
    public static class GenericInvokeSafelyPatch
    {
        public static void PatchEvents(Harmony harmony, Type eventList)
        {
            var baseType = eventList
                .GetMethods()
                .First(x => x.Name == nameof(Exiled.Events.Extensions.Event.InvokeSafely) && x.IsGenericMethod);
            var types = Assembly.GetAssembly(eventList).GetTypes().Where(x => x.IsSubclassOf(typeof(System.EventArgs))).ToArray();
            for (int i = 0; i < types.Length; i++)
            {
                Type item = types[i];
                try
                {
                    Log.Debug(item.FullName);
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
            foreach (var item in Assembly.GetAssembly(eventList).GetTypes().Where(x => x.IsSubclassOf(typeof(System.EventArgs))))
                harmony.Unpatch(baseType.MakeGenericMethod(item), HarmonyPatchType.Prefix);
        }

        private static class Patch<T>
            where T : EventArgs
        {
            private static bool Prefix(Exiled.Events.Events.CustomEventHandler<T> ev, T arg)
            {
                if (ev == null)
                    return false;
                try
                {
                    DateTime fullStartTime = DateTime.UtcNow;
                    DateTime startTime;
                    double time;
                    string fullName = ev.GetType().FullName;
                    var invocationList = ev.GetInvocationList();
                    for (int i = 0; i < invocationList.Length; i++)
                    {
                        var customEventHandler = invocationList[i];
                        try
                        {
                            // MasterHandler.LogJunk(customEventHandler.Method.FullDescription());
                            // Log.Debug($"[DIAGNOSTICS] [{typeof(T).FullName}] Handling by {fullName}");
                            startTime = DateTime.UtcNow;
                            customEventHandler.DynamicInvoke(arg);
                            time = (DateTime.UtcNow - startTime).TotalMilliseconds;

                            // Log.Debug($"[DIAGNOSTICS] [{typeof(T).FullName}] Handled by {fullName}");
                            Extensions.Utilities.LogTime(customEventHandler.Method, time);
                        }
                        catch (TargetInvocationException ex)
                        {
                            Extensions.Utilities.LogException(ex.InnerException, customEventHandler.Method, fullName);
                        }
                        catch (Exception ex)
                        {
                            Extensions.Utilities.LogException(ex, customEventHandler.Method, fullName);
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
}
