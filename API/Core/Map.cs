using JetBrains.Annotations;
using LightContainmentZoneDecontamination;
using Mistaken.API.Enums;
using PlayerRoles.PlayableScps.Scp079;
using PluginAPI.Core;
using System.Collections.Generic;
using System.Linq;

namespace Mistaken.API.Core;

/// <summary>
/// Map Utilities.
/// </summary>
[PublicAPI]
public static class Map
{
    /// <summary>
    /// Gets or sets a value indicating whether respawn should be locked or not.
    /// </summary>
    public static bool RespawnLock { get; set; } = false;

    /// <summary>
    /// Gets or sets tesla mode.
    /// </summary>
    public static TeslaMode TeslaMode { get; set; } = TeslaMode.Enabled;

    /// <summary>
    /// List of Player Ragdolls.
    /// </summary>
    public static List<BasicRagdoll> Ragdolls { get; set; } = new();

    /// <summary>
    /// Gets time to decontamination end.
    /// </summary>
    public static float DecontaminationEndTime =>
        DecontaminationController.Singleton.DecontaminationPhases
            .First(i => i.Function == DecontaminationController.DecontaminationPhase.PhaseFunction.Final)
            .TimeTrigger;

    /// <summary>
    /// Gets a value indicating whether SCP079's recontainment is in progresses.
    /// </summary>
    public static bool IsScp079ReadyForRecontainment => Scp079Recontainer._prevEngaged == 3;

    /// <summary>
    /// Gets a value indicating whether SCP079 recontainment has finished.
    /// </summary>
    public static bool IsScp079Recontained => Scp079Recontainer._alreadyRecontained;

    /// <summary>
    /// <see cref="PlayerRoles.PlayableScps.Scp079.Scp079Recontainer"/> instance.
    /// </summary>
    public static Scp079Recontainer Scp079Recontainer;

    /// <summary>
    /// Is LCZ Decontaminated.
    /// </summary>
    /// <param name="minTimeLeft">Offset.</param>
    /// <returns>If LCZ is Decontaminated.</returns>
    public static bool IsLCZDecontaminated(float minTimeLeft = 0)
        => IsLCZDecontaminated(out _, minTimeLeft);

    /// <summary>
    /// If LCZ was decontaminated with out param.
    /// </summary>
    /// <param name="lczTime">Time to decontamination.</param>
    /// <param name="minTimeLeft">Offset.</param>
    /// <returns>If LCZ is decontaminated.</returns>
    public static bool IsLCZDecontaminated(out float lczTime, float minTimeLeft = 0)
    {
        lczTime = DecontaminationEndTime - (float)LightContainmentZoneDecontamination.DecontaminationController.GetServerTime;
        return lczTime < minTimeLeft;
    }

    /// <summary>
    /// Opens all doors.
    /// </summary>
    public static void OpenAllDoors()
    {
        foreach (var door in Facility.Doors)
            door.IsOpened = true;
    }

    /// <summary>
    /// Closes all doors.
    /// </summary>
    public static void CloseAllDoors()
    {
        foreach (var door in Facility.Doors)
            door.IsOpened = false;
    }
}
