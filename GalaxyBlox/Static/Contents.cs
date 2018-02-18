using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GalaxyBlox.Utils;

namespace GalaxyBlox.Static
{
    public static class Contents
    {
        public static class Textures
        {
            public static Texture2D Pix;
            public static Texture2D ControlButton_fall;
            public static Texture2D ControlButton_rotate;
            public static Texture2D ControlButton_left;
            public static Texture2D ControlButton_right;
            public static Texture2D Button_pause;

            public static Texture2D Button_exit;
            public static Texture2D Button_play;
            public static Texture2D Button_settings;
            public static Texture2D Button_highscore;
            public static Texture2D Button_left;
            public static Texture2D Button_right;
            public static Texture2D Buttons_empty;

            public static Texture2D BorderedButtonBackground;

            public static Texture2D BackgroundGame;
            public static Texture2D BackgroundMenu;
            public static Texture2D Dialog_background;
            public static Texture2D Dialog_icon_settings;
            public static Texture2D Dialog_icon_questionMark;
        }

        public static class Fonts
        {
            public static SpriteFont PlainTextFont;
            public static SpriteFont PixelArtTextFont;
        }

        public static class Colors
        {
            public static List<Color> GameCubesColors = new List<Color>
            {
                new Color(255, 255, 255) * 0.3f, // first possition reserved for neutral/ empty like color
                new Color(76, 255, 0, 255), // Green
                //Color.Red,
                //Color.Yellow,
                //new Color(42, 0, 255), // Blue
                //new Color(255, 114, 0) // Orange
            };

            public static float NonActiveColorFactor = 0.4f;

            public static Color IndicatorColor = Color.Red;// new Color(255, 255, 255) * 0.7f;

            public static Color PlaygroundColor = Color.TransparentBlack;
            public static Color PlaygroundBorderColor = new Color(255, 255, 255) * 0.3f;

            public static Color ActorViewerBackgroundColor = Color.Transparent;

            public static Color MenuButtonBackgroundColor = new Color(121, 140, 170);
            public static Color MenuButtonSelectedColor = new Color(153, 189, 247);
            public static Color MenuButtonPressColor = new Color(121, 140, 170);//new Color(89, 107, 135);
            public static Color MenuButtonTextColor = new Color(91, 6, 0);

            public static Color ButtonBackgroundColor = new Color(216, 216, 216);
            public static Color ButtonSelectedColor = new Color(255, 255, 255);

            public static Color ControlButtonBackgroundColor = new Color(198, 215, 255);
            public static Color ControlButtonSelectColor = new Color(239, 244, 255);

            public static Color BonusButtonBackgroundColor = new Color(107, 151, 255);
            public static Color BonusButtonSelectedColor = new Color(107, 151, 255);

            public static Color ControlPanelBackgroundColor = new Color(107, 151, 255);
            public static Color ScorePanelBackgroundColor = new Color(107, 151, 255);

            public static Color BonusPanelBackgroundColor = new Color(142, 176, 255);

            public static Color PanelHeaderBackgroundColor = new Color(121, 140, 170);
            public static Color PanelContentBackgroundColor = new Color(100, 118, 147);

            public static Color RoomsSeparateColor = new Color(0, 0, 0, 150);
        }

        public static class Shapes
        {
            public static readonly List<bool[,]> ShapeBank = new List<bool[,]>() // basic shapes
            {
                new bool[,]
                {
                    {true, false },
                    {true, true },
                    {true, false }
                },
                new bool[,]
                {
                    {true, true, true, true }
                },
                new bool[,]
                {
                    {true, false },
                    {true, false },
                    {true, true }
                },
                new bool[,]
                {
                    {true, true },
                    {true, true }
                },
                new bool[,]
                {
                    {false, true },
                    {true, true },
                    {true, false }
                },
            };

            private static List<int> ReturnedRandomShapes = new List<int>();

            public static bool[,] GetRandomShape()
            {
                ShapeBank.Shuffle(); // Shuffle all shapes for more random
                var maxCount = ShapeBank.Count();
                var nextShape = Game1.Random.Next(0, maxCount);
                while (ReturnedRandomShapes.Where(shp => shp == nextShape).Count() >= 2) // if are last 2 shapes same generate new shape
                {
                    nextShape = Game1.Random.Next(0, maxCount);
                }

                ReturnedRandomShapes.Add(nextShape);
                if (ReturnedRandomShapes.Count > 2) // max 2 last shapes to remember
                    ReturnedRandomShapes.RemoveAt(0);

                return ShapeBank[nextShape];
                //return ShapeBank[(Game1.Random.Next(0, maxCount * 100) % maxCount)];
            }
        }
    }
}