using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GalaxyBlox.Models;

namespace GalaxyBlox.Static
{
    public static class Contents
    {
        public static class Textures
        {
            public static Texture2D Pix;
            public static Texture2D Buttons;
            public static Texture2D Stars;
            public static Texture2D Dialog_Icons;
            public static Texture2D Logo;
            public static Texture2D FrozenStars;

            public static Texture2D Dialog_Backgrounds;
            public static Texture2D GameUI_Backgrounds;
            public static Texture2D Game_Background;

            public static Texture2D Animations;
        }

        public static class Sprites
        {
            public static Sprite Pix;
            public static Sprite ControlButton_fall;
            public static Sprite ControlButton_rotate;
            public static Sprite ControlButton_left;
            public static Sprite ControlButton_right;
            public static Sprite Button_pause;

            public static Sprite Button_exit;
            public static Sprite Button_play;
            public static Sprite Button_settings;
            public static Sprite Button_highscore;
            public static Sprite Button_left;
            public static Sprite Button_right;
            public static Sprite Button_left_small;
            public static Sprite Button_right_small;
            public static Sprite Button_up_medium;
            public static Sprite Button_down_medium;
            public static Sprite Button_empty;
            public static Sprite Button_bonus;

            public static Sprite GameUI_top_background;
            public static Sprite GameUI_bottom_classic_background;
            public static Sprite GameUI_bottom_normal_background;
            public static Sprite GameUI_playingArena_border;

            public static Sprite Game_Background;
            public static Sprite Dialog_background;
            public static Sprite Dialog_inside;
            public static Sprite Dialog_icon_settings;
            public static Sprite Dialog_icon_questionMark;
            public static Sprite Dialog_icon_highscore;

            public static Sprite Star_small;
            public static Sprite Star_medium_01;
            public static Sprite Star_medium_02;
            public static Sprite Star_big;

            public static Sprite Logo;

            public static Sprite FrozenStars;
        }

        public static class Animations
        {
            public static SpriteAnimation Explosion;
            public static SpriteAnimation Laser;
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
                new Color(76, 255, 0),  // Green
                new Color(255, 208, 0), // Orange
                new Color(233, 255, 0), // Lime green
                new Color(255, 0, 246), // Toxic pink
                new Color(0, 255, 233), // Aqua
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

            public static Color ButtonBackgroundColor = new Color(230, 230, 230);
            public static Color ButtonSelectedColor = new Color(255, 255, 255);

            public static Color PanelHeaderBackgroundColor = new Color(121, 140, 170);
            public static Color PanelContentBackgroundColor = new Color(100, 118, 147);

            public static Color RoomsSeparateColor = new Color(0, 0, 0, 150);

            public static Color BlinkColor = new Color(255, 255, 255);
        }

        public static class Shapes
        {
            /// <summary>
            /// Clasic / basic shapes 
            /// </summary>
            public static readonly List<bool[,]> ShapeBank = new List<bool[,]>()
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

            /// <summary>
            /// More shapes inspired by original shapes
            /// </summary>
            public static readonly List<bool[,]> ExtendedShapeBank = new List<bool[,]>()
            {
                new bool[,]
                {
                    { true, true, false },
                    { false, true, false },
                    { false, true, true }
                },
                //new bool[,]
                //{
                //    { false, true, false },
                //    { true, true, true },
                //    { false, true, false }
                //},
                new bool[,]
                {
                    { true, true },
                    { true, true },
                    { true, false }
                },
                new bool[,]
                {
                    { true, true, true },
                    { true, false, false },
                    { true, false, false }
                },
                new bool[,]
                {
                    {true, true, true }
                },
            };

            //private static List<int> returnedRandomShapes;
            private static Random shapeRandom;

            public static bool[,] GetRandomShape()
            {
                if (shapeRandom == null)
                    shapeRandom = new Random((int)DateTime.Now.Ticks);

                var nextShape = shapeRandom.Next(0, ShapeBank.Count);
                return ShapeBank[nextShape];

                //ShapeBank.Shuffle(); // Shuffle all shapes for more random
                //var maxCount = ShapeBank.Count();
                //var nextShape = Game1.Random.Next(0, maxCount);
                //while (returnedRandomShapes.Where(shp => shp == nextShape).Count() >= 2) // if are last 2 shapes same generate new shape
                //{
                //    nextShape = Game1.Random.Next(0, maxCount);
                //}

                //returnedRandomShapes.Add(nextShape);
                //if (returnedRandomShapes.Count > 2) // max 2 last shapes to remember
                //    returnedRandomShapes.RemoveAt(0);

                //return ShapeBank[nextShape];
                //return ShapeBank[(Game1.Random.Next(0, maxCount * 100) % maxCount)];
            }

            public static bool[,] GetExtendedRandomShape()
            {
                if (shapeRandom == null)
                    shapeRandom = new Random((int)DateTime.Now.Ticks);

                var shapes = new List<bool[,]>();
                shapes.AddRange(ShapeBank);
                shapes.AddRange(ExtendedShapeBank);

                var nextShape = shapeRandom.Next(0, shapes.Count);
                return shapes[nextShape];
            }
        }
    }
}