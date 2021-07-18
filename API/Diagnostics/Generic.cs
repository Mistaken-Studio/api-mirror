// -----------------------------------------------------------------------
// <copyright file="Generic.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Exiled.API.Features;

namespace Mistaken.API.Diagnostics
{
    public static partial class MasterHandler
    {
        private static class Generic<T>
            where T : EventArgs
        {
            public static readonly Dictionary<Module, Dictionary<string, Exiled.Events.Events.CustomEventHandler<T>>> TypedHandlers = new Dictionary<Module, Dictionary<string, Exiled.Events.Events.CustomEventHandler<T>>>();

            public static Exiled.Events.Events.CustomEventHandler<T> Handle(Module module, Action<T> action)
            {
                if (!TypedHandlers.ContainsKey(module))
                    TypedHandlers[module] = new Dictionary<string, Exiled.Events.Events.CustomEventHandler<T>>();
                string name = typeof(T).Name;
                if (TypedHandlers[module].ContainsKey(name))
                    return TypedHandlers[module][name];
                DateTime start;
                DateTime end;
                TimeSpan diff;
                void Tor(T ev)
                {
                    /*if(ev is Exiled.Events.EventArgs.InteractingDoorEventArgs)
                    {
                        Log.Warn($"[{module.Name}: {ev.GetType().Name}] Denied running {ev}");
                        return;
                    }*/
                    start = DateTime.Now;
                    try
                    {
                        action(ev);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error($"[{module.Name}: {name}] Caused Exception");
                        Log.Error(ex.Message);
                        Log.Error(ex.StackTrace);
                        LogError(ex, module, name);
                        ErrorBacklog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [{module.Name}: {name}] Caused Exception");
                        ErrorBacklog.Add(ex.Message);
                        ErrorBacklog.Add(ex.StackTrace);

                        OnError?.Invoke(ex, module, name);
                    }

                    end = DateTime.Now;
                    diff = end - start;
                    Backlog.Add($"[{DateTime.Now:HH:mm:ss.fff}] [{module.Name}: {name}] {diff.TotalMilliseconds}");
                }

                TypedHandlers[module][name] = Tor;
                return Tor;
            }
        }
    }
}
