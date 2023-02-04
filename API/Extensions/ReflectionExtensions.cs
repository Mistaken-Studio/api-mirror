// -----------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using LiteNetLib.Utils;
using PluginAPI.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Reflection Extensions.
    /// </summary>
    public static class ReflectionExtensions
    {
        public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
        {
            type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod)?.Invoke(null, param);
        }

        public static void CopyProperties(this object target, object source)
        {
            Type type = target.GetType();
            if (type != source.GetType())
            {
                throw new InvalidTypeException("Target and source type mismatch!");
            }
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                type.GetProperty(propertyInfo.Name)?.SetValue(target, propertyInfo.GetValue(source, null), null);
            }
        }

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
                Log.Error(ex.ToString());
                return ex.Types.Where(x => x != null).ToArray();
            }
        }
    }
}
