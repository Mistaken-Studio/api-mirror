using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;

namespace Mistaken.API.Enums;

/// <summary>
/// Session Vars.
/// </summary>
public enum SessionVarType
{
    /// <plugin>CommandsExtender-Admin</plugin>
    Talk = 2,

    /// <plugin>Globaly used</plugin>
    ItemlessClassChange,

    /// <plugin>RAMod</plugin>
    Hidden,

    /// <plugin>PrivateSystems</plugin>
    LongOverwatch,

    /// <plugin>PrivateSystems</plugin>
    NoSpawnProtect,

    /// <plugin>API</plugin>
    Vanish,

    /// <plugin>PrivateSystems</plugin>
    PlayerPreferences,

    /// <plugin>CommandsExtender-Admin</plugin>
    AdminMark,

    /// <plugin>PrivateSystems</plugin>
    SpawnProtect,

    /// <summary>
    /// Used to grant SCPs access to human voice chat
    /// </summary>
    /// <plugin>BetterSCP</plugin>
    HumanVcAccess,

    /// <plugin>CustomRolesExtensions</plugin>
    BuiltinDoorAccess,

    /// <plugin>API</plugin>
    IgnoreScp207Damage,

    /// <plugin>PrivateSystems</plugin>
    OverwatchStartTime,

    /// <plugin>API</plugin>
    InfiniteAmmo,

    /// <plugin>BetterDoors</plugin>
    /// <returns><see cref="ItemPickupBase"/>, <see cref="KeycardPickup"/></returns>
    ThrowItem,

    /// <plugin>CommandsExtender-Admin</plugin>
    PostTalk,

    /// <plugin>RAMod</plugin>
    StreamerMode,

    /// <plugin>BetterSCP.SCP500</plugin>
    RespawnBlock,

    /// <plugin>Globaly used</plugin>
    BlockInventoryInteraction,
}
