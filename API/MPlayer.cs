using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

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
        public static List<MPlayer> List
            => GetPlayers<MPlayer>();

        /// <summary>
        /// Gets random list of players.
        /// </summary>
        public static List<MPlayer> RandomList
        {
            get
            {
                var list = GetPlayers<MPlayer>();
                list.ShuffleList();
                return list;
            }
        }

        /// <summary>
        /// Returns players that belong to specified team.
        /// </summary>
        /// <param name="value">Team.</param>
        /// <returns>Maching players.</returns>
        public static List<MPlayer> Get(Team value)
            => List.Where(p => p.ReferenceHub.GetTeam() == value).ToList();

        /// <summary>
        /// Returns players that play as specified class.
        /// </summary>
        /// <param name="value">Role.</param>
        /// <returns>Matching players.</returns>
        public static List<MPlayer> Get(RoleTypeId value)
            => List.Where(p => p.Role == value).ToList();

        /// <summary>
        /// Player with uid/playerId/nickname.
        /// </summary>
        /// <param name="value">Arg.</param>
        /// <returns>Matching player.</returns>
        public static new MPlayer Get(string value)
            => string.IsNullOrEmpty(value) ? null : List.Where(p => p.UserId == value || value.Split('.').Contains(p.PlayerId.ToString()) || p.Nickname == value || p.DisplayNickname == value).FirstOrDefault();

        /// <summary>
        /// Returns if there is maching player.
        /// </summary>
        /// <param name="value">Team.</param>
        /// <returns>If there is maching player.</returns>
        public static bool Any(Team value)
            => List.Any(p => p.ReferenceHub.GetTeam() == value);

        /// <summary>
        /// Returns if there is maching player.
        /// </summary>
        /// <param name="value">Role.</param>
        /// <returns>If there is maching player.</returns>
        public static bool Any(RoleTypeId value)
            => List.Any(p => p.Role == value);

        /// <summary>
        /// Initializes a new instance of the <see cref="MPlayer"/> class.
        /// </summary>
        /// <param name="component">Component.</param>
        public MPlayer(IGameComponent component)
            : base(component)
        {
        }
    }
}