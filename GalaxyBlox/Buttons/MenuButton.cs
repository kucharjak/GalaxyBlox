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
            TextColor = Contents.Colors.MenuButtonTextColor;

            ShowText = true;
            TextSpriteFont = Contents.Fonts.MenuButtonText;
        }

        public override void Touch()
        {
            base.Touch();
            Scale = 1.05f;
        }

        public override void Press()
        {
            base.Press();
            Scale = 1.05f;
        }

        public override void Release()
        {
            base.Release();
            Scale = 1f;
        }

        public override void PressAction()
        {
            switch (ButttonID)
            {
                case 1: break;
                case 2: break;
                case 3: break;
                case 4: System.Environment.Exit(0); break;
            }
        }
    }
}