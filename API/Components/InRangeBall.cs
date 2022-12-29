// -----------------------------------------------------------------------
// <copyright file="InRangeBall.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mistaken.API.Extensions;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

namespace Mistaken.API.Components
{
    /// <summary>
    /// Component used to detect players.
    /// </summary>
    [PublicAPI]
    public sealed class InRangeBall : MonoBehaviour
    {
        /// <summary>
        /// Spawns <see cref="InRangeBall"/>.
        /// </summary>
        /// <param name="pos">Position of trigger.</param>
        /// <param name="radius">Radius of trigger.</param>
        /// <param name="height">Height of trigger.</param>
        /// <param name="onEnter">Action called when someone enters trigger.</param>
        /// <param name="onExit">Action called when someone exits trigger.</param>
        /// <returns>Spawned <see cref="InRangeBall"/>.</returns>
        public static InRangeBall Spawn(Vector3 pos, float radius, float height, Action<MPlayer> onEnter = null, Action<MPlayer> onExit = null)
        {
            try
            {
                var obj = Instantiate(Prefab, pos, Quaternion.identity);
                var component = obj.GetComponent<InRangeBall>();
                component.onEnter = onEnter;
                component.onExit = onExit;
                var collider = obj.GetComponent<CapsuleCollider>();
                collider.radius = radius;
                collider.height = height;

                return component;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Spawns <see cref="InRangeBall"/>.
        /// </summary>
        /// <param name="parent">Parent transform.</param>
        /// <param name="offset">Offset from parent.</param>
        /// <param name="radius">Radius of trigger.</param>
        /// <param name="height">Height of trigger.</param>
        /// <param name="onEnter">Action called when someone enters trigger.</param>
        /// <param name="onExit">Action called when someone exits trigger.</param>
        /// <returns>Spawned <see cref="InRangeBall"/>.</returns>
        public static InRangeBall Spawn(Transform parent, Vector3 offset, float radius, float height, Action<MPlayer> onEnter = null, Action<MPlayer> onExit = null)
        {
            try
            {
                var obj = Instantiate(Prefab, parent);
                obj.transform.localPosition = offset;
                obj.transform.rotation = Quaternion.identity;
                var component = obj.GetComponent<InRangeBall>();
                component.onEnter = onEnter;
                component.onExit = onExit;
                var collider = obj.GetComponent<CapsuleCollider>();
                collider.radius = radius;
                collider.height = height;

                return component;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Gets hashSet of gameObjects inside trigger.
        /// </summary>
        public HashSet<GameObject> ColliderInArea { get; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether trigger should detect NPCs.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool AllowNPCs { get; set; }

        private static readonly int Layer = LayerMask.GetMask("TransparentFX", "Ignore Raycast");
        private static GameObject prefab;

        private static GameObject Prefab
        {
            get
            {
                if (prefab == null)
                {
                    prefab = new(nameof(InRangeBall), typeof(InRangeBall), typeof(CapsuleCollider))
                    {
                        layer = Layer,
                    };
                    var collider = prefab.GetComponent<CapsuleCollider>();
                    collider.isTrigger = true;
                }

                return prefab;
            }
        }

        private Action<MPlayer> onEnter;
        private Action<MPlayer> onExit;

        private void Start()
        {
            EventManager.RegisterEvents(this);
        }

        private void OnDestroy()
        {
            EventManager.UnregisterEvents(this);
        }

        [UsedImplicitly]
        [PluginEvent(ServerEventType.PlayerDeath)]
        private void OnPlayerDeath(MPlayer player, MPlayer attacker, DamageHandlerBase damageHandler)
        {
            if (player is null)
                return;

            this.onExit?.Invoke(player);
            this.ColliderInArea.Remove(player.GameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<CharacterClassManager>())
                return;

            var player = Player.Get<MPlayer>(other.gameObject);

            if (!player?.IsAlive ?? true)
                return;

            if (player.GetSessionVariable<bool>("IsNPC") && !this.AllowNPCs)
                return;

            this.ColliderInArea.Add(other.gameObject);
            this.onEnter?.Invoke(player);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!this.ColliderInArea.Contains(other.gameObject))
                return;

            this.ColliderInArea.Remove(other.gameObject);
            var player = Player.Get<MPlayer>(other.gameObject);
            this.onExit?.Invoke(player);
        }
    }
}
