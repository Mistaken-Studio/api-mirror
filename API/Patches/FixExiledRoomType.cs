// -----------------------------------------------------------------------
// <copyright file="FixExiledRoomType.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;

namespace Mistaken.API.Patches
{
    [HarmonyPatch(typeof(Exiled.API.Features.Room), "FindType", typeof(string))]
    internal static class FixExiledRoomType
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        private static bool Prefix(ref RoomType __result, string rawName)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            if (rawName.RemoveBracketsOnEndOfName() == "LCZ_330")
            {
                __result = RoomType.Lcz012;

                return false;
            }

            return true;
        }
    }
}
