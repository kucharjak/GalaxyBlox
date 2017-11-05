using System;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Android.Util;
using Microsoft.Xna.Framework;
using GalaxyBlox.Buttons;
using System.Linq;

namespace GalaxyBlox.Rooms
{
    class MenuRoom : Room
    {
        Button btnContinue;

        public MenuRoom(RoomManager parent, string name, Size realSize, Size gameSize) : base(parent, name, realSize, gameSize)
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
            var btnCount = 5;
            var btnStartPosY = (RoomSize.Height - (50 * btnCount + 10 * (btnCount - 1))) / 2;

            ////// ADDING BUTTONS //////
            btnContinue = new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY),
                TextIsCentered = true,
                Text = "Pokračovat",
                LayerDepth = 0.5f,
                Enabled = false
            };
            btnContinue.Click += btnContinue_Click;
            objects.Add(btnContinue);

            objToAdd = new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65),
                TextIsCentered = true,
                Text = "Nová hra",
                LayerDepth = 0.5f   
            };
            (objToAdd as Button).Click += btnNewGame_Click;
            objects.Add(objToAdd);

            objToAdd = new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65 * 2),
                TextIsCentered = true,
                Text = "Ovládání",
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnControls_Click;
            objects.Add(objToAdd);

            objToAdd = new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65 * 3),
                TextIsCentered = true,
                Text = "Nastavení",
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnSettings_Click;
            objects.Add(objToAdd);

            objToAdd = new MenuButton(this)
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65 * 4),
                TextIsCentered = true,
                Text = "Konec",
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnFinish_Click;
            objects.Add(objToAdd);
        }

        public override void AfterChangeEvent(string args)
        {
            base.AfterChangeEvent(args);

            if (args == "newGame")
            {
                btnContinue.Enabled = true;
            }
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

        private void btnContinue_Click(object sender, EventArgs e)
        {
            ParentRoomManager.ChangeRooms(args: "continue");
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            ParentRoomManager.ChangeRooms(args: "newGame");
        }
    }
}