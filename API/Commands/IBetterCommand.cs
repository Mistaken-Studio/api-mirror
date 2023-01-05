// -----------------------------------------------------------------------
// <copyright file="IBetterCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mistaken.API.Extensions;
using Utils;

using static Exiled.Permissions.Extensions.Permissions;

namespace Mistaken.API.Commands
{
    /// <summary>
    /// Command Handler.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public abstract class IBetterCommand : ICommand
    {
        /// <inheritdoc cref="ICommand.Command"/>
        public abstract string Command { get; }

        /// <inheritdoc cref="ICommand.Aliases"/>
        public virtual string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc cref="ICommand.Description"/>
        public virtual string Description { get; } = string.Empty;

        /// <summary>
        /// Gets full permission name.
        /// </summary>
        public string FullPermission
        {
            get
            {
                if (this is IPermissionLocked pl)
                    return $"{pl.PluginName}.{pl.Permission}";
                return string.Empty;
            }
        }

        /// <inheritdoc cref="ICommand.Execute(ArraySegment{string}, ICommandSender, out string)"/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var start = DateTime.Now;
            if (sender.IsPlayer())
            {
                if (this is IPermissionLocked && !sender.CheckPermission(this.FullPermission))
                {
                    response = $"<b>Access Denied</b>\nMissing {this.FullPermission}";
                    Diagnostics.MasterHandler.LogTime("Command", this.Command, start, DateTime.Now);
                    return false;
                }
            }

            var bc = false;
            var argsString = string.Join(" ", arguments.Array!);
            var playerId = sender.IsPlayer() ? sender.GetPlayer().Id : 1;
            if (argsString.Contains("@me"))
                argsString = argsString.Replace("@me", playerId.ToString());

            List<string> args = NorthwoodLib.Pools.ListPool<string>.Shared.Rent(arguments.Array);
            foreach (var item in args.ToArray())
            {
                args.Remove(item);

                switch (item)
                {
                    case "@-cbc":
                        sender.GetPlayer().ClearBroadcasts();
                        break;
                    case "@-bc":
                        bc = true;
                        break;
                }
            }

            var newQuery = argsString;
            if (!newQuery.StartsWith("@"))
            {
                if (argsString.Contains("@!me"))
                {
                    newQuery = argsString.Replace(
                        "@!me",
                        string.Join(".", RealPlayers.List.Where(p => p.Id != playerId).Select(p => p.Id)));
                }

                if (argsString.Contains("@all"))
                    newQuery = argsString.Replace("@all", string.Join(".", RealPlayers.List.Select(p => p.Id)));
                if (argsString.Contains("@team:"))
                {
                    foreach (var item in args.Where(arg => arg.StartsWith("@team:")))
                    {
                        var values = item.Split(':');
                        if (values.Length <= 0)
                            continue;
                        var value = values[1];
                        if (Enum.TryParse<Team>(value, true, out var teamType))
                        {
                            newQuery = argsString.Replace(
                                "@team:" + value,
                                string.Join(".", RealPlayers.Get(teamType).Select(p => p.Id)));
                        }
                    }
                }

                if (argsString.Contains("@!team:"))
                {
                    foreach (var item in args.Where(arg => arg.StartsWith("@!team:")))
                    {
                        var values = item.Split(':');
                        if (values.Length <= 0)
                            continue;
                        var value = values[1];
                        if (Enum.TryParse<Team>(value, true, out var teamType))
                        {
                            newQuery = argsString.Replace(
                                "@!team:" + value,
                                string.Join(
                                    ".",
                                    RealPlayers.List
                                        .Where(x => x.Role.Team != teamType)
                                        .Select(p => p.Id)));
                        }
                    }
                }

                if (argsString.Contains("@role:"))
                {
                    foreach (var item in args.Where(arg => arg.StartsWith("@role:")))
                    {
                        var values = item.Split(':');
                        if (values.Length <= 0)
                            continue;
                        var value = values[1];
                        if (Enum.TryParse<RoleType>(value, true, out var roleType))
                        {
                            newQuery = argsString.Replace(
                                "@role:" + value,
                                string.Join(".", RealPlayers.Get(roleType).Select(p => p.Id)));
                        }
                    }
                }

                if (argsString.Contains("@!role:"))
                {
                    foreach (var item in args.Where(arg => arg.StartsWith("@!role:")))
                    {
                        var values = item.Split(':');
                        if (values.Length <= 0)
                            continue;
                        var value = values[1];
                        if (Enum.TryParse<RoleType>(value, true, out var roleType))
                        {
                            newQuery = argsString.Replace(
                                "@!role:" + value,
                                string.Join(
                                    ".",
                                    RealPlayers.List
                                        .Where(p => p.Role.Type != roleType)
                                        .Select(p => p.Id)));
                        }
                    }
                }

                if (argsString.Contains("@zone:"))
                {
                    foreach (var item in args.Where(arg => arg.StartsWith("@zone:")))
                    {
                        var values = item.Split(':');
                        if (values.Length <= 0)
                            continue;
                        var value = values[1];
                        if (Enum.TryParse<ZoneType>(value, true, out var zoneType))
                        {
                            newQuery = argsString.Replace(
                                "@zone:" + value,
                                string.Join(
                                    ".",
                                    RealPlayers.List
                                        .Where(x => x.CurrentRoom.Zone == zoneType)
                                        .Select(p => p.Id)));
                        }
                    }
                }

                if (argsString.Contains("@!zone:"))
                {
                    foreach (var item in args.Where(arg => arg.StartsWith("@!zone:")))
                    {
                        var values = item.Split(':');
                        if (values.Length <= 0)
                            continue;
                        var value = values[1];
                        if (Enum.TryParse<ZoneType>(value, true, out var zoneType))
                        {
                            newQuery = argsString.Replace(
                                "@!zone:" + value,
                                string.Join(
                                    ".",
                                    RealPlayers.List
                                        .Where(x => x.CurrentRoom.Zone != zoneType)
                                        .Select(p => p.Id)));
                        }
                    }
                }

                newQuery = Regex.Replace(newQuery, RAUtils.PlayerNameRegex, (match) =>
                {
                    if (!match.Success)
                        return match.Value;

                    foreach (var player in RealPlayers.List)
                    {
                        if (match.Value != player.Nickname && match.Value != player.DisplayNickname)
                            continue;

                        Log.Debug($"Replaced {match.Value} with {player.Id}", PluginHandler.Debug);
                        return player.Id.ToString();
                    }

                    Log.Debug($"No mach found for {match.Value}", PluginHandler.Debug);
                    return match.Value;
                });
            }

            response = string.Join(
                "\n",
                this.Execute(
                    sender,
                    newQuery
                        .Split(' ')
                        .Skip(1)
                        .ToArray(),
                    out var successful));
            if (bc)
                sender.GetPlayer().Broadcast(this.Command, 10, string.Join("\n", response));

            NorthwoodLib.Pools.ListPool<string>.Shared.Return(args);
            Diagnostics.MasterHandler.LogTime("Command", this.Command, start, DateTime.Now);
            return successful;
        }

