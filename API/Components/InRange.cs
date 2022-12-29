// -----------------------------------------------------------------------
// <copyright file="InRange.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mistaken.API.Extensions;
using PlayerRoles;
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
    public sealed class InRange : MonoBehaviour
    {
        /// <summary>
        /// Spawns <see cref="InRange"/>.
        /// </summary>
        /// <param name="pos">Position of trigger.</param>
        /// <param name="size">Size of trigger.</param>
        /// <param name="onEnter">Action called when someone enters trigger.</param>
        /// <param name="onExit">Action called when someone exits trigger.</param>
        /// <returns>Spawned <see cref="InRange"/>.</returns>
        public static InRange Spawn(Vector3 pos, Vector3 size, Action<MPlayer> onEnter = null, Action<MPlayer> onExit = null)
        {
            try
            {
                var obj = Instantiate(Prefab, pos, Quaternion.identity);
                var component = obj.GetComponent<InRange>();
                component.onEnter = onEnter;
                component.onExit = onExit;
                obj.GetComponent<BoxCollider>().size = size;

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
        /// Spawns <see cref="InRange"/>.
        /// </summary>
        /// <param name="parent">Parent transform.</param>
        /// <param name="offset">Offset from parent.</param>
        /// <param name="size">Size of trigger.</param>
        /// <param name="onEnter">Action called when someone entrees trigger.</param>
        /// <param name="onExit">Action called when someone exits trigger.</param>
        /// <returns>Spawned <see cref="InRange"/>.</returns>
        public static InRange Spawn(Transform parent, Vector3 offset, Vector3 size, Action<MPlayer> onEnter = null, Action<MPlayer> onExit = null)
        {
            try
            {
                var obj = Instantiate(Prefab, parent);
                obj.transform.localPosition = offset;
                obj.transform.localRotation = Quaternion.identity;
                var component = obj.GetComponent<InRange>();
                component.onEnter = onEnter;
                component.onExit = onExit;
                obj.GetComponent<BoxCollider>().size = size;

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
                if (prefab != null)
                    return prefab;

                prefab = new GameObject(nameof(InRange), typeof(InRange), typeof(BoxCollider))
                {
                    layer = Layer,
                };
                var collider = prefab.GetComponent<BoxCollider>();
                collider.isTrigger = true;

                return prefab;
            }
        }

        private readonly HashSet<GameObject> safetyCheck = new();

        private Action<MPlayer> onEnter;
        private Action<MPlayer> onExit;

        private void FixedUpdate()
        {
            if (this.ColliderInArea.Count == 0)
                return;

            foreach (var item in this.ColliderInArea.ToArray())
            {
                try
                {
                    if (item.gameObject.Equals(null))
                        this.ColliderInArea.Remove(item);

                    _ = item.transform.position;
                }
                catch
                {
                    this.ColliderInArea.Remove(item);
                }

                if (Vector3.Distance(item.transform.position, this.transform.position) > 100)
                {
                    if (!this.safetyCheck.Contains(item))
                        this.safetyCheck.Add(item);
                    else
                    {
                        Log.Error("[InRange] Failed to remove object from trigger");

                        // Diagnostics.MasterHandler.LogError(new Exception("[InRange] Failed to remove object from trigger"), null, "InRange.FixedUpdate");
                        this.ColliderInArea.Remove(item);
                        var player = Player.Get<MPlayer>(item.gameObject);
                        this.onExit?.Invoke(player);
                        this.safetyCheck.Remove(item);
                    }
                }
                else
                    this.safetyCheck.Remove(item);
            }
        }

        private void Start()
        {
            EventManager.RegisterEvents(this);
        }

        private void OnDestroy()
        {
            EventManager.UnregisterEvents(this);
        }

        [UsedImplicitly]
        [PluginEvent(ServerEventType.PlayerChangeRole)]
        private void OnPlayerChangeRole(MPlayer player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            // if (ev.Lite) return;
            if (player is null)
                return;

            if (!this.ColliderInArea.Contains(player.GameObject))
                return;

            this.onExit?.Invoke(player);
            this.ColliderInArea.Remove(player.GameObject);
        }

        [UsedImplicitly]
        [PluginEvent(ServerEventType.PlayerDeath)]
        private void OnPlayerDeath(MPlayer player, MPlayer attacker, DamageHandlerBase damageHandler)
        {
            if (player is null)
                return;

            if (!this.ColliderInArea.Contains(player.GameObject))
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
