// -----------------------------------------------------------------------
// <copyright file="ShieldedManager.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Exiled.API.Features;
using MEC;
using Mistaken.API.Diagnostics;

namespace Mistaken.API.Shield
{
#pragma warning disable
    /// <inheritdoc/>
    [System.Obsolete("Use script", true)]
    public class ShieldedManager : Module
    {
        public override bool Enabled => false;

        /// <summary>
        /// Adds shield.
        /// </summary>
        /// <param name="shielded">Shield to add.</param>
        public static void Add(Shielded shielded)
        {
            Remove(shielded.player);
            Shieldeds[shielded.player] = shielded;
        }

        /// <summary>
        /// Removes shield for player.
        /// </summary>
        /// <param name="player">Player to remove shield for.</param>
        public static void Remove(Player player)
        {
            if (Shieldeds.ContainsKey(player))
            {
                Shieldeds[player].Disable();
                Shieldeds.Remove(player);
            }
        }

        /// <summary>
        /// Checks if <paramref name="player"/> has shield.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns>If player has shield.</returns>
        public static bool Has(Player player)
            => Shieldeds.ContainsKey(player);

        /// <summary>
        /// Gets <paramref name="player"/>'s shield or <see langword="null"/> has none.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns>Shield.</returns>
        public static Shielded Get(Player player)
            => TryGet(player, out var value) ? value : null;

        /// <summary>
        /// Tries to get <paramref name="player"/>'s shield.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <param name="result">Shield.</param>
        /// <returns>If player has shield.</returns>
        public static bool TryGet(Player player, out Shielded result)
            => Shieldeds.TryGetValue(player, out result);

        /// <inheritdoc/>
        public override bool IsBasic => true;

        /// <inheritdoc/>
        public override string Name => "ShieldManager";

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Server.RestartingRound += this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Server.RestartingRound -= this.Handle(() => this.Server_RestartingRound(), "RoundRestart");

            foreach (var item in Shieldeds.Values.ToArray())
                item.Disable();
        }

        internal static readonly Dictionary<Player, Shielded> Shieldeds = new Dictionary<Player, Shielded>();

        internal ShieldedManager(PluginHandler p)
            : base(p)
        {
            this.Server_RestartingRound();
        }

        private static IEnumerator<float> ExecuteCycle()
        {
            yield return Timing.WaitForSeconds(1);
            int rid = RoundPlus.RoundId;
            while (Round.IsStarted && rid == RoundPlus.RoundId)
            {
                foreach (var shielded in Shieldeds.Values.ToArray())
                    shielded.DoRegenerationCicle();
                yield return Timing.WaitForSeconds(1);
            }
        }

        private void Server_RoundStarted()
        {
            this.RunCoroutine(ExecuteCycle(), "ExecuteCycle");
        }

        private void Server_RestartingRound()
        {
            foreach (var item in Shieldeds.Values.ToArray())
                item.Disable();
            Shieldeds.Clear();
        }
    }
}
