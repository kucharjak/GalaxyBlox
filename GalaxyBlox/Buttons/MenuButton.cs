using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using Microsoft.Xna.Framework;
using GalaxyBlox.Static;

namespace GalaxyBlox.Buttons
{
    class MenuButton : Button
    {
        public MenuButton(Room parentRoom) : base (parentRoom)
        {
            BackgroundImage = Contents.Textures.Pix;
            Alpha = 0.5f;
            BackgroundColor = Contents.Colors.MenuButtonBackgroundColor;
            DefaultBackgroundColor = Contents.Colors.MenuButtonBackgroundColor;
            SelectedBackgroundColor = Contents.Colors.MenuButtonSelectedBackgroundColor;
            PressBackgroundColor = Contents.Colors.MenuButtonPressBackgroundColor;
            TextColor = Contents.Colors.MenuButtonTextColor;

            ShowText = true;
            TextSpriteFont = Contents.Fonts.MenuButtonText;
        }

        public override void Touch()
        {
            Scale = 1.05f;

            base.Touch();
        }

        public override void Press()
        {
            Scale = 1f;

            base.Press();
        }

        public override void Release()
        {
            Scale = 1f;

            base.Release();
        }

        public override void PressAction()
        {
            switch (ButttonID)
            {
                case 1: break;
                case 2: break;
                case 3: break;
                case 4: Game.Activity.Finish(); break;
            }
        }
    }
}