using System.Collections.Generic;
using MEC;
using PlayerRoles;
using PluginAPI.Core;

namespace Mistaken.API.Utilities
{
    public static partial class Map
    {
        /// <summary>
        /// Overheat.
        /// </summary>
        public static class Overheat
        {
            /// <summary>
            /// Gets a value indicating whether blackout should be locked for SCP-079.
            /// </summary>
            public static bool LockBlackout { get; internal set; }

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
                        handle = Timing.RunCoroutine(HandleOverheat(RoundPlus.RoundId, ohLevel, ohLevel), nameof(HandleOverheat));
                }
            }

            private static int ohLevel = -1;
            private static CoroutineHandle? handle;

            private static IEnumerator<float> HandleOverheat(int roundId, int proggressLevel, int startLevel)
            {
                if (RoundPlus.RoundId != roundId)
                    yield break;

                if (!Round.IsRoundStarted)
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
                            foreach (var item in Facility.Rooms)
                                item.Lights.FlickerLights(3000); // TurnOffLights(3000)

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
                                foreach (var item in Facility.Rooms)
                                    item.Lights.FlickerLights(3000); // TurnOffLights(3000)

                                LockBlackout = true;
                            }
                            else
                            {
                                NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(
                                    "DANGER . REACTOR OVERHEAT STATUS .  REACTOR WILL OVERHEAT IN T MINUS 90 SECONDS . STARTING COUNTDOWN",
                                    0.40f,
                                    0.35f);
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

                                Cassie.Message(
                                    "T MINUS 60 SECONDS",
                                    false,
                                    false);
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

                                Cassie.Message(
                                    "T MINUS 30 SECONDS",
                                    false,
                                    false);
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

                                Cassie.Message(
                                    "10 SECONDS 9 . 8 . 7 . 6 . 5 . 4 . 3 . 2 . 1",
                                    false,
                                    false);
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
                            AlphaWarheadController.Singleton.InstantPrepare();
                            AlphaWarheadController.Singleton.StartDetonation();
                            AlphaWarheadController.Singleton.ForceTime(0.1f);
                            RespawnLock = false;
                            foreach (var player in Player.GetPlayers())
                            {
                                // Nie mam na to pomysłu
                                // player.ReferenceHub.playerStats.TargetReceiveSpecificDeathReason(new PlayerStatsSystem.CustomReasonDamageHandler("Facility Reactor"));
                                player.SetRole(RoleTypeId.Spectator, RoleChangeReason.Died);
                            }

                            Round.IsLocked = false;
                            LockBlackout = false;
                            MapPlus.SCP079Recontainer.BeginOvercharge();
                            break;
                        }

                    default:
                        {
                            handle = null;
                            yield break;
                        }
                }

                handle = Timing.RunCoroutine(HandleOverheat(roundId, proggressLevel + 1, startLevel), nameof(HandleOverheat));
            }
        }
    }
}
