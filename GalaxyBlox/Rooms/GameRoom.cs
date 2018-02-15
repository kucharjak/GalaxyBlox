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
using static GalaxyBlox.Static.SettingOptions;
using System.Collections.Generic;
using GalaxyBlox.Objects.PlayingArenas;

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

        private GameObject pnlBonusBtns;
        private List<Button> bonusButtons;

        private SettingOptions.GameMode gameMode;

        public GameRoom(Room parent, string name, Vector2 size, Vector2 position) : base(parent, name, size, position)
        {
        }

        public GameRoom(string name, Vector2 size, Vector2 position) : base(name, size, position)
        {
        }

        protected override void Initialize()
        {
            gameMode = Settings.Game.UserSettings.LastGameMode;
            FullScreen = true;
            Background = Contents.Textures.BackgroundGame;

            GameObject lastObj;

            // ADDING FIRST PANEL
            // SETTINGS 
            var viewerSize = 110;
            var viewerPadding = 15;
            var btnPauseSize = 100;
            var lblScoreSize = new Vector2(Size.X, 45);
            var lblLevelSize = new Vector2(Size.X, 35);
            var scoreLineSize = new Vector2(220, 3);

            // PANEL BACKGROUND
            Objects.Add(new GameObject(this)
            {
                Size = new Vector2(Size.X, viewerSize + 2 * viewerPadding),
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
            btnPause.Position = new Vector2(Size.X - btnPauseSize - viewerPadding, ((viewerSize + 2 * viewerPadding) - btnPauseSize) / 2);
            btnPause.Click += btnPause_Click;
            Objects.Add(btnPause);

            var scoreStartPosY = ((viewerSize + 2 * viewerPadding) - (lblScoreSize.Y + lblLevelSize.Y + scoreLineSize.Y)) / 2;
            // SCORE
            lblScore = new GameObject(this)
            {
                Size = lblScoreSize,
                Position = new Vector2(0, scoreStartPosY),
                TextSpriteFont = Contents.Fonts.PlainTextFont,
                Text = "Skóre",
                TextHeight = (int)lblScoreSize.Y,
                TextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                ShowText = true,
                LayerDepth = 0.05f
            };
            Objects.Add(lblScore);

            // LINE BETWEEN SCORE AND LEVEL
            Objects.Add(new GameObject(this)
            {
                BackgroundImage = Contents.Textures.Pix,
                BaseColor = Color.Black,
                Size = scoreLineSize,
                Position = new Vector2((Size.X - scoreLineSize.X) / 2, scoreStartPosY + lblScoreSize.Y),
                LayerDepth = 0.05f
            });

            // LEVEL
            lblLevel = new GameObject(this)
            {
                Size = lblLevelSize,
                Position = new Vector2(0, scoreStartPosY +  lblScoreSize.Y + scoreLineSize.Y),
                TextSpriteFont = Contents.Fonts.PlainTextFont,
                Text = "Level",
                TextHeight = (int)lblLevelSize.Y,
                TextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                ShowText = true,
                LayerDepth = 0.05f
            };
            Objects.Add(lblLevel);

            // ADDING SECOND PANEL
            // SETTINGS
            var btnSize = 130;
            var btnPadding = 20;
            var btnCount = 4;
            var btnMargin = ((Size.X - 2 * btnPadding) - (btnSize * btnCount)) / (btnCount - 1);

            // PANEL BACKGROUND
            Objects.Add(new GameObject(this)
            {
                Size = new Vector2(Size.X, btnSize + 2 * btnPadding),
                Position = new Vector2(0, Size.Y - (btnSize + 2 * btnPadding)),
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
                var panelHeight = 100;

                // ADDING BONUS PANEL
                pnlBonusBtns = new GameObject(this)
                {
                    Size = new Vector2(Size.X, panelHeight),
                    Position = new Vector2(0, Size.Y - (btnSize + 2 * btnPadding) - panelHeight),
                    BackgroundImage = Contents.Textures.Pix,
                    BaseColor = Contents.Colors.BonusPanelBackgroundColor,
                    LayerDepth = 0.01f
                };
                Objects.Add(pnlBonusBtns);
                lastObj = Objects.Last();
                playingArenaEnd = lastObj.Position.Y - 5;

                bonusButtons = new List<Button>();
            }

            // ADDING PLAYING ARENA
            switch(gameMode)
            {
                case GameMode.Classic:
                    {
                        arena = new PlayingArena_Classic(this, new Vector2(Size.X, playingArenaEnd - playingArenaStart), new Vector2(0, playingArenaStart));
                    } break;
                case GameMode.Normal:
                    {
                        arena = new PlayingArena_Normal(this, new Vector2(Size.X, playingArenaEnd - playingArenaStart), new Vector2(0, playingArenaStart));
                        (arena as PlayingArena_Normal).AvailableBonusesChanged += Arena_AvailableBonusesChanged;
                        (arena as PlayingArena_Normal).ActiveBonusChanged += Arena_ActiveBonusChanged;
                    } break;
                case GameMode.Extreme:
                    {
                        arena = new PlayingArena_Extreme(this, new Vector2(Size.X, playingArenaEnd - playingArenaStart), new Vector2(0, playingArenaStart));
                        (arena as PlayingArena_Extreme).AvailableBonusesChanged += Arena_AvailableBonusesChanged;
                        (arena as PlayingArena_Extreme).ActiveBonusChanged += Arena_ActiveBonusChanged;
                    } break;
            }

            arena.LayerDepth = 0.05f;
            arena.ActorsQueueChanged += Arena_ActorsQueueChanged;
            arena.ScoreChanged += Arena_ScoreChanged;
            arena.GameEnded += Arena_GameEnded;
            arena.StartNewGame();
            Objects.Add(arena);
        }

        private void Arena_ActiveBonusChanged(object sender, EventArgs e)
        {
            var eventArgs = (ActiveBonusChangedEventArgs)e;
            if (eventArgs.ActiveBonus != null)
                PrepareInterfaceForBonus(eventArgs.ActiveBonus.Type);
            else
                PrepareInterfaceForBonus(BonusType.None);
        }

        private void Arena_AvailableBonusesChanged(object sender, EventArgs e)
        {
            var eventArgs = (AvailableBonusesChangeEventArgs)e;
            RefreshBonusButtons(eventArgs.GameBonuses);
        }

        private void RefreshBonusButtons(List<GameBonus> newBonuses)
        {
            // remove old bonus buttons
            foreach (var oldButton in bonusButtons)
                Objects.Remove(oldButton);
            bonusButtons.Clear();

            // add new bonus buttons
            var btnBonusPadding = 10;
            var btnBonusSize = pnlBonusBtns.Size.Y - 2 * btnBonusPadding;
            var btnBonusTextHeight = (int)(btnBonusSize * 0.45f);
            var btnMargin = (pnlBonusBtns.Size.X - (newBonuses.Count * btnBonusSize)) / (newBonuses.Count + 1);

            var i = 0;
            foreach (var bonus in newBonuses)
            {
                var btn = new Button(this)
                {
                    Size = new Vector2(btnBonusSize),
                    Position = new Vector2(btnMargin + i * (btnBonusSize + btnMargin), pnlBonusBtns.Position.Y + btnBonusPadding),
                    BackgroundImage = Contents.Textures.BorderedButtonBackground,
                    BaseColor = Color.White,
                    DefaultBackgroundColor = Color.White,
                    SelectedBackgroundColor = Color.White,
                    LayerDepth = 0.05f,
                    TextSpriteFont = Contents.Fonts.PlainTextFont,
                    TextHeight = btnBonusTextHeight,
                    ShowText = true,
                    Enabled = bonus.Enabled,
                    TextAlignment = TextAlignment.Center,
                    Text = bonus.SpecialText,
                    Data = newBonuses[i]
                };
                btn.Click += Btn_Click;
                Objects.Add(btn);
                bonusButtons.Add(btn);

                i++;
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            var btn = (sender as Button);
            if (btn != null)
            {
                var bonus = (btn.Data as GameBonus);
                PrepareInterfaceForBonus(bonus.Type);
                (arena as PlayingArena_Normal).Control_ActivateBonus(bonus);
            }
                
        }

        private void PrepareInterfaceForBonus(BonusType bonus)
        {
            switch(bonus)
            {
                case BonusType.None:
                case BonusType.TimeSlowdown:
                    {
                        btnControlLeft.Enabled = true;
                        btnControlRight.Enabled = true;
                        btnControlFall.Enabled = true;
                        btnControlRotate.Enabled = true;
                    } break;
                case BonusType.Laser:
                    {
                        btnControlRotate.Enabled = false;
                    } break;
                case BonusType.SwipeCubes:
                    {
                        btnControlRotate.Enabled = false;
                        btnControlFall.Enabled = false;
                    } break;
            }
        }

        private string TranslateBonusToText(BonusType bonus)
        {
            string result = "!err";
            switch(bonus)
            {
                //case GameBonus.TimeRewind: result = "Rew"; break;
                case BonusType.TimeSlowdown: result = "Slo"; break;
                case BonusType.Laser: result = "Lsr"; break;
                case BonusType.SwipeCubes: result = "Swp"; break;
                case BonusType.CancelLastCube: result = "Cnl"; break;
                case BonusType.CubesExplosion: result = "EXP"; break;
            }
            return result;
        }

        private void Arena_GameEnded(object sender, EventArgs e)
        {
            if (Parent != null)
                this.End();
            else
            {
                arena.StartNewGame();
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
            {
                if (arena.Score > 0)
                    lblScore.Text = Strings.ScoreToLongString(arena.Score); //Strings.ScoreToString(arena.Score, 3); 
                else
                    lblScore.Text = "Skóre";
            }

            if (lblLevel != null)
            {
                if (arena.Level > 0)
                    lblLevel.Text = arena.Level.ToString();
                else
                    lblLevel.Text = "Level";
            }
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
            arena.ControlRotate_Click();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var btn = (sender as Button);
            if (btn.HoverTime < 150)
                arena.ControlDown_Click();
        }

        private void btnDown_Hover(object sender, EventArgs e)
        {
            arena.ControlDown_Down();
        }

        private void btnDown_Release(object sender, EventArgs e)
        {
            arena.ControlDown_Up();
        }

        private void btnRight_Hover(object sender, EventArgs e)
        {
            arena.ControlRight_Down();
        }

        private void btnLeft_Hover(object sender, EventArgs e)
        {
            arena.ControlLeft_Down();
        }

        private void btnRight_Release(object sender, EventArgs e)
        {
            arena.ControlRight_Up();
        }

        private void btnLeft_Release(object sender, EventArgs e)
        {
            arena.ControlLeft_Up();
        }
    }
}