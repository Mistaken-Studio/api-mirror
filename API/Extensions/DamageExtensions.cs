// -----------------------------------------------------------------------
// <copyright file="DamageExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Exiled.API.Features;
using PlayerStatsSystem;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Damage Extensions.
    /// </summary>
    public static class DamageExtensions
    {
        /// <summary>
        /// Returns if player will die because of damage caused by <paramref name="handler"/>.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="handler">Damage Cause.</param>
        /// <returns>If player will die because of this damage.</returns>
        [System.Obsolete("Use overload with StandardDamageHandler!", true)]
        public static bool WillDie(this Player player, DamageHandlerBase handler)
            => WillDie(player, (StandardDamageHandler)handler);

        /// <summary>
        /// Returns if player will die because of damage caused by <paramref name="handler"/>.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="handler">Damage Cause.</param>
        /// <returns>If player will die because of this damage.</returns>
        public static bool WillDie(this Player player, StandardDamageHandler handler)
        {
            var tmp = player.ActiveArtificialHealthProcesses.Select(x => new { Process = x, x.CurrentAmount });
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
            var tmp = player.ActiveArtificialHealthProcesses.Select(x => new { Process = x, x.CurrentAmount });
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
            => GetRealDamageAmount(player, handler, out _, out _);
    }
}
