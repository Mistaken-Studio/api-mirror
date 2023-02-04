using CommandSystem;
using Mistaken.API.Handlers;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Linq;

namespace Mistaken.API.Extensions;

/// <summary>
/// Player Extensions.
/// </summary>
public static class PlayerExtensions
{
    /// <inheritdoc cref="Player.SendBroadcast(string, ushort, global::Broadcast.BroadcastFlags, bool)"/>
    public static void BroadcastWithTag(this Player player, string tag, string message, ushort duration, Broadcast.BroadcastFlags flags = global::Broadcast.BroadcastFlags.Normal)
        => player.SendBroadcast($"<color=orange>[<color=green>{tag}</color>]</color> {message}", duration, flags);

    /// <summary>
    /// Returns player.
    /// </summary>
    /// <param name="sender">Potently player.</param>
    /// <returns>Player.</returns>
    public static T GetPlayer<T>(this CommandSender sender) where T : Player
        => sender is PlayerCommandSender player ? Player.Get<T>(player.ReferenceHub) : null;

    /// <summary>
    /// Returns player.
    /// </summary>
    /// <param name="sender">Potently player.</param>
    /// <returns>Player.</returns>
    public static T GetPlayer<T>(this ICommandSender sender) where T : Player
        => GetPlayer<T>(sender as CommandSender);

    /// <summary>
    /// Returns true if <paramref name="sender"/> is Player.
    /// </summary>
    /// <param name="sender">To Check.</param>
    /// <returns>Result.</returns>
    public static bool IsPlayer(this CommandSender sender) => sender is PlayerCommandSender;

    /// <summary>
    /// Returns true if <paramref name="sender"/> is Player.
    /// </summary>
    /// <param name="sender">To Check.</param>
    /// <returns>Result.</returns>
    public static bool IsPlayer(this ICommandSender sender) => sender is PlayerCommandSender;

    /// <summary>
    /// Returns if UserId is Dev's userId.
    /// </summary>
    /// <param name="userId">UserId.</param>
    /// <returns>If belongs to dev.</returns>
    public static bool IsDevUserId(this string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return false;

        return userId.Split('@')[0] switch
        {
            // WW
            "76561198134629649" => true,
            "356174382655209483" => true,

            // Barwa
            "76561198035545880" => true,
            "373551302292013069" => true,
            "barwa" => true,

            // Xname
            "76561198123437513" => true,
            "373911388575236096" => true,
            _ => false
        };
    }

    /*/// <summary>
    /// Returns if player is real, ready player.
    /// </summary>
    /// <param name="player">Player to check.</param>
    /// <returns>If player is ready, real player.</returns>
    public static bool IsReadyPlayer(this Player player)
        => player.IsConnected() && player.IsVerified && player.UserId != null && player.ReferenceHub.Ready;*/

    /*/// <summary>
    /// Gets player's current room.
    /// </summary>
    /// <param name="player">Player to get room from.</param>
    /// <returns>Player's current room.</returns>
    public static Room GetCurrentRoom(this Player player)
    {
        return player.Role switch
        {
            Scp079Role role079 => Map.FindParentRoom(role079.Camera.GameObject),
            SpectatorRole roleSpec => roleSpec.SpectatedPlayer?.GetCurrentRoom(),
            _ => Physics.RaycastNonAlloc(
                new Ray(player.GameObject.transform.position, Vector3.down),
                CachedFindParentRoomRaycast,
                25f,
                1,
                QueryTriggerInteraction.Ignore) == 1
                ? CachedFindParentRoomRaycast[0].collider.gameObject.GetComponentInParent<Room>()
                : null
        };
    }

    private static readonly RaycastHit[] CachedFindParentRoomRaycast = new RaycastHit[1];*/

    /// <summary>
    /// Get's spectated player.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <returns>Spectated player or null if not spectating anyone.</returns>
    public static T GetSpectatedPlayer<T>(this T player) where T : Player
        => player.ReferenceHub.roleManager.CurrentRole is SpectatorRole spectator ? Player.Get<T>(spectator.SyncedSpectatedNetId) : null;

    /// <summary>
    /// Checks if player has base game permission.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="perms">Permission.</param>
    /// <returns>If has permission.</returns>
    public static bool CheckPermissions(this Player player, PlayerPermissions perms)
        => PermissionsHandler.IsPermitted(player.ReferenceHub.serverRoles.Permissions, perms);

    /// <summary>
    /// If player is Dev.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <returns>Is Dev.</returns>
    public static bool IsDev(this Player player)
        => player.UserId.IsDevUserId();

