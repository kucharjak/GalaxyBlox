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
            Initialize();
        }

        public GameRoom(string name, Vector2 size, Vector2 position) : base(name, size, position)
        {
            Initialize();
        }

        protected override void Initialize()
        {
            gameMode = Settings.Game.UserSettings.LastGameMode;
            FullScreen = true;
            Background = Contents.Textures.BackgroundGame;

            GameObject lastObj;

            var playingArenaPadding = 5;

            // ADDING FIRST PANEL
            // PANEL BACKGROUND
            Objects.Add(new GameObject(this)
            {
                Size = new Vector2(712, 140),
                Position = new Vector2(4, 4),
                BackgroundImage = Contents.Textures.GameUI_top_background,
                LayerDepth = 0.01f
            });
            lastObj = Objects.Last();
            var playingArenaStart = lastObj.Position.Y + lastObj.Size.Y + 5;

            // ACTOR VIEWER
            var viewerPos = new Vector2(lastObj.Position.X + 24, lastObj.Position.Y + 20);
            var viewerSize = new Vector2(100);
            Objects.Add(new DynamicBackgroundObject(this, Contents.Textures.Dialog_inside, 4)
            {
                Size = viewerSize,
                Position = viewerPos,
                LayerDepth = 0.045f
            });
            actorViewer = new ActorViewer(this, viewerSize, Contents.Colors.ActorViewerBackgroundColor)
            {
                Position = viewerPos,
                LayerDepth = 0.05f
            };
            Objects.Add(actorViewer);

            var btnPauseSize = 100;
            // PAUSE BUTTON
            btnPause = Bank.Buttons.GetPauseButton(this);
            btnPause.Size = new Vector2(btnPauseSize);
            btnPause.Position = new Vector2(lastObj.Position.X + lastObj.Size.X - btnPauseSize - 24, lastObj.Position.Y + 20);
            btnPause.Click += btnPause_Click;
            Objects.Add(btnPause);
            
            // SETTINGS 
            var lblScoreSize = new Vector2(416, 40);
            var lblLevelSize = new Vector2(416, 24);
            var scoreLineSize = new Vector2(284, 4);

            // SCORE
            lblScore = new GameObject(this)
            {
                Size = lblScoreSize,
                Position = new Vector2(148, lastObj.Position.Y + 28),
                TextSpriteFont = Contents.Fonts.PixelArtTextFont,
                Text = "SCORE",
                TextHeight = (int)lblScoreSize.Y,
                TextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                ShowText = true,
                LayerDepth = 0.05f
            };
            Objects.Add(lblScore);
            
            // LINE BETWEEN SCORE AND LEVEL
            Objects.Add(new GameObject(this)
            {
                BackgroundImage = Contents.Textures.Pix,
                BaseColor = Color.White,
                Size = scoreLineSize,
                Position = new Vector2(148 + (416 - scoreLineSize.X) / 2, lastObj.Position.Y + 76),
                LayerDepth = 0.05f
            });
            
            // LEVEL
            lblLevel = new GameObject(this)
            {
                Size = lblLevelSize,
                Position = new Vector2(148, lastObj.Position.Y + 88),
                TextSpriteFont = Contents.Fonts.PixelArtTextFont,
                Text = "LEVEL",
                TextHeight = (int)lblLevelSize.Y,
                TextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                ShowText = true,
                LayerDepth = 0.05f
            };
            Objects.Add(lblLevel);

            // ADDING SECOND PANEL
            // SETTINGS
            var btnSize = 136;
            var btnPadding = 10;

            // PANEL BACKGROUND
            float playingArenaEndY = 0;
            float controlButtonsStartY = 0;
            if (gameMode == SettingOptions.GameMode.Classic)
            {
                Objects.Add(new GameObject(this)
                {
                    Size = new Vector2(712, 184),
                    Position = new Vector2(4, Size.Y - 184 - 4),
                    BackgroundImage = Contents.Textures.GameUI_bottom_classic_background,
                    LayerDepth = 0.01f
                });
                lastObj = Objects.Last();
                playingArenaEndY = lastObj.Position.Y - playingArenaPadding;
                controlButtonsStartY = lastObj.Position.Y + 24;
            }
            else
            {
                Objects.Add(new GameObject(this)
                {
                    Size = new Vector2(712, 300),
                    Position = new Vector2(4, Size.Y - 300 - 4),
                    BackgroundImage = Contents.Textures.GameUI_bottom_normal_background,
                    LayerDepth = 0.01f
                });
                lastObj = Objects.Last();
                playingArenaEndY = lastObj.Position.Y - playingArenaPadding;
                controlButtonsStartY = lastObj.Position.Y + 140;

                // ADDING BONUS PANEL
                pnlBonusBtns = new GameObject(this)
                {
                    Size = new Vector2(672, 84),
                    Position = new Vector2(lastObj.Position.X + 20, lastObj.Position.Y + 20),
                    LayerDepth = 0.02f
                };
                Objects.Add(pnlBonusBtns);
                bonusButtons = new List<Button>();
            }

            // CONTROL BUTTON LEFT
            btnControlLeft = Bank.Buttons.GetControlButton(this);
            btnControlLeft.BackgroundImage = Contents.Textures.ControlButton_left;
            btnControlLeft.Size = new Vector2(btnSize);
            btnControlLeft.Position = new Vector2(28, controlButtonsStartY);
            btnControlLeft.Release += btnLeft_Release;
            btnControlLeft.Hover += btnLeft_Hover;
            Objects.Add(btnControlLeft);

            // CONTROL BUTTON FALL
            btnControlFall = Bank.Buttons.GetControlButton(this);
            btnControlFall.BackgroundImage = Contents.Textures.ControlButton_fall;
            btnControlFall.Size = new Vector2(btnSize);
            btnControlFall.Position = new Vector2(204, controlButtonsStartY);
            btnControlFall.Click += btnDown_Click;
            btnControlFall.Hover += btnDown_Hover;
            btnControlFall.Release += btnDown_Release;
            Objects.Add(btnControlFall);

            // CONTROL BUTTON ROTATE
            btnControlRotate = Bank.Buttons.GetControlButton(this);
            btnControlRotate.BackgroundImage = Contents.Textures.ControlButton_rotate;
            btnControlRotate.Size = new Vector2(btnSize);
            btnControlRotate.Position = new Vector2(380, controlButtonsStartY);
            btnControlRotate.Click += btnRotate_Click;
            Objects.Add(btnControlRotate);

            // CONTROL BUTTON RIGHT
            btnControlRight = Bank.Buttons.GetControlButton(this);
            btnControlRight.BackgroundImage = Contents.Textures.ControlButton_right;
            btnControlRight.Size = new Vector2(btnSize);
            btnControlRight.Position = new Vector2(556, controlButtonsStartY);
            btnControlRight.Release += btnRight_Release;
            btnControlRight.Hover += btnRight_Hover;
            Objects.Add(btnControlRight);

            // ADDING PLAYING ARENA
            switch (gameMode)
            {
                case GameMode.Classic:
                    {
                        arena = new PlayingArena_Classic(this, new Vector2(Size.X - playingArenaPadding, playingArenaEndY - playingArenaStart), new Vector2(playingArenaPadding, playingArenaStart));
                    } break;
                case GameMode.Normal:
                    {
                        arena = new PlayingArena_Normal(this, new Vector2(Size.X - playingArenaPadding, playingArenaEndY - playingArenaStart), new Vector2(playingArenaPadding, playingArenaStart));
                        (arena as PlayingArena_Normal).AvailableBonusesChanged += Arena_AvailableBonusesChanged;
                        (arena as PlayingArena_Normal).ActiveBonusChanged += Arena_ActiveBonusChanged;
                    } break;
                case GameMode.Extreme:
                    {
                        arena = new PlayingArena_Extreme(this, new Vector2(Size.X - playingArenaPadding, playingArenaEndY - playingArenaStart), new Vector2(playingArenaPadding, playingArenaStart));
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
                    BackgroundImage = Contents.Textures.Pix,
                    BaseColor = Color.Red,
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
                    lblScore.Text = "SCORE";
            }

            if (lblLevel != null)
            {
                if (arena.Level > 0)
                    lblLevel.Text = arena.Level.ToString();
                else
                    lblLevel.Text = "LEVEL";
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