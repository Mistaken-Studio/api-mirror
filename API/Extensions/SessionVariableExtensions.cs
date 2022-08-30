// -----------------------------------------------------------------------
// <copyright file="SessionVariableExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

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
        /// <param name="me">Player.</param>
        /// <param name="type">Session Var.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Value.</returns>
        public static T GetSessionVariable<T>(this Player me, SessionVarType type, T defaultValue = default)
            => me.GetSessionVariable(type.ToString(), defaultValue);

        /// <summary>
        /// Returns SessionVarValue or <paramref name="defaultValue"/> if was not found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="me">Player.</param>
        /// <param name="name">Session Var.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <returns>Value.</returns>
        public static T GetSessionVariable<T>(this Player me, string name, T defaultValue = default)
        {
            if (me.TryGetSessionVariable(name, out T value))
                return value;
            return defaultValue;
        }

        /// <summary>
        /// If SessionVar was found.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="me">Player.</param>
        /// <param name="type">Session Var.</param>
        /// <param name="value">Value.</param>
        /// <returns>If session var was found.</returns>
        public static bool TryGetSessionVariable<T>(this Player me, SessionVarType type, out T value)
            => me.TryGetSessionVariable(type.ToString(), out value);

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="type">Session Var.</param>
        /// <param name="value">Value.</param>
        public static void SetSessionVariable(this Player me, SessionVarType type, object value)
            => me.SetSessionVariable(type.ToString(), value);

        /// <summary>
        /// Sets SessionVarValue.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="name">Session Var.</param>
        /// <param name="value">Value.</param>
        public static void SetSessionVariable(this Player me, string name, object value)
            => me.SessionVariables[name] = value;

        /// <summary>
        /// Removes SessionVar.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="type">Session Var.</param>
        public static void RemoveSessionVariable(this Player me, SessionVarType type)
            => me.RemoveSessionVariable(type.ToString());

        /// <summary>
        /// Removes SessionVar.
        /// </summary>
        /// <param name="me">Player.</param>
        /// <param name="name">Session Var.</param>
        public static void RemoveSessionVariable(this Player me, string name)
            => me.SessionVariables.Remove(name);
    }
}