    /// <summary>
    /// Returns <see cref="Player.DisplayNickname"/> or <see cref="Player.Nickname"/> if first is null or "NULL" if player is null.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <returns>Name.</returns>
    public static string GetDisplayName(this Player player)
        => player is null ? "NULL" : player.DisplayNickname ?? player.Nickname;

    /// <summary>
    /// Checks if player is really connected to the server.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <returns>True if player is connected. Otherwise false.</returns>
    public static bool IsConnected(this Player player)
        => player?.GameObject != null && player.Connection is not null;

    /// <summary>
    /// Converts player to string.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="userId">If userId should be shown.</param>
    /// <returns>String version of player.</returns>
    public static string ToString(this Player player, bool userId)
    {
        return userId ?
            $"({player.PlayerId}) {player.GetDisplayName()} | {player.UserId}"
            :
            $"({player.PlayerId}) {player.GetDisplayName()}";
    }

    /// <summary>
    /// Converts player to string for use in round logs.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <returns>String version of player.</returns>
    public static string ToStringLogs(this Player player)
        => $"{player.Nickname} ({player.UserId}) as {player.Role}";

    public static string GetFormatedUserId(this Player player)
    {
        if (player is null)
            return "NONE";

        var split = player.UserId.Split('@');

        return split[1] switch
        {
            "steam" => $"[{player.Nickname}](https://steamcommunity.com/profiles/{split[0]})",
            "discord" => $"{player.Nickname} (<@{split[0]}>)",
            "server" => "Server",
            _ => player.UserId
        };
    }

    #region SessionVarsExtensions
    /// <summary>
    /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="player">Player.</param>
    /// <param name="type">Session Var.</param>
    /// <param name="defaultValue">Default Value.</param>
    /// <returns>Value.</returns>
    public static T GetSessionVariable<T>(this Player player, SessionVarType type, T defaultValue = default)
        => player.GetSessionVariable(type.ToString(), defaultValue);

    /// <summary>
    /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="player">Player.</param>
    /// <param name="name">Session Var.</param>
    /// <param name="defaultValue">Default Value.</param>
    /// <returns>Value.</returns>
    public static T GetSessionVariable<T>(this Player player, string name, T defaultValue = default)
    {
        if (player.TryGetSessionVariable(name, out T value))
            return value;

        return defaultValue;
    }

    /// <summary>
    /// If SessionVar was found.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="player">Player.</param>
    /// <param name="name">Session Var.</param>
    /// <param name="value">Value.</param>
    /// <returns>If session var was found.</returns>
    public static bool TryGetSessionVariable<T>(this Player player, string name, out T value)
    {
        value = default;

        if (!player.TemporaryData.StoredData.TryGetValue(name, out var val))
            return false;

        if (val is T t)
        {
            value = t;
            return true;
        }

        return false;
    }

    /// <summary>
    /// If SessionVar was found.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="player">Player.</param>
    /// <param name="type">Session Var.</param>
    /// <param name="value">Value.</param>
    /// <returns>If session var was found.</returns>
    public static bool TryGetSessionVariable<T>(this Player player, SessionVarType type, out T value)
        => player.TryGetSessionVariable(type.ToString(), out value);

    /// <summary>
    /// Sets SessionVarValue.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="type">Session Var.</param>
    /// <param name="value">Value.</param>
    public static void SetSessionVariable(this Player player, SessionVarType type, object value)
        => player.SetSessionVariable(type.ToString(), value);

    /// <summary>
    /// Sets SessionVarValue.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="name">Session Var.</param>
    /// <param name="value">Value.</param>
    public static void SetSessionVariable(this Player player, string name, object value)
        => player.TemporaryData.StoredData[name] = value;

    /// <summary>
    /// Removes SessionVar.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="type">Session Var.</param>
    public static void RemoveSessionVariable(this Player player, SessionVarType type)
        => player.RemoveSessionVariable(type.ToString());

    /// <summary>
    /// Removes SessionVar.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="name">Session Var.</param>
    public static void RemoveSessionVariable(this Player player, string name)
        => player.TemporaryData.StoredData.Remove(name);
    #endregion

    #region DamageExtensions
    /// <summary>
    /// Returns if player will die because of damage caused by <paramref name="handler"/>.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="handler">Damage Cause.</param>
    /// <returns>If player will die because of this damage.</returns>
    public static bool WillDie(this Player player, StandardDamageHandler handler)
    {
        var tmp = ((AhpStat)player.ReferenceHub.playerStats.StatModules[1])._activeProcesses.Select(x => new { Process = x, x.CurrentAmount });
        var hp = player.Health;
        var damage = handler.Damage;
        var death = handler.ApplyDamage(player.ReferenceHub) == DamageHandlerBase.HandlerOutput.Death;
        handler.Damage = damage;
        player.Health = hp;

        foreach (var item in tmp)
            item.Process.CurrentAmount = item.CurrentAmount;

        return death;
    }

