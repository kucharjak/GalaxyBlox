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
                    BackgroundImage = Contents.Textures.ControlButton_pause,
                    Alpha = 1f,
                    BaseColor = Contents.Colors.PauseButtonBackgroundColor,
                    DefaultBackgroundColor = Contents.Colors.PauseButtonBackgroundColor,
                    SelectedBackgroundColor = Contents.Colors.PauseButtonSelectedColor,
                    PressBackgroundColor = Contents.Colors.PauseButtonBackgroundColor,
                    ShowText = false,
                    TextSpriteFont = Contents.Fonts.MenuButtonText,
                    TextColor = Contents.Colors.MenuButtonTextColor,
                    LayerDepth = 0.5f
                };

                return button;
            }

            public static Button GetControlButton(Room parentRoom)
            {
                var button = new Button(parentRoom)
                {
                    Alpha = 1f,
                    BaseColor = Contents.Colors.ControlButtonBackgroundColor,
                    DefaultBackgroundColor = Contents.Colors.ControlButtonBackgroundColor,
                    SelectedBackgroundColor = Contents.Colors.ControlButtonSelectColor,
                    PressBackgroundColor = Contents.Colors.ControlButtonBackgroundColor,
                    ShowText = false,
                    TextSpriteFont = Contents.Fonts.MenuButtonText,
                    TextColor = Contents.Colors.MenuButtonTextColor,
                    LayerDepth = 0.5f
                };

                return button;
            }

            public static Button GetMenuButton(Room parentRoom)
            {
                var button = new Button(parentRoom)
                {
                    BackgroundImage = Contents.Textures.Pix,
                    Alpha = 0.5f,
                    BaseColor = Contents.Colors.MenuButtonBackgroundColor,
                    DefaultBackgroundColor = Contents.Colors.MenuButtonBackgroundColor,
                    SelectedBackgroundColor = Contents.Colors.MenuButtonSelectedColor,
                    PressBackgroundColor = Contents.Colors.MenuButtonPressColor,
                    TextColor = Contents.Colors.MenuButtonTextColor,
                    TextSpriteFont = Contents.Fonts.MenuButtonText,
                    Text = "",
                    ShowText = true,
                    TextAlignment = TextAlignment.Center,
                    LayerDepth = 0.5f
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
                    LayerDepth = 0.5f,
                    BackgroundImage = Contents.Textures.Pix,
                    BaseColor = Contents.Colors.PanelHeaderBackgroundColor,
                    Alpha = 1f,
                    TextAlignment = TextAlignment.Center,
                    TextSpriteFont = Contents.Fonts.PanelHeaderText,
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
                    LayerDepth = 0.5f,
                    TextAlignment = TextAlignment.Center,
                    BackgroundImage = Contents.Textures.Pix,
                    BaseColor = Contents.Colors.PanelContentBackgroundColor,
                    Alpha = 1f,
                    TextSpriteFont = Contents.Fonts.PanelContentText,
                    Text = "",
                    ShowText = true,
                    TextColor = Color.White,
                };

                return label;
            }
        }

    }
}