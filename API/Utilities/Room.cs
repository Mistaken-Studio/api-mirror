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
using MEC;

namespace Mistaken.API.Utilities
{
    /// <summary>
    /// Mapped <see cref="Exiled.API.Features.Room"/>.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Gets lCZ Rooms.
        /// </summary>
        public static Room[,] LCZ { get; private set; } = new Room[0, 0];

        /// <summary>
        /// Gets hCZ Rooms.
        /// </summary>
        public static Room[,] HCZ { get; private set; } = new Room[0, 0];

        /// <summary>
        /// Gets eZ Rooms.
        /// </summary>
        public static Room[,] EZ { get; private set; } = new Room[0, 0];

        /// <summary>
        /// Gets eZ and HCZ Rooms.
        /// </summary>
        public static Room[,] EZ_HCZ { get; private set; } = new Room[0, 0];

        /// <inheritdoc cref="Exiled.API.Features.Room"/>
        public Exiled.API.Features.Room ExiledRoom { get; }

        /// <summary>
        /// Gets neighbor rooms.
        /// </summary>
        public Room[] Neighbors { get; private set; }

        /// <summary>
        /// Gets far Neighbor rooms (Neighbors + Neighbors' Neighbors forward from this room the same way as neighbor.
        /// </summary>
        public Room[] FarNeighbors
        {
            get => this.farNeighbors ?? this.UpdateFarNeighbors();
        }

