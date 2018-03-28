using GalaxyBlox.Models;
using GalaxyBlox.Static;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Objects
{
    public static class Bank
    {
        public static class Buttons
        {
            public static Button GetPauseButton(Room parentRoom)
            {
                var button = new Button(parentRoom)
                {
                    SpriteImage = Contents.Sprites.Button_pause,
                    Alpha = 1f,
                    BaseColor = Contents.Colors.ButtonBackgroundColor,
                    DefaultBackgroundColor = Contents.Colors.ButtonBackgroundColor,
                    SelectedBackgroundColor = Contents.Colors.ButtonSelectedColor,
                    PressBackgroundColor = Contents.Colors.ButtonBackgroundColor,
                    ShowText = false,
                    TextSpriteFont = Contents.Fonts.PlainTextFont,
                    TextColor = Contents.Colors.MenuButtonTextColor,
                    LayerDepth = 0.05f
                };

                return button;
            }

            public static Button GetControlButton(Room parentRoom)
            {
                var button = new Button(parentRoom)
                {
                    Alpha = 1f,
                    BaseColor = Contents.Colors.ButtonBackgroundColor,
                    DefaultBackgroundColor = Contents.Colors.ButtonBackgroundColor,
                    SelectedBackgroundColor = Contents.Colors.ButtonSelectedColor,
                    PressBackgroundColor = Contents.Colors.ButtonSelectedColor,
                    ShowText = false,
                    TextSpriteFont = Contents.Fonts.PlainTextFont,
                    TextColor = Contents.Colors.MenuButtonTextColor,
                    LayerDepth = 0.05f
                };

                return button;
            }

            public static Button GetMenuButton(Room parentRoom)
            {
                var button = new Button(parentRoom)
                {
                    SpriteImage = Contents.Sprites.Pix,
                    Alpha = 1f,
                    BaseColor = Contents.Colors.ButtonBackgroundColor,
                    DefaultBackgroundColor = Contents.Colors.ButtonBackgroundColor,
                    SelectedBackgroundColor = Contents.Colors.ButtonSelectedColor,
                    PressBackgroundColor = Contents.Colors.ButtonSelectedColor,
                    TextSpriteFont = Contents.Fonts.PlainTextFont,
                    Text = "",
                    ShowText = true,
                    TextAlignment = TextAlignment.Center,
                    LayerDepth = 0.05f
                };

                return button;
            }

            public static Button GetEmptyButton(Room parentRoom)
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

            public static Button GetGameOverButton(Room parentRoom)
            {
                var button = new Button(parentRoom)
                {
                    SpriteImage = Contents.Sprites.Pix,
                    Alpha = 0.5f,
                    BaseColor = Contents.Colors.MenuButtonBackgroundColor,
                    DefaultBackgroundColor = Contents.Colors.MenuButtonBackgroundColor,
                    SelectedBackgroundColor = Contents.Colors.MenuButtonSelectedColor,
                    PressBackgroundColor = Contents.Colors.MenuButtonPressColor,
                    TextColor = Contents.Colors.MenuButtonTextColor,
                    TextSpriteFont = Contents.Fonts.PlainTextFont,
                    Text = "",
                    ShowText = true,
                    TextAlignment = TextAlignment.Center,
                    LayerDepth = 0.05f
                };

                return button;
            }
        }

        public static class Visuals
        {
            public static GameObject GetPanelLabel(Room parentRoom)
            {
                var label = new GameObject(parentRoom)
                {
                    LayerDepth = 0.05f,
                    SpriteImage = Contents.Sprites.Pix,
                    BaseColor = Contents.Colors.PanelHeaderBackgroundColor,
                    Alpha = 1f,
                    TextAlignment = TextAlignment.Center,
                    TextSpriteFont = Contents.Fonts.PlainTextFont,
                    Text = "",
                    ShowText = true,
                    TextColor = Color.White,
                };

                return label;
            }

            public static GameObject GetPanelBoard(Room parentRoom)
            {
                var label = new GameObject(parentRoom)
                {
                    LayerDepth = 0.05f,
                    TextAlignment = TextAlignment.Center,
                    SpriteImage = Contents.Sprites.Pix,
                    BaseColor = Contents.Colors.PanelContentBackgroundColor,
                    Alpha = 1f,
                    TextSpriteFont = Contents.Fonts.PlainTextFont,
                    Text = "",
                    ShowText = true,
                    TextColor = Color.White,
                };

                return label;
            }

            public static GameObject GetSettingsLabel(Room parentRoom)
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

            public static GameObject GetGameOverLabel(Room parentRoom)
            {
                var label = new GameObject(parentRoom)
                {
                    LayerDepth = 0.05f,
                    Alpha = 1f,
                    TextAlignment = TextAlignment.Center,
                    TextSpriteFont = Contents.Fonts.PixelArtTextFont,
                    Text = "",
                    ShowText = true,
                    TextColor = Color.White,
                };

                return label;
            }
        }
    }
}