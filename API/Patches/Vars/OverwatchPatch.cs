// -----------------------------------------------------------------------
// <copyright file="OverwatchPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using JetBrains.Annotations;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mistaken.API.Patches.Vars
{
    [UsedImplicitly]
    internal static class OverwatchPatch
    {
        // ReSharper disable once InconsistentNaming
        public static void Postfix(ServerRoles __instance, bool status)
        {
            AnnonymousEvents.Call("OVERWATCH", (Player.Get(__instance.gameObject), status));
        }
    }
}
