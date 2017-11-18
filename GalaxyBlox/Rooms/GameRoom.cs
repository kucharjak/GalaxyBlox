using System;
using System.Linq;
using Android.Util;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GalaxyBlox.Objects;
using GalaxyBlox.Static;
using GalaxyBlox.Utils;
using GalaxyBlox.EventArgsClasses;
using Microsoft.Xna.Framework.Input;

namespace GalaxyBlox.Rooms
{
    class GameRoom : Room
    {
        private PlayingArena arena;
        private GameObject scoreBoard;
        private GameObject levelBoard;
        private ActorViewer nextActor;

        public GameRoom(Room parent, string name, Size realSize, Size gameSize) : base(parent, name, realSize, gameSize)
        {
        }

        public GameRoom(string name, Size realSize, Size gameSize) : base(name, realSize, gameSize)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            Background = content.Load<Texture2D>("Backgrounds/game");
        }

        protected override void Initialize()
        {
            LayerDepth = 0.2f;
            GameObject objToAdd;
            var leftPanelWidth = 60;
            var padding = 15;

            var btnSize = new Vector2(75); //new Vector2(RoomSize.Width / 4 - 5);
            var btnCount = 4;
            var btnPartSize = (RoomSize.Width - 2f * padding) / btnCount;
            ///// ADDING BACKGROUNDS AND FRAMES /////
            objToAdd = new GameObject(this)
            {
                Size = new Vector2(RoomSize.Width + 10, btnSize.Y + 2 * padding + 5), // +5 correction
                Position = new Vector2(-5, RoomSize.Height - btnSize.Y - 2 * padding),
                BaseColor = Contents.Colors.BackgroundControlsColor,
                BackgroundImage = Contents.Textures.Pix,
                LayerDepth = 0.03f
            };
            Objects.Add(objToAdd);

            ///// ADDING CONTROL BUTTONS //////
            objToAdd = Bank.Buttons.GetControlButton(this); // LEFT BUTTON
                objToAdd.Size = btnSize;
                objToAdd.Position = new Vector2(btnPartSize * 0 + padding + ((btnPartSize - btnSize.X) / 2f), RoomSize.Height - padding - btnSize.Y);
                objToAdd.BackgroundImage = Contents.Textures.ControlButton_left;
            (objToAdd as Button).Hover += btnLeft_Hover;
            (objToAdd as Button).Release += btnLeft_Release;
            Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetControlButton(this); // DOWN BUTTON
                objToAdd.Size = btnSize;
                objToAdd.Position = new Vector2(btnPartSize * 1 + padding + ((btnPartSize - btnSize.X) / 2f), RoomSize.Height - padding - btnSize.Y);
                objToAdd.BackgroundImage = Contents.Textures.ControlButton_down;
            (objToAdd as Button).Click += btnDown_Click;
            (objToAdd as Button).Hover += btnDown_Hover;
            (objToAdd as Button).Release += btnDown_Release;
            Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetControlButton(this); // ROTATE BUTTON
                objToAdd.Size = btnSize;
                objToAdd.Position = new Vector2(btnPartSize * 2 + padding + ((btnPartSize - btnSize.X) / 2f), RoomSize.Height - padding - btnSize.Y);
                objToAdd.BackgroundImage = Contents.Textures.ControlButton_rotate;
            (objToAdd as Button).Click += btnRotate_Click;
            Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetControlButton(this); // RIGHT BUTTON
                objToAdd.Size = btnSize;
                objToAdd.Position = new Vector2(btnPartSize * 3 + padding + ((btnPartSize - btnSize.X) / 2f), RoomSize.Height - padding - btnSize.Y);
                objToAdd.BackgroundImage = Contents.Textures.ControlButton_right;
            (objToAdd as Button).Hover += btnRight_Hover;
            (objToAdd as Button).Release += btnRight_Release;
            Objects.Add(objToAdd);

            var plyArnPosY = padding + 65;
            
            arena = new PlayingArena(this,
                new Vector2(RoomSize.Width - leftPanelWidth - 2 * padding, RoomSize.Height - plyArnPosY - (btnSize.Y + 2 * padding)),
                new Vector2(0, plyArnPosY))
            {
                Name = "main_playground",
                LayerDepth = 0.05f
            };
            arena.ScoreChanged += Arena_ScoreChanged;
            arena.ActorsQueueChanged += Arena_ActorsQueueChanged;
            Objects.Add(arena);

            // ADDING LEFT PANEL
            var objectsAlpha = 0.6f;

            ///// ADDING PAUSE BUTTON //////
            leftPanelWidth = (int)(RoomSize.Width - arena.Size.X - 2 * padding);
            var leftPanelPosY = (int)arena.Position.Y;
            objToAdd = Bank.Buttons.GetPauseButton(this);
                objToAdd.Size = new Vector2(leftPanelWidth, leftPanelWidth);
                objToAdd.Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY);
                objToAdd.LayerDepth = 0.05f;
                (objToAdd as Button).Click += btnPause_Click;
            Objects.Add(objToAdd);

