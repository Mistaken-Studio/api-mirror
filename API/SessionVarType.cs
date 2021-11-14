// -----------------------------------------------------------------------
// <copyright file="SessionVarType.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Mistaken.API
{
    /// <summary>
    /// Session Vars.
    /// </summary>
    public enum SessionVarType
    {
        /// <plugin>.</plugin>
        [System.Obsolete("Not used, will be removed", true)]
        RUN_SPEED,

        /// <plugin>.</plugin>
        [System.Obsolete("Not used, will be removed", true)]
        WALK_SPEED,

        /// <plugin>CommandsExtender-Admin</plugin>
        TALK,

        /// <plugin>Globaly used</plugin>
        ITEM_LESS_CLSSS_CHANGE,

        /// <plugin>RAMod</plugin>
        HIDDEN,

        /// <plugin>PrivateSystems?</plugin>
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
    }
}
