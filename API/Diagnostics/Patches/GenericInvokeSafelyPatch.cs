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

namespace Mistaken.API.Diagnostics.Patches
{
    // [HarmonyPatch(typeof(Exiled.Events.Extensions.Event), nameof(Exiled.Events.Extensions.Event.InvokeSafely), typeof(Exiled.Events.Events.CustomEventHandler<>))]
    // [HarmonyPatch]
    internal static class GenericInvokeSafelyPatch
    {
        internal static void PatchEvents(Harmony harmony)
        {
            var baseType = typeof(Exiled.Events.Extensions.Event)
                .GetMethods()
                .First(x => x.Name == nameof(Exiled.Events.Extensions.Event.InvokeSafely) && x.IsGenericMethod);
            var types = Assembly.GetAssembly(typeof(Exiled.Events.Extensions.Event)).GetTypes().Where(x => x.IsSubclassOf(typeof(System.EventArgs))).ToArray();
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

        internal static void UnpatchEvents(Harmony harmony)
        {
            var baseType = typeof(Exiled.Events.Extensions.Event).GetMethods().First(x => x.Name == nameof(Exiled.Events.Extensions.Event.InvokeSafely) && x.IsGenericMethod);
            foreach (var item in Assembly.GetAssembly(typeof(Exiled.Events.Extensions.Event)).GetTypes().Where(x => x.IsSubclassOf(typeof(System.EventArgs))))
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
                            startTime = DateTime.UtcNow;
                            customEventHandler.DynamicInvoke(arg);
                            time = (DateTime.UtcNow - startTime).TotalMilliseconds;

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
