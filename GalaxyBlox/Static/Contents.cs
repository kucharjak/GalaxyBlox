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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Static
{
    public static class Contents
    {
        public static class Textures
        {
            public static Texture2D Pix;
        }

        public static class Fonts
        {
            public static SpriteFont MenuButtonText;
        }

        public static class Colors
        {
            public static Color MenuButtonBackgroundColor = new Color(121, 140, 170);
            public static Color MenuButtonSelectedBackgroundColor = new Color(153, 189, 247);
            public static Color MenuButtonPressBackgroundColor = new Color(89, 107, 135);
            public static Color MenuButtonTextColor = Color.White;
        }
    }
}