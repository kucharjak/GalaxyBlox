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

        private GameObject lblScore;
        private GameObject lblLevel;

        private ActorViewer actorViewer;

        private Button btnPause;
        private Button btnControlLeft;
        private Button btnControlRight;
        private Button btnControlFall;
        private Button btnControlRotate;

        private Button btnBonus1;
        private Button btnBonus2;
        private Button btnBonus3;

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

            GameObject lastObj;

            // ADDING FIRST PANEL
            // SETTINGS 
            var viewerSize = 110;
            var viewerPadding = 15;
            var btnPauseSize = 100;
            var lblScoreSize = new Vector2(Size.Width, 55);
            var lblLevelSize = new Vector2(Size.Width, 45);
            var scoreLineSize = new Vector2(220, 3);

            // PANEL BACKGROUND
            Objects.Add(new GameObject(this)
            {
                Size = new Vector2(Size.Width, viewerSize + 2 * viewerPadding),
                Position = new Vector2(0, 0),
                BackgroundImage = Contents.Textures.Pix,
                BaseColor = Contents.Colors.ScorePanelBackgroundColor,
                LayerDepth = 0.01f
            });
            lastObj = Objects.Last();
            var playingArenaStart = lastObj.Position.Y + lastObj.Size.Y + 5;

            // ACTOR VIEWER
            Objects.Add(new GameObject(this)
            {
                BackgroundImage = Contents.Textures.BorderedButtonBackground,
                Size = new Vector2(viewerSize, viewerSize),
                Position = new Vector2(viewerPadding, viewerPadding),
                LayerDepth = 0.045f
            });
            actorViewer = new ActorViewer(this, new Vector2(viewerSize, viewerSize), Contents.Colors.ActorViewerBackgroundColor)
            {
                Position = new Vector2(viewerPadding, viewerPadding),
                LayerDepth = 0.05f
            };
            Objects.Add(actorViewer);

            // PAUSE BUTTON
            btnPause = Bank.Buttons.GetPauseButton(this);
            btnPause.Size = new Vector2(btnPauseSize, btnPauseSize);
            btnPause.Position = new Vector2(Size.Width - btnPauseSize - viewerPadding, ((viewerSize + 2 * viewerPadding) - btnPauseSize) / 2);
            btnPause.Click += btnPause_Click;
            Objects.Add(btnPause);

            var scoreStartPosY = ((viewerSize + 2 * viewerPadding) - (lblScoreSize.Y + lblLevelSize.Y + scoreLineSize.Y)) / 2;
            // SCORE
            lblScore = new GameObject(this)
            {
                Size = lblScoreSize,
                Position = new Vector2(0, scoreStartPosY),
                TextSpriteFont = Contents.Fonts.PanelContentText,
                Text = "Skóre",
                TextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                ShowText = true,
                LayerDepth = 0.05f,
                Scale = 1.05f,
                Origin = new Vector2(0.5f, 1)
            };
            Objects.Add(lblScore);

            // LINE BETWEEN SCORE AND LEVEL
            Objects.Add(new GameObject(this)
            {
                BackgroundImage = Contents.Textures.Pix,
                BaseColor = Color.Black,
                Size = scoreLineSize,
                Position = new Vector2((Size.Width - scoreLineSize.X) / 2, scoreStartPosY + lblScoreSize.Y),
                LayerDepth = 0.05f
            });

            // LEVEL
            lblLevel = new GameObject(this)
            {
                Size = lblLevelSize,
                Position = new Vector2(0, scoreStartPosY +  lblScoreSize.Y + scoreLineSize.Y),
                TextSpriteFont = Contents.Fonts.PanelContentText,
                Text = "Level",
                TextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                ShowText = true,
                LayerDepth = 0.05f,
                Scale = 0.85f,
                Origin = new Vector2(0.5f, 0)
            };
            Objects.Add(lblLevel);

            // ADDING SECOND PANEL
            // SETTINGS
            var btnSize = 130;
            var btnPadding = 20;
            var btnCount = 4;
            var btnMargin = ((Size.Width - 2 * btnPadding) - (btnSize * btnCount)) / (btnCount - 1);

            // PANEL BACKGROUND
            Objects.Add(new GameObject(this)
            {
                Size = new Vector2(Size.Width, btnSize + 2 * btnPadding),
                Position = new Vector2(0, Size.Height - (btnSize + 2 * btnPadding)),
                BackgroundImage = Contents.Textures.Pix,
                BaseColor = Contents.Colors.ControlPanelBackgroundColor,
                LayerDepth = 0.01f
            });
            lastObj = Objects.Last();
            var playingArenaEnd = lastObj.Position.Y - 5;

            // CONTROL BUTTON LEFT
            btnControlLeft = Bank.Buttons.GetControlButton(this);
            btnControlLeft.BackgroundImage = Contents.Textures.ControlButton_left;
            btnControlLeft.Size = new Vector2(btnSize, btnSize);
            btnControlLeft.Position = new Vector2(btnPadding, lastObj.Position.Y + btnPadding);
            btnControlLeft.Release += btnLeft_Release;
            btnControlLeft.Hover += btnLeft_Hover;
            Objects.Add(btnControlLeft);

            // CONTROL BUTTON FALL
            btnControlFall = Bank.Buttons.GetControlButton(this);
            btnControlFall.BackgroundImage = Contents.Textures.ControlButton_fall;
            btnControlFall.Size = new Vector2(btnSize, btnSize);
            btnControlFall.Position = new Vector2(btnPadding + (btnSize + btnMargin) * 1, lastObj.Position.Y + btnPadding);
            btnControlFall.Click += btnDown_Click;
            btnControlFall.Hover += btnDown_Hover;
            btnControlFall.Release += btnDown_Release;
            Objects.Add(btnControlFall);

            // CONTROL BUTTON ROTATE
            btnControlRotate = Bank.Buttons.GetControlButton(this);
            btnControlRotate.BackgroundImage = Contents.Textures.ControlButton_rotate;
            btnControlRotate.Size = new Vector2(btnSize, btnSize);
            btnControlRotate.Position = new Vector2(btnPadding + (btnSize + btnMargin) * 2, lastObj.Position.Y + btnPadding);
            btnControlRotate.Click += btnRotate_Click;
            Objects.Add(btnControlRotate);

            // CONTROL BUTTON RIGHT
            btnControlRight = Bank.Buttons.GetControlButton(this);
            btnControlRight.BackgroundImage = Contents.Textures.ControlButton_right;
            btnControlRight.Size = new Vector2(btnSize, btnSize);
            btnControlRight.Position = new Vector2(btnPadding + (btnSize + btnMargin) * 3, lastObj.Position.Y + btnPadding);
            btnControlRight.Release += btnRight_Release;
            btnControlRight.Hover += btnRight_Hover;
            Objects.Add(btnControlRight);

            if (gameMode != SettingOptions.GameMode.Classic)
            {
                // ADDING BONUS
                // SETTINGS
                var btnBonusSize = 80;
                var btnBonusCount = 3;
                var btnBonusPadding = 10;
                var btnBonusLeftPadding = 120;
                var btnBonusMargin = ((Size.Width - 2 * btnBonusLeftPadding ) - (btnBonusCount * btnBonusSize)) / (btnBonusCount - 1);

                // ADDING BONUS PANEL
                Objects.Add(new GameObject(this)
                {
                    Size = new Vector2(Size.Width, btnBonusSize + 2 * btnBonusPadding),
                    Position = new Vector2(0, Size.Height - (btnSize + 2 * btnPadding) - (btnBonusSize + 2 * btnBonusPadding)),
                    BackgroundImage = Contents.Textures.Pix,
                    BaseColor = Contents.Colors.BonusPanelBackgroundColor,
                    LayerDepth = 0.01f
                });
                lastObj = Objects.Last();
                playingArenaEnd = lastObj.Position.Y - 5;

                btnBonus1 = new Button(this)
                {
                    Size = new Vector2(btnBonusSize),
                    Position = new Vector2(btnBonusLeftPadding + (btnBonusMargin + btnBonusSize) * 0, lastObj.Position.Y + btnBonusPadding),
                    BackgroundImage =  Contents.Textures.BorderedButtonBackground,
                    BaseColor = Color.White,
                    DefaultBackgroundColor = Color.White,
                    SelectedBackgroundColor = Color.White,
                    LayerDepth = 0.05f
                };
                Objects.Add(btnBonus1);

                btnBonus2 = new Button(this)
                {
                    Size = new Vector2(btnBonusSize),
                    Position = new Vector2(btnBonusLeftPadding + (btnBonusMargin + btnBonusSize) * 1, lastObj.Position.Y + btnBonusPadding),
                    BackgroundImage = Contents.Textures.BorderedButtonBackground,
                    BaseColor = Color.White,
                    DefaultBackgroundColor = Color.White,
                    SelectedBackgroundColor = Color.White,
                    LayerDepth = 0.05f
                };
                Objects.Add(btnBonus2);

                btnBonus3 = new Button(this)
                {
                    Size = new Vector2(btnBonusSize),
                    Position = new Vector2(btnBonusLeftPadding + (btnBonusMargin + btnBonusSize) * 2, lastObj.Position.Y + btnBonusPadding),
                    BackgroundImage = Contents.Textures.BorderedButtonBackground,
                    BaseColor = Color.White,
                    DefaultBackgroundColor = Color.White,
                    SelectedBackgroundColor = Color.White,
                    LayerDepth = 0.05f
                };
                Objects.Add(btnBonus3);
            }

            // ADDING PLAYING ARENA
            arena = new PlayingArena(this, new Vector2(Size.Width, playingArenaEnd - playingArenaStart), new Vector2(0, playingArenaStart), SettingOptions.GameMode.Test);
            arena.LayerDepth = 0.05f;
            arena.ActorsQueueChanged += Arena_ActorsQueueChanged;
            arena.ScoreChanged += Arena_ScoreChanged;
            arena.GameEnded += Arena_GameEnded;
            Objects.Add(arena);
        }

        private void Arena_GameEnded(object sender, EventArgs e)
        {
            if (Parent != null)
                this.End();
            else
            {
                arena.StartNewGame();
                lblLevel.Text = "Level";
                lblScore.Text = "Skóre";
            }
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
            actorViewer.SetActor(args.NewActor, args.NewActorsColor);
        }

        private void Arena_ScoreChanged(object sender, EventArgs e)
        {
            if (lblScore != null)
                lblScore.Text = Strings.ScoreToLongString(arena.Score); //Strings.ScoreToString(arena.Score, 3); 

            if (lblLevel != null)
                lblLevel.Text = arena.Level.ToString();
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