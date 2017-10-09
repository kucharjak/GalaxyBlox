using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Buttons
{
    class MenuButton : Button
    {
        public MenuButton(float scale) : base (scale)
        {
            Alpha = 0.5f;
            BackgroundColor = new Color(121, 140, 170);
            ShowText = true;
            TextColor = Color.White;
            TextSpriteFont = Game1.Contents.ButtonText;
        }

        public override void Touch()
        {
            base.Touch();

            BackgroundColor = Color.Red;
        }

        public override void Press()
        {
            base.Press();
            
            Text = "HELL YEA";
            BackgroundColor = new Color(121, 140, 170);
        }

        public override void Release()
        {
            base.Release();

            BackgroundColor = new Color(121, 140, 170);
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