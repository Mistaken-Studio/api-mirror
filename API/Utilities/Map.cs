// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using JetBrains.Annotations;
using MEC;

namespace Mistaken.API.Utilities
{
    /// <summary>
    /// Map Utilities.
    /// </summary>
    [PublicAPI]
    public static partial class Map
    {
        /// <summary>
        /// Event called before tesla mode change.
        /// </summary>
        public static event Action<TeslaMode> OnTeslaModeChange;

        /// <summary>
        /// Gets or sets a value indicating whether respawn should be locked or not.
        /// </summary>
        public static bool RespawnLock { get; set; }

        /// <summary>
        /// Gets or sets tesla mode.
        /// </summary>
        public static TeslaMode TeslaMode
        {
            get => teslaMode;
            set
            {
                OnTeslaModeChange?.Invoke(value);
                teslaMode = value;
            }
        }

        /// <summary>
        /// Fires all tesla gates 3 times.
        /// </summary>
        /// <param name="loud">If <see langword="true"/> CASSIE message will be played.</param>
        public static void RestartTeslaGates(bool loud) =>
            Diagnostics.Module.RunSafeCoroutine(RestartTeslaGatesMec(loud), "Utilities.API.Map.RestartTeslaGatesMec");

        /// <summary>
        /// Opens all doors.
        /// </summary>
        public static void OpenAllDoors()
        {
            foreach (var d in Door.List)
                d.IsOpen = true;
        }

        /// <summary>
        /// Closes all doors.
        /// </summary>
        public static void CloseAllDoors()
        {
            foreach (var d in Door.List)
                d.IsOpen = false;
        }

        /// <summary>
        /// Closes all doors with CASSIE message.
        /// </summary>
        public static void RestartDoors() =>
            Diagnostics.Module.RunSafeCoroutine(RestartDoorsMec(), "Utilities.API.Map.RestartDoorsMec");

        internal static void Restart()
        {
            TeslaMode = TeslaMode.ENABLED;
            RespawnLock = false;

            Blackout.Restart();
            Overheat.OverheatLevel = -1;
            Overheat.LockBlackout = false;
        }

        private static TeslaMode teslaMode;

        private static IEnumerator<float> RestartTeslaGatesMec(bool loud)
        {
            if (loud)
            {
                while (Cassie.IsSpeaking)
                    yield return Timing.WaitForOneFrame;
                Cassie.Message("FACILITY TESLA GATES REACTIVATION IN 3 . 2 . 1 . . ");
                yield return Timing.WaitForSeconds(8);
            }

            for (var i = 0; i < 5; i++)
            {
                Exiled.API.Features.TeslaGate.List.ToList().ForEach(tesla => tesla.ForceTrigger());
                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        private static IEnumerator<float> RestartDoorsMec()
        {
            while (Cassie.IsSpeaking)
                yield return Timing.WaitForOneFrame;

            Cassie.Message("FACILITY DOOR SYSTEM REACTIVATION IN 3 . 2 . 1 . . . . . PROCEDURE SUCCESSFUL");

            yield return Timing.WaitForSeconds(8);

            CloseAllDoors();
        }
    }
}
