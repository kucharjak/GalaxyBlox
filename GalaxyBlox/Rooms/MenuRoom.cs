
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GalaxyBlox.Models;
using GalaxyBlox.Objects;
using GalaxyBlox.Static;
using static GalaxyBlox.Static.SettingOptions;

namespace GalaxyBlox.Rooms
{
    class MenuRoom : Room
    {
        readonly private List<GameMode> availableGameModes = new List<GameMode>() { GameMode.Normal, GameMode.Extreme, GameMode.Classic };

        private List<GameMode> selectebleGameModes;
        private GameMode selectedGameMode;
        public GameMode SelectedGameMode
        {
            get { return selectedGameMode; }
            set
            {
                selectedGameMode = value;
                if (lblSelectedGameMode != null)
                    lblSelectedGameMode.Text = selectedGameMode.ToString().ToUpper();
            }
        }

        private GameObject lblSelectedGameMode;
        private BreathingObject tapToStart;
        private Button btnTapToStart;
        private ObjectHider hider;

        /// <summary>
        /// Waiting time to hide menu in milliseconds. Set for 1m 30s.
        /// </summary>
        private const int hideWaitingTime = 90000;
        private int hideTimer = hideWaitingTime;

        GameRoom mainGame
        {
            get
            {
                var rooms = RoomManager.Rooms.Where(rm => rm.Name == "Room_Game");
                if (rooms.Count() == 0)
                    return null;
                else
                    return (GameRoom)rooms.First();
            }
        }

        public MenuRoom(Room parent, string name, Vector2 size, Vector2 position) : base(parent, name, size, position)
        {
            Initialize();
        }

        public MenuRoom(string name, Vector2 size, Vector2 position) : base(name, size, position)
        {
            Initialize();
        }

