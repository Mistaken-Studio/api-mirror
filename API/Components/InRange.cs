// -----------------------------------------------------------------------
// <copyright file="InRange.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using JetBrains.Annotations;
using Mistaken.API.Extensions;
using UnityEngine;

namespace Mistaken.API.Components
{
    /// <summary>
    /// Component used to detect players.
    /// </summary>
    [PublicAPI]
    public class InRange : MonoBehaviour
    {
        /// <summary>
        /// Spawns <see cref="InRange"/>.
        /// </summary>
        /// <param name="pos">Position of trigger.</param>
        /// <param name="size">Size of trigger.</param>
        /// <param name="onEnter">Action called when someone enters trigger.</param>
        /// <param name="onExit">Action called when someone exits trigger.</param>
        /// <returns>Spawned <see cref="InRange"/>.</returns>
        public static InRange Spawn(Vector3 pos, Vector3 size, Action<Player> onEnter = null, Action<Player> onExit = null)
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
        public static InRange Spawn(Transform parent, Vector3 offset, Vector3 size, Action<Player> onEnter = null, Action<Player> onExit = null)
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

        private Action<Player> onEnter;
        private Action<Player> onExit;

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
                        Diagnostics.MasterHandler.LogError(new Exception("[InRange] Failed to remove object from trigger"), null, "InRange.FixedUpdate");
                        this.ColliderInArea.Remove(item);
                        var player = Player.Get(item.gameObject);
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
            Exiled.Events.Handlers.Player.Died += this.Player_Died;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;
        }

        private void OnDestroy()
        {
            Exiled.Events.Handlers.Player.Died -= this.Player_Died;
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Lite)
                return;

            if (!this.ColliderInArea.Contains(ev.Player.GameObject))
                return;

            this.onExit?.Invoke(ev.Player);
            this.ColliderInArea.Remove(ev.Player.GameObject);
        }

        private void Player_Died(Exiled.Events.EventArgs.DiedEventArgs ev)
        {
            if (!this.ColliderInArea.Contains(ev.Target.GameObject))
                return;

            this.onExit?.Invoke(ev.Target);
            this.ColliderInArea.Remove(ev.Target.GameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<CharacterClassManager>())
                return;

            var player = Player.Get(other.gameObject);
            if (player?.IsDead ?? true)
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
            var player = Player.Get(other.gameObject);
            this.onExit?.Invoke(player);
        }
    }
}
