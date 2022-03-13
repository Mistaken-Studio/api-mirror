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
using Utils.Networking;

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
                d.IsOpen = true;
        }

        /// <summary>
        /// Closes all doors.
        /// </summary>
        public static void CloseAllDoors()
        {
            foreach (var d in Exiled.API.Features.Map.Doors)
                d.IsOpen = false;
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

            private static CoroutineHandle? handle;
            private static bool enabled = false;

            private static IEnumerator<float> ExecuteBlackout()
            {
                while (Enabled)
                {
                    try
                    {
                        foreach (var item in Exiled.API.Features.Map.Rooms)
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
            private static CoroutineHandle? handle;

            private const string Overheat_Detected_Evacuate_Alert = "ALERT ALERT .";
            private const string Overheat_Detected_Evacuate_Alert_Transcript = "<color=red>ALERT ALERT</color>, ";

            private const string Overheat_Minutes = "MINUTES";
            private const string Overheat_Minutes_Transcript = "MINUTES</color>";

            private const string Overheat_Seconds = "SECONDS";
            private const string Overheat_Seconds_Transcript = "SECONDS</color>";

            private const string Overheat_Detected_Evacuate_Detected = "DETECTED FACILITY REACTOR CORE OVERHEAT . REACTOR WILL OVERHEAT IN TMINUS ";
            private const string Overheat_Detected_Evacuate_Detected_Transcript = "DETECTED FACILITY REACTOR CORE OVERHEAT, REACTOR WILL OVERHEAT IN T-MINUS <color=yellow>";

            private const string Overheat_Detected_Evacuate_Message = ". ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT . IT WILL CAUSE THERMAL EXPLOSION OF FACILITY";
            private const string Overheat_Detected_Evacuate_Message_Transcript = "<split> ALL PERSONNEL HAVE TO EVACUATE UNTIL OVERHEAT, IT WILL CAUSE THERMAL EXPLOSION OF FACILITY";

            private static string Overheat_Begin_Message => $"{Overheat_Detected_Evacuate_Alert} {Overheat_Detected_Evacuate_Detected} {{0}} {{1}} {Overheat_Detected_Evacuate_Message}";

            private static string Overheat_Begin_Message_Transcript => $"{Overheat_Detected_Evacuate_Alert_Transcript} {Overheat_Detected_Evacuate_Detected_Transcript} {{0}} {{1}} {Overheat_Detected_Evacuate_Message_Transcript}";

            private static string Overheat_Update_Message => $"DANGER . REACTOR OVERHEAT STATUS .  REACTOR WILL OVERHEAT IN TMINUS {{0}} {{1}}";

            private static string Overheat_Update_Message_Transcript => $"<color=red>DANGER</color>, REACTOR OVERHEAT STATUS <split> REACTOR WILL OVERHEAT IN T-MINUS <color=yellow>{{0}} {{1}}";

            private static string Overheat_LightSystem_Message => "FACILITY LIGHT SYSTEM CRITICAL DAMAGE . LIGHTS OUT";

            private static string Overheat_LightSystem_Message_Transcript => "FACILITY LIGHT SYSTEM CRITICAL DAMAGE . LIGHTS OUT";

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
                                string.Format(Overheat_Begin_Message, "30", Overheat_Minutes),
                                0.15f,
                                0.10f);
                            new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                            {
                                new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Begin_Message, "30", Overheat_Minutes) }),
                            }).SendToAuthenticated();
                            Log.Debug("HMMMMM");
                            Log.Debug(string.Format(Overheat_Begin_Message_Transcript, "30", Overheat_Minutes_Transcript).Replace("<", "[").Replace(">", "]"));
                            yield return Timing.WaitForSeconds(300);
                            break;
                        }

                    case 1:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Begin_Message, "25", Overheat_Minutes),
                                    0.15f,
                                    0.10f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Begin_Message_Transcript, "25", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Update_Message, "25", Overheat_Minutes),
                                    0.20f,
                                    0.15f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Update_Message_Transcript, "25", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }

                            yield return Timing.WaitForSeconds(300);
                            break;
                        }

                    case 2:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                        string.Format(Overheat_Begin_Message, "20", Overheat_Minutes),
                                        0.15f,
                                        0.10f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Begin_Message_Transcript, "20", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Update_Message, "20", Overheat_Minutes),
                                    0.20f,
                                    0.15f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Update_Message_Transcript, "20", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }

                            yield return Timing.WaitForSeconds(300);
                            break;
                        }

                    case 3:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Begin_Message, "15", Overheat_Minutes),
                                    0.20f,
                                    0.15f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Begin_Message_Transcript, "15", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Update_Message, "15", Overheat_Minutes),
                                    0.20f,
                                    0.15f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Update_Message_Transcript, "15", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }

                            yield return Timing.WaitForSeconds(300);
                            break;
                        }

                    case 4:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Begin_Message, "10", Overheat_Minutes),
                                    0.25f,
                                    0.20f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Begin_Message_Transcript, "10", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Update_Message, "10", Overheat_Minutes),
                                    0.25f,
                                    0.20f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Update_Message_Transcript, "10", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }

                            yield return Timing.WaitForSeconds(300);
                            break;
                        }

                    case 5:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Begin_Message, "5", Overheat_Minutes),
                                    0.30f,
                                    0.25f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Begin_Message_Transcript, "5", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Update_Message, "5", Overheat_Minutes),
                                    0.30f,
                                    0.25f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Update_Message_Transcript, "5", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }

                            yield return Timing.WaitForSeconds(120);
                            break;
                        }

                    case 6:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Begin_Message, "3", Overheat_Minutes),
                                    0.35f,
                                    0.30f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Begin_Message_Transcript, "3", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Update_Message, "3", Overheat_Minutes),
                                    0.35f,
                                    0.30f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Update_Message_Transcript, "3", Overheat_Minutes_Transcript) }),
                                }).SendToAuthenticated();
                            }

                            RespawnLock = true;
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                Overheat_LightSystem_Message,
                                0.35f,
                                0.30f);
                            new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                            {
                                new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { Overheat_LightSystem_Message_Transcript }),
                            }).SendToAuthenticated();
                            foreach (var item in Exiled.API.Features.Map.Rooms)
                                item.TurnOffLights(3000);
                            LockBlackout = true;
                            yield return Timing.WaitForSeconds(90);
                            break;
                        }

                    case 7:
                        {
                            while (Cassie.IsSpeaking)
                                yield return Timing.WaitForOneFrame;
                            if (startLevel == proggressLevel)
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Begin_Message, "90", Overheat_Seconds),
                                    0.40f,
                                    0.35f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Begin_Message_Transcript, "90", Overheat_Seconds_Transcript) }),
                                }).SendToAuthenticated();
                                RespawnLock = true;
                                while (Cassie.IsSpeaking)
                                    yield return Timing.WaitForOneFrame;
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    Overheat_LightSystem_Message,
                                    0.35f,
                                    0.30f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { Overheat_LightSystem_Message_Transcript }),
                                }).SendToAuthenticated();
                                foreach (var item in Exiled.API.Features.Map.Rooms)
                                    item.TurnOffLights(3000);
                                LockBlackout = true;
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    string.Format(Overheat_Update_Message, "90", Overheat_Seconds) + " . STARTING COUNTDOWN",
                                    0.40f,
                                    0.35f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { string.Format(Overheat_Update_Message_Transcript, "90", Overheat_Seconds_Transcript) + ", STARTING COUNTDOWN" }),
                                }).SendToAuthenticated();
                            }

                            yield return Timing.WaitForSeconds(30);
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
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                   "TMINUS 60 SECONDS",
                                   0.40f,
                                   0.35f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { "T-MINUS <color=yellow>60 SECONDS</color>" }),
                                }).SendToAuthenticated();
                            }

                            yield return Timing.WaitForSeconds(30);
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
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                   "TMINUS 30 SECONDS",
                                   0.40f,
                                   0.35f);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { "T-MINUS <color=yellow>30 SECONDS</color>" }),
                                }).SendToAuthenticated();
                            }

                            yield return Timing.WaitForSeconds(20);
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

                                Respawning.RespawnEffectsController.PlayCassieAnnouncement(
                                    "10 SECONDS 9 yield_1 8 yield_1 7 yield_1 6 yield_1 5 yield_1 4 yield_1 3 yield_1 2 yield_1 1",
                                    false,
                                    false);
                                new Subtitles.SubtitleMessage(new Subtitles.SubtitlePart[]
                                {
                                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[] { "10 SECONDS, 9, 8, 7, 6, 5, 4, 3, 2, 1" }),
                                }).SendToAuthenticated();
                            }

                            yield return Timing.WaitForSeconds(5);
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
                            yield return Timing.WaitForSeconds(1);
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
                                player.ReferenceHub.playerStats.TargetReceiveSpecificDeathReason(new PlayerStatsSystem.CustomReasonDamageHandler("Facility Reactor"));
                                player.Role = RoleType.Spectator;
                            }

                            Round.IsLocked = false;
                            LockBlackout = false;
                            UnityEngine.Object.FindObjectOfType<Recontainer079>().BeginOvercharge();
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
                Respawning.RespawnEffectsController.PlayCassieAnnouncement(
                    "FACILITY TESLA GATES REACTIVATION IN 3 . 2 . 1 . . ",
                    false,
                    false,
                    true);
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
            Respawning.RespawnEffectsController.PlayCassieAnnouncement(
                    "FACILITY DOOR SYSTEM REACTIVATION IN 3 . 2 . 1 . . . . . PROCEDURE SUCCESSFUL",
                    false,
                    false,
                    true);
            yield return Timing.WaitForSeconds(8);
            CloseAllDoors();
        }
    }
}
