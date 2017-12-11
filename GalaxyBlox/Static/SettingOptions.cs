using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Xml.Serialization;

namespace GalaxyBlox.Static
{
    public class SettingOptions
    {
        [Serializable]
        public enum Indicator
        {
            [XmlEnum("none")]
            None,
            [XmlEnum("shadow")]
            Shadow,
            [XmlEnum("shape")]
            Shape
        }

        [Serializable]
        public enum GameMode
        {
            [XmlEnum("vanilla")]
            Classic,
            [XmlEnum("normal")]
            Normal,
            [XmlEnum("extreme")]
            Extreme,
            [XmlEnum("test")]
            Test
        }

        public enum GameSpeed
        {
            Normal,
            Speedup,
            Falling
        }

        public enum GameBonus
        {
            None, // first place must be empty
            //TimeRewind,
            TimeSlowdown,
            SwipeCubes,
            Laser
        }
    }
}