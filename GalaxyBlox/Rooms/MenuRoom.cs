using System;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Android.Util;
using Microsoft.Xna.Framework;
using GalaxyBlox.Buttons;

namespace GalaxyBlox.Rooms
{
    class MenuRoom : Room
    {
        public MenuRoom(Size realSize, Size gameSize) : base(realSize, gameSize)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            Background = content.Load<Texture2D>("Backgrounds/menu");
        }

        protected override void AddObjects()
        {
            GameObject objToAdd;

            var padding = 30;
            var btnSize = new Vector2(RoomSize.Width - 2 * padding, 50);
            var btnCount = 4;
            var btnStartPosY = (RoomSize.Height - (50 * btnCount + 10 * (btnCount - 1))) / 2;

            ////// ADDING BUTTONS //////
            objToAdd = new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY),
                TextIsCentered = true,
                Text = "Nová hra",
                LayerDepth = 0.5f   
            };
            (objToAdd as Button).Click += btnNewGame_Click;
            objects.Add(objToAdd);

            objToAdd = new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65),
                TextIsCentered = true,
                Text = "Ovládání",
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnControls_Click;
            objects.Add(objToAdd);

            objToAdd = new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65 * 2),
                TextIsCentered = true,
                Text = "Nastavení",
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnSettings_Click;
            objects.Add(objToAdd);

            objToAdd = new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65 * 3),
                TextIsCentered = true,
                Text = "Konec",
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnFinish_Click;
            objects.Add(objToAdd);
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            Game1.Activity.Finish();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
        }

        private void btnControls_Click(object sender, EventArgs e)
        {
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            Game1.ActiveGame.ChangeRooms();
        }
    }
}