// -----------------------------------------------------------------------
// <copyright file="SessionVariableExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using PluginAPI.Core;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// SessionVariable Extensions.
    /// </summary>
    public static class SessionVariableExtensions
    {
        /// <summary>
        /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="player">Player.</param>
        /// <param name="type">Session Var.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Value.</returns>
        public static T GetSessionVariable<T>(this MPlayer player, SessionVarType type, T defaultValue = default)
            => player.GetSessionVariable(type.ToString(), defaultValue);

        /// <summary>
        /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="player">Player.</param>
        /// <param name="name">Session Var.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Value.</returns>
        public static T GetSessionVariable<T>(this MPlayer player, string name, T defaultValue = default)
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
        public static bool TryGetSessionVariable<T>(this MPlayer player, string name, out T value)
        {
            value = default;

            if (!player.SessionVariables.TryGetValue(name, out var val))
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
        public static bool TryGetSessionVariable<T>(this MPlayer player, SessionVarType type, out T value)
            => player.TryGetSessionVariable(type.ToString(), out value);

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="type">Session Var.</param>
        /// <param name="value">Value.</param>
        public static void SetSessionVariable(this MPlayer player, SessionVarType type, object value)
            => player.SetSessionVariable(type.ToString(), value);

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="name">Session Var.</param>
        /// <param name="value">Value.</param>
        public static void SetSessionVariable(this MPlayer player, string name, object value)
            => player.SessionVariables[name] = value;

        /// <summary>
        /// Removes SessionVar.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="type">Session Var.</param>
        public static void RemoveSessionVariable(this MPlayer player, SessionVarType type)
            => player.RemoveSessionVariable(type.ToString());

        /// <summary>
        /// Removes SessionVar.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="name">Session Var.</param>
        public static void RemoveSessionVariable(this MPlayer player, string name)
            => player.SessionVariables.Remove(name);
    }
}
