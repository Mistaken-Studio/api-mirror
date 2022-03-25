// -----------------------------------------------------------------------
// <copyright file="RegisterDoorTypesOnLevelLoadPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.Events.Extensions;
using static Exiled.Events.Events;

namespace Mistaken.API.Patches
{
    internal static class RegisterDoorTypesOnLevelLoadPatch
    {
        public static event CustomEventHandler GeneratedCache;

        public static void Postfix()
        {
            GeneratedCache.InvokeSafely();
        }
    }
}
