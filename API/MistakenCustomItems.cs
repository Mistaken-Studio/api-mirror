// -----------------------------------------------------------------------
// <copyright file="MistakenCustomItems.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Mistaken.API
{
    /// <summary>
    /// Enum containing list of all custom items made by Mistaken Devs.
    /// Used for storing item ids.
    /// </summary>
    public enum MistakenCustomItems : uint
    {
        /// <plugin>CustomScientists</plugin>
        ZONE_MANAGER_KEYCARD = 4026531840,

        /// <plugin>CustomScientists</plugin>
        DEPUTY_FACILITY_MANAGER_KEYCARD,

        /// <plugin>CustomMTF</plugin>
        GUARD_COMMANDER_KEYCARD,

        /// <plugin>CustomMTF</plugin>
        GUARD_COMMANDER_KEYCARD_OWNER,

        /// <plugin>CustomMTF</plugin>
        STICKY_GRENADE,

        /// <plugin>CustomMTF</plugin>
        GRENADE_LAUNCHER,

        /// <plugin>CustomMTF</plugin>
        MEDIC_GUN,

        /// <plugin>SCP-1499</plugin>
        SCP_1499,

        /// <plugin>Impact Grenade</plugin>
        IMPACT_GRENADE,

        /// <plugin>Taser</plugin>
        TASER,

        /// <plugin>SNav</plugin>
        SNAV_3000,

        /// <plugin>SNav</plugin>
        SNAV_ULTIMATE,
    }
}
