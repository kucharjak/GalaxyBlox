using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Android.Util;
using Microsoft.Xna.Framework;
using GalaxyBlox.Buttons;

namespace GalaxyBlox.Rooms
{
    class Menu : Room
    {
        public Menu(Size RealSize, Size GameSize) : base(RealSize, GameSize)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            Background = content.Load<Texture2D>("Backgrounds/menu");
            loadObjects();
        }

        private void loadObjects()
        {
            padding = 30;
            var btnSize = new Vector2(RoomSize.Width - 2 * padding, 50);
            var btnCount = 4;
            var btnStartPosY = (RoomSize.Height - (50 * btnCount + 10 * (btnCount - 1))) / 2;
            objects.Add(new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY),
                TextIsCentered = true,
                Text = "Nová hra",
                ButttonID = 1
            });
            objects.Add(new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65),
                TextIsCentered = true,
                Text = "Ovládání",
                ButttonID = 2
            });
            objects.Add(new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65 * 2),
                TextIsCentered = true,
                Text = "Nastavení",
                ButttonID = 3
            });
            objects.Add(new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65 * 3),
                TextIsCentered = true,
                Text = "Konec",
                ButttonID = 4
            });
        }
    }
}