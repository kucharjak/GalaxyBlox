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
        private SettingOptions.GameMode gameMode;

        public GameRoom(Room parent, string name, Size size, Vector2 position) : base(parent, name, size, position)
        {
        }

        public GameRoom(string name, Size size, Vector2 position) : base(name, size, position)
        {
        }

        protected override void Initialize()
        {
            gameMode = Settings.Game.Mode;
            FullScreen = true;
            Background = Contents.Textures.BackgroundGame;
            GameObject objToAdd;
            var padding = 15;

            objToAdd = new SwipeArea(this, Position = new Vector2(0, 0), new Vector2(Size.Width, Size.Height)) // label for Score
            {
                LayerDepth = 0.05f,
                Alpha = 1f
            };
            (objToAdd as SwipeArea).Swipe += GameRoom_Swipe;
            Objects.Add(objToAdd);

            var btnSize = new Vector2(75); //new Vector2(RoomSize.Width / 4 - 5);
            var btnCount = 4;
            var btnPartSize = (Size.Width - 2f * padding) / btnCount;
            ///// ADDING BACKGROUNDS AND FRAMES /////
            objToAdd = new GameObject(this)
            {
                Size = new Vector2(Size.Width + 10, btnSize.Y + 2 * padding + 5), // +5 correction
                Position = new Vector2(-5, Size.Height - btnSize.Y - 2 * padding),
                BaseColor = Contents.Colors.BackgroundControlsColor,
                BackgroundImage = Contents.Textures.Pix,
                LayerDepth = 0.03f
            };
            Objects.Add(objToAdd);

            ///// ADDING CONTROL BUTTONS //////
            objToAdd = Bank.Buttons.GetControlButton(this); // LEFT BUTTON
            objToAdd.Size = btnSize;
            objToAdd.Position = new Vector2(btnPartSize * 0 + padding + ((btnPartSize - btnSize.X) / 2f), Size.Height - padding - btnSize.Y);
            objToAdd.BackgroundImage = Contents.Textures.ControlButton_left;
            (objToAdd as Button).Hover += btnLeft_Hover;
            (objToAdd as Button).Release += btnLeft_Release;
            Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetControlButton(this); // DOWN BUTTON
            objToAdd.Size = btnSize;
            objToAdd.Position = new Vector2(btnPartSize * 1 + padding + ((btnPartSize - btnSize.X) / 2f), Size.Height - padding - btnSize.Y);
            objToAdd.BackgroundImage = Contents.Textures.ControlButton_down;
            (objToAdd as Button).Click += btnDown_Click;
            (objToAdd as Button).Hover += btnDown_Hover;
            (objToAdd as Button).Release += btnDown_Release;
            Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetControlButton(this); // ROTATE BUTTON
            objToAdd.Size = btnSize;
            objToAdd.Position = new Vector2(btnPartSize * 2 + padding + ((btnPartSize - btnSize.X) / 2f), Size.Height - padding - btnSize.Y);
            objToAdd.BackgroundImage = Contents.Textures.ControlButton_rotate;
            (objToAdd as Button).Click += btnRotate_Click;
            Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetControlButton(this); // RIGHT BUTTON
            objToAdd.Size = btnSize;
            objToAdd.Position = new Vector2(btnPartSize * 3 + padding + ((btnPartSize - btnSize.X) / 2f), Size.Height - padding - btnSize.Y);
            objToAdd.BackgroundImage = Contents.Textures.ControlButton_right;
            (objToAdd as Button).Hover += btnRight_Hover;
            (objToAdd as Button).Release += btnRight_Release;
            Objects.Add(objToAdd);

            var plyArnPosY = padding + 85;
            arena = new PlayingArena(this,
                new Vector2(380, 600),
                new Vector2(50, plyArnPosY),
                gameMode)
            {
                Name = "main_playground",
                LayerDepth = 0.05f
            };
            arena.GameEnded += Arena_GameEnded;
            arena.ScoreChanged += Arena_ScoreChanged;
            arena.ActorsQueueChanged += Arena_ActorsQueueChanged;
            Objects.Add(arena);
            
            var objectsAlpha = 0.6f;

            //// ADDING SCORE BOARD
            scoreBoard = Bank.Visuals.GetPanelBoard(this);
            scoreBoard.Size = new Vector2(arena.Size.X, 30);
            scoreBoard.Position = new Vector2(arena.Position.X, plyArnPosY - scoreBoard.Size.Y - 5);
            scoreBoard.Alpha = objectsAlpha;
            scoreBoard.Text = arena.Score.ToString();
            Objects.Add(scoreBoard);

            //// ADDING LABEL FOR SCORE
            objToAdd = Bank.Visuals.GetPanelLabel(this);
            objToAdd.Size = new Vector2(arena.Size.X, 25);
            objToAdd.Position = new Vector2(arena.Position.X, plyArnPosY - objToAdd.Size.Y - scoreBoard.Size.Y - 5);
            objToAdd.Alpha = objectsAlpha;
            objToAdd.Text = "Skore";
            Objects.Add(objToAdd);

            // ADDING RIGH PANEL
            var rightPanelWidth = Size.Width - (arena.Position.X + arena.Size.X) - 10;
            var rightPanelPosX = arena.Position.X + arena.Size.X + 5;
            var rightPanelPosY = arena.Position.Y;

            ///// ADDING PAUSE BUTTON //////

            objToAdd = Bank.Buttons.GetPauseButton(this);
            objToAdd.Size = new Vector2(rightPanelWidth, rightPanelWidth);
            objToAdd.Position = new Vector2(rightPanelPosX, rightPanelPosY);
            objToAdd.LayerDepth = 0.05f;
            //objToAdd.Enabled = Parent != null;
            (objToAdd as Button).Click += btnPause_Click;
            Objects.Add(objToAdd);
            rightPanelPosY += (int)(padding + objToAdd.Size.Y);

            //// ADDING LABEL BONUS
            objToAdd = objToAdd = Bank.Visuals.GetPanelLabel(this);
            objToAdd.Size = new Vector2(rightPanelWidth, 35);
            objToAdd.Position = new Vector2(rightPanelPosX, rightPanelPosY);
            objToAdd.Alpha = objectsAlpha;
            objToAdd.Text = "Bonus";
            Objects.Add(objToAdd);
            rightPanelPosY += (int)objToAdd.Size.Y;

            //// ADDING BONUS BTN
            objToAdd = Bank.Visuals.GetPanelBoard(this);
            objToAdd.Position = new Vector2(rightPanelPosX, rightPanelPosY);
            objToAdd.Size = new Vector2(rightPanelWidth, rightPanelWidth);
            objToAdd.Alpha = objectsAlpha;
            Objects.Add(objToAdd);
            rightPanelPosY += (int)(padding + objToAdd.Size.Y);


            ///// ADDING LEFT PANEL
            var leftPanelWidth = Size.Width - (arena.Position.X + arena.Size.X) - 10;
            var leftPanelPosX = 5;
            var leftPanelPosY = arena.Position.Y;
            
            //// ADDING LABEL FOR LEVEL
            objToAdd = Bank.Visuals.GetPanelLabel(this);
            objToAdd.Position = new Vector2(leftPanelPosX, leftPanelPosY);
            objToAdd.Size = new Vector2(leftPanelWidth, 35);
            objToAdd.Alpha = objectsAlpha;
            objToAdd.Text = "Level";
            Objects.Add(objToAdd);
            leftPanelPosY += (int)objToAdd.Size.Y;

            //// ADDING LEVEL BOARD
            levelBoard = Bank.Visuals.GetPanelBoard(this);
            levelBoard.Position = new Vector2(leftPanelPosX, leftPanelPosY);
            levelBoard.Size = new Vector2(rightPanelWidth, 55);
            levelBoard.Alpha = objectsAlpha;
            levelBoard.Text = arena.Level.ToString();
            Objects.Add(levelBoard);
            leftPanelPosY += (int)(padding + levelBoard.Size.Y);

            //// NEXT ACTOR LABEL
            objToAdd = Bank.Visuals.GetPanelLabel(this);
            objToAdd.Position = new Vector2(leftPanelPosX, leftPanelPosY);
            objToAdd.Size = new Vector2(leftPanelWidth, 35);
            objToAdd.Alpha = objectsAlpha;
            objToAdd.Text = "Další";
            Objects.Add(objToAdd);
            leftPanelPosY += (int)objToAdd.Size.Y;

            //// NEXT ACTOR BOARD
            nextActor = new ActorViewer(this, new Vector2(leftPanelWidth, leftPanelWidth), Contents.Colors.PanelContentBackgroundColor * 0f, arena.CubeSize)
            {
                Position = new Vector2(leftPanelPosX, leftPanelPosY),
                LayerDepth = 0.05f,
                Alpha = 1f
            };
            Objects.Add(nextActor);

            //// ADDING BACKGROUND FOR ACTOR BOARD
            objToAdd = Bank.Visuals.GetPanelBoard(this);
            objToAdd.Size = new Vector2(leftPanelWidth, leftPanelWidth);
            objToAdd.Position = new Vector2(leftPanelPosX, leftPanelPosY);
            objToAdd.LayerDepth = 0.049f;
            objToAdd.Alpha = objectsAlpha;
            Objects.Add(objToAdd);
            leftPanelPosY += (int)objToAdd.Size.Y;
        }

        private void Arena_GameEnded(object sender, EventArgs e)
        {
            if (Parent != null)
                this.End();
            else
                arena.StartNewGame();
        }

        private void GameRoom_Swipe(object sender, EventArgs e)
        {
            var args = (e as SwipeEventArgs);
            switch (args.Direction)
            {
                case SwipeArea.SwipeDirection.Down: arena.MakeActorFall(); break;
            }
        }

        private void Arena_ActorsQueueChanged(object sender, EventArgs e)
        {
            var args = (e as QueueChangeEventArgs);
            nextActor.SetActor(args.NewActor, args.NewActorsColor);
        }

        private void Arena_ScoreChanged(object sender, EventArgs e)
        {
            if (scoreBoard != null)
                scoreBoard.Text = Strings.ScoreToLongString(arena.Score); //Strings.ScoreToString(arena.Score, 3); 

            if (levelBoard != null)
                levelBoard.Text = arena.Level.ToString();
        }

        protected override void HandleBackButton()
        {
            btnPause_Click(this, new EventArgs());
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (Parent != null)
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