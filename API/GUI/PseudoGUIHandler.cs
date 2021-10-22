// -----------------------------------------------------------------------
// <copyright file="PseudoGUIHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace Mistaken.API.GUI
{
    /// <summary>
    /// PseudGUIHandler component.
    /// </summary>
    public partial class PseudoGUIHandler : MonoBehaviour
    {
        /// <summary>
        /// Gets instance of <see cref="PseudoGUIHandler"/>.
        /// </summary>
        public static PseudoGUIHandler Instance { get; private set; }

        /// <summary>
        /// Adds <see cref="PseudoGUIHandler"/> to <see cref="Server.Host"/>'s gameObject.
        /// </summary>
        public static void Ini()
        {
            Server.Host.GameObject.AddComponent<PseudoGUIHandler>();
        }

        /// <summary>
        /// Stops updating GUI.
        /// </summary>
        /// <param name="p">player to ignore.</param>
        public static void Ignore(Player p)
        {
            ToIgnore.Add(p);
            ToUpdate.Remove(p);
        }

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
            if (string.IsNullOrWhiteSpace(content))
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
        private readonly Dictionary<Player, string> constructedStrings = new Dictionary<Player, string>();
        private int frames = 0;
        private Task guiCalculationThread;
        private bool active = true;

        private void Start()
        {
            Instance = this;
            Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound;

            this.active = true;
            this.guiCalculationThread = Task.Run(async () =>
            {
                while (this.active)
                {
                    await Task.Delay(100);

                    this.frames += 1;

                    // 10s
                    if (this.frames > 99)
                    {
                        foreach (var item in RealPlayers.List)
                        {
                            try
                            {
                                if (!ToIgnore.Contains(item))
                                    this.ConstructString(item);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex.Message);
                                Log.Error(ex.StackTrace);
                            }
                        }

                        ToUpdate.Clear();
                        this.frames = 0;
                        continue;
                    }

                    if (ToUpdate.Count == 0)
                        continue;
                    foreach (var item in ToUpdate.ToArray())
                    {
                        try
                        {
                            if ((item?.IsConnected ?? false) && !ToIgnore.Contains(item))
                                this.ConstructString(item);
                            ToUpdate.Remove(item);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
            });
        }

        private void Destroy()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= this.Server_RestartingRound;
            this.active = false;
        }

        private void Server_RestartingRound()
        {
            CustomInfo.Clear();
            ToUpdate.Clear();
            ToIgnore.Clear();
            this.constructedStrings.Clear();
        }

        private void FixedUpdate()
        {
            if (this.constructedStrings.Count == 0)
                return;
            foreach (var item in this.constructedStrings.Keys.ToArray())
            {
                try
                {
                    if (!(item?.IsConnected ?? false))
                        this.constructedStrings.Remove(item);
                    else if (!ToIgnore.Contains(item))
                        this.UpdateGUI(item);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }
            }
        }

        private void ConstructString(Player player)
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

            this.constructedStrings[player] = $"<size=75%><color=#FFFFFFFF>{toWrite}</color><br><br><br><br><br><br><br><br><br><br></size>";
        }

        private void UpdateGUI(Player player)
        {
            if (!this.constructedStrings.TryGetValue(player, out string text))
                return;
            player.ShowHint(text, 7200);
            this.constructedStrings.Remove(player);
        }
    }
}