        protected override void Initialize()
        {
            FullScreen = true;
            Background = Contents.Sprites.Game_Background;

            selectebleGameModes = availableGameModes.ToList(); // fill modes that i can select from 
            SelectedGameMode = selectebleGameModes[selectebleGameModes.FindIndex(mode => mode ==  GameMode.Normal)];

            GameObject objToAdd;
            var padding = 30;
            var btnSize = new Vector2(Size.X - 2 * padding, 75);
            var btnTextSize = (int)(btnSize.Y * 0.45f);
            var btnPadding = 35;
            var btnCount = 4;
            var btnStartPosY = (Size.Y - ((btnCount * btnSize.Y) + ((btnCount - 1) * btnPadding))) / 2;

            hider = new ObjectHider(this)
            {
                HideTimePeriod = 500
            };
            Objects.Add(hider);

            ////// ADDING BUTTONS //////
            //// EXIT BUTTON ////
            //var btnPauseSize = 100;
            //objToAdd = Bank.Buttons.GetPlainButtonWithText(this);
            //objToAdd.SpriteImage = Contents.Sprites.Button_exit;
            //objToAdd.Size = new Vector2(btnPauseSize);
            //objToAdd.Position = new Vector2(15, 15);
            //(objToAdd as Button).Click += btnFinish_Click;
            //Objects.Add(objToAdd);
            //hider.HideObject(objToAdd, HidePlace.Top);

            var nameSize = new Vector2(Size.X - 15, 25);
            objToAdd = new GameObject(this)
            {
                Size = nameSize,
                Position = new Vector2(0, 15),
                TextSpriteFont = Contents.Fonts.PixelArtTextFont,
                Text = Constants.Texts.Creator,
                TextColor = Color.White,
                TextHeight = (int)(nameSize.Y * 0.8f),
                TextAlignment = TextAlignment.Right,
                ShowText = true,
                LayerDepth = 0.05f,
            };
            Objects.Add(objToAdd);
            hider.HideObject(objToAdd, HidePlace.Top);

            //// PLAY BUTTON ////
            var playButtonSize = new Vector2(280, 180);

            objToAdd = Bank.Buttons.GetPlainButtonWithText(this);
            objToAdd.SpriteImage = Contents.Sprites.Button_play;
            objToAdd.Size = playButtonSize;
            objToAdd.Position = new Vector2((Size.X - objToAdd.Size.X) / 2, Size.Y - objToAdd.Size.Y - padding);
            (objToAdd as Button).Click += btnPlayGame_Click;
            Objects.Add(objToAdd);
            hider.HideObject(objToAdd, HidePlace.Bottom);

            var playButtonsPosition = objToAdd.Position;
            var sideButtonsSize = new Vector2(136, 112);

            //// SETTINGS BUTTON ////
            objToAdd = Bank.Buttons.GetPlainButtonWithText(this);
            objToAdd.SpriteImage = Contents.Sprites.Button_settings;
            objToAdd.Size = sideButtonsSize;
            objToAdd.Position = new Vector2((playButtonsPosition.X - sideButtonsSize.X) / 2, playButtonsPosition.Y + (playButtonSize.Y - sideButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnSettings_Click;
            Objects.Add(objToAdd);
            hider.HideObject(objToAdd, HidePlace.Left);

            var offset = playButtonsPosition.X + playButtonSize.X;

            //// HIGHSCORE BUTTON ////
            objToAdd = Bank.Buttons.GetPlainButtonWithText(this);
            objToAdd.SpriteImage = Contents.Sprites.Button_highscore;
            objToAdd.Size = sideButtonsSize;
            objToAdd.Position = new Vector2(offset + (Size.X - offset - sideButtonsSize.X) / 2, playButtonsPosition.Y + (playButtonSize.Y - sideButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnHighscore_click;
            Objects.Add(objToAdd);
            hider.HideObject(objToAdd, HidePlace.Right);

            var gamemodeTextSize = 45;

            //// GAMEMODE LABEL ////
            lblSelectedGameMode = new GameObject(this);
            lblSelectedGameMode.Size = new Vector2(Size.X, gamemodeTextSize);
            lblSelectedGameMode.Position = new Vector2(0, playButtonsPosition.Y - 4 * padding - lblSelectedGameMode.Size.Y);
            lblSelectedGameMode.LayerDepth = 0.04f;
            lblSelectedGameMode.Alpha = 1f;
            lblSelectedGameMode.TextSpriteFont = Contents.Fonts.PixelArtTextFont;
            lblSelectedGameMode.TextAlignment = TextAlignment.Center;
            lblSelectedGameMode.TextHeight = gamemodeTextSize;
            lblSelectedGameMode.ShowText = true;
            lblSelectedGameMode.TextColor = Color.White;
            lblSelectedGameMode.Text = selectedGameMode.ToString().ToUpper();
            Objects.Add(lblSelectedGameMode);
            hider.HideObject(lblSelectedGameMode, HidePlace.Bottom);

            var modeLabelTextSize = 23;

            //// MODE LABEL ////
            objToAdd = new GameObject(this);
            objToAdd.Size = new Vector2(Size.X, modeLabelTextSize);
            objToAdd.Position = new Vector2(0, lblSelectedGameMode.Position.Y - 2 * padding - objToAdd.Size.Y);
            objToAdd.LayerDepth = 0.04f;
            objToAdd.Alpha = 1f;
            objToAdd.TextSpriteFont = Contents.Fonts.PixelArtTextFont;
            objToAdd.TextAlignment = TextAlignment.Center;
            objToAdd.TextHeight = modeLabelTextSize;
            objToAdd.ShowText = true;
            objToAdd.TextColor = Color.White;
            objToAdd.Text = Constants.Texts.SelectGame;
            Objects.Add(objToAdd);
            hider.HideObject(objToAdd, HidePlace.Bottom);

            var arrowButtonsSize = new Vector2(60, 112);

            //// SELECT LEFT GAMEMODE BUTTON ////
            objToAdd = Bank.Buttons.GetPlainButtonWithText(this);
            objToAdd.SpriteImage = Contents.Sprites.Button_left;
            objToAdd.Size = arrowButtonsSize;
            objToAdd.Position = new Vector2(padding, lblSelectedGameMode.Position.Y + (lblSelectedGameMode.Size.Y - arrowButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnSelectLeft_Click;
            Objects.Add(objToAdd);
            hider.HideObject(objToAdd, HidePlace.Left);

            //// SELECT RIGHT GAMEMODE BUTTON ////
            objToAdd = Bank.Buttons.GetPlainButtonWithText(this);
            objToAdd.SpriteImage = Contents.Sprites.Button_right;
            objToAdd.Size = arrowButtonsSize;
            objToAdd.Position = new Vector2(Size.X - padding - arrowButtonsSize.X, lblSelectedGameMode.Position.Y + (lblSelectedGameMode.Size.Y - arrowButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnSelectRight_Click;
            Objects.Add(objToAdd);
            hider.HideObject(objToAdd, HidePlace.Right);

            //// ADDING STAR SYSTEM ////
            //objToAdd = new StarSystem(this, new Vector2(720, 664), new Vector2(0, 40));
            objToAdd = new StarSystem(this, new Vector2(680, 1200), new Vector2(20, 40));
            objToAdd.LayerDepth = 0.039f;
            (objToAdd as StarSystem).Start(218884, 1, 3, 3, 5, 15);
            Objects.Add(objToAdd);

            //// ADDING LOGO ////
            objToAdd = new BreathingObject(this)
            {
                SpriteImage = Contents.Sprites.Logo,
                Position = new Vector2(0, 140),
                Size = new Vector2(720, 256),
                LayerDepth = 0.05f,
                MaxScale = 1f,
                MinScale = 0.9f,
                TimerLimit = 5000,
                Timer = 2500
            };
            Objects.Add(objToAdd);
            hider.HideObject(objToAdd, HidePlace.Top);

            hider.Hide(false);
            hider.AllHidden += Hider_AllHidden;

            //// ADDING TAP START BUTTON ////
            btnTapToStart = Bank.Buttons.GetPlainButtonWithText(this);
            btnTapToStart.Position = new Vector2(0, 0);
            btnTapToStart.Size = this.Size;
            btnTapToStart.Alpha = 0f;
            btnTapToStart.Click += TapStartButton_Click;
            btnTapToStart.Hover += TapStartButton_Click;
            Objects.Add(btnTapToStart);

            tapToStart = new BreathingObject(this)
            {
                Position = new Vector2(0, Size.Y / 2),
                Size = new Vector2(Size.X, Size.Y / 2),
                TextSpriteFont = Contents.Fonts.PixelArtTextFont,
                Text = Constants.Texts.TapToStart,
                TextColor = new Color(252, 239, 0),
                TextHeight = 50,
                TextAlignment = TextAlignment.Center,
                ShowText = true,
                LayerDepth = 0.05f,
                MaxScale = 1f,
                MinScale = 0.8f,
                MaxAlpha = 1f,
                MinAlpha = 0.4f,
                TimerLimit = 4000,
                Timer = 2000,
            };
            Objects.Add(tapToStart);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (hideTimer < hideWaitingTime)
            {
                hideTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (hideTimer >= hideWaitingTime)
                    hider.Hide(true);
            }
        }

        private void Hider_AllHidden(object sender, EventArgs e)
        {
            if (hideTimer >= hideWaitingTime)
            {
                tapToStart.IsPaused = false;
                tapToStart.Alpha = 1f;
            }
            else
                PlayGame();
        }

        private void PlayGame()
        {
            if (selectedGameMode == GameMode.Continue && mainGame != null)
            {
                mainGame.Show();
                return;
            }

            if (mainGame != null)
                mainGame.End();

            switch (selectedGameMode)
            {
                case GameMode.Normal:
                    {
                        Settings.UserSettings.LastGameMode = GameMode.Normal;
                        Settings.ArenaSize = new Vector2(12, 20);
                        new GameRoom(this, "Room_Game", Settings.WindowSize, new Vector2()).Show();
                        break;
                    }
                case GameMode.Extreme:
                    {
                        Settings.UserSettings.LastGameMode = GameMode.Extreme;
                        Settings.ArenaSize = new Vector2(18, 30);
                        new GameRoom(this, "Room_Game", Settings.WindowSize, new Vector2()).Show();
                        break;
                    }
                case GameMode.Classic:
                    {
                        Settings.UserSettings.LastGameMode = GameMode.Classic;
                        Settings.ArenaSize = new Vector2(12, 20);
                        new GameRoom(this, "Room_Game", Settings.WindowSize, new Vector2()).Show();
                        break;
                    }
            }

            Settings.SaveUserSettings();
        }

        private void TapStartButton_Click(object sender, EventArgs e)
        {
            if (hider != null && hider.IsAllHidden)
                hider.Show(true);

            tapToStart.IsPaused = true;
            tapToStart.Alpha = 0f;

            hideTimer = 0;
        }

        private void btnSelectRight_Click(object sender, EventArgs e)
        {
            var index = selectebleGameModes.FindIndex(mode => mode == SelectedGameMode) + 1;
            if (index >= selectebleGameModes.Count)
                index = 0;

            SelectedGameMode = selectebleGameModes[index];
        }

        private void btnSelectLeft_Click(object sender, EventArgs e)
        {
            var index = selectebleGameModes.FindIndex(mode => mode == SelectedGameMode) - 1;
            if (index < 0)
                index = selectebleGameModes.Count - 1;

            SelectedGameMode = selectebleGameModes[index];
        }

        public override void AfterChangeEvent(Room previousRoom)
        {
            if (previousRoom == null)
                return;

            if (previousRoom.Name == "Room_Game")
            {
                if (mainGame != null)
                { //Continue
                    selectebleGameModes = new List<GameMode>() { GameMode.Continue };
                    selectebleGameModes.AddRange(availableGameModes);
                    SelectedGameMode = selectebleGameModes.First();
                }
                else
                {
                    selectebleGameModes = availableGameModes.ToList();
                    SelectedGameMode = selectebleGameModes[selectebleGameModes.FindIndex(mode => mode == Settings.UserSettings.LastGameMode)];
                }

                hider.Show(true);
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            new CloseGameRoom(this, "Room_CloseDialog", Vector2.Zero, Vector2.Zero).Show();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            new SettingsRoom(this, "Room_Settings", Vector2.Zero, Vector2.Zero).Show();
        }

        private void btnHighscore_click(object sender, EventArgs e)
        {
            new HighscoresRoom(this, "Room_Highscores", Vector2.Zero, Vector2.Zero).Show();
        }

        private void btnControls_Click(object sender, EventArgs e)
        {
        }

        private void btnPlayGame_Click(object sender, EventArgs e)
        {
            //var star = Objects.Where(obj => (obj as StarSystem) != null).First();
            //(star as StarSystem).Start(Game1.Random.Next(0, 2000000), 1, 3, 3, 5, 15);
            //return;

            hider.HideTimePeriod = 350;
            hider.Hide(true);
        }
    }
}