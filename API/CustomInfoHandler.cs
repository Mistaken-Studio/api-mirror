// -----------------------------------------------------------------------
// <copyright file="CustomInfoHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using Mistaken.API.Diagnostics;

namespace Mistaken.API
{
    /// <inheritdoc/>
    public class CustomInfoHandler : Module
    {
        /// <summary>
        /// Sets CustomInfo.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void Set(Player player, string key, string value)
        {
            if (!CustomInfo.ContainsKey(player))
                CustomInfo[player] = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(value))
                CustomInfo[player].Remove(key);
            else if (!CustomInfo[player].TryGetValue(key, out string oldValue) || oldValue != value)
                CustomInfo[player][key] = value;
            else
                return;
            ToUpdate.Add(player);
        }

        /// <summary>
        /// Sets CustomInfo for players maching criteria.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="selector">Func which selects players maching criteria.</param>
        public static void SetTargets(Player player, string key, string value, Func<Player, bool> selector)
        {
            var players = RealPlayers.List.Where(selector).ToArray();
            if (players.Length == 0)
                return;
            if (!CustomInfoTargeted.ContainsKey(player))
                CustomInfoTargeted[player] = new Dictionary<Player, Dictionary<string, string>>();
            bool changedAny = false;
            foreach (var target in players)
            {
                if (!CustomInfoTargeted[player].ContainsKey(target))
                    CustomInfoTargeted[player][target] = new Dictionary<string, string>();
                if (string.IsNullOrWhiteSpace(value))
                    CustomInfoTargeted[player][target].Remove(key);
                else if (!CustomInfoTargeted[player][target].TryGetValue(key, out string oldValue) || oldValue != value)
                    CustomInfoTargeted[player][target][key] = value;
                else
                    continue;
                changedAny = true;
            }

            if (changedAny)
                ToUpdate.Add(player);
        }

        /// <summary>
        /// Sets CustomInfo for specific player.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <param name="target">Target.</param>
        public static void SetTarget(Player player, string key, string value, Player target)
        {
            if (!CustomInfoTargeted.ContainsKey(player))
                CustomInfoTargeted[player] = new Dictionary<Player, Dictionary<string, string>>();
            if (!CustomInfoTargeted[player].ContainsKey(target))
                CustomInfoTargeted[player][target] = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(value))
                CustomInfoTargeted[player][target].Remove(key);
            else if (!CustomInfoTargeted[player][target].TryGetValue(key, out string oldValue) || oldValue != value)
                CustomInfoTargeted[player][target][key] = value;
            else
                return;
            ToUpdate.Add(player);
        }

        /// <inheritdoc cref="Module(Exiled.API.Interfaces.IPlugin{Exiled.API.Interfaces.IConfig})"/>
        public CustomInfoHandler(PluginHandler p)
            : base(p)
        {
        }

        /// <inheritdoc/>
        public override bool IsBasic => true;

        /// <inheritdoc/>
        public override string Name => "CustomInfo";

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= this.Server_RoundStarted;
            Exiled.Events.Handlers.Server.RestartingRound -= this.Server_RestartingRound;
        }

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += this.Server_RoundStarted;
            Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound;
        }

        private static readonly Dictionary<Player, Dictionary<string, string>> CustomInfo = new Dictionary<Player, Dictionary<string, string>>();
        private static readonly Dictionary<Player, Dictionary<Player, Dictionary<string, string>>> CustomInfoTargeted = new Dictionary<Player, Dictionary<Player, Dictionary<string, string>>>();
        private static readonly List<Player> ToUpdate = new List<Player>();

        private void Server_RestartingRound()
        {
            CustomInfo.Clear();
            ToUpdate.Clear();
        }

        private void Server_RoundStarted()
        {
            this.RunCoroutine(this.DoRoundLoop(), "DoRoundLoop");
        }

        private IEnumerator<float> DoRoundLoop()
        {
            yield return Timing.WaitForSeconds(1);
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(2);
                if (ToUpdate.Count == 0)
                    continue;
                foreach (var item in ToUpdate.ToArray())
                {
                    try
                    {
                        if (item.IsConnected)
                            this.Update(item);
                        ToUpdate.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        this.Log.Error(ex.Message);
                        this.Log.Error(ex.StackTrace);
                    }
                }
            }
        }

        private void Update(Player player)
        {
            if (!CustomInfo.ContainsKey(player))
                CustomInfo[player] = new Dictionary<string, string>();
            if (!CustomInfoTargeted.ContainsKey(player))
                CustomInfoTargeted[player] = new Dictionary<Player, Dictionary<string, string>>();

            string for_players = string.Join(Environment.NewLine, CustomInfo[player].Values);
            if (string.IsNullOrWhiteSpace(for_players))
            {
                for_players = Regex.Replace(for_players, "<[.^\\w\\/=#%]*>", string.Empty);
                player.CustomInfo = for_players;
            }
            else
                player.CustomInfo = null;

            if (CustomInfoTargeted[player].Count > 0)
            {
                if (!(player?.IsConnected ?? false))
                    return;
                if (player?.Connection?.identity == null)
                    return;
                foreach (var item in CustomInfoTargeted[player])
                {
                    if (item.Value.Count == 0)
                        continue;
                    if (!(item.Key?.IsConnected ?? false))
                        continue;
                    if (item.Key?.Connection?.identity == null)
                        continue;
                    var tmp = item.Value.Values.ToList();
                    tmp.AddRange(CustomInfo[player].Values);
                    this.CallDelayed(
                        1,
                        () =>
                        {
                            if (!(item.Key?.IsConnected ?? false))
                                return;
                            if (item.Key?.Connection?.identity == null)
                                return;
                            var toSet = string.Join(Environment.NewLine, tmp);
                            toSet = Regex.Replace(toSet, "<[.^\\w\\/=#%]*>", string.Empty);
                            item.Key.SetPlayerInfoForTargetOnly(player, toSet);
                        },
                        "Update");
                }
            }
        }
    }
}
