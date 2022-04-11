// -----------------------------------------------------------------------
// <copyright file="RegisterDoorTypesOnLevelLoadPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.Events.Extensions;
using static Exiled.Events.Events;

namespace Mistaken.API.Patches
{
    internal static class RegisterDoorTypesOnLevelLoadPatch
    {
        public static event CustomEventHandler GeneratedCache;

        public static void Postfix()
        {
            MEC.Timing.RunCoroutine(InvokeDelay());
        }

        private static IEnumerator<float> InvokeDelay()
        {
            yield return MEC.Timing.WaitForSeconds(0.3f);
            GeneratedCache.InvokeSafely();
        }
    }
}
