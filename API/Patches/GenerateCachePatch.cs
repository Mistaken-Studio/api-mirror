// -----------------------------------------------------------------------
// <copyright file="GenerateCachePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.Events.Extensions;
using static Exiled.Events.Events;

namespace Mistaken.API.Patches
{
    internal static class GenerateCachePatch
    {
        public static event CustomEventHandler GeneratedCache;

        public static void Postfix()
        {
            MEC.Timing.RunCoroutine(InvokeDelay());
        }

        private static IEnumerator<float> InvokeDelay()
        {
            yield return MEC.Timing.WaitForOneFrame;
            GeneratedCache.InvokeSafely();
        }
    }
}
