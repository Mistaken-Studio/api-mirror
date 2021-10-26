// -----------------------------------------------------------------------
// <copyright file="MasterHandler.Generic.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Mistaken.API.Diagnostics
{
    public static partial class MasterHandler
    {
        private static class Generic<T>
            where T : EventArgs
        {
            [System.Obsolete("Will removed because of change in Handlers")]
            public static readonly Dictionary<Module, Dictionary<string, Exiled.Events.Events.CustomEventHandler<T>>> TypedHandlers = new Dictionary<Module, Dictionary<string, Exiled.Events.Events.CustomEventHandler<T>>>();

            [System.Obsolete("Functionality of diagnostics system was moved to exiled.events system, call directly event")]
            public static Exiled.Events.Events.CustomEventHandler<T> Handle(Module module, Action<T> action)
            {
                if (!TypedHandlers.ContainsKey(module))
                    TypedHandlers[module] = new Dictionary<string, Exiled.Events.Events.CustomEventHandler<T>>();
                string name = typeof(T).Name;
                if (TypedHandlers[module].ContainsKey(name))
                    return TypedHandlers[module][name];

                Exiled.Events.Events.CustomEventHandler<T> tor = new Exiled.Events.Events.CustomEventHandler<T>(action);

                TypedHandlers[module][name] = tor;
                return tor;
            }
        }
    }
}
