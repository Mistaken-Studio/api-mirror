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
using JetBrains.Annotations;
using MEC;
using Mistaken.API.Diagnostics;

namespace Mistaken.API.Handlers
{
    /// <inheritdoc/>
    [PublicAPI]
    public sealed class CustomInfoHandler : Module
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
                CustomInfo[player] = new();
            if (string.IsNullOrWhiteSpace(value))
                CustomInfo[player].Remove(key);
            else if (!CustomInfo[player].TryGetValue(key, out var oldValue) || oldValue != value)
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
                CustomInfoTargeted[player] = new();
            var changedAny = false;
            foreach (var target in players)
            {
                if (!CustomInfoTargeted[player].ContainsKey(target))
                    CustomInfoTargeted[player][target] = new();
                if (string.IsNullOrWhiteSpace(value))
                    CustomInfoTargeted[player][target].Remove(key);
                else if (!CustomInfoTargeted[player][target].TryGetValue(key, out var oldValue) || oldValue != value)
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
                CustomInfoTargeted[player] = new();
            if (!CustomInfoTargeted[player].ContainsKey(target))
                CustomInfoTargeted[player][target] = new();
            if (string.IsNullOrWhiteSpace(value))
                CustomInfoTargeted[player][target].Remove(key);
            else if (!CustomInfoTargeted[player][target].TryGetValue(key, out var oldValue) || oldValue != value)
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

        private static readonly Dictionary<Player, Dictionary<string, string>> CustomInfo = new();
        private static readonly Dictionary<Player, Dictionary<Player, Dictionary<string, string>>> CustomInfoTargeted = new();
        private static readonly List<Player> ToUpdate = new();

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

            var forPlayers = string.Join("\n", CustomInfo[player].Values);
            if (!string.IsNullOrWhiteSpace(forPlayers))
            {
                // for_players = Regex.Replace(for_players, $"<color=[^{string.Join("|", Misc.AllowedColors.Select(x => x.Key.ToString().ToLower() + "|" + x.Value))}]>", string.Empty);
                // Log.Debug(for_players.Replace('<', '[').Replace('>', ']'), true);
                /*for_players = Regex.Replace(for_players, "(<color=.*>)|(</color>)", string.Empty);
                for_players = Regex.Replace(for_players, "(<b>)|(</b>)", string.Empty);
                for_players = Regex.Replace(for_players, "(<i>)|(</i>)", string.Empty);
                for_players = Regex.Replace(for_players, "<|>", string.Empty);*/
                // for_players = for_players.Replace('<', '[').Replace('>', ']');
                // Log.Debug(for_players, true);
                forPlayers = Regex.Replace(forPlayers, "<[.^\\w\\/=#%]*>", string.Empty);

                // for_players = for_players.Substring(0, Math.Min(400, for_players.Length));
                player.CustomInfo = forPlayers;
            }
            else
                player.CustomInfo = null;

            if (CustomInfoTargeted[player].Count == 0)
                return;

            if (!player.IsConnected)
                return;

            if (player.Connection?.identity == null)
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
                        var toSet = string.Join("\n", tmp);

                        // toSet = Regex.Replace(toSet, $"<color=[^{string.Join("|", Misc.AllowedColors.Select(x => x.Key.ToString().ToLower() + "|" + x.Value))}]^>", string.Empty);
                        // Log.Debug(toSet.Replace("<", "[").Replace(">", "]").Replace("\n", "|_n"), true);
                        // toSet = Regex.Replace(toSet, "<color=.*>", $"<color={Misc.AllowedColors[Misc.PlayerInfoColorTypes.Yellow]}>");
                        /*toSet = Regex.Replace(toSet, "(<color=.*>)|(</color>)", string.Empty);
                            toSet = Regex.Replace(toSet, "(<b>)|(</b>)", string.Empty);
                            toSet = Regex.Replace(toSet, "(<i>)|(</i>)", string.Empty);
                            toSet = Regex.Replace(toSet, "<|>", string.Empty);*/

                        // Log.Debug(toSet, true);
                        toSet = Regex.Replace(toSet, "<[.^\\w\\/=#%]*>", string.Empty);

                        // toSet = toSet.Substring(0, Math.Min(400, toSet.Length));
                        item.Key.SetPlayerInfoForTargetOnly(player, toSet);
                    },
                    "Update");
            }
        }
    }
}
