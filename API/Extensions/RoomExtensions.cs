// -----------------------------------------------------------------------
// <copyright file="RoomExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
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
        /// <param name="me">Room.</param>
        /// <param name="offset">Offset.</param>
        /// <returns>Position.</returns>
        public static Vector3 GetByRoomOffset(this Room me, Vector3 offset)
        {
            var basePos = me.Position;
            offset = (me.transform.right * -offset.x) + (me.transform.forward * -offset.z) + (Vector3.up * offset.y);
            basePos += offset;
            return basePos;
        }
    }
}
