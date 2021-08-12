// -----------------------------------------------------------------------
// <copyright file="GodModePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mistaken.API.Patches.Vars
{
    internal static class GodModePatch
    {
        public static void Postfix(Player __instance, bool value)
        {
            AnnonymousEvents.Call("GOD_MODE", (__instance, value));
        }
    }
}
