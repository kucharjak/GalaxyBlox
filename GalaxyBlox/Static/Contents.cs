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
            public static Texture2D ControlButton_down;
            public static Texture2D ControlButton_rotate;
            public static Texture2D ControlButton_left;
            public static Texture2D ControlButton_right;
            public static Texture2D ControlButton_pause;
        }

        public static class Fonts
        {
            public static SpriteFont MenuButtonText;
        }

        public static class Colors
        {
            public static List<Color> GameCubesColors = new List<Color>
            {
                new Color(158, 194, 255) * 0.6f,
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.Orange,
                Color.Yellow,
                Color.Purple
            };

            public static Color PlaygroundColor = new Color(73, 127, 216) * 0.75f;
            public static Color PlaygroundBorderColor = new Color(248, 255, 175);

            public static Color MenuButtonBackgroundColor = new Color(121, 140, 170);
            public static Color MenuButtonSelectedColor = new Color(153, 189, 247);
            public static Color MenuButtonPressColor = new Color(121, 140, 170);//new Color(89, 107, 135);
            public static Color MenuButtonTextColor = Color.White;

            public static Color PauseButtonBackgroundColor = new Color(108, 239, 21);
            public static Color PauseButtonSelectedColor = new Color(200, 255, 20); //new Color(108, 239, 21);

            public static Color ControlButtonBackgroundColor = new Color(186, 197, 216);
            public static Color ControlButtonSelectColor = Color.White;

            public static Color BackgroundControlsColor = new Color(100, 114, 137);
        }
    }
}