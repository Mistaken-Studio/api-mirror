// -----------------------------------------------------------------------
// <copyright file="Room.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;

namespace Mistaken.API.Utilities
{
    /// <summary>
    /// Mapped <see cref="Exiled.API.Features.Room"/>.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Rooms.
        /// </summary>
        public static readonly Dictionary<Exiled.API.Features.Room, Room> Rooms = new Dictionary<Exiled.API.Features.Room, Room>();

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
        /// <param name="room">Exiled's Room.</param>
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
        public IEnumerable<Door> ExternalDoors => (this.doors ?? this.UpdateDoors()).Except(this.ExiledRoom.Doors);

        /// <summary>
        /// Gets doors.
        /// </summary>
        public IEnumerable<Door> Doors => this.doors.Union(this.ExiledRoom.Doors);

        internal static void Reload()
        {
            Rooms.Clear();
            try
            {
                var lczRooms = Exiled.API.Features.Map.Rooms.Where(r => r.Zone == ZoneType.LightContainment);

                List<int> zAxis = new List<int>();
                List<int> xAxis = new List<int>();
                foreach (var item in lczRooms)
                {
                    try
                    {
                        int z = Round(item.Position.z, 5);
                        int x = Round(item.Position.x, 5);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                xAxis.Sort();
                zAxis.Sort();
                for (int i = 0; i < xAxis.Count; i++)
                {
                    try
                    {
                        var x = xAxis[i];
                        if (!lczRooms.Any(p => Round(p.Position.x, 5) == x))
                        {
                            xAxis.RemoveAt(i);
                            i--;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                zAxis.Reverse();
                try
                {
                    LCZ = new Room[zAxis.Count, xAxis.Count];
                    for (int i = 0; i < zAxis.Count; i++)
                    {
                        var z = zAxis[i];
                        for (int j = 0; j < xAxis.Count; j++)
                        {
                            try
                            {
                                var x = xAxis[j];
                                var room = lczRooms.FirstOrDefault(p => Round(p.Position.z, 5) == z && Round(p.Position.x, 5) == x);
                                if (room is null)
                                    LCZ[i, j] = null;
                                else
                                    LCZ[i, j] = Room.Get(room);
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }

                var hczRooms = Exiled.API.Features.Map.Rooms.Where(r => r.Zone == ZoneType.HeavyContainment && r.Type != RoomType.Pocket);

                zAxis.Clear();
                xAxis.Clear();
                try
                {
                    foreach (var item in hczRooms)
                    {
                        int z = Round(item.Position.z, 5);
                        int x = Round(item.Position.x, 5);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }

                xAxis.Sort();
                zAxis.Sort();

                for (int i = 0; i < xAxis.Count; i++)
                {
                    try
                    {
                        var x = xAxis[i];
                        if (!hczRooms.Any(p => Round(p.Position.x, 5) == x))
                        {
                            xAxis.RemoveAt(i);
                            i--;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                zAxis.Reverse();
                try
                {
                    HCZ = new Room[zAxis.Count, xAxis.Count];
                    for (int i = 0; i < zAxis.Count; i++)
                    {
                        var z = zAxis[i];
                        for (int j = 0; j < xAxis.Count; j++)
                        {
                            try
                            {
                                var x = xAxis[j];
                                var room = hczRooms.FirstOrDefault(p => Round(p.Position.z, 5) == z && Round(p.Position.x, 5) == x);
                                if (room is null)
                                    HCZ[i, j] = null;
                                else
                                    HCZ[i, j] = Room.Get(room);
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }

                var ezRooms = Exiled.API.Features.Map.Rooms.Where(r => r.Zone == ZoneType.Entrance);

                zAxis.Clear();
                xAxis.Clear();
                try
                {
                    foreach (var item in ezRooms)
                    {
                        int z = Round(item.Position.z, 5);
                        int x = Round(item.Position.x, 5);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }

                xAxis.Sort();
                zAxis.Sort();

                for (int i = 0; i < xAxis.Count; i++)
                {
                    try
                    {
                        var x = xAxis[i];
                        if (!ezRooms.Any(p => Round(p.Position.x, 5) == x))
                        {
                            xAxis.RemoveAt(i);
                            i--;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                zAxis.Reverse();
                try
                {
                    EZ = new Room[zAxis.Count, xAxis.Count];
                    for (int i = 0; i < zAxis.Count; i++)
                    {
                        var z = zAxis[i];
                        for (int j = 0; j < xAxis.Count; j++)
                        {
                            try
                            {
                                var x = xAxis[j];
                                var room = ezRooms.FirstOrDefault(p => Round(p.Position.z, 5) == z && Round(p.Position.x, 5) == x);
                                if (room is null)
                                    EZ[i, j] = null;
                                else
                                    EZ[i, j] = Room.Get(room);
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }

                var ezHczRooms = Exiled.API.Features.Map.Rooms.Where(r => (r.Zone == ZoneType.HeavyContainment || r.Zone == ZoneType.Entrance) && r.Type != RoomType.Pocket);

                zAxis.Clear();
                xAxis.Clear();
                foreach (var item in ezHczRooms)
                {
                    try
                    {
                        int z = Round(item.Position.z, 5);
                        int x = Round(item.Position.x, 5);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                xAxis.Sort();
                zAxis.Sort();
                for (int i = 0; i < xAxis.Count; i++)
                {
                    try
                    {
                        var x = xAxis[i];
                        if (!ezHczRooms.Any(p => Round(p.Position.x, 5) == x))
                        {
                            xAxis.RemoveAt(i);
                            i--;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex);
                    }
                }

                zAxis.Reverse();
                EZ_HCZ = new Room[zAxis.Count, xAxis.Count];
                try
                {
                    for (int i = 0; i < zAxis.Count; i++)
                    {
                        var z = zAxis[i];
                        for (int j = 0; j < xAxis.Count; j++)
                        {
                            try
                            {
                                var x = xAxis[j];
                                var room = ezHczRooms.FirstOrDefault(p => Round(p.Position.z, 5) == z && Round(p.Position.x, 5) == x);
                                if (room is null)
                                    EZ_HCZ[i, j] = null;
                                else
                                    EZ_HCZ[i, j] = Room.Get(room);
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex);
                }

                foreach (var item in LCZ)
                    item?.Initialize();

                foreach (var item in EZ_HCZ)
                    item?.Initialize();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
            }
        }

        internal Room(Exiled.API.Features.Room exiledRoom)
        {
            if (exiledRoom is null)
                throw new ArgumentNullException(nameof(exiledRoom));

            this.ExiledRoom = exiledRoom;
            Rooms.Add(exiledRoom, this);
        }

        internal int MyX { get; private set; }

        internal int MyY { get; private set; }

        internal void Initialize()
        {
            this.UpdateNeighbors();
        }

        private static int Round(float toRound, int roundTo = 5)
            => (int)(toRound - (toRound % roundTo));

        private Room[] farNeighbors = null;
        private Room[] neighbors = null;
        private Door[] doors = null;

        private void UpdateMyXAndMyY()
        {
            switch (this.ExiledRoom.Zone)
            {
                case ZoneType.Entrance:
                case ZoneType.HeavyContainment:
                    for (int x = 0; x < EZ_HCZ.GetLength(0); x++)
                    {
                        for (int y = 0; y < EZ_HCZ.GetLength(1); y++)
                        {
                            if (EZ_HCZ[x, y] == this)
                            {
                                this.MyX = x;
                                this.MyY = y;
                                return;
                            }
                        }
                    }

                    throw new Exception($"Can't find myX and myY for {this.ExiledRoom.Type} in Entrance or Heavy ({this.ExiledRoom.Zone})");

                case ZoneType.LightContainment:
                    for (int x = 0; x < LCZ.GetLength(0); x++)
                    {
                        for (int y = 0; y < LCZ.GetLength(1); y++)
                        {
                            if (LCZ[x, y] == this)
                            {
                                this.MyX = x;
                                this.MyY = y;
                                return;
                            }
                        }
                    }

                    throw new Exception($"Can't find myX and myY for {this.ExiledRoom.Type} in Light ({this.ExiledRoom.Zone})");

                default:
                    this.MyX = -1;
                    this.MyY = -1;
                    break;
            }
        }

        private Room[] UpdateNeighbors()
        {
            this.UpdateMyXAndMyY();
            HashSet<Room> list = new HashSet<Room>();
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
                    this.neighbors = new Room[0];
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
            HashSet<Room> list = new HashSet<Room>();
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

        private Door[] UpdateDoors()
        {
            HashSet<Door> list = new HashSet<Door>();
            foreach (var door in Exiled.API.Features.Map.Doors)
            {
                var dist = Vector3.Distance(door.Position, this.ExiledRoom.Position);
                if (dist <= 11 && dist >= 5)
                    list.Add(door);
            }

            this.doors = list.ToArray();
            return this.doors;
        }
    }
}
