// -----------------------------------------------------------------------
// <copyright file="MPlayer.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using PlayerRoles;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Interfaces;

namespace Mistaken.API
{
    /// <summary>
    /// MPlayer.
    /// </summary>
    public sealed class MPlayer : Player
    {
        /// <summary>
        /// Gets list of players.
        /// </summary>
        public static List<MPlayer> List => GetPlayers<MPlayer>();

        /// <summary>
        /// Gets random list of players.
        /// </summary>
        public static List<MPlayer> RandomList
        {
            get
            {
                var list = List;
                list.ShuffleList();
                return list;
            }
        }

        /// <summary>
        /// Returns players that belong to specified team.
        /// </summary>
        /// <param name="value">Team.</param>
        /// <returns>Maching players.</returns>
        public static List<MPlayer> Get(Team value) => List.Where(p => p.ReferenceHub.GetTeam() == value).ToList();

        /// <summary>
        /// Returns players that play as specified class.
        /// </summary>
        /// <param name="value">Role.</param>
        /// <returns>Matching players.</returns>
        public static List<MPlayer> Get(RoleTypeId value) => List.Where(p => p.Role == value).ToList();

        /// <summary>
        /// Player with uid/playerId/nickname.
        /// </summary>
        /// <param name="value">Arg.</param>
        /// <returns>Matching player.</returns>
        public static new MPlayer Get(string value) => string.IsNullOrEmpty(value) ? null : List.Where(p => p.UserId == value || value.Split('.').Contains(p.PlayerId.ToString()) || p.Nickname == value || p.DisplayNickname == value).FirstOrDefault();

        /// <summary>
        /// Returns if there is maching player.
        /// </summary>
        /// <param name="value">Team.</param>
        /// <returns>If there is maching player.</returns>
        public static bool Any(Team value) => List.Any(p => p.ReferenceHub.GetTeam() == value);

        /// <summary>
        /// Returns if there is maching player.
        /// </summary>
        /// <param name="value">Role.</param>
        /// <returns>If there is maching player.</returns>
        public static bool Any(RoleTypeId value) => List.Any(p => p.Role == value);

        /// <summary>
        /// Initializes a new instance of the <see cref="MPlayer"/> class.
        /// </summary>
        /// <param name="component">Component.</param>
        public MPlayer(IGameComponent component)
            : base(component)
        {
        }

        /// <summary>
        /// Gets player's session variables.
        /// </summary>
        public Dictionary<string, object> SessionVariables { get; } = new();

        /// <summary>
        /// Get's spectated player.
        /// </summary>
        /// <returns>Spectated player or null if not spectating anyone.</returns>
        public MPlayer GetSpectatedPlayer()
            => ReferenceHub.roleManager.CurrentRole is SpectatorRole spectator ? Get<MPlayer>(spectator.SyncedSpectatedNetId) : null;

        /// <summary>
        /// Checks if player has base game permission.
        /// </summary>
        /// <param name="perms">Permission.</param>
        /// <returns>If has permission.</returns>
        public bool CheckPermissions(PlayerPermissions perms)
            => PermissionsHandler.IsPermitted(ReferenceHub.serverRoles.Permissions, perms);

        /// <summary>
        /// If player is Dev.
        /// </summary>
        /// <returns>Is Dev.</returns>
        public bool IsDev() => UserId.IsDevUserId();

        /// <summary>
        /// Returns <see cref="Player.DisplayNickname"/> or <see cref="Player.Nickname"/> if first is null or "NULL" if player is null.
        /// </summary>
        /// <returns>Name.</returns>
        public string GetDisplayName() => this is null ? "NULL" : DisplayNickname ?? Nickname;

        /// <summary>
        /// Checks if player is really connected to the server.
        /// </summary>
        /// <returns>True if player is connected. Otherwise false.</returns>
        public bool IsConnected()
            => GameObject != null && Connection is not null;

        /// <summary>
        /// Converts player to string.
        /// </summary>
        /// <param name="userId">If userId should be shown.</param>
        /// <returns>String version of player.</returns>
        public string ToString(bool userId)
        {
            return userId ?
                $"({PlayerId}) {GetDisplayName()} | {UserId}"
                :
                $"({PlayerId}) {GetDisplayName()}";
        }

        #region SessionVarsExtensions
        /// <summary>
        /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="type">Session Var.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Value.</returns>
        public T GetSessionVariable<T>(SessionVarType type, T defaultValue = default)
            => GetSessionVariable(type.ToString(), defaultValue);

