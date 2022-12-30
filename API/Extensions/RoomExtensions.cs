// -----------------------------------------------------------------------
// <copyright file="RoomExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using PluginAPI.Core.Zones;
using UnityEngine;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Room Extensions.
    /// </summary>
    public static class RoomExtensions
    {
        /// <summary>
        /// Returns room offseted position.
        /// </summary>
        /// <param name="room">Room.</param>
        /// <param name="offset">Offset.</param>
        /// <returns>Position.</returns>
        public static Vector3 GetByRoomOffset(this FacilityRoom room, Vector3 offset)
        {
            var basePos = room.Position;
            offset = (room.Transform.right * -offset.x) + (room.Transform.forward * -offset.z) + (Vector3.up * offset.y);
            basePos += offset;
            return basePos;
        }
    }
}
