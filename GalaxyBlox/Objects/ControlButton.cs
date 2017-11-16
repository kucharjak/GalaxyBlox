using GalaxyBlox.Models;
using GalaxyBlox.Static;

namespace GalaxyBlox.Objects
{
    class ControlButton : Button
    {
        public ControlButton(Room parentRoom) : base (parentRoom)
        {
            Alpha = 1f;
            BaseColor = Contents.Colors.ControlButtonBackgroundColor;
            DefaultBackgroundColor = Contents.Colors.ControlButtonBackgroundColor;
            SelectedBackgroundColor = Contents.Colors.ControlButtonSelectColor;
            PressBackgroundColor = Contents.Colors.ControlButtonBackgroundColor;

            ShowText = false;
            TextSpriteFont = Contents.Fonts.MenuButtonText;
            TextColor = Contents.Colors.MenuButtonTextColor;
        }

        protected override void Hovered()
        {
            Scale = 1.2f;
            base.Hovered();
        }
    }
}