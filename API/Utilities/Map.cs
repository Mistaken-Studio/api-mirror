// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;

namespace Mistaken.API.Utilities
{
    /// <summary>
    /// Map Utilities.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// Event called before tesla mode change.
        /// </summary>
        public static event Action<TeslaMode> OnTeslaModeChange;

        /// <summary>
        /// Gets or sets a value indicating whether respawns should be locked or not.
        /// </summary>
        public static bool RespawnLock { get; set; } = false;

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
            Diagnostics.Module.RunSafeCoroutine(IRestartTeslaGates(loud), "Utilities.API.Map.IRestartTeslaGates");

        /// <summary>
        /// Opens all doors.
        /// </summary>
        public static void OpenAllDoors()
        {
            foreach (var d in Exiled.API.Features.Map.Doors)
                d.NetworkTargetState = true;
        }

        /// <summary>
        /// Closes all doors.
        /// </summary>
        public static void CloseAllDoors()
        {
            foreach (var d in Exiled.API.Features.Map.Doors)
                d.NetworkTargetState = false;
        }

        /// <summary>
        /// Closes all doors with CASSIE message.
        /// </summary>
        public static void RestartDoors() =>
            Diagnostics.Module.RunSafeCoroutine(IRestartDoors(), "Utilities.API.Map.IRestartDoors");

        /// <summary>
        /// Blackout utilities.
        /// </summary>
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
                    if (value)
                    {
                        if (handle.HasValue)
                            Timing.KillCoroutines(handle.Value);
                        handle = Diagnostics.Module.RunSafeCoroutine(ExecuteBlackout(), "Utilities.API.Map.ExecuteBlackout");
                    }
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

            internal static void Restart()
            {
                Enabled = false;
                Length = 10;
                Delay = 10;
                OnlyHCZ = false;
            }

            private static MEC.CoroutineHandle? handle;
            private static bool enabled = false;

            private static IEnumerator<float> ExecuteBlackout()
            {
                while (Map.Blackout.Enabled)
                {
                    try
                    {
                        Generator079.mainGenerator.ServerOvercharge(Map.Blackout.Length, Map.Blackout.OnlyHCZ);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex.Message);
                        Log.Error(ex.StackTrace);
                    }

                    yield return MEC.Timing.WaitForSeconds(Map.Blackout.Delay);
                }
            }
        }

        /// <summary>
        /// Overheat.
        /// </summary>
        public static class Overheat
        {
            /// <summary>
            /// Gets a value indicating whether blackout should be locked for SCP-079.
            /// </summary>
            public static bool LockBlackout { get; internal set; } = false;

            /// <summary>
            /// Gets or sets current OverheatLevel.
            /// </summary>
            public static int OverheatLevel
            {
                get => ohLevel;
                set
                {
                    ohLevel = value;
                    if (handle.HasValue)
                        Timing.KillCoroutines(handle.Value);
                    if (ohLevel != -1)
                        handle = Diagnostics.Module.RunSafeCoroutine(HandleOverheat(RoundPlus.RoundId, ohLevel, ohLevel), "Utilities.API.Map.HandleOverheat");
                }
            }

            private static int ohLevel = -1;
            private static MEC.CoroutineHandle? handle;

            private static IEnumerator<float> HandleOverheat(int roundId, int proggressLevel, int startLevel)
            {
                if (RoundPlus.RoundId != roundId)
                    yield break;
                if (!Round.IsStarted)
                    yield break;
                ohLevel = proggressLevel;
                switch (proggressLevel)
                {
                    case -1:
                        yield break;
                    case 0:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                "ALERT  ALERT .  DETECTED FACILITY REACTOR CORE OVERHEAT . REACTOR WILL OVERHEAT IN T MINUS 30 MINUTES . ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT . IT WILL CAUSE THERMAL EXPLOSION OF FACILITY",
                                0.15f,
                                0.10f);
                            yield return MEC.Timing.WaitForSeconds(300);
                            break;
                        }

                    case 1:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "ALERT  ALERT .  DETECTED FACILITY REACTOR CORE OVERHEAT . REACTOR WILL OVERHEAT IN T MINUS 25 MINUTES . ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT . IT WILL CAUSE THERMAL EXPLOSION OF FACILITY",
                                    0.15f,
                                    0.10f);
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "DANGER . REACTOR OVERHEAT STATUS .  REACTOR WILL OVERHEAT IN T MINUS 25 MINUTES",
                                    0.20f,
                                    0.15f);
                            }

                            yield return MEC.Timing.WaitForSeconds(300);
                            break;
                        }

                    case 2:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                        "ALERT  ALERT .  DETECTED FACILITY REACTOR CORE OVERHEAT . REACTOR WILL OVERHEAT IN T MINUS 20 MINUTES . ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT . IT WILL CAUSE THERMAL EXPLOSION OF FACILITY",
                                        0.15f,
                                        0.10f);
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "DANGER . REACTOR OVERHEAT STATUS .  REACTOR WILL OVERHEAT IN T MINUS 20 MINUTES",
                                    0.20f,
                                    0.15f);
                            }

                            yield return MEC.Timing.WaitForSeconds(300);
                            break;
                        }

                    case 3:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "ALERT  ALERT .  DETECTED FACILITY REACTOR CORE OVERHEAT . REACTOR WILL OVERHEAT IN T MINUS 15 MINUTES . ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT . IT WILL CAUSE THERMAL EXPLOSION OF FACILITY",
                                    0.20f,
                                    0.15f);
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "DANGER . REACTOR OVERHEAT STATUS .  REACTOR WILL OVERHEAT IN T MINUS 15 MINUTES",
                                    0.20f,
                                    0.15f);
                            }

                            yield return MEC.Timing.WaitForSeconds(300);
                            break;
                        }

                    case 4:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "ALERT  ALERT .  DETECTED FACILITY REACTOR CORE OVERHEAT . REACTOR WILL OVERHEAT IN T MINUS 10 MINUTES . ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT . IT WILL CAUSE THERMAL EXPLOSION OF FACILITY",
                                    0.25f,
                                    0.20f);
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "DANGER . REACTOR OVERHEAT STATUS .  REACTOR WILL OVERHEAT IN T MINUS 10 MINUTES",
                                    0.25f,
                                    0.20f);
                            }

                            yield return MEC.Timing.WaitForSeconds(300);
                            break;
                        }

                    case 5:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "ALERT  ALERT .  DETECTED FACILITY REACTOR CORE OVERHEAT . REACTOR WILL OVERHEAT IN T MINUS 5 MINUTES . ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT . IT WILL CAUSE THERMAL EXPLOSION OF FACILITY",
                                    0.30f,
                                    0.25f);
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "DANGER . REACTOR OVERHEAT STATUS .  REACTOR WILL OVERHEAT IN T MINUS 5 MINUTES",
                                    0.30f,
                                    0.25f);
                            }

                            yield return MEC.Timing.WaitForSeconds(120);
                            break;
                        }

                    case 6:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "ALERT  ALERT .  DETECTED FACILITY REACTOR CORE OVERHEAT . REACTOR WILL OVERHEAT IN T MINUS 3 MINUTES . ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT . IT WILL CAUSE THERMAL EXPLOSION OF FACILITY",
                                    0.35f,
                                    0.30f);
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "DANGER . REACTOR OVERHEAT STATUS .  REACTOR WILL OVERHEAT IN T MINUS 3 MINUTES",
                                    0.35f,
                                    0.30f);
                            }

                            RespawnLock = true;
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                "FACILITY LIGHT SYSTEM CRITICAL DAMAGE . LIGHTS OUT",
                                0.35f,
                                0.30f);
                            Generator079.mainGenerator.ServerOvercharge(3000, false);
                            LockBlackout = true;
                            yield return MEC.Timing.WaitForSeconds(90);
                            break;
                        }

                    case 7:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "ALERT  ALERT .  DETECTED FACILITY REACTOR CORE OVERHEAT . REACTOR WILL OVERHEAT IN T MINUS 90 SECONDS . ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT . IT WILL CAUSE THERMAL EXPLOSION OF FACILITY",
                                    0.40f,
                                    0.35f);
                                RespawnLock = true;
                                while (Cassie.IsSpeaking)
                                    yield return Timing.WaitForOneFrame;
                                Cassie.Message(
                                    "FACILITY LIGHT SYSTEM CRITICAL DAMAGE . LIGHTS OUT",
                                    false,
                                    false);
                                Generator079.mainGenerator.ServerOvercharge(3000, false);
                                LockBlackout = true;
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "DANGER . REACTOR OVERHEAT STATUS .  REACTOR WILL OVERHEAT IN T MINUS 90 SECONDS . STARTING COUNTDOWN",
                                    0.40f,
                                    0.35f);
                            }

                            yield return MEC.Timing.WaitForSeconds(30);
                            break;
                        }

                    case 8:
                        {
                            if (startLevel == proggressLevel)
                                yield break;
                            else
                            {
                                while (Cassie.IsSpeaking)
                                    yield return Timing.WaitForOneFrame;
                                Cassie.Message(
                                    "T MINUS 60 SECONDS",
                                    false,
                                    false);
                            }

                            yield return MEC.Timing.WaitForSeconds(30);
                            break;
                        }

                    case 9:
                        {
                            if (startLevel == proggressLevel)
                                yield break;
                            else
                            {
                                while (Cassie.IsSpeaking)
                                    yield return Timing.WaitForOneFrame;
                                Cassie.Message(
                                    "T MINUS 30 SECONDS",
                                    false,
                                    false);
                            }

                            yield return MEC.Timing.WaitForSeconds(20);
                            break;
                        }

                    case 10:
                        {
                            if (startLevel == proggressLevel)
                                yield break;
                            else
                            {
                                while (Cassie.IsSpeaking)
                                    yield return Timing.WaitForOneFrame;
                                Cassie.Message(
                                    "10 SECONDS 9 . 8 . 7 . 6 . 5 . 4 . 3 . 2 . 1",
                                    false,
                                    false);
                            }

                            yield return MEC.Timing.WaitForSeconds(5);
                            break;
                        }

                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                        {
                            if (startLevel == proggressLevel)
                                yield break;
                            else
                                Warhead.Shake();
                            yield return MEC.Timing.WaitForSeconds(1);
                            break;
                        }

                    case 16:
                        {
                            AlphaWarheadController.Host.InstantPrepare();
                            AlphaWarheadController.Host.StartDetonation();
                            AlphaWarheadController.Host.NetworktimeToDetonation = 0.1f;
                            RespawnLock = false;
                            foreach (var player in RealPlayers.List)
                            {
                                player.ReferenceHub.characterClassManager.TargetDeathScreen(player.Connection, new PlayerStats.HitInfo(-1f, "*Facility Reactor", new DamageTypes.DamageType("Facility Reactor"), 0));
                                player.Role = RoleType.Spectator;
                            }

                            Round.IsLocked = false;
                            LockBlackout = false;
                            Generator079.mainGenerator.ServerOvercharge(0, false);
                            break;
                        }

                    default:
                        {
                            handle = null;
                            yield break;
                        }
                }

                handle = Diagnostics.Module.RunSafeCoroutine(HandleOverheat(roundId, proggressLevel + 1, startLevel), "Utilities.API.Map.HandleOverheat");
            }
        }

        internal static void Restart()
        {
            TeslaMode = TeslaMode.ENABLED;
            RespawnLock = false;

            Blackout.Restart();
            Overheat.OverheatLevel = -1;
            Overheat.LockBlackout = false;
        }

        private static TeslaMode teslaMode;

        private static IEnumerator<float> IRestartTeslaGates(bool loud)
        {
            if (loud)
            {
                while (Cassie.IsSpeaking)
                    yield return Timing.WaitForOneFrame;
                Cassie.Message("FACILITY TESLA GATES REACTIVATION IN 3 . 2 . 1 . . ");
                yield return Timing.WaitForSeconds(8);
            }

            for (int i = 0; i < 5; i++)
            {
                Exiled.API.Features.Map.TeslaGates.ToList().ForEach(tesla => tesla.RpcInstantBurst());
                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        private static IEnumerator<float> IRestartDoors()
        {
            while (Cassie.IsSpeaking)
                yield return Timing.WaitForOneFrame;
            Exiled.API.Features.Cassie.Message("FACILITY DOOR SYSTEM REACTIVATION IN 3 . 2 . 1 . . . . . PROCEDURE SUCCESSFUL", false, true);
            yield return Timing.WaitForSeconds(8);
            CloseAllDoors();
        }
    }
}
