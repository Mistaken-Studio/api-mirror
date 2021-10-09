// -----------------------------------------------------------------------
// <copyright file="NicknamePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mistaken.API.Patches.Vars
{
    internal static class NicknamePatch
    {
        public static void Postfix(NicknameSync __instance, string value)
        {
            AnnonymousEvents.Call("NICKNAME", (Player.Get(__instance.gameObject), value));
        }
    }
}
