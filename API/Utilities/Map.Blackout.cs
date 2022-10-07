// -----------------------------------------------------------------------
// <copyright file="Map.Blackout.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Exiled.API.Features;
using JetBrains.Annotations;
using MEC;

// ReSharper disable InconsistentNaming
namespace Mistaken.API.Utilities
{
    public static partial class Map
    {
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

                    handle = Diagnostics.Module.RunSafeCoroutine(ExecuteBlackout(), "Utilities.API.Map.ExecuteBlackout");
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
            public static bool OnlyHCZ { get; set; }

            // ReSharper disable once MemberHidesStaticFromOuterClass
            internal static void Restart()
            {
                Enabled = false;
                Length = 10;
                Delay = 10;
                OnlyHCZ = false;
            }

            private static CoroutineHandle? handle;
            private static bool enabled;

            private static IEnumerator<float> ExecuteBlackout()
            {
                while (Enabled)
                {
                    try
                    {
                        foreach (var item in Exiled.API.Features.Room.List)
                        {
                            if (!OnlyHCZ || item.Zone == Exiled.API.Enums.ZoneType.HeavyContainment)
                                item.TurnOffLights(Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                        Log.Error(ex.StackTrace);
                    }

                    yield return Timing.WaitForSeconds(Delay);
                }
            }
        }
    }
}
