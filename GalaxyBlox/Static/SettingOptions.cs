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
    }
}