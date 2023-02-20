using Mirror;
using Mistaken.API.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mistaken.API.Core;

/// <summary>
/// Structures.
/// </summary>
public static class Structure
{
    /// <summary>
    /// Spawn's structures.
    /// </summary>
    /// <param name="type">Structure type.</param>
    /// <param name="position">Structure's position.</param>
    /// <param name="rotation">Structure's rotation.</param>
    /// <returns>Spawned structure.</returns>
    public static GameObject Spawn(StructureType type, Vector3 position, Quaternion rotation)
    {
        if (!NetworkClient.prefabs.TryGetValue(_structurePrefabs[type], out var prefab))
            return null;

        var obj = Object.Instantiate(prefab, position, rotation);
        NetworkServer.Spawn(obj);
        return obj;
    }

    /// <summary>
    /// Spawn's structures.
    /// </summary>
    /// <param name="type">Structure type.</param>
    /// <param name="parent">Structure's parent.</param>
    /// <returns>Spawned structure.</returns>
    public static GameObject Spawn(StructureType type, Transform parent)
    {
        if (!NetworkClient.prefabs.TryGetValue(_structurePrefabs[type], out var prefab))
            return null;

        var obj = Object.Instantiate(prefab, parent.position, parent.rotation);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        NetworkServer.Spawn(obj);
        return obj;
    }

    private static readonly Dictionary<StructureType, Guid> _structurePrefabs = new()
    {
        { StructureType.Scp018Pedestal, new("a149d3eb-11bd-de24-f9dd-57187f5771ef") },
        { StructureType.Scp207Pedestal, new("17054030-9461-d104-5b92-9456c9eb0ab7") },
        { StructureType.Scp244Pedestal, new("fa602fdc-724c-d2a4-8b8c-1fb314b82746") },
        { StructureType.Scp268Pedestal, new("68f13209-e652-6024-2b89-0f75fb88a998") },
        { StructureType.Scp500Pedestal, new("f4149b66-c503-87a4-0b93-aabfe7c352da") },
        { StructureType.Scp1853Pedestal, new("4f36c701-ea0c-9064-2a58-2c89240e51ba") },
        { StructureType.Scp2176Pedestal, new("fff1c10c-a719-bea4-d95c-3e262ed03ab2") },
        { StructureType.RegularMedkitLocker, new("5b227bd2-1ed2-8fc4-2aa1-4856d7cb7472") },
        { StructureType.AdrenalineMedkitLocker, new("db602577-8d4f-97b4-890b-8c893bfcd553") },
        { StructureType.RifleRackLocker, new("850f84ad-e273-1824-8885-11ae5e01e2f4") },
        { StructureType.MiscLocker, new("d54bead1-286f-3004-facd-74482a872ad8") },
        { StructureType.LargeGunLocker, new("5ad5dc6d-7bc5-3154-8b1a-3598b96e0d5b") },
        { StructureType.Generator, new("daf3ccde-4392-c0e4-882d-b7002185c6b8") },
        { StructureType.Workstation, new("ad8a455f-062d-dea4-5b47-ac9217d4c58b") },
    };
}
