// -----------------------------------------------------------------------
// <copyright file="CassieExtensions.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using JetBrains.Annotations;
using NorthwoodLib.Pools;
using Respawning;
using System.Collections.Generic;
using System.Text;

namespace Mistaken.API.Extensions
{
    /// <summary>
    /// Cassie extensions.
    /// </summary>
    [PublicAPI]
    public static class CassieExtensions
    {
        /// <summary>
        /// Plays an CASSIE announcement with custom subtitles and gliches if <paramref name="glitchChance"/> or <paramref name="jamChance"/> is set higher than 0f.
        /// </summary>
        /// <param name="message">Message for CASSIE.</param>
        /// <param name="translation">Message displayed in subtitles.</param>
        /// <param name="glitchChance">Glitch chance.</param>
        /// <param name="jamChance">Jam chance.</param>
        /// <param name="isHeld">Is held.</param>
        /// <param name="isNoisy">Is noisy.</param>
        public static void MessageTranslated(string message, string translation, float glitchChance = 0f, float jamChance = 0f, bool isHeld = false, bool isNoisy = true)
        {
            if (glitchChance > 0f || jamChance > 0f)
            {
                var array = message.Split(' ');
                List<string> newWords = ListPool<string>.Shared.Rent();

                for (var i = 0; i < array.Length; i++)
                {
                    newWords.Add(array[i]);

                    if (i < array.Length - 1)
                    {
                        if (UnityEngine.Random.value < glitchChance)
                            newWords.Add(".G" + UnityEngine.Random.Range(1, 7));

                        if (UnityEngine.Random.value < jamChance)
                            newWords.Add("JAM_" + UnityEngine.Random.Range(0, 70).ToString("000") + "_" + UnityEngine.Random.Range(2, 6));
                    }
                }

                message = string.Empty;

                foreach (var newWord in newWords)
                    message += newWord + " ";

                ListPool<string>.Shared.Return(newWords);
            }

            StringBuilder announcement = StringBuilderPool.Shared.Rent();
            string[] cassies = message.Split('\n');
            string[] translations = translation.Split('\n');

            for (int i = 0; i < cassies.Length; i++)
                announcement.Append($"{translations[i].Replace(' ', ' ')}<size=0> {cassies[i]} </size><split>");

            RespawnEffectsController.PlayCassieAnnouncement(announcement.ToString(), isHeld, isNoisy, true);
            StringBuilderPool.Shared.Return(announcement);
        }
    }
}
