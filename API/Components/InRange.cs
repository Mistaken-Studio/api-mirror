// -----------------------------------------------------------------------
// <copyright file="InRange.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Exiled.API.Features;
using Mistaken.API.Extensions;
using UnityEngine;

namespace Mistaken.API.Components
{
    /// <summary>
    /// Component used to detect players.
    /// </summary>
    public class InRange : MonoBehaviour
    {
        /// <summary>
        /// Spawnes <see cref="InRange"/>.
        /// </summary>
        /// <param name="pos">Position of trigger.</param>
        /// <param name="size">Size of trigger.</param>
        /// <param name="onEnter">Action called when someone enteres trigger.</param>
        /// <param name="onExit">Action called when someone exits trigger.</param>
        /// <returns>Spawned <see cref="InRange"/>.</returns>
        public static InRange Spawn(Vector3 pos, Vector3 size, Action<Player> onEnter = null, Action<Player> onExit = null)
        {
            try
            {
                var obj = GameObject.Instantiate(Prefab, pos, Quaternion.identity);
                var component = obj.GetComponent<InRange>();
                component.onEnter = onEnter;
                component.onExit = onExit;
                obj.GetComponent<BoxCollider>().size = size;

                return component;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Spawnes <see cref="InRange"/>.
        /// </summary>
        /// <param name="parrent">Parrent transform.</param>
        /// <param name="offset">Offset from parrent.</param>
        /// <param name="size">Size of trigger.</param>
        /// <param name="onEnter">Action called when someone enteres trigger.</param>
        /// <param name="onExit">Action called when someone exits trigger.</param>
        /// <returns>Spawned <see cref="InRange"/>.</returns>
        public static InRange Spawn(Transform parrent, Vector3 offset, Vector3 size, Action<Player> onEnter = null, Action<Player> onExit = null)
        {
            try
            {
                var obj = GameObject.Instantiate(Prefab, parrent);
                obj.transform.localPosition = offset;
                obj.transform.rotation = Quaternion.identity;
                var component = obj.GetComponent<InRange>();
                component.onEnter = onEnter;
                component.onExit = onExit;
                obj.GetComponent<BoxCollider>().size = size;

                return component;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Gets hashSet of gameObjects inside trigger.
        /// </summary>
        public HashSet<GameObject> ColliderInArea { get; } = new HashSet<GameObject>();

        /// <summary>
        /// Gets or sets a value indicating whether trigger should detect NPCs.
        /// </summary>
        public bool AllowNPCs { get; set; } = false;

        private const bool DEBUG = false;
        private static readonly int Layer = LayerMask.GetMask("TransparentFX", "Ignore Raycast");
        private static GameObject prefab;

        private static GameObject Prefab
        {
            get
            {
                if (prefab == null)
                {
                    prefab = new GameObject(nameof(InRange), typeof(InRange), typeof(BoxCollider))
                    {
                        layer = Layer,
                    };
                    var collider = prefab.GetComponent<BoxCollider>();
                    collider.isTrigger = true;
                }

                return prefab;
            }
        }

        private readonly HashSet<GameObject> safetyCheck = new HashSet<GameObject>();

        private Action<Player> onEnter;
        private Action<Player> onExit;

        private void FixedUpdate()
        {
            if (this.ColliderInArea.Count == 0)
                return;
            foreach (var item in this.ColliderInArea.ToArray())
            {
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
            Log.Debug("CR_0", DEBUG);
            if (ev.ShouldPreservePosition)
            {
                Log.Debug("CR_1", DEBUG);
                return;
            }

            if (!this.ColliderInArea.Contains(ev.Player.GameObject))
            {
                Log.Debug("CR_2", DEBUG);
                return;
            }

            Log.Debug("CR_3", DEBUG);
            this.onExit?.Invoke(ev.Player);
            this.ColliderInArea.Remove(ev.Player.GameObject);
        }

        private void Player_Died(Exiled.Events.EventArgs.DiedEventArgs ev)
        {
            Log.Debug("D_0", DEBUG);
            if (this.ColliderInArea.Contains(ev.Target.GameObject))
            {
                Log.Debug("D_1", DEBUG);
                this.onExit?.Invoke(ev.Target);
                this.ColliderInArea.Remove(ev.Target.GameObject);
                Log.Debug("D_2", DEBUG);
            }

            Log.Debug("D_3", DEBUG);
        }

        private void OnTriggerEnter(Collider other)
        {
            try
            {
                Log.Debug($"Trigger enter: {other.gameObject.name}", DEBUG);
            }
            catch
            {
                // Ignored
            }

            if (!other.GetComponent<CharacterClassManager>())
                return;
            var player = Player.Get(other.gameObject);
            if (player?.IsDead ?? true)
                return;
            if (player.GetSessionVar<bool>("IsNPC") && !this.AllowNPCs)
                return;
            this.ColliderInArea.Add(other.gameObject);
            this.onEnter?.Invoke(player);
        }

        private void OnTriggerExit(Collider other)
        {
            try
            {
                Log.Debug($"Trigger exit: {other.gameObject.name}", DEBUG);
            }
            catch
            {
                // Ignored
            }

            if (!this.ColliderInArea.Contains(other.gameObject))
                return;
            this.ColliderInArea.Remove(other.gameObject);
            var player = Player.Get(other.gameObject);
            this.onExit?.Invoke(player);
        }
    }
}