        /// <summary>
        /// Executes command.
        /// </summary>
        /// <param name="sender">Command sender.</param>
        /// <param name="args">Arguments.</param>
        /// <param name="success">If command was successful.</param>
        /// <returns>Command response.</returns>
        public abstract string[] Execute(ICommandSender sender, string[] args, out bool success);

        #region Extensions

        /// <summary>
        /// Returns player list with ids.
        /// </summary>
        /// <param name="arg">Ids.</param>
        /// <param name="allowPets">If pets can be included.</param>
        /// <returns><see cref="List{T}"/> of <see cref="Player"/>s.</returns>
        public List<Player> GetPlayers(string arg, bool allowPets = false)
        {
            List<Player> tor = new();
            foreach (var item in arg.Split('.'))
            {
                if (!int.TryParse(item, out var pid))
                    continue;

                var p = allowPets ? Player.Get(pid) : RealPlayers.Get(pid);
                if (p != null)
                    tor.Add(p);
            }

            return tor;
        }

        /// <summary>
        /// Runs <paramref name="toExecute"/> on every matching player.
        /// </summary>
        /// <param name="arg">Ids.</param>
        /// <param name="toExecute">Action.</param>
        /// <param name="allowPets">If pets can be included.</param>
        /// <returns>If found any player.</returns>
        public bool ForeachPlayer(string arg, Action<Player> toExecute, bool allowPets = false)
        {
            var players = this.GetPlayers(arg, allowPets).ToArray();
            if (players.Length == 0)
                return false;
            foreach (var item in players)
                toExecute?.Invoke(item);
            return true;
        }

        /// <summary>
        /// Runs <paramref name="toExecute"/> on every matching player.
        /// </summary>
        /// <param name="arg">Ids.</param>
        /// <param name="success">If found any player.</param>
        /// <param name="toExecute">Func.</param>
        /// <param name="allowPets">If pets can be included.</param>
        /// <returns>Joined results of all <paramref name="toExecute"/>.</returns>
        public string[] ForeachPlayer(
            string arg,
            out bool success,
            Func<Player, string[]> toExecute,
            bool allowPets = false)
        {
            List<string> tor = NorthwoodLib.Pools.ListPool<string>.Shared.Rent();
            var players = this.GetPlayers(arg, allowPets).ToArray();
            success = players.Length != 0;
            foreach (var item in players)
            {
                tor.AddRange(
                    toExecute?
                        .Invoke(item)
                        .Select(i => $"{item.Nickname} | {i}")
                    ?? Array.Empty<string>());
            }

            var torArray = tor.ToArray();
            NorthwoodLib.Pools.ListPool<string>.Shared.Return(tor);
            return torArray;
        }

        #endregion
    }
}
