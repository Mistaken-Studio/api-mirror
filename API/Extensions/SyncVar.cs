// -----------------------------------------------------------------------
// <copyright file="SyncVar.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;

namespace Mistaken.API.Extensions
{
    internal static class SyncVar
    {
        private static void TargetSetBadge(this Player player, Player target, string name, string color)
            => SyncVarExtensions.TargetSetBadge(player, target, name, color);

        private static void TargetSetNickname(this Player player, Player target, string nickname)
            => SyncVarExtensions.TargetSetNickname(player, target, nickname);

        private static void ChangeAppearance(this Player player, Player target, RoleType type)
            => SyncVarExtensions.ChangeAppearance(player, target, type);
    }
}
