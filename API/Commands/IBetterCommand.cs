// -----------------------------------------------------------------------
// <copyright file="IBetterCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Mistaken.API.Extensions;

namespace Mistaken.API.Commands
{
    /// <summary>
    /// Command Handler.
    /// </summary>
    public abstract class IBetterCommand : ICommand
    {
        /// <inheritdoc cref="ICommand.Command"/>
        public abstract string Command { get; }

        /// <inheritdoc cref="ICommand.Aliases"/>
        public virtual string[] Aliases { get; } = new string[0];

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
            DateTime start = DateTime.Now;
            if (sender.IsPlayer())
            {
                if (this is IPermissionLocked && !((CommandSender)sender).CheckPermission(this.FullPermission))
                {
                    response = $"<b>Access Denied</b>\nMissing {this.FullPermission}";
                    Diagnostics.MasterHandler.LogTime("Command", this.Command, start, DateTime.Now);
                    return false;
                }
            }

            bool bc = false;
            string argsString = string.Join(" ", arguments.Array);
            int playerId = sender.IsPlayer() ? sender.GetPlayer().Id : 1;
            if (argsString.Contains("@me"))
                argsString = argsString.Replace("@me", playerId.ToString());

            List<string> args = NorthwoodLib.Pools.ListPool<string>.Shared.Rent(arguments.Array);
            foreach (var item in args.ToArray())
            {
                if (item == "@-cbc")
                {
                    args.Remove(item);
                    sender.GetPlayer().ClearBroadcasts();
                }

                if (item == "@-bc")
                {
                    args.Remove(item);
                    bc = true;
                }
            }

            var newQuery = argsString;
            if (argsString.Contains("@!me"))
                newQuery = argsString.Replace("@!me", string.Join(".", RealPlayers.List.Where(p => p.Id != playerId).Select(p => p.Id)));
            if (argsString.Contains("@all"))
                newQuery = argsString.Replace("@all", string.Join(".", RealPlayers.List.Select(p => p.Id)));
            if (argsString.Contains("@team:"))
            {
                foreach (var item in args.Where(arg => arg.StartsWith("@team:")))
                {
                    var values = item.Split(':');
                    if (values.Length > 0)
                    {
                        var value = values[1];
                        if (int.TryParse(value, out int teamId))
                        {
                            if (teamId > -1 && teamId < 7)
                            {
                                newQuery = argsString.Replace("@team:" + value, string.Join(".", RealPlayers.Get((Team)teamId).Select(p => p.Id)));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                if (((Team)i).ToString().ToLower() == value.ToLower())
                                    newQuery = argsString.Replace("@team:" + value, string.Join(".", RealPlayers.Get((Team)i).Select(p => p.Id)));
                            }
                        }
                    }
                }
            }

            if (argsString.Contains("@!team:"))
            {
                foreach (var item in args.Where(arg => arg.StartsWith("@!team:")))
                {
                    var values = item.Split(':');
                    if (values.Length > 0)
                    {
                        var value = values[1];
                        if (int.TryParse(value, out int teamId))
                        {
                            if (teamId > -1 && teamId < 7)
                            {
                                newQuery = argsString.Replace("@!team:" + value, string.Join(".", RealPlayers.Get((Team)teamId).Select(p => p.Id)));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                if (((Team)i).ToString().ToLower() == value.ToLower())
                                    newQuery = argsString.Replace("@!team:" + value, string.Join(".", RealPlayers.Get((Team)i).Select(p => p.Id)));
                            }
                        }
                    }
                }
            }

            if (argsString.Contains("@role:"))
            {
                foreach (var item in args.Where(arg => arg.StartsWith("@role:")))
                {
                    var values = item.Split(':');
                    if (values.Length > 0)
                    {
                        var value = values[1];
                        if (int.TryParse(value, out int roleId))
                        {
                            if (roleId > -1 && roleId < 18)
                            {
                                newQuery = argsString.Replace("@role:" + value, string.Join(".", RealPlayers.Get((RoleType)roleId).Select(p => p.Id)));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 18; i++)
                            {
                                if (((RoleType)i).ToString().ToLower() == value.ToLower())
                                    newQuery = argsString.Replace("@role:" + value, string.Join(".", RealPlayers.Get((RoleType)i).Select(p => p.Id)));
                            }
                        }
                    }
                }
            }

