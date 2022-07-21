// -----------------------------------------------------------------------
// <copyright file="PseudoGUIHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using Mistaken.API.Extensions;
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
            if (Instance is null)
                Server.Host.GameObject.AddComponent<PseudoGUIHandler>();
        }

        /// <summary>
        /// Stops updating GUI.
        /// </summary>
        /// <param name="p">player to ignore.</param>
        public static void Ignore(Player p)
        {
            ToIgnore.TryAdd(p, null);
            ToUpdate.TryRemove(p, out _);
        }

        /// <summary>
        /// Starts updating GUI.
        /// </summary>
        /// <param name="p">player to stop ignoring.</param>
        public static void StopIgnore(Player p)
        {
            ToIgnore.TryRemove(p, out _);
            ToUpdate.TryAdd(p, null);
        }

        internal static void Set(Player player, string key, PseudoGUIPosition type, string content, float duration)
        {
            Set(player, key, type, content);
            Timing.CallDelayed(duration, () => Set(player, key, type, null));
        }

        internal static void Set(Player player, string key, PseudoGUIPosition type, string content)
        {
            if (player == null)
            {
                Log.Warn("Tried to set GUI for null player");
                Log.Warn(Environment.StackTrace);
                return;
            }

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

            ToUpdate.TryAdd(player, null);
        }

        private static readonly string DirectoryPath = Path.Combine(Paths.Plugins, "PseudoGUI", Server.Port.ToString());
        private static readonly Dictionary<Player, Dictionary<string, (string Content, PseudoGUIPosition Type)>> CustomInfo = new Dictionary<Player, Dictionary<string, (string Content, PseudoGUIPosition Type)>>();
        private static readonly ConcurrentDictionary<Player, object> ToUpdate = new ConcurrentDictionary<Player, object>(); // ConcurrentHashSet
        private static readonly ConcurrentDictionary<Player, object> ToIgnore = new ConcurrentDictionary<Player, object>(); // ConcurrentHashSet
        private readonly ConcurrentDictionary<Player, string> constructedStrings = new ConcurrentDictionary<Player, string>();
        private readonly object lck = new object();
        private StreamWriter fileStream;
        private int frames = 0;
        private Task guiCalculationThread;
        private bool active = true;

        private void Start()
        {
            Instance = this;
            Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound;
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            this.fileStream = File.CreateText(Path.Combine(DirectoryPath, DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".log"));
            this.GUILog("START", "Start of log");

            this.active = true;
            this.guiCalculationThread = Task.Run(async () =>
            {
                while (this.active)
                {
                    try
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
                                    if (!ToIgnore.ContainsKey(item))
                                    {
                                        this.GUILog("CALC_THREAD", $"Constructing string for player {item.Nickname} (10s)");
                                        this.ConstructString(item);
                                    }
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

                        foreach (var item in ToUpdate.Keys.ToArray())
                        {
                            if ((item?.IsConnected ?? false) && !ToIgnore.ContainsKey(item))
                            {
                                this.GUILog("CALC_THREAD", $"Constructing string for player {item.Nickname} (0.1s)");
                                this.ConstructString(item);
                            }
                        }

                        ToUpdate.Clear();

                        /*while (ToUpdate.Count != 0)
                        {
                            try
                            {
                                var item = ToUpdate[0];
                                if ((item?.IsConnected ?? false) && !ToIgnore.Contains(item))
                                    this.ConstructString(item);
                                try
                                {
                                    ToUpdate.RemoveAt(0);
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    Log.Warn($"[PSUEDOGUI] {nameof(ArgumentOutOfRangeException)} thrown, breaking");
                                    try
                                    {
                                        ToUpdate.Clear();
                                    }
                                    catch
                                    {
                                    }

                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }*/
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
            });
        }

        private void Destroy()
        {
            Instance = null;
            Exiled.Events.Handlers.Server.RestartingRound -= this.Server_RestartingRound;
            this.active = false;
            this.fileStream.Dispose();
        }

        private void Server_RestartingRound()
        {
            CustomInfo.Clear();
            ToUpdate.Clear();
            ToIgnore.Clear();
            this.constructedStrings.Clear();
            this.GUILog("ROUND_RESTART", "End of log");
            lock (this.lck)
            {
                this.fileStream.Dispose();
                this.fileStream = File.CreateText(Path.Combine(DirectoryPath, DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss") + ".log"));
            }

            this.GUILog("ROUND_RESTART", "Start of log");
        }

        private void FixedUpdate()
        {
            if (this.constructedStrings.Count == 0)
                return;

            foreach (var item in this.constructedStrings.Keys.ToArray())
            {
                try
                {
                    if (item == null)
                        continue;

                    if (!(item?.IsConnected ?? false))
                    {
                        this.GUILog("FIXED_UPDATE", $"Removing player {item.Nickname} from constructed strings list (player disconnected)");
                        this.constructedStrings.TryRemove(item, out _);
                    }
                    else if (!ToIgnore.ContainsKey(item))
                    {
                        this.GUILog("FIXED_UPDATE", $"Updating GUI for player {item.Nickname}");
                        this.UpdateGUI(item);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }
            }
        }

        private void GUILog(string module, string message)
        {
            string log = $"{DateTime.UtcNow.ToString("HH:mm:ss.fff")} | {module} | {message}";
            lock (this.lck)
            {
                try
                {
                    this.fileStream.WriteLine(log);
                }
                catch
                {
                    Log.Error(log);
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
            this.GUILog("CONSTRUCT_STRING", $"Constructed string for player {player.Nickname}");
        }

        private void UpdateGUI(Player player)
        {
            if (!this.constructedStrings.TryGetValue(player, out string text))
            {
                this.GUILog("UPDATE_GUI", $"List of constructed strings was empty for player {player.Nickname}");
                return;
            }

            try
            {
                if (player.IsConnected())
                {
                    this.GUILog("UPDATE_GUI", $"Showing hint for player {player.Nickname}");
                    player.ShowHint(text, 7200);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            this.GUILog("UPDATE_GUI", $"Removing player {player.Nickname} from constructed strings list (hint already shown)");
            this.constructedStrings.TryRemove(player, out _);
        }
    }
}
