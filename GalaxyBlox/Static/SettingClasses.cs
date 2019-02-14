using System;
using System.Collections.Generic;
using Android.Views;
using System.Xml.Serialization;
using GalaxyBlox.Utils;

namespace GalaxyBlox.Static
{
    public class SettingClasses
    {
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

            [XmlElement]
            public bool UseSingleColor = true;

            /// <summary>
            /// Settings for using more than classic shapes.
            /// </summary>
            [XmlElement]
            public bool UseExtendedShapeLibrary = true;
        }
    }
}