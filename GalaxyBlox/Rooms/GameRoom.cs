using System;
using System.Linq;
using Android.Util;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GalaxyBlox.Buttons;
using GalaxyBlox.Static;

namespace GalaxyBlox.Rooms
{
    class GameRoom : Room
    {
        public GameRoom(Size realSize, Size gameSize) : base(realSize, gameSize)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            Background = content.Load<Texture2D>("Backgrounds/game");
        }

        protected override void AddObjects()
        {
            GameObject objToAdd;
            var btnPauseSize = new Vector2(50);
            var padding = 15;

            ///// ADDING PAUSE BUTTON //////
            objToAdd = new PauseButton(this)
            {
                Size = btnPauseSize,
                Position = new Vector2(RoomSize.Width - btnPauseSize.X - padding, padding),
                BackgroundImage = Contents.Textures.ControlButton_pause,
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnPause_Click;
            objects.Add(objToAdd);

            var btnSize = new Vector2(75); //new Vector2(RoomSize.Width / 4 - 5);
            var btnCount = 4;
            var btnPartSize = (RoomSize.Width - 2f * padding) / btnCount;
            ///// ADDING BACKGROUNDS AND FRAMES /////
            objToAdd = new GameObject(this)
            {
                Size = new Vector2(RoomSize.Width + 10, btnSize.Y + 2 * padding + 5), // +5 correction
                Position = new Vector2(-5, RoomSize.Height - btnSize.Y - 2 * padding),
                Color = Contents.Colors.BackgroundControlsColor,
                BackgroundImage = Contents.Textures.Pix,
                LayerDepth = 0.5f
            };
            objects.Add(objToAdd);

            ///// ADDING CONTROL BUTTONS //////
            objToAdd = new ControlButton(this) // LEFT BUTTON
            {
                Size = btnSize,
                Position = new Vector2(btnPartSize * 0 + padding + ((btnPartSize - btnSize.X) / 2f), RoomSize.Height - padding - btnSize.Y),
                BackgroundImage = Contents.Textures.ControlButton_left,
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnLeft_Click;
            objects.Add(objToAdd);

            objToAdd = new ControlButton(this) // DOWN BUTTON
            {
                Size = btnSize,
                Position = new Vector2(btnPartSize * 1 + padding + ((btnPartSize - btnSize.X) / 2f), RoomSize.Height - padding - btnSize.Y),
                BackgroundImage = Contents.Textures.ControlButton_down,
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnDown_Click;
            (objToAdd as Button).Hover += btnDown_Hover;
            (objToAdd as Button).Release += btnDown_Release;
            objects.Add(objToAdd);

            objToAdd = new ControlButton(this) // ROTATE BUTTON
            {
                Size = btnSize,
                Position = new Vector2(btnPartSize * 2 + padding + ((btnPartSize - btnSize.X) / 2f), RoomSize.Height - padding - btnSize.Y),
                BackgroundImage = Contents.Textures.ControlButton_rotate,
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnRotate_Click;
            objects.Add(objToAdd);

            objToAdd = new ControlButton(this) // RIGHT BUTTON
            {
                Size = btnSize,
                Position = new Vector2(btnPartSize * 3 + padding + ((btnPartSize - btnSize.X) / 2f), RoomSize.Height - padding - btnSize.Y),
                BackgroundImage = Contents.Textures.ControlButton_right,
                LayerDepth = 0.5f
            };
            (objToAdd as Button).Click += btnRight_Click;
            objects.Add(objToAdd);

            var plyArnPosY = padding + 65;
            objToAdd = new PlayingArena(this,
                new Vector2(RoomSize.Width, RoomSize.Height - plyArnPosY - (btnSize.Y + 2 * padding)),
                new Vector2(0, plyArnPosY))
            {
                Name = "main_playground",
                LayerDepth = 0.5f
            };
            objects.Add(objToAdd);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Game1.ActiveGame.ChangeRooms();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            var playground = objects.Where(obj => obj.Name == "main_playground").FirstOrDefault();
            if (playground != null)
                (playground as PlayingArena).MoveRight();
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            var playground = objects.Where(obj => obj.Name == "main_playground").FirstOrDefault();
            if (playground != null)
                (playground as PlayingArena).Rotate();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var playground = objects.Where(obj => obj.Name == "main_playground").FirstOrDefault();
            if (playground != null && (sender as Button).HoverTime < 150)
                (playground as PlayingArena).MakeActorFall();
        }

        private void btnDown_Hover(object sender, EventArgs e)
        {
            var playground = objects.Where(obj => obj.Name == "main_playground").FirstOrDefault();
            if (playground != null)
                (playground as PlayingArena).MakeActorSpeedup();
        }

        private void btnDown_Release(object sender, EventArgs e)
        {
            var playground = objects.Where(obj => obj.Name == "main_playground").FirstOrDefault();
            if (playground != null)
                (playground as PlayingArena).SlowDownActor();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            var playground = objects.Where(obj => obj.Name == "main_playground").FirstOrDefault();
            if (playground != null)
                (playground as PlayingArena).MoveLeft();
        }
    }
}