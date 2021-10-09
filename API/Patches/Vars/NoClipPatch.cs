// -----------------------------------------------------------------------
// <copyright file="NoClipPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mistaken.API.Patches.Vars
{
    internal static class NoClipPatch
    {
        public static void Postfix(ServerRoles __instance, byte status)
        {
            AnnonymousEvents.Call("NOCLIP", (Player.Get(__instance.gameObject), status));
        }
    }
}
