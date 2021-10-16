// -----------------------------------------------------------------------
// <copyright file="MistakenCustomRoles.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Mistaken.API.CustomRoles
{
    /// <summary>
    /// Enum containing list of all custom roles made by Mistaken Devs.
    /// Used for storing role ids.
    /// </summary>
    public enum MistakenCustomRoles : uint
    {
        /// <plugin>CustomScientists</plugin>
        ZONE_MANAGER = 4026531840,

        /// <plugin>CustomScientists</plugin>
        DEPUTY_FACILITY_MANAGER,

        /// <plugin>CustomMTF</plugin>
        MTF_MEDIC,

        /// <plugin>CustomMTF</plugin>
        MTF_EXPLOSIVE_SPECIALIST,

        /// <plugin>CustomMTF</plugin>
        MTF_CONTAINMENT_ENGINNER,

        /// <plugin>CustomMTF</plugin>
        GUARD_COMMANDER,

        /// <plugin>Tau-5</plugin>
        TAU_5,
    }
}