            //// ADDING LABEL FOR SCORE
            leftPanelPosY += (int)(padding + objToAdd.Size.Y);
            objToAdd = Bank.Visuals.GetPanelLabel(this);
                objToAdd.Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY);
                objToAdd.Size = new Vector2(leftPanelWidth, 35);
                objToAdd.Alpha = objectsAlpha;
                objToAdd.Text = "Skore";
            Objects.Add(objToAdd);

            //// ADDING SCORE BOARD
            leftPanelPosY += (int)objToAdd.Size.Y;
                scoreBoard = Bank.Visuals.GetPanelBoard(this);
                scoreBoard.Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY);
                scoreBoard.Size = new Vector2(leftPanelWidth, 55);
                scoreBoard.Alpha = objectsAlpha;
                scoreBoard.Text = arena.Score.ToString();
            Objects.Add(scoreBoard);

            //// ADDING LABEL FOR LEVEL
            leftPanelPosY += (int)(padding + scoreBoard.Size.Y);
                objToAdd = Bank.Visuals.GetPanelLabel(this);
                objToAdd.Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY);
                objToAdd.Size = new Vector2(leftPanelWidth, 35);
                objToAdd.Alpha = objectsAlpha;
                objToAdd.Text = "Level";
            Objects.Add(objToAdd);

            //// ADDING LEVEL BOARD
            leftPanelPosY += (int)objToAdd.Size.Y;
            levelBoard = Bank.Visuals.GetPanelBoard(this);
                levelBoard.Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY);
                levelBoard.Size = new Vector2(leftPanelWidth, 55);
                levelBoard.Alpha = objectsAlpha;
                levelBoard.Text = arena.Level.ToString();
            Objects.Add(levelBoard);

            //// NEXT ACTOR LABEL
            leftPanelPosY += (int)(padding + levelBoard.Size.Y);
                objToAdd = Bank.Visuals.GetPanelLabel(this);
                objToAdd.Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY);
                objToAdd.Size = new Vector2(leftPanelWidth, 35);
                objToAdd.Alpha = objectsAlpha;
                objToAdd.Text = "Další";
            Objects.Add(objToAdd);

            //// NEXT ACTOR BOARD
            leftPanelPosY += (int)objToAdd.Size.Y;
            nextActor = new ActorViewer(this, new Vector2(leftPanelWidth, leftPanelWidth), Contents.Colors.PanelContentBackgroundColor * 0f, arena.CubeSize)
            {
                Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY),
                LayerDepth = 0.05f,
                Alpha = 1f
            };
            Objects.Add(nextActor);

            //// ADDING BACKGROUND FOR ACTOR BOARD
            objToAdd = Bank.Visuals.GetPanelBoard(this);
                objToAdd.Size = new Vector2(leftPanelWidth, leftPanelWidth);
                objToAdd.Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY);
                objToAdd.LayerDepth = 0.049f;
                objToAdd.Alpha = objectsAlpha;
            Objects.Add(objToAdd);

            //// ADDING LABEL BONUS
            leftPanelPosY += (int)(padding + nextActor.Size.Y);
            objToAdd = objToAdd = Bank.Visuals.GetPanelLabel(this);
                objToAdd.Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY);
                objToAdd.Size = new Vector2(leftPanelWidth, 35);
                objToAdd.Alpha = objectsAlpha;
                objToAdd.Text = "Bonus";
            Objects.Add(objToAdd);

            //// ADDING BONUS BTN
            leftPanelPosY += (int)objToAdd.Size.Y;
                objToAdd = Bank.Visuals.GetPanelBoard(this);
                objToAdd.Position = new Vector2(RoomSize.Width - leftPanelWidth - padding, leftPanelPosY);
                objToAdd.Size = new Vector2(leftPanelWidth, leftPanelWidth);
                objToAdd.Alpha = objectsAlpha;
            Objects.Add(objToAdd);
        }

        private void Arena_ActorsQueueChanged(object sender, EventArgs e)
        {
            var args = (e as QueueChangeEventArgs);
            nextActor.SetActor(args.NewActor, args.NewActorsColor);
        }

        private void Arena_ScoreChanged(object sender, EventArgs e)
        {
            if (scoreBoard != null)
                scoreBoard.Text = Strings.ScoreToString(arena.Score, 3); 

            if (levelBoard != null)
                levelBoard.Text = arena.Level.ToString();
        }

        protected override void HandleBackButton()
        {
            btnPause_Click(this, new EventArgs());
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            //ParentRoomManager.ChangeRooms(args: "pause");
            Close();
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            arena.Rotate();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var btn = (sender as Button);
            if (btn.HoverTime < 150)
                arena.MakeActorFall();
        }

        private void btnDown_Hover(object sender, EventArgs e)
        {
            arena.MakeActorSpeedup();
        }

        private void btnDown_Release(object sender, EventArgs e)
        {
            arena.SlowDownActor();
        }

        private void btnRight_Hover(object sender, EventArgs e)
        {
            arena.MoveRight();
        }

        private void btnLeft_Hover(object sender, EventArgs e)
        {
            arena.MoveLeft();
        }

        private void btnRight_Release(object sender, EventArgs e)
        {
            arena.StopMovingRight();
        }

        private void btnLeft_Release(object sender, EventArgs e)
        {
            arena.StopMovingLeft();
        }
    }
}