    /// <summary>
    /// Returns real dealt damage to the player.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="handler">Damage Cause.</param>
    /// <param name="dealtHealthDamage">Damage Absorbed by HP.</param>
    /// <param name="absorbedAhpDamage">Damage Absorbed by AHP.</param>
    /// <returns>Real dealt damage, damage absorbed by AHP and damage absorbed by HP.</returns>
    public static float GetRealDamageAmount(this Player player, StandardDamageHandler handler, out float dealtHealthDamage, out float absorbedAhpDamage)
    {
        var tmp = ((AhpStat)player.ReferenceHub.playerStats.StatModules[1])._activeProcesses.Select(x => new { Process = x, x.CurrentAmount });
        var hp = player.Health;
        var damage = handler.Damage;
        handler.ApplyDamage(player.ReferenceHub);
        var realDamage = handler.Damage;
        absorbedAhpDamage = handler.AbsorbedAhpDamage;
        dealtHealthDamage = handler.DealtHealthDamage;
        handler.Damage = damage;
        player.Health = hp;

        foreach (var item in tmp)
            item.Process.CurrentAmount = item.CurrentAmount;

        return realDamage;
    }

    /// <summary>
    /// Returns real dealt damage to the player.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="handler">Damage Cause.</param>
    /// <returns>Real dealt damage.</returns>
    public static float GetRealDamageAmount(this Player player, StandardDamageHandler handler)
        => player.GetRealDamageAmount(handler, out _, out _);
    #endregion

    #region CustomInfoExtensions
    /// <summary>
    /// Sets CustomInfo.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    public static void Set(this Player player, string key, string value)
    {
        if (!CustomInfoHandler.CustomInfo.ContainsKey(player))
            CustomInfoHandler.CustomInfo[player] = new();

        if (string.IsNullOrWhiteSpace(value))
            CustomInfoHandler.CustomInfo[player].Remove(key);

        else if (!CustomInfoHandler.CustomInfo[player].TryGetValue(key, out var oldValue) || oldValue != value)
            CustomInfoHandler.CustomInfo[player][key] = value;
        else
            return;

        CustomInfoHandler.ToUpdate.Add(player);
    }

    /// <summary>
    /// Sets CustomInfo for players maching criteria.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    /// <param name="selector">Func which selects players maching criteria.</param>
    public static void SetTargets(this Player player, string key, string value, Func<Player, bool> selector)
    {
        var players = Player.GetPlayers().Where(selector).ToArray();
        if (players.Length == 0)
            return;

        if (!CustomInfoHandler.CustomInfoTargeted.ContainsKey(player))
            CustomInfoHandler.CustomInfoTargeted[player] = new();

        var changedAny = false;
        foreach (var target in players)
        {
            if (!CustomInfoHandler.CustomInfoTargeted[player].ContainsKey(target))
                CustomInfoHandler.CustomInfoTargeted[player][target] = new();

            if (string.IsNullOrWhiteSpace(value))
                CustomInfoHandler.CustomInfoTargeted[player][target].Remove(key);

            else if (!CustomInfoHandler.CustomInfoTargeted[player][target].TryGetValue(key, out var oldValue) || oldValue != value)
                CustomInfoHandler.CustomInfoTargeted[player][target][key] = value;
            else
                continue;

            changedAny = true;
        }

        if (changedAny)
            CustomInfoHandler.ToUpdate.Add(player);
    }

    /// <summary>
    /// Sets CustomInfo for specific player.
    /// </summary>
    /// <param name="player">Player.</param>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    /// <param name="target">Target.</param>
    public static void SetTarget(this Player player, string key, string value, Player target)
    {
        if (!CustomInfoHandler.CustomInfoTargeted.ContainsKey(player))
            CustomInfoHandler.CustomInfoTargeted[player] = new();

        if (!CustomInfoHandler.CustomInfoTargeted[player].ContainsKey(target))
            CustomInfoHandler.CustomInfoTargeted[player][target] = new();

        if (string.IsNullOrWhiteSpace(value))
            CustomInfoHandler.CustomInfoTargeted[player][target].Remove(key);

        else if (!CustomInfoHandler.CustomInfoTargeted[player][target].TryGetValue(key, out var oldValue) || oldValue != value)
            CustomInfoHandler.CustomInfoTargeted[player][target][key] = value;
        else
            return;

        CustomInfoHandler.ToUpdate.Add(player);
    }
    #endregion
}
