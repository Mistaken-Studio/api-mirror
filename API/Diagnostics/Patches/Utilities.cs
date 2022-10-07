// -----------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reflection;
using Exiled.API.Features;

namespace Mistaken.API.Diagnostics.Patches
{
    internal static class Utilities
    {
        internal static void LogTime(MethodInfo method, double time)
        {
            MasterHandler.LogTime(method.ReflectedType?.FullName + "." + method.Name, time);
        }

        internal static void LogException(Exception ex, MethodInfo method, string eventName)
        {
            var sourceClassName = method.ReflectedType?.FullName;
            var methodName = method.Name;
            MasterHandler.LogError(ex, sourceClassName + "." + methodName);
            Log.Error(string.Concat(new string[]
            {
                "Method \"",
                methodName,
                "\" of the class \"",
                sourceClassName,
                "\" caused an exception when handling the event \"",
                eventName,
                "\"",
            }));
            Log.Error(ex);
        }
    }
}
