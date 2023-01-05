// -----------------------------------------------------------------------
// <copyright file="AnnonymousEvents.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Exiled.API.Features;

namespace Mistaken.API
{
    /// <summary>
    /// Announymous Events.
    /// </summary>
    public static class AnnonymousEvents
    {
        /// <summary>
        /// Calls Event.
        /// </summary>
        /// <param name="name">Event Name.</param>
        /// <param name="arg">Event args.</param>
        public static void Call(string name, object arg)
        {
            Log.Debug("Running " + name);
            if (Subscribers.TryGetValue(name, out List<Action<object>> handlers))
            {
                foreach (var item in handlers)
                    item(arg);
            }
        }

        /// <summary>
        /// Subscribes to event.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handler">Event handler.</param>
        public static void Subscribe(string name, Action<object> handler)
        {
            Log.Debug("Subscribing to " + name);
            if (!Subscribers.ContainsKey(name))
                Subscribers[name] = new();
            Subscribers[name].Add(handler);
        }

        /// <summary>
        /// UnSubscribes to event.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handler">Event handler.</param>
        public static void UnSubscribe(string name, Action<object> handler)
        {
            Log.Debug("UnSubscribing to " + name);
            if (Subscribers.ContainsKey(name))
                Subscribers[name].Remove(handler);
        }

        private static readonly Dictionary<string, List<Action<object>>> Subscribers = new();
    }
}
