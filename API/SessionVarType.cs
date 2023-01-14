using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;

namespace Mistaken.API
{
    /// <summary>
    /// Session Vars.
    /// </summary>
    public enum SessionVarType
    {
        /// <plugin>CommandsExtender-Admin</plugin>
        TALK = 2,

        /// <plugin>Globaly used</plugin>
        ITEM_LESS_CLSSS_CHANGE,

        /// <plugin>RAMod</plugin>
        HIDDEN,

        /// <plugin>PrivateSystems</plugin>
        LONG_OVERWATCH,

        /// <plugin>PrivateSystems</plugin>
        NO_SPAWN_PROTECT,

        /// <plugin>API</plugin>
        VANISH,

        /// <plugin>PrivateSystems</plugin>
        PLAYER_PREFERENCES,

        /// <plugin>CommandsExtender-Admin</plugin>
        ADMIN_MARK,

        /// <plugin>PrivateSystems</plugin>
        SPAWN_PROTECT,

        /// <summary>
        /// Used to grant SCPs access to human voice chat
        /// </summary>
        /// <plugin>BetterSCP</plugin>
        HUMAN_VC_ACCESS,

        /// <plugin>CustomRolesExtensions</plugin>
        BUILTIN_DOOR_ACCESS,

        /// <plugin>API</plugin>
        IGNORE_SCP207_DAMAGE,

        /// <plugin>PrivateSystems</plugin>
        OVERWATCH_START_TIME,

        /// <plugin>API</plugin>
        INFINITE_AMMO,

        /// <plugin>BetterDoors</plugin>
        /// <returns><see cref="ItemPickupBase"/>, <see cref="KeycardPickup"/></returns>
        THROWN_ITEM,

        /// <plugin>CommandsExtender-Admin</plugin>
        POST_TALK,

        /// <plugin>RAMod</plugin>
        STREAMER_MODE,

        /// <plugin>BetterSCP.SCP500</plugin>
        RESPAWN_BLOCK,

        /// <plugin>Globaly used</plugin>
        BLOCK_INVENTORY_INTERACTION,
    }
}
