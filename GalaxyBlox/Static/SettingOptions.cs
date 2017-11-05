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

namespace GalaxyBlox.Static
{
    public class SettingOptions
    {
        public enum Indicator
        {
            None,
            Shadow,
            Shape
        }

        public enum GameSpeed
        {
            Normal,
            Speedup,
            Falling
        }
    }
}