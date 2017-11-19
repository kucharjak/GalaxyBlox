using Android.Util;
using GalaxyBlox.Static;
using GalaxyBlox.Utils;
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
            private static IsolatedStorageFile dataFile = IsolatedStorageFile.GetUserStoreForDomain();
            private const string dataFilePath = "userSettings.xml";

            public static readonly Size WindowSize = new Size(480, 800); // new Size(720, 1200);
            public static readonly Size ArenaSize = new Size(12, 20);
            public static readonly int MaxHighscoresPerGameMod = 1;

            public static UserSettings User;

            public static void LoadUser()
            {
                try
                {
                    using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(dataFilePath, FileMode.Open, iso))
                        {
                            // Read the data from the file
                            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                            Game.User = (UserSettings)serializer.Deserialize(stream);
                        }
                    }
                }
                catch
                { // default settings
                    Game.User = new UserSettings()
                    {
                        Indicator = SettingOptions.Indicator.Shape,
                        HighScores = new SerializableDictionary<string, List<long>>()  
                    };
                }
            }

            public static void SaveUser()
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(dataFilePath, FileMode.Create, iso))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                        serializer.Serialize(stream, User);
                    }
                }
            }
        }

        [XmlRoot]
        public class UserSettings
        {
            [XmlElement]
            public SettingOptions.Indicator Indicator = SettingOptions.Indicator.None;

            [XmlElement]
            public SerializableDictionary<string, List<long>> HighScores = new SerializableDictionary<string, List<long>>();

            // METHODS
            public void SaveHighScore(string gameMode, List<long> scores)
            {
                if (HighScores == null)
                    HighScores = new SerializableDictionary<string, List<long>>();

                if (HighScores.ContainsKey(gameMode))
                    HighScores[gameMode] = scores;
                else
                    HighScores.Add(gameMode, scores);
            }
        }
    }
}