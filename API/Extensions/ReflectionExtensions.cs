// -----------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Reflection Extensions.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets all loadable types from <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <returns>Array of loadable types.</returns>
        public static Type[] GetLoadableTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Log.Error($"Assemby: {assembly.FullName}");
                Log.Error(ex);
                return ex.Types.Where(x => x != null).ToArray();
            }
        }
    }
}
