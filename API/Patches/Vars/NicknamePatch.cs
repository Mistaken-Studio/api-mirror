// -----------------------------------------------------------------------
// <copyright file="NicknamePatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using JetBrains.Annotations;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mistaken.API.Patches.Vars
{
    [UsedImplicitly]
    internal static class NicknamePatch
    {
        // ReSharper disable once InconsistentNaming
        public static void Postfix(NicknameSync __instance, string value)
        {
            API.AnnonymousEvents.Call("NICKNAME", (Player.Get(__instance.gameObject), value));
        }
    }
}