        /// <summary>
        /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="name">Session Var.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Value.</returns>
        public T GetSessionVariable<T>(string name, T defaultValue = default)
        {
            if (TryGetSessionVariable(name, out T value))
                return value;

            return defaultValue;
        }

        /// <summary>
        /// If SessionVar was found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="name">Session Var.</param>
        /// <param name="value">Value.</param>
        /// <returns>If session var was found.</returns>
        public bool TryGetSessionVariable<T>(string name, out T value)
        {
            value = default;

            if (!SessionVariables.TryGetValue(name, out var val))
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
        /// <param name="type">Session Var.</param>
        /// <param name="value">Value.</param>
        /// <returns>If session var was found.</returns>
        public bool TryGetSessionVariable<T>(SessionVarType type, out T value)
            => TryGetSessionVariable(type.ToString(), out value);

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="type">Session Var.</param>
        /// <param name="value">Value.</param>
        public void SetSessionVariable(SessionVarType type, object value)
            => SetSessionVariable(type.ToString(), value);

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="name">Session Var.</param>
        /// <param name="value">Value.</param>
        public void SetSessionVariable(string name, object value)
            => SessionVariables[name] = value;

        /// <summary>
        /// Removes SessionVar.
        /// </summary>
        /// <param name="type">Session Var.</param>
        public void RemoveSessionVariable(SessionVarType type)
            => RemoveSessionVariable(type.ToString());

        /// <summary>
        /// Removes SessionVar.
        /// </summary>
        /// <param name="name">Session Var.</param>
        public void RemoveSessionVariable(string name)
            => SessionVariables.Remove(name);
        #endregion

        #region PseudoGUIExtensions
        /// <summary>
        /// Sets GUI Element.
        /// </summary>
        /// <param name="key">key.</param>
        /// <param name="type">position.</param>
        /// <param name="content">content.</param>
        /// <param name="duration">duration.</param>
        public void SetGUI(string key, PseudoGUIPosition type, string content, float duration) =>
            PseudoGUIHandler.Set(this, key, type, content, duration);

        /// <summary>
        /// Sets GUI Element.
        /// </summary>
        /// <param name="key">key.</param>
        /// <param name="type">position.</param>
        /// <param name="content">content.</param>
        public void SetGUI(string key, PseudoGUIPosition type, string content) =>
            PseudoGUIHandler.Set(this, key, type, content);
        #endregion

        #region DamageExtensions
        /// <summary>
        /// Returns if player will die because of damage caused by <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">Damage Cause.</param>
        /// <returns>If player will die because of this damage.</returns>
        public bool WillDie(StandardDamageHandler handler)
        {
            var tmp = ((AhpStat)ReferenceHub.playerStats.StatModules[1])._activeProcesses.Select(x => new { Process = x, x.CurrentAmount });
            var hp = Health;
            var damage = handler.Damage;
            var death = handler.ApplyDamage(ReferenceHub) == DamageHandlerBase.HandlerOutput.Death;
            handler.Damage = damage;
            Health = hp;

            foreach (var item in tmp)
                item.Process.CurrentAmount = item.CurrentAmount;

            return death;
        }

        /// <summary>
        /// Returns real dealt damage to the player.
        /// </summary>
        /// <param name="handler">Damage Cause.</param>
        /// <param name="dealtHealthDamage">Damage Absorbed by HP.</param>
        /// <param name="absorbedAhpDamage">Damage Absorbed by AHP.</param>
        /// <returns>Real dealt damage, damage absorbed by AHP and damage absorbed by HP.</returns>
        public float GetRealDamageAmount(StandardDamageHandler handler, out float dealtHealthDamage, out float absorbedAhpDamage)
        {
            var tmp = ((AhpStat)ReferenceHub.playerStats.StatModules[1])._activeProcesses.Select(x => new { Process = x, x.CurrentAmount });
            var hp = Health;
            var damage = handler.Damage;
            handler.ApplyDamage(ReferenceHub);
            var realDamage = handler.Damage;
            absorbedAhpDamage = handler.AbsorbedAhpDamage;
            dealtHealthDamage = handler.DealtHealthDamage;
            handler.Damage = damage;
            Health = hp;

            foreach (var item in tmp)
                item.Process.CurrentAmount = item.CurrentAmount;

            return realDamage;
        }

        /// <summary>
        /// Returns real dealt damage to the player.
        /// </summary>
        /// <param name="handler">Damage Cause.</param>
        /// <returns>Real dealt damage.</returns>
        public float GetRealDamageAmount(StandardDamageHandler handler)
            => GetRealDamageAmount(handler, out _, out _);
        #endregion
    }
}