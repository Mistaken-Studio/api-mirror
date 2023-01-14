/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MEC;
using Mistaken.API.Extensions;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace Mistaken.API.Handlers
{
    internal sealed class CustomInfoHandler
    {
        public static readonly Dictionary<Player, Dictionary<string, string>> CustomInfo = new();
        public static readonly Dictionary<Player, Dictionary<Player, Dictionary<string, string>>> CustomInfoTargeted = new();
        public static readonly List<Player> ToUpdate = new();

        public CustomInfoHandler()
        {
            EventManager.RegisterEvents(this);
        }

        ~CustomInfoHandler()
        {
            EventManager.UnregisterEvents(this);
        }

        [PluginEvent(ServerEventType.RoundStart)]
        private void OnRoundStart()
            => Timing.RunCoroutine(this.DoRoundLoop(), nameof(DoRoundLoop));

        [PluginEvent(ServerEventType.RoundRestart)]
        private void OnRoundRestart()
        {
            CustomInfo.Clear();
            ToUpdate.Clear();
        }

        private IEnumerator<float> DoRoundLoop()
        {
            yield return Timing.WaitForSeconds(1);
            while (Round.IsRoundStarted)
            {
                yield return Timing.WaitForSeconds(2);
                if (ToUpdate.Count == 0)
                    continue;

                foreach (var item in ToUpdate.ToArray())
                {
                    try
                    {
                        if (item.IsConnected())
                            this.Update(item);

                        ToUpdate.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                        Log.Error(ex.StackTrace);
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
                // for_players = Regex.Replace(for_players, "(<color=.*>)|(</color>)", string.Empty);
                // for_players = Regex.Replace(for_players, "(<b>)|(</b>)", string.Empty);
                // for_players = Regex.Replace(for_players, "(<i>)|(</i>)", string.Empty);
                // for_players = Regex.Replace(for_players, "<|>", string.Empty);
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

            if (!player.IsConnected())
                return;

            if (player.Connection?.identity == null)
                return;

            foreach (var item in CustomInfoTargeted[player])
            {
                if (item.Value.Count == 0)
                    continue;

                if (!(item.Key?.IsConnected() ?? false))
                    continue;

                if (item.Key?.Connection?.identity == null)
                    continue;

                var tmp = item.Value.Values.ToList();
                tmp.AddRange(CustomInfo[player].Values);
                Timing.CallDelayed(1, () =>
                {
                    if (!(item.Key?.IsConnected() ?? false))
                        return;

                    if (item.Key?.Connection?.identity == null)
                        return;

                    var toSet = string.Join("\n", tmp);

                    // toSet = Regex.Replace(toSet, $"<color=[^{string.Join("|", Misc.AllowedColors.Select(x => x.Key.ToString().ToLower() + "|" + x.Value))}]^>", string.Empty);
                    // Log.Debug(toSet.Replace("<", "[").Replace(">", "]").Replace("\n", "|_n"), true);
                    // toSet = Regex.Replace(toSet, "<color=.*>", $"<color={Misc.AllowedColors[Misc.PlayerInfoColorTypes.Yellow]}>");
                    // toSet = Regex.Replace(toSet, "(<color=.*>)|(</color>)", string.Empty);
                    // toSet = Regex.Replace(toSet, "(<b>)|(</b>)", string.Empty);
                    // toSet = Regex.Replace(toSet, "(<i>)|(</i>)", string.Empty);
                    // toSet = Regex.Replace(toSet, "<|>", string.Empty);

                    // Log.Debug(toSet, true);
                    toSet = Regex.Replace(toSet, "<[.^\\w\\/=#%]*>", string.Empty);

                    // toSet = toSet.Substring(0, Math.Min(400, toSet.Length));
                    item.Key.SetPlayerInfoForTargetOnly(player, toSet);
                },
                nameof(Update));
            }
        }
    }
}*/
