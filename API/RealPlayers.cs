﻿// -----------------------------------------------------------------------
// <copyright file="RealPlayers.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Mistaken.API.Extensions;
using PlayerRoles;

namespace Mistaken.API
{
    /// <summary>
    /// RealPlayers.
    /// </summary>
    public static class RealPlayers
    {
        /// <summary>
        /// Gets list of players.
        /// </summary>
        public static IEnumerable<Player> List
        {
            get
            {
                lock (Player.List)
                {
                    return Player.List.Where(p => p?.IsReadyPlayer() ?? false);
                }
            }
        }

        /// <summary>
        /// Gets random list of players.
        /// </summary>
        public static IEnumerable<Player> RandomList =>
            List.ToArray().Shuffle();

        /// <summary>
        /// Returns players that belong to specified team.
        /// </summary>
        /// <param name="value">Team.</param>
        /// <returns>Maching players.</returns>
        public static IEnumerable<Player> Get(Team value) => List.Where(p => p.Role.Team == value);

        /// <summary>
        /// Returns players that play as specified class.
        /// </summary>
        /// <param name="value">Role.</param>
        /// <returns>Matching players.</returns>
        public static IEnumerable<Player> Get(RoleTypeId value) => List.Where(p => p.Role == value);

        /// <summary>
        /// Player with playerId.
        /// </summary>
        /// <param name="value">PlayerId.</param>
        /// <returns>Maching playerId.</returns>
        public static Player Get(int value) => List.FirstOrDefault(p => p.Id == value);

        /// <summary>
        /// Player with uid/playerId/nickname.
        /// </summary>
        /// <param name="value">Arg.</param>
        /// <returns>Matching player.</returns>
        public static Player Get(string value) => value == null ? null : List.Where(p => p.UserId == value || value.Split('.').Contains(p.Id.ToString()) || p.Nickname == value || p.DisplayNickname == value).FirstOrDefault();

        /// <summary>
        /// Returns if there is maching player.
        /// </summary>
        /// <param name="value">Team.</param>
        /// <returns>If there is maching player.</returns>
        public static bool Any(Team value) => List.Any(p => p.Role.Team == value);

        /// <summary>
        /// Returns if there is maching player.
        /// </summary>
        /// <param name="value">Role.</param>
        /// <returns>If there is maching player.</returns>
        public static bool Any(RoleTypeId value) => List.Any(p => p.Role == value);
    }
}