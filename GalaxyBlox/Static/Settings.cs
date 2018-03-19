﻿using Android.Util;
using GalaxyBlox.Static;
using GalaxyBlox.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace GalaxyBlox.Static
{
    public static class Settings
    {
        /// <summary>
        /// Vital settings of the game.
        /// </summary>
        public static class Game
        {
            // DEBUG SETTINGS - TODO: Do it smarter
            public static readonly bool ShowFPS = false;

            private static IsolatedStorageFile dataFile = IsolatedStorageFile.GetUserStoreForDomain();
            private const string settingsPath = "settings.xml";
            private const string highscoresPath = "highscores.xml";
            public const int MaxHighscoresPerGameMod = 5;
            public const bool UseLastHighscoreName = false;

            public static readonly Vector2 WindowSize = new Vector2(720, 1208);

            public static Vector2 ArenaSize = new Vector2(12, 20);

            public static UserSettings UserSettings;
            public static Highscores Highscores;

            public static void LoadAll()
            {
                LoadUserSettings();
                LoadHighscores();
            }

            public static void SaveAll()
            {
                SaveUserSettings();
                SaveHighscores();
            }

            public static void LoadUserSettings()
            {
                var tmpUserSettings = new UserSettings();
                if (!Utils.Xml.TryDeserialize(out tmpUserSettings, settingsPath))
                    tmpUserSettings = new UserSettings() { Indicator = SettingOptions.Indicator.Shape, LastGameMode = SettingOptions.GameMode.Normal };

                UserSettings = tmpUserSettings;
            }

            public static void SaveUserSettings()
            {
                Utils.Xml.Serialize(UserSettings, settingsPath);
            }

            public static void LoadHighscores()
            {
                var tmpHighscores = new Highscores();
                if (!Utils.Xml.TryDeserialize(out tmpHighscores, highscoresPath))
                    tmpHighscores = new Highscores();

                Highscores = tmpHighscores;
            }

            public static void SaveHighscores()
            {
                Utils.Xml.Serialize(Highscores, highscoresPath);
            }
        }

        [XmlRoot]
        public class Highscores
        {
            [XmlElement]
            public SerializableDictionary<SettingOptions.GameMode, List<Score>> Items = new SerializableDictionary<SettingOptions.GameMode, List<Score>>();

            // METHODS
            public void SaveHighScore(SettingOptions.GameMode gameMode, List<Score> scores)
            {
                if (Items == null)
                    Items = new SerializableDictionary<SettingOptions.GameMode, List<Score>>();

                if (Items.ContainsKey(gameMode))
                    Items[gameMode] = scores;
                else
                    Items.Add(gameMode, scores);
            }
        }

        [Serializable]
        public struct Score
        {
            public string Name { get; set; }
            public long Value { get; set; }

            public Score(string name, long value)
            {
                Name = name;
                Value = value;
            }
        }


        [XmlRoot]
        public class UserSettings
        {
            [XmlElement]
            public string LastName;

            [XmlElement]
            public SettingOptions.GameMode LastGameMode = SettingOptions.GameMode.Normal;

            [XmlElement]
            public SettingOptions.Indicator Indicator = SettingOptions.Indicator.Shape;

            [XmlElement]
            public bool Vibration = true;
        }
    }
}