using PluginAPI.Core;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Mistaken.API.Utilities
{
    /// <summary>
    /// Class that provides a way of executing code on the main thread from other threads.
    /// </summary>
    public sealed class ThreadDispatcher : MonoBehaviour
    {
        /// <summary>
        /// Instance of ThreadDispatcher class.
        /// </summary>
        public static ThreadDispatcher Instance { get; private set; }

        /// <summary>
        /// Enqueues an Action to be performed on the main thread.
        /// </summary>
        /// <param name="action">Action.</param>
        public static void Invoke(Action action) => _invokeQueue.Enqueue(action);

        internal static void Initialize()
        {
            if (Instance is not null)
                return;

            Instance = Server.Instance.GameObject.AddComponent<ThreadDispatcher>();
        }

        private static readonly ConcurrentQueue<Action> _invokeQueue = new();

        private void FixedUpdate()
        {
            if (_invokeQueue.TryDequeue(out Action result))
                result.Invoke();
        }
    }
}
