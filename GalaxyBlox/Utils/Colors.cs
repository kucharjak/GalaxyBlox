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
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Utils
{
    public static class Colors
    {
        public static Color MixMultipleColors(List<Color> colors)
        {
            int r = 0, g = 0, b = 0;

            foreach(var col in colors)
            {
                r += col.R / colors.Count;
                g += col.G / colors.Count;
                b += col.B / colors.Count;
            }

            return new Color(r, g, b);
        }
    }
}