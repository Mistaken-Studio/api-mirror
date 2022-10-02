// -----------------------------------------------------------------------
// <copyright file="AnnonymousEvents.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

// ReSharper disable once CheckNamespace
namespace Mistaken.API
{
    /// <summary>
    /// Announymous Events.
    /// </summary>
    [Obsolete("Moved to Mistaken.API.API", true)]
    public static class AnnonymousEvents
    {
        /// <summary>
        /// Calls Event.
        /// </summary>
        /// <param name="name">Event Name.</param>
        /// <param name="arg">Event args.</param>
        public static void Call(string name, object arg)
        {
            API.AnnonymousEvents.Call(name, arg);
        }

        /// <summary>
        /// Subscribes to event.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handler">Event handler.</param>
        public static void Subscribe(string name, Action<object> handler)
        {
            API.AnnonymousEvents.Subscribe(name, handler);
        }

        /// <summary>
        /// UnSubscribes to event.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handler">Event handler.</param>
        public static void UnSubscribe(string name, Action<object> handler)
        {
            API.AnnonymousEvents.UnSubscribe(name, handler);
        }
    }
}