            if (argsString.Contains("@!role:"))
            {
                foreach (var item in args.Where(arg => arg.StartsWith("@!role:")))
                {
                    var values = item.Split(':');
                    if (values.Length > 0)
                    {
                        var value = values[1];
                        if (int.TryParse(value, out int roleId))
                        {
                            if (roleId > -1 && roleId < 18)
                            {
                                newQuery = argsString.Replace("@!role:" + value, string.Join(".", RealPlayers.List.Where(p => p.Role != (RoleType)roleId).Select(p => p.Id)));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 18; i++)
                            {
                                if (((RoleType)i).ToString().ToLower() == value.ToLower())
                                    newQuery = argsString.Replace("@!role:" + value, string.Join(".", RealPlayers.List.Where(p => p.Role != (RoleType)i).Select(p => p.Id)));
                            }
                        }
                    }
                }
            }

            if (argsString.Contains("@zone:"))
            {
                foreach (var item in args.Where(arg => arg.StartsWith("@zone:")))
                {
                    var values = item.Split(':');
                    if (values.Length > 0)
                    {
                        var value = values[1];
                        for (int i = 0; i < 4; i++)
                        {
                            if (((ZoneType)i).ToString().ToLower() == value.ToLower())
                                newQuery = argsString.Replace("@zone:" + value, string.Join(".", RealPlayers.List.Where(p => p.CurrentRoom.Zone == (ZoneType)i).Select(p => p.Id)));
                        }
                    }
                }
            }

            if (argsString.Contains("@!zone:"))
            {
                foreach (var item in args.Where(arg => arg.StartsWith("@!zone:")))
                {
                    var values = item.Split(':');
                    if (values.Length > 0)
                    {
                        var value = values[1];
                        for (int i = 0; i < 4; i++)
                        {
                            if (((ZoneType)i).ToString().ToLower() == value.ToLower())
                                newQuery = argsString.Replace("@!zone:" + value, string.Join(".", RealPlayers.List.Where(p => p.CurrentRoom.Zone != (ZoneType)i).Select(p => p.Id)));
                        }
                    }
                }
            }

            while (newQuery.Contains("@\""))
            {
                int index1 = newQuery.IndexOf("@\"") + 2;
                int index2 = newQuery.IndexOf('"', index1 + 1);

                var value = newQuery.Substring(index1, index2 - index1);
                foreach (var player in RealPlayers.List)
                {
                    if (value == player.Nickname || value == player.DisplayNickname)
                    {
                        newQuery = newQuery.Replace($"@\"{value}\"", player.Id.ToString());
                        Log.Debug($"Replaced @\"{value}\" with {player.Id}");
                        break;
                    }
                }

                Log.Debug($"Not Replaced @\"{value}\"");
            }

            response = string.Join("\n", this.Execute(sender, newQuery.Split(' ').Skip(1).ToArray(), out bool successfull));
            if (bc)
                sender.GetPlayer().Broadcast(this.Command, 10, string.Join("\n", response));
            NorthwoodLib.Pools.ListPool<string>.Shared.Return(args);
            Diagnostics.MasterHandler.LogTime("Command", this.Command, start, DateTime.Now);
            return successfull;
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
            List<Player> tor = new List<Player>();
            foreach (var item in arg.Split('.'))
            {
                if (int.TryParse(item, out int pid))
                {
                    var p = allowPets ? Player.Get(pid) : RealPlayers.Get(pid);
                    if (p != null)
                        tor.Add(p);
                }
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
        public string[] ForeachPlayer(string arg, out bool success, Func<Player, string[]> toExecute, bool allowPets = false)
        {
            List<string> tor = NorthwoodLib.Pools.ListPool<string>.Shared.Rent();
            var players = this.GetPlayers(arg, allowPets).ToArray();
            if (players.Length == 0)
                success = false;
            else
                success = true;
            foreach (var item in players)
                tor.AddRange(toExecute?.Invoke(item).Select(i => $"{item.Nickname} | {i}"));
            var torArray = tor.ToArray();
            NorthwoodLib.Pools.ListPool<string>.Shared.Rent(tor);
            return torArray;
        }
        #endregion
    }
}
