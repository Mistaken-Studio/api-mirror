// -----------------------------------------------------------------------
// <copyright file="MasterHandler.Entry.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Mistaken.API.Diagnostics
{
    public static partial class MasterHandler
    {
        internal struct Entry
        {
            public DateTime Time;
            public string Name;
            public double TimeTook;

            public Entry(string name, double timeTook)
            {
                this.Time = DateTime.Now;
                this.Name = name;
                this.TimeTook = Math.Round(timeTook, 3);
            }

            public override string ToString()
                => $"{this.Time.Ticks}|{this.Name}|{this.TimeTook}";
        }
    }
}
