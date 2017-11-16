using GalaxyBlox.Models;
using GalaxyBlox.Static;

namespace GalaxyBlox.Objects
{
    class MenuButton : Button
    {
        public MenuButton(Room parentRoom) : base (parentRoom)
        {
            BackgroundImage = Contents.Textures.Pix;
            Alpha = 0.5f;
            BaseColor = Contents.Colors.MenuButtonBackgroundColor;
            DefaultBackgroundColor = Contents.Colors.MenuButtonBackgroundColor;
            SelectedBackgroundColor = Contents.Colors.MenuButtonSelectedColor;
            PressBackgroundColor = Contents.Colors.MenuButtonPressColor;
            TextColor = Contents.Colors.MenuButtonTextColor;

            ShowText = true;
            TextSpriteFont = Contents.Fonts.MenuButtonText;
        }
    }
}