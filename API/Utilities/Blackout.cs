using JetBrains.Annotations;
using MapGeneration;
using MEC;
using PluginAPI.Core;
using System;
using System.Collections.Generic;

namespace Mistaken.API.Utilities;

/// <summary>
/// Blackout utilities.
/// </summary>
[PublicAPI]
public static class Blackout
{
    /// <summary>
    /// Gets or sets a value indicating whether blackout is enabled or not.
    /// </summary>
    public static bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            if (!value)
                return;

            if (handle.HasValue)
                Timing.KillCoroutines(handle.Value);

            handle = Timing.RunCoroutine(ExecuteBlackout(), nameof(ExecuteBlackout));
        }
    }

    /// <summary>
    /// Gets or sets how long is single blackout.
    /// </summary>
    public static float Length { get; set; } = 10;

    /// <summary>
    /// Gets or sets a delay between blackouts.
    /// </summary>
    public static float Delay { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether blackout is only in HCZ.
    /// </summary>
    public static bool OnlyHCZ { get; set; } = false;

    private static CoroutineHandle? handle;
    private static bool enabled;

    private static IEnumerator<float> ExecuteBlackout()
    {
        while (Enabled)
        {
            try
            {
                foreach (var item in Facility.Rooms)
                {
                    if (!OnlyHCZ || item.Zone.ZoneType == FacilityZone.HeavyContainment)
                        item.Lights.FlickerLights(Length); // TurnOffLights(Length)
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            yield return Timing.WaitForSeconds(Delay);
        }
    }
}
