using GalaxyBlox.Models;
using GalaxyBlox.Static;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Objects
{
    /// <summary>
    /// Bank with game objects for easier object initialization. 
    /// </summary>
    public static class Bank
    {
        /// <summary>
        /// Game buttons
        /// </summary>
        public static class Buttons
        {
            /// <summary>
            /// Returns basic button with visible text, default font, colors and empty sprite image.
            /// </summary>
            /// <param name="parentRoom"></param>
            /// <returns></returns>
            public static Button GetBasicButton(Room parentRoom)
            {
                var button = new Button(parentRoom)
                {
                    SpriteImage = Contents.Sprites.Button_empty,
                    Alpha = 1f,
                    BaseColor = Contents.Colors.ButtonBackgroundColor,
                    DefaultBackgroundColor = Contents.Colors.ButtonBackgroundColor,
                    SelectedBackgroundColor = Contents.Colors.ButtonSelectedColor,
                    PressBackgroundColor = Contents.Colors.ButtonSelectedColor,
                    TextColor = Contents.Colors.MenuButtonTextColor,
                    TextSpriteFont = Contents.Fonts.PixelArtTextFont,
                    Text = "",
                    ShowText = true,
                    TextAlignment = TextAlignment.Center,
                    LayerDepth = 0.05f
                };

                return button;
            }

            /// <summary>
            /// Returns plain button without visible text and blank image sprite.
            /// </summary>
            /// <param name="parentRoom"></param>
            /// <returns></returns>
            public static Button GetPlainButton(Room parentRoom)
            {
                var button = GetBasicButton(parentRoom);
                button.SpriteImage = Contents.Sprites.Pix;
                button.ShowText = false;

                return button;
            }

            /// <summary>
            /// Returns plain button with visible text, default font, colors and blank image sprite.
            /// </summary>
            /// <param name="parentRoom"></param>
            /// <returns></returns>
            public static Button GetPlainButtonWithText(Room parentRoom)
            {
                var button = GetBasicButton(parentRoom);
                button.SpriteImage = Contents.Sprites.Pix;

                return button;
            }
        }

        /// <summary>
        /// Visual objects like labels.
        /// </summary>
        public static class Visuals
        {
            /// <summary>
            /// Returns label like object with basic font, colors and text set to left.
            /// </summary>
            /// <param name="parentRoom"></param>
            /// <returns></returns>
            public static GameObject GetLabelLeft(Room parentRoom)
            {
                var label = new GameObject(parentRoom)
                {
                    LayerDepth = 0.05f,
                    Alpha = 1f,
                    TextAlignment = TextAlignment.Left,
                    TextSpriteFont = Contents.Fonts.PixelArtTextFont,
                    Text = "",
                    ShowText = true,
                    TextColor = Color.White,
                };

                return label;
            }

            /// <summary>
            /// Returns label like object with basic font, colors and text set to center.
            /// </summary>
            /// <param name="parentRoom"></param>
            /// <returns></returns>
            public static GameObject GetLabelCenter(Room parentRoom)
            {
                var label = GetLabelLeft(parentRoom);
                label.TextAlignment = TextAlignment.Center;

                return label;
            }
        }
    }
}