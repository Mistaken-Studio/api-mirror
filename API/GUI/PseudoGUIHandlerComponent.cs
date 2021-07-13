// -----------------------------------------------------------------------
// <copyright file="PseudoGUIHandlerComponent.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace Mistaken.API.GUI
{
    /// <summary>
    /// PseudGUIHandler component.
    /// </summary>
    public partial class PseudoGUIHandlerComponent : MonoBehaviour
    {
        /// <summary>
        /// Gets instance of <see cref="PseudoGUIHandlerComponent"/>.
        /// </summary>
        public static PseudoGUIHandlerComponent Instance { get; private set; }

        /// <summary>
        /// Adds <see cref="PseudoGUIHandlerComponent"/> to <see cref="Server.Host"/>'s gameObject.
        /// </summary>
        public static void Ini()
        {
            Server.Host.GameObject.AddComponent<PseudoGUIHandlerComponent>();
        }

        /// <summary>
        /// Stops updating GUI.
        /// </summary>
        /// <param name="p">player to ignore.</param>
        public static void Ignore(Player p) => ToIgnore.Add(p);

        /// <summary>
        /// Starts updating GUI.
        /// </summary>
        /// <param name="p">player to stop ignoring.</param>
        public static void StopIgnore(Player p)
        {
            ToIgnore.Remove(p);
            ToUpdate.Add(p);
        }

        internal static void Set(Player player, string key, PseudoGUIPosition type, string content, float duration)
        {
            Set(player, key, type, content);
            Timing.CallDelayed(duration, () => Set(player, key, type, null));
        }

        internal static void Set(Player player, string key, PseudoGUIPosition type, string content)
        {
            bool remove = string.IsNullOrWhiteSpace(content);
            if (remove)
            {
                if (!CustomInfo.TryGetValue(player, out Dictionary<string, (string, PseudoGUIPosition)> value) || !value.ContainsKey(key))
                    return;
                value.Remove(key);
            }
            else
            {
                if (!CustomInfo.ContainsKey(player))
                    CustomInfo[player] = new Dictionary<string, (string Content, PseudoGUIPosition Type)>();
                else if (CustomInfo[player].TryGetValue(key, out (string Conetent, PseudoGUIPosition Type) value) && value.Conetent == content)
                    return;
                CustomInfo[player][key] = (content, type);
            }

            ToUpdate.Add(player);
        }

        private static readonly Dictionary<Player, Dictionary<string, (string Content, PseudoGUIPosition Type)>> CustomInfo = new Dictionary<Player, Dictionary<string, (string Content, PseudoGUIPosition Type)>>();
        private static readonly List<Player> ToUpdate = new List<Player>();
        private static readonly HashSet<Player> ToIgnore = new HashSet<Player>();
        private int frames = 0;

        private void Start()
        {
            Instance = this;
            Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound;
        }

        private void Destroy()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= this.Server_RestartingRound;
        }

        private void Server_RestartingRound()
        {
            CustomInfo.Clear();
            ToUpdate.Clear();
            ToIgnore.Clear();
        }

        private void FixedUpdate()
        {
            this.frames += 1;
            if (this.frames > 500)
            {
                foreach (var item in RealPlayers.List)
                {
                    try
                    {
                        if (item.IsConnected && !ToIgnore.Contains(item))
                            this.UpdateGUI(item);
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex.Message);
                        Log.Error(ex.StackTrace);
                    }
                }

                ToUpdate.Clear();
                this.frames = 0;
                return;
            }

            if (ToUpdate.Count == 0)
                return;
            foreach (var item in ToUpdate.ToArray())
            {
                try
                {
                    if (item.IsConnected && !ToIgnore.Contains(item))
                        this.UpdateGUI(item);
                    ToUpdate.Remove(item);
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }
            }
        }

        private void UpdateGUI(Player player)
        {
            if (!CustomInfo.ContainsKey(player))
                CustomInfo[player] = new Dictionary<string, (string Content, PseudoGUIPosition Type)>();

            string topContent = string.Empty;
            int topLines = 0;

            string middleContent = string.Empty;
            int middleLines = 0;

            string bottomContent = string.Empty;
            int bottomLines = 0;

            foreach (var item in CustomInfo[player].Values)
            {
                int lines = item.Content.Split(new string[] { "<br>" }, StringSplitOptions.None).Length;
                switch (item.Type)
                {
                    case PseudoGUIPosition.TOP: // 18
                        if (topContent.Length > 0)
                        {
                            topContent += "<br>";
                            topLines++;
                        }

                        topContent += item.Content;
                        topLines += lines;
                        break;
                    case PseudoGUIPosition.MIDDLE:
                        if (middleContent.Length > 0)
                        {
                            middleContent += "<br>";
                            middleLines++;
                        }

                        middleContent += item.Content;
                        middleLines += lines;
                        break;
                    case PseudoGUIPosition.BOTTOM: // 15
                        if (bottomContent.Length > 0)
                        {
                            bottomContent = "<br>" + bottomContent;
                            bottomLines++;
                        }

                        bottomContent = item.Content + bottomContent;
                        bottomLines += lines;
                        break;
                }
            }

            string toWrite = string.Empty;
            toWrite += topContent;
            int linesToAddTop = 18 - topLines - ((middleLines - (middleLines % 2)) / 2);
            for (int i = 0; i < linesToAddTop; i++)
                toWrite += "<br>";
            toWrite += middleContent;
            int linesToAddBottom = 15 - bottomLines - ((middleLines - (middleLines % 2)) / 2);
            for (int i = 0; i < linesToAddBottom; i++)
                toWrite += "<br>";
            toWrite += bottomContent;

            player.ShowHint($"<size=75%><color=#FFFFFFFF>{toWrite}</color><br><br><br><br><br><br><br><br><br><br></size>", 7200);

            // Log.Debug($"Updating {player.Id} with {toWrite}");
        }
    }
}
