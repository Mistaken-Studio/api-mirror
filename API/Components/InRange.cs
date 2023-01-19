using JetBrains.Annotations;
using Mistaken.API.Extensions;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mistaken.API.Components;

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
    /// <returns>Spawned <see cref="InRange"/>.</returns>
    public static InRange Spawn(Vector3 pos, Vector3 size)
    {
        try
        {
            var obj = Instantiate(Prefab, pos, Quaternion.identity);
            obj.GetComponent<BoxCollider>().size = size;
            return obj.GetComponent<InRange>();
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
    /// <returns>Spawned <see cref="InRange"/>.</returns>
    public static InRange Spawn(Transform parent, Vector3 offset, Vector3 size)
    {
        try
        {
            var obj = Instantiate(Prefab, parent);
            obj.transform.localPosition = offset;
            obj.transform.localRotation = Quaternion.identity;
            obj.GetComponent<BoxCollider>().size = size;
            return obj.GetComponent<InRange>();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            Log.Error(ex.StackTrace);
            return null;
        }
    }

    /// <summary>
    /// Invoked when Player enters the collider.
    /// </summary>
    public event Action<Player> OnEnter;

    /// <summary>
    /// Invoked when Player exits the collider.
    /// </summary>
    public event Action<Player> OnExit;

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

    private void FixedUpdate()
    {
        if (ColliderInArea.Count == 0)
            return;

        foreach (var item in ColliderInArea.ToArray())
        {
            try
            {
                if (item.gameObject.Equals(null))
                    ColliderInArea.Remove(item);

                _ = item.transform.position;
            }
            catch
            {
                ColliderInArea.Remove(item);
            }

            if (Vector3.Distance(item.transform.position, transform.position) > 100)
            {
                if (!safetyCheck.Contains(item))
                    safetyCheck.Add(item);
                else
                {
                    Log.Error("[InRange] Failed to remove object from trigger");

                    // Diagnostics.MasterHandler.LogError(new Exception("[InRange] Failed to remove object from trigger"), null, "InRange.FixedUpdate");
                    ColliderInArea.Remove(item);
                    var player = Player.Get(item.gameObject);
                    OnExit?.Invoke(player);
                    safetyCheck.Remove(item);
                }
            }
            else
                safetyCheck.Remove(item);
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
    private void OnPlayerChangeRole(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
    {
        // if (ev.Lite) return;
        if (player is null)
            return;

        if (!ColliderInArea.Contains(player.GameObject))
            return;

        OnExit?.Invoke(player);
        ColliderInArea.Remove(player.GameObject);
    }

    [UsedImplicitly]
    [PluginEvent(ServerEventType.PlayerDeath)]
    private void OnPlayerDeath(Player player, Player attacker, DamageHandlerBase damageHandler)
    {
        if (player is null)
            return;

        if (!ColliderInArea.Contains(player.GameObject))
            return;

        OnExit?.Invoke(player);
        ColliderInArea.Remove(player.GameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Log.Debug("AA");
        if (!other.GetComponent<CharacterClassManager>())
            return;

        var player = Player.Get(other.gameObject);

        if (!player?.IsAlive ?? true)
            return;

        if (player.GetSessionVariable<bool>("IsNPC") && !AllowNPCs)
            return;

        ColliderInArea.Add(other.gameObject);
        Log.Debug("A");
        OnEnter?.Invoke(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!ColliderInArea.Contains(other.gameObject))
            return;

        ColliderInArea.Remove(other.gameObject);
        var player = Player.Get(other.gameObject);
        Log.Debug("B");
        OnExit?.Invoke(player);
    }
}
