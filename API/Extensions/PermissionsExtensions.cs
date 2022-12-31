// -----------------------------------------------------------------------
// <copyright file="PermissionsExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PluginAPI.Core;
using RemoteAdmin;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Permissions Extensions.
    /// </summary>
    public static class PermissionsExtensions
    {
        /// <summary>
        /// Returns if player has permission.
        /// </summary>
        /// <param name="sender">Player.</param>
        /// <param name="permission">Permission.</param>
        /// <returns>If has permisison.</returns>
        public static bool CheckPermission(this CommandSender sender, string permission)
        {
            if (sender.FullPermissions || sender is ServerConsoleSender)
                return true;

            if (sender.IsPlayer())
            {
                MPlayer player = sender.GetPlayer();
                return player is not null && player.CheckPermission(permission);
            }

            return false;
        }

        /// <summary>
        /// Returns if player has permission.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="permission">Permission.</param>
        /// <returns>If has permisison.</returns>
        public static bool CheckPermission(this MPlayer player, string permission)
        {
            if (player.IsDev())
                return true;
            else
            {
                try
                {
                    var perms = new List<string>();
                    var group = player.GroupName;

                    while (!string.IsNullOrWhiteSpace(group))
                    {
                        var groupObj = Exiled.Permissions.Extensions.Permissions.Groups[group];
                        perms.AddRange(groupObj.Permissions);
                        group = groupObj.Inheritance.FirstOrDefault();
                    }

                    if (perms.Contains(".*") || perms.Contains(permission) || perms.Contains(permission.Split('.')[0] + ".*"))
                        return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                return Exiled.Permissions.Extensions.Permissions.CheckPermission(player, permission);
            }
        }
    }
}
