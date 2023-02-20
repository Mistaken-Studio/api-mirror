using System.Collections.Generic;
using System.Linq;
using PluginAPI.Core;

namespace Mistaken.API.Utilities;

public static class CustomInfo
{
    public static readonly Dictionary<Player, Dictionary<string, string>> CustomInfos = new();
    public static readonly Dictionary<Player, Dictionary<Player, Dictionary<string, string>>> CustomInfosTargeted = new();
    public static readonly List<Player> ToUpdate = new();

    internal static void SetCustomInfoInternal(Player player, string key, string value)
    {
        if (!CustomInfos.ContainsKey(player))
            CustomInfos[player] = new();

        if (string.IsNullOrWhiteSpace(value))
            CustomInfos[player].Remove(key);

        else if (!CustomInfos[player].TryGetValue(key, out var oldValue) || oldValue != value)
            CustomInfos[player][key] = value;
        else
            return;

        ToUpdate.Add(player);
    }

    internal static void SetCustomInfoForTargetsInternal(Player player, string key, string value, IEnumerable<Player> players)
    {
        var targets = players.ToArray();
        if (targets.Length == 0)
            return;

        if (!CustomInfosTargeted.ContainsKey(player))
            CustomInfosTargeted[player] = new();

        var changedAny = false;
        foreach (var target in targets)
        {
            if (!CustomInfosTargeted[player].ContainsKey(target))
                CustomInfosTargeted[player][target] = new();

            if (string.IsNullOrWhiteSpace(value))
                CustomInfosTargeted[player][target].Remove(key);

            else if (!CustomInfosTargeted[player][target].TryGetValue(key, out var oldValue) || oldValue != value)
                CustomInfosTargeted[player][target][key] = value;
            else
                continue;

            changedAny = true;
        }

        if (changedAny)
            ToUpdate.Add(player);
    }

    internal static void SetCustomInfoForTargetInternal(Player player, string key, string value, Player target)
    {
        if (!CustomInfosTargeted.ContainsKey(player))
            CustomInfosTargeted[player] = new();

        if (!CustomInfosTargeted[player].ContainsKey(target))
            CustomInfosTargeted[player][target] = new();

        if (string.IsNullOrWhiteSpace(value))
            CustomInfosTargeted[player][target].Remove(key);

        else if (!CustomInfosTargeted[player][target].TryGetValue(key, out var oldValue) || oldValue != value)
            CustomInfosTargeted[player][target][key] = value;
        else
            return;

        ToUpdate.Add(player);
    }
}
