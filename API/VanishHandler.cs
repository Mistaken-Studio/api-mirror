// -----------------------------------------------------------------------
// <copyright file="VanishHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;

namespace Mistaken.API
{
    /// <inheritdoc/>
    public class VanishHandler : Module
    {
        /// <summary>
        /// Gets list of players with active ghostmode and their levels.
        /// </summary>
        public static Dictionary<int, int> Vanished { get; } = new Dictionary<int, int>();

        /// <summary>
        /// Sets GhostMode status for <paramref name="player"/>.
        ///     <list type="table">
        ///         <item>Level 1 -> All admins</item>
        ///         <item>Level 2 -> <see cref="RoleType.Tutorial"/> and <see cref="RoleType.Spectator"/> admins only</item>
        ///         <item>Level 3 -> Greather or equal <see cref="UserGroup.KickPower"/> admins only</item>
        ///     </list>
        /// </summary>
        /// <param name="player">Target player.</param>
        /// <param name="value">New state.</param>
        /// <param name="level">New level (Only used when <paramref name="value"/> is <see langword="true"/>).</param>
        /// <param name="silent">If <see langword="true"/> then <see cref="AnnonymousEvents"/> will not be fired.</param>
        public static void SetGhost(Player player, bool value, byte level = 1, bool silent = false)
        {
            if (value)
            {
                if (Vanished.ContainsKey(player.Id))
                    SetGhost(player, false, level, true);
                Vanished.Add(player.Id, level);
                player.SetSessionVar(SessionVarType.VANISH, level);
                if (!silent)
                    AnnonymousEvents.Call("VANISH", (player, level));
            }
            else
            {
                if (Vanished.ContainsKey(player.Id))
                {
                    if (!silent)
                        AnnonymousEvents.Call("VANISH", (player, (byte)0));
                }

                Vanished.Remove(player.Id);
                player.SetSessionVar(SessionVarType.VANISH, (byte)0);
            }
        }

        /// <inheritdoc/>
        public override string Name => "Vanish";

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Server.RestartingRound += this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
            Exiled.Events.Handlers.Player.ChangingRole += this.Handle<Exiled.Events.EventArgs.ChangingRoleEventArgs>((ev) => this.Player_ChangingRole(ev));
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Server.RestartingRound -= this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
            Exiled.Events.Handlers.Player.ChangingRole -= this.Handle<Exiled.Events.EventArgs.ChangingRoleEventArgs>((ev) => this.Player_ChangingRole(ev));
        }

        internal VanishHandler(PluginHandler p)
            : base(p)
        {
        }

        private static bool HasAdminChat(ulong perm)
        {
            if (perm == 0)
                return false;
            return (perm & (ulong)PlayerPermissions.AdminChat) != 0;
        }

        private static void Hide(Player seeingPlayer, int seenPlayer)
        {
            seeingPlayer.TargetGhostsHashSet.Add(seenPlayer);
        }

        private void Server_RoundStarted()
        {
            this.RunCoroutine(this.IRoundStarted(), "IRoundStarted");
        }

        private void Server_RestartingRound()
        {
            Vanished.Clear();
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.Player == null)
                return;
            try
            {
                SetGhost(ev.Player, false);
                if (Vanished.ContainsKey(ev.Player.Id))
                {
                    SetGhost(ev.Player, false);
                    ev.Player.Broadcast("VANISH", 10, "Ghostmode: <color=red><b>DISABLED</b></color>");
                }
            }
            catch (Exception ex)
            {
                this.Log.Error(ex.Message);
                this.Log.Error(ex.StackTrace);
            }
        }

        private IEnumerator<float> IRoundStarted()
        {
            yield return Timing.WaitForSeconds(1);
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(1);
                var start = DateTime.Now;
                foreach (var player in RealPlayers.List)
                {
                    player.TargetGhostsHashSet.Clear();
                    foreach (var seenPlayer in Player.List)
                    {
                        if (Vanished.Count == 0)
                            continue;
                        if (seenPlayer == player)
                            continue;
                        if (!Vanished.TryGetValue(seenPlayer.Id, out int level))
                            continue;
                        if (level == 1)
                        {
                            if (!HasAdminChat(player.ReferenceHub.serverRoles.Permissions))
                                Hide(player, seenPlayer.Id);
                        }
                        else if (level == 2)
                        {
                            if (!HasAdminChat(player.ReferenceHub.serverRoles.Permissions) || !(player.Team == Team.TUT || player.Team == Team.RIP))
                                Hide(player, seenPlayer.Id);
                        }
                        else if (level == 3)
                        {
                            if (!HasAdminChat(player.ReferenceHub.serverRoles.Permissions) || player.ReferenceHub.serverRoles.KickPower < seenPlayer.ReferenceHub.serverRoles.KickPower)
                                Hide(player, seenPlayer.Id);
                        }
                    }
                }

                MasterHandler.LogTime("VanisHandler", "IRoundStarted", start, DateTime.Now);
            }
        }
    }
}
