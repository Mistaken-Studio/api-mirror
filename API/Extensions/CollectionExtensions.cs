// -----------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Collection Extensions.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Randomizes supplied list.
        /// </summary>
        /// <typeparam name="T">List Type.</typeparam>
        /// <param name="list">List to randomize.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random random = new System.Random();
            int i = list.Count;
            while (i > 1)
            {
                i--;
                int index = random.Next(i + 1);
                T value = list[index];
                list[index] = list[i];
                list[i] = value;
            }
        }

        /// <summary>
        /// Randomizes supplied list with seed.
        /// </summary>
        /// <typeparam name="T">List Type.</typeparam>
        /// <param name="list">List to randomize.</param>
        /// <param name="seed">Seed used to randomize.</param>
        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            System.Random random = new System.Random(seed);
            int i = list.Count;
            while (i > 1)
            {
                i--;
                int index = random.Next(i + 1);
                T value = list[index];
                list[index] = list[i];
                list[i] = value;
            }
        }

        /// <summary>
        /// Randomizes supplied array.
        /// </summary>
        /// <typeparam name="T">Array Type.</typeparam>
        /// <param name="list">Array to randomize.</param>
        public static T[] Shuffle<T>(this T[] list)
        {
            System.Random random = new System.Random();
            int i = list.Length;
            while (i > 1)
            {
                i--;
                int index = random.Next(i + 1);
                T value = list[index];
                list[index] = list[i];
                list[i] = value;
            }

            return list;
        }

        /// <summary>
        /// Randomizes supplied array with seed.
        /// </summary>
        /// <typeparam name="T">Array Type.</typeparam>
        /// <param name="list">Array to randomize.</param>
        /// <param name="seed">Seed used to randomize.</param>
        public static T[] Shuffle<T>(this T[] list, int seed)
        {
            System.Random random = new System.Random(seed);
            int i = list.Length;
            while (i > 1)
            {
                i--;
                int index = random.Next(i + 1);
                T value = list[index];
                list[index] = list[i];
                list[i] = value;
            }

            return list;
        }
    }
}