        internal static void Reload()
        {
            try
            {
                var lczRooms = Exiled.API.Features.Map.Rooms.Where(r => r.Zone == ZoneType.LightContainment);

                List<int> zAxis = new List<int>();
                List<int> xAxis = new List<int>();
                try
                {
                    foreach (var item in lczRooms)
                    {
                        int z = (int)Math.Floor(item.Position.z);
                        int x = (int)Math.Floor(item.Position.x);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 3");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                xAxis.Sort();
                zAxis.Sort();
                try
                {
                    for (int i = 0; i < xAxis.Count; i++)
                    {
                        try
                        {
                            var x = xAxis[i];
                            if (!lczRooms.Any(p => (int)Math.Floor(p.Position.x) == x))
                            {
                                xAxis.RemoveAt(i);
                                i--;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("CatchId: 6.1");
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 6");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                zAxis.Reverse();
                try
                {
                    LCZ = new Room[zAxis.Count, xAxis.Count];
                    for (int i = 0; i < zAxis.Count; i++)
                    {
                        try
                        {
                            var z = zAxis[i];
                            for (int j = 0; j < xAxis.Count; j++)
                            {
                                try
                                {
                                    var x = xAxis[j];
                                    LCZ[i, j] = new Room(lczRooms.FirstOrDefault(p => (int)Math.Floor(p.Position.z) == z && (int)Math.Floor(p.Position.x) == x));
                                }
                                catch (System.Exception ex)
                                {
                                    Log.Error("CatchId: 4.2");
                                    Log.Error(ex.Message);
                                    Log.Error(ex.StackTrace);
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("CatchId: 4.1");
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 4");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                var hczRooms = Exiled.API.Features.Map.Rooms.Where(r => r.Zone == ZoneType.HeavyContainment && r.Type != RoomType.Pocket);

                zAxis.Clear();
                xAxis.Clear();
                try
                {
                    foreach (var item in hczRooms)
                    {
                        int z = (int)Math.Floor(item.Position.z);
                        int x = (int)Math.Floor(item.Position.x);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 3");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                xAxis.Sort();
                zAxis.Sort();
                try
                {
                    for (int i = 0; i < xAxis.Count; i++)
                    {
                        try
                        {
                            var x = xAxis[i];
                            if (!hczRooms.Any(p => (int)Math.Floor(p.Position.x) == x))
                            {
                                xAxis.RemoveAt(i);
                                i--;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("CatchId: 6.1");
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 6");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                zAxis.Reverse();
                try
                {
                    HCZ = new Room[zAxis.Count, xAxis.Count];
                    for (int i = 0; i < zAxis.Count; i++)
                    {
                        try
                        {
                            var z = zAxis[i];
                            for (int j = 0; j < xAxis.Count; j++)
                            {
                                try
                                {
                                    var x = xAxis[j];
                                    HCZ[i, j] = new Room(hczRooms.FirstOrDefault(p => (int)Math.Floor(p.Position.z) == z && (int)Math.Floor(p.Position.x) == x));
                                }
                                catch (System.Exception ex)
                                {
                                    Log.Error("CatchId: 4.2");
                                    Log.Error(ex.Message);
                                    Log.Error(ex.StackTrace);
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("CatchId: 4.1");
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 4");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                var ezRooms = Exiled.API.Features.Map.Rooms.Where(r => r.Zone == ZoneType.Entrance);

                zAxis.Clear();
                xAxis.Clear();
                try
                {
                    foreach (var item in ezRooms)
                    {
                        int z = (int)Math.Floor(item.Position.z);
                        int x = (int)Math.Floor(item.Position.x);
                        if (!zAxis.Contains(z))
                            zAxis.Add(z);
                        if (!xAxis.Contains(x))
                            xAxis.Add(x);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 3");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                xAxis.Sort();
                zAxis.Sort();
                try
                {
                    for (int i = 0; i < xAxis.Count; i++)
                    {
                        try
                        {
                            var x = xAxis[i];
                            if (!ezRooms.Any(p => (int)Math.Floor(p.Position.x) == x))
                            {
                                xAxis.RemoveAt(i);
                                i--;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("CatchId: 6.1");
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 6");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                zAxis.Reverse();
                try
                {
                    EZ = new Room[zAxis.Count, xAxis.Count];
                    for (int i = 0; i < zAxis.Count; i++)
                    {
                        try
                        {
                            var z = zAxis[i];
                            for (int j = 0; j < xAxis.Count; j++)
                            {
                                try
                                {
                                    var x = xAxis[j];
                                    EZ[i, j] = new Room(ezRooms.FirstOrDefault(p => (int)Math.Floor(p.Position.z) == z && (int)Math.Floor(p.Position.x) == x));
                                }
                                catch (System.Exception ex)
                                {
                                    Log.Error("CatchId: 4.2");
                                    Log.Error(ex.Message);
                                    Log.Error(ex.StackTrace);
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("CatchId: 4.1");
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 4");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                var ezHczRooms = Exiled.API.Features.Map.Rooms.Where(r => (r.Zone == ZoneType.HeavyContainment || r.Zone == ZoneType.Entrance) && r.Type != RoomType.Pocket);

                zAxis.Clear();
                xAxis.Clear();
                try
                {
                    foreach (var item in ezHczRooms)
                    {
                        try
                        {
                            int z = (int)Math.Floor(item.Position.z);
                            int x = (int)Math.Floor(item.Position.x);
                            if (!zAxis.Contains(z))
                                zAxis.Add(z);
                            if (!xAxis.Contains(x))
                                xAxis.Add(x);
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("CatchId: 5.1");
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 5");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                xAxis.Sort();
                zAxis.Sort();
                try
                {
                    for (int i = 0; i < xAxis.Count; i++)
                    {
                        try
                        {
                            var x = xAxis[i];
                            if (!ezHczRooms.Any(p => (int)Math.Floor(p.Position.x) == x))
                            {
                                xAxis.RemoveAt(i);
                                i--;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("CatchId: 6.1");
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 6");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }

                zAxis.Reverse();
                EZ_HCZ = new Room[zAxis.Count, xAxis.Count];
                try
                {
                    for (int i = 0; i < zAxis.Count; i++)
                    {
                        try
                        {
                            var z = zAxis[i];
                            for (int j = 0; j < xAxis.Count; j++)
                            {
                                try
                                {
                                    var x = xAxis[j];
                                    EZ_HCZ[i, j] = new Room(ezHczRooms.FirstOrDefault(p => (int)Math.Floor(p.Position.z) == z && (int)Math.Floor(p.Position.x) == x));
                                }
                                catch (System.Exception ex)
                                {
                                    Log.Error("CatchId: 7.2");
                                    Log.Error(ex.Message);
                                    Log.Error(ex.StackTrace);
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("CatchId: 7.1");
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("CatchId: 7");
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("CatchId: 0");
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }

            foreach (var item in LCZ)
                item.Initialize();

            foreach (var item in EZ_HCZ)
                item.Initialize();
        }

        internal Room(Exiled.API.Features.Room exiledRoom)
        {
            if (exiledRoom is null)
                throw new ArgumentNullException(nameof(exiledRoom));

            this.ExiledRoom = exiledRoom;
        }

        internal int MyX { get; private set; }

        internal int MyY { get; private set; }

        internal void Initialize()
        {
            this.UpdateMyXAndMyY();
            this.UpdateNeighbors();
        }

        private Room[] farNeighbors = null;

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
                    break;
            }
        }

        private void UpdateNeighbors()
        {
            HashSet<Room> list = new HashSet<Room>();
            switch (this.ExiledRoom.Zone)
            {
                case ZoneType.Entrance:
                case ZoneType.HeavyContainment:
                    for (int x = Math.Max(0, this.MyX - 1); x < Math.Min(EZ_HCZ.GetLength(0), this.MyX + 1); x++)
                    {
                        for (int y = Math.Max(0, this.MyY - 1); y < Math.Min(EZ_HCZ.GetLength(1), this.MyY + 1); y++)
                        {
                            list.Add(EZ_HCZ[x, y]);
                        }
                    }

                    break;

                case ZoneType.LightContainment:
                    for (int x = Math.Max(0, this.MyX - 1); x < Math.Min(LCZ.GetLength(0), this.MyX + 1); x++)
                    {
                        for (int y = Math.Max(0, this.MyY - 1); y < Math.Min(LCZ.GetLength(1), this.MyY + 1); y++)
                        {
                            list.Add(LCZ[x, y]);
                        }
                    }

                    break;

                default:
                    break;
            }

            list.Remove(this);
            this.Neighbors = list.ToArray();
        }

        private Room[] UpdateFarNeighbors()
        {
            HashSet<Room> list = new HashSet<Room>();
            foreach (var neighbor in this.Neighbors)
            {
                list.Add(neighbor);
                foreach (var farNeighbor in neighbor.Neighbors)
                {
                    if (this.MyX > neighbor.MyX && neighbor.MyX > farNeighbor.MyX)
                        list.Add(farNeighbor);
                    else if (this.MyX < neighbor.MyX && neighbor.MyX < farNeighbor.MyX)
                        list.Add(farNeighbor);
                    else if (this.MyY > neighbor.MyY && neighbor.MyY > farNeighbor.MyY)
                        list.Add(farNeighbor);
                    else if (this.MyY < neighbor.MyY && neighbor.MyY < farNeighbor.MyY)
                        list.Add(farNeighbor);
                }
            }

            list.Remove(this);
            this.farNeighbors = list.ToArray();
            return this.farNeighbors;
        }
    }
}
