using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using Microsoft.Xna.Framework;
using GalaxyBlox.Static;

namespace GalaxyBlox.Buttons
{
    class PauseButton : Button
    {
        public PauseButton(Room parentRoom) : base (parentRoom)
        {
            Alpha = 1f;
            Color = Contents.Colors.PauseButtonBackgroundColor;
            DefaultBackgroundColor = Contents.Colors.PauseButtonBackgroundColor;
            SelectedBackgroundColor = Contents.Colors.PauseButtonSelectedColor;
            PressBackgroundColor = Contents.Colors.PauseButtonBackgroundColor;
            
            ShowText = false;
            TextSpriteFont = Contents.Fonts.MenuButtonText;
            TextColor = Contents.Colors.MenuButtonTextColor;
        }

        protected override void Hovered()
        {
            Scale = 1.4f;
            base.Hovered();
        }
    }
}