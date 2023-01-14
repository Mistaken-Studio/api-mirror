// -----------------------------------------------------------------------
// <copyright file="Room.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

/*using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming
namespace Mistaken.API.Utilities
{
    /// <summary>
    /// Mapped <see cref="Exiled.API.Features.Room"/>.
    /// </summary>
    [PublicAPI]
    public class Room
    {
        /// <summary>
        /// Rooms.
        /// </summary>
        public static readonly Dictionary<Exiled.API.Features.Room, Room> Rooms = new();

        /// <summary>
        /// Gets LCZ Rooms.
        /// </summary>
        public static Room[,] LCZ { get; private set; } = new Room[0, 0];

        /// <summary>
        /// Gets HCZ Rooms.
        /// </summary>
        public static Room[,] HCZ { get; private set; } = new Room[0, 0];

        /// <summary>
        /// Gets EZ Rooms.
        /// </summary>
        public static Room[,] EZ { get; private set; } = new Room[0, 0];

        /// <summary>
        /// Gets EZ and HCZ Rooms.
        /// </summary>
        public static Room[,] EZ_HCZ { get; private set; } = new Room[0, 0];

        /// <summary>
        /// Gets room.
        /// </summary>
        /// <param name="room">Exiled Room.</param>
        /// <returns>Room.</returns>
        public static Room Get(Exiled.API.Features.Room room)
            => room is null ? null : (Rooms.ContainsKey(room) ? Rooms[room] : new Room(room));

        /// <inheritdoc cref="Exiled.API.Features.Room"/>
        public Exiled.API.Features.Room ExiledRoom { get; }

        /// <summary>
        /// Gets neighbor rooms.
        /// </summary>
        public Room[] Neighbors => this.neighbors ?? this.UpdateNeighbors();

        /// <summary>
        /// Gets far Neighbor rooms (Neighbors + Neighbors' Neighbors forward from this room the same way as neighbor.
        /// </summary>
        public Room[] FarNeighbors => this.farNeighbors ?? this.UpdateFarNeighbors();

        /// <summary>
        /// Gets external doors.
        /// </summary>
        public IEnumerable<Door> ExternalDoors => this.ExiledRoom.Doors.Except(this.doors ?? this.UpdateInternalDoors());

        /// <summary>
        /// Gets doors.
        /// </summary>
        public IEnumerable<Door> Doors => this.doors ?? this.UpdateInternalDoors();

        internal static void Reload()
        {
            Rooms.Clear();
            try
            {
                var lczRooms = Exiled.API.Features.Room.List
                    .Where(r => r.Zone == ZoneType.LightContainment)
                    .ToArray();

                List<int> zAxis = new();
                List<int> xAxis = new();
                foreach (var item in lczRooms)
                {
                    try
                    {
                        var z = Round(item.Position.z);
                        var x = Round(item.Position.x);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                xAxis.Sort();
                zAxis.Sort();
                for (var i = 0; i < xAxis.Count; i++)
                {
                    try
                    {
                        var x = xAxis[i];
                        if (lczRooms.Any(p => Round(p.Position.x) == x))
                            continue;
                        xAxis.RemoveAt(i);
                        i--;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                zAxis.Reverse();
                try
                {
                    LCZ = new Room[zAxis.Count, xAxis.Count];
                    for (var i = 0; i < zAxis.Count; i++)
                    {
                        var z = zAxis[i];
                        for (var j = 0; j < xAxis.Count; j++)
                        {
                            try
                            {
                                var x = xAxis[j];
                                var room = lczRooms.FirstOrDefault(p => Round(p.Position.z) == z && Round(p.Position.x) == x);
                                if (room is null)
                                    LCZ[i, j] = null;
                                else
                                    LCZ[i, j] = Get(room);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                var hczRooms = Exiled.API.Features.Room.List
                    .Where(r => r.Zone == ZoneType.HeavyContainment && r.Type != RoomType.Pocket)
                    .ToArray();

                zAxis.Clear();
                xAxis.Clear();
                try
                {
                    foreach (var item in hczRooms)
                    {
                        var z = Round(item.Position.z);
                        var x = Round(item.Position.x);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                xAxis.Sort();
                zAxis.Sort();

                for (var i = 0; i < xAxis.Count; i++)
                {
                    try
                    {
                        var x = xAxis[i];

                        if (hczRooms.Any(p => Round(p.Position.x) == x))
                            continue;

                        xAxis.RemoveAt(i);
                        i--;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                zAxis.Reverse();
                try
                {
                    HCZ = new Room[zAxis.Count, xAxis.Count];
                    for (var i = 0; i < zAxis.Count; i++)
                    {
                        var z = zAxis[i];
                        for (var j = 0; j < xAxis.Count; j++)
                        {
                            try
                            {
                                var x = xAxis[j];
                                var room = hczRooms.FirstOrDefault(p => Round(p.Position.z) == z && Round(p.Position.x) == x);
                                if (room is null)
                                    HCZ[i, j] = null;
                                else
                                    HCZ[i, j] = Get(room);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                var ezRooms = Exiled.API.Features.Room.List
                    .Where(r => r.Zone == ZoneType.Entrance)
                    .ToArray();

                zAxis.Clear();
                xAxis.Clear();
                try
                {
                    foreach (var item in ezRooms)
                    {
                        var z = Round(item.Position.z);
                        var x = Round(item.Position.x);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                xAxis.Sort();
                zAxis.Sort();

                for (var i = 0; i < xAxis.Count; i++)
                {
                    try
                    {
                        var x = xAxis[i];
                        if (ezRooms.Any(p => Round(p.Position.x) == x))
                            continue;

                        xAxis.RemoveAt(i);
                        i--;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                zAxis.Reverse();
                try
                {
                    EZ = new Room[zAxis.Count, xAxis.Count];
                    for (var i = 0; i < zAxis.Count; i++)
                    {
                        var z = zAxis[i];
                        for (var j = 0; j < xAxis.Count; j++)
                        {
                            try
                            {
                                var x = xAxis[j];
                                var room = ezRooms.FirstOrDefault(p => Round(p.Position.z) == z && Round(p.Position.x) == x);
                                if (room is null)
                                    EZ[i, j] = null;
                                else
                                    EZ[i, j] = Get(room);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                var ezHczRooms = Exiled.API.Features.Room.List
                    .Where(r => r.Zone is ZoneType.HeavyContainment or ZoneType.Entrance && r.Type != RoomType.Pocket)
                    .ToArray();

                zAxis.Clear();
                xAxis.Clear();
                foreach (var item in ezHczRooms)
                {
                    try
                    {
                        var z = Round(item.Position.z);
                        var x = Round(item.Position.x);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                xAxis.Sort();
                zAxis.Sort();
                for (var i = 0; i < xAxis.Count; i++)
                {
                    try
                    {
                        var x = xAxis[i];
                        if (ezHczRooms.Any(p => Round(p.Position.x) == x))
                            continue;
                        xAxis.RemoveAt(i);
                        i--;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                zAxis.Reverse();
                EZ_HCZ = new Room[zAxis.Count, xAxis.Count];
                try
                {
                    for (var i = 0; i < zAxis.Count; i++)
                    {
                        var z = zAxis[i];
                        for (var j = 0; j < xAxis.Count; j++)
                        {
                            try
                            {
                                var x = xAxis[j];
                                var room = ezHczRooms.FirstOrDefault(p => Round(p.Position.z) == z && Round(p.Position.x) == x);
                                if (room is null)
                                    EZ_HCZ[i, j] = null;
                                else
                                    EZ_HCZ[i, j] = Get(room);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                foreach (var item in LCZ)
                    item?.Initialize();

                foreach (var item in EZ_HCZ)
                    item?.Initialize();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        internal Room(Exiled.API.Features.Room exiledRoom)
        {
            this.ExiledRoom = exiledRoom ?? throw new ArgumentNullException(nameof(exiledRoom));
            Rooms.Add(exiledRoom, this);
        }

        internal int MyX { get; private set; } = -1;

        internal int MyY { get; private set; } = -1;

        internal void Initialize()
        {
            this.UpdateNeighbors();
        }

        private static int Round(float toRound, int roundTo = 5)
            => (int)(toRound - (toRound % roundTo));

        private Room[] farNeighbors;
        private Room[] neighbors;
        private Door[] doors;

        private void UpdateMyXAndMyY()
        {
            if (this.ExiledRoom.Type == RoomType.Pocket)
            {
                this.MyX = -1;
                this.MyY = -1;
                return;
            }

            switch (this.ExiledRoom.Zone)
            {
                case ZoneType.Entrance:
                case ZoneType.HeavyContainment:
                    for (var x = 0; x < EZ_HCZ.GetLength(0); x++)
                    {
                        for (var y = 0; y < EZ_HCZ.GetLength(1); y++)
                        {
                            if (EZ_HCZ[x, y] == this)
                            {
                                this.MyX = x;
                                this.MyY = y;
                                return;
                            }
                        }
                    }

                    throw new($"Can't find myX and myY for {this.ExiledRoom.Type} in Entrance or Heavy ({this.ExiledRoom.Zone})");

                case ZoneType.LightContainment:
                    for (var x = 0; x < LCZ.GetLength(0); x++)
                    {
                        for (var y = 0; y < LCZ.GetLength(1); y++)
                        {
                            if (LCZ[x, y] == this)
                            {
                                this.MyX = x;
                                this.MyY = y;
                                return;
                            }
                        }
                    }

                    throw new($"Can't find myX and myY for {this.ExiledRoom.Type} in Light ({this.ExiledRoom.Zone})");

                default:
                    this.MyX = -1;
                    this.MyY = -1;
                    break;
            }
        }

        private Room[] UpdateNeighbors()
        {
            this.UpdateMyXAndMyY();
            if (this.MyX == -1 || this.MyY == -1)
            {
                this.neighbors = Array.Empty<Room>();
                return this.neighbors;
            }

            HashSet<Room> list = new();
            switch (this.ExiledRoom.Zone)
            {
                case ZoneType.Entrance:
                case ZoneType.HeavyContainment:

                    if (this.MyX - 1 >= 0)
                        list.Add(EZ_HCZ[this.MyX - 1, this.MyY]);
                    if (this.MyX + 1 < EZ_HCZ.GetLength(0))
                        list.Add(EZ_HCZ[this.MyX + 1, this.MyY]);

                    if (this.MyY - 1 >= 0)
                        list.Add(EZ_HCZ[this.MyX, this.MyY - 1]);
                    if (this.MyY + 1 < EZ_HCZ.GetLength(1))
                        list.Add(EZ_HCZ[this.MyX, this.MyY + 1]);

                    break;

                case ZoneType.LightContainment:

                    if (this.MyX - 1 >= 0)
                        list.Add(LCZ[this.MyX - 1, this.MyY]);
                    if (this.MyX + 1 < LCZ.GetLength(0))
                        list.Add(LCZ[this.MyX + 1, this.MyY]);

                    if (this.MyY - 1 >= 0)
                        list.Add(LCZ[this.MyX, this.MyY - 1]);
                    if (this.MyY + 1 < LCZ.GetLength(1))
                        list.Add(LCZ[this.MyX, this.MyY + 1]);

                    break;

                default:
                    this.neighbors = Array.Empty<Room>();
                    return this.neighbors;
            }

            list.Remove(this);
            list.Remove(null);
            list.RemoveWhere(other => !other.ExternalDoors.Any(otherDoor => this.ExternalDoors.Contains(otherDoor)));
            this.neighbors = list.ToArray();
            return this.neighbors;
        }

        private Room[] UpdateFarNeighbors()
        {
            if (this.MyX == -1 || this.MyY == -1)
            {
                this.farNeighbors = Array.Empty<Room>();
                return this.farNeighbors;
            }

            HashSet<Room> list = new();
            if (this.Neighbors is null)
            {
                Log.Warn("Neighbor list is null");
                return null;
            }

            foreach (var neighbor in this.Neighbors)
            {
                if (neighbor is null)
                {
                    Log.Warn("Neighbor is null");
                    continue;
                }

                list.Add(neighbor);
                if (neighbor.Neighbors is null)
                {
                    Log.Warn("Neighbor's Neighbor list is null");
                    continue;
                }

                foreach (var farNeighbor in neighbor.Neighbors)
                {
                    if (farNeighbor is null)
                    {
                        Log.Warn("FarNeighbor is null");
                        continue;
                    }

                    if (this.MyY == farNeighbor.MyY)
                    {
                        if (this.MyX > neighbor.MyX && neighbor.MyX > farNeighbor.MyX)
                            list.Add(farNeighbor);
                        else if (this.MyX < neighbor.MyX && neighbor.MyX < farNeighbor.MyX)
                            list.Add(farNeighbor);
                    }
                    else if (this.MyX == farNeighbor.MyX)
                    {
                        if (this.MyY > neighbor.MyY && neighbor.MyY > farNeighbor.MyY)
                            list.Add(farNeighbor);
                        else if (this.MyY < neighbor.MyY && neighbor.MyY < farNeighbor.MyY)
                            list.Add(farNeighbor);
                    }
                }
            }

            list.Remove(this);
            this.farNeighbors = list.ToArray();
            return this.farNeighbors;
        }

        private Door[] UpdateInternalDoors()
        {
            HashSet<Door> list = this.ExiledRoom.Doors.ToHashSet();
            foreach (var room in Exiled.API.Features.Room.List)
            {
                if (room == this.ExiledRoom)
                    continue;

                foreach (var thisRoomsDoor in list.ToArray())
                {
                    if (room.Doors.Contains(thisRoomsDoor))
                        list.Remove(thisRoomsDoor);
                }
            }

            this.doors = list.ToArray();
            return this.doors;
        }
    }
}*/
