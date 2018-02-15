using System;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Android.Util;
using Microsoft.Xna.Framework;
using GalaxyBlox.Objects;
using System.Linq;
using GalaxyBlox.Static;
using static GalaxyBlox.Static.SettingOptions;
using System.Collections.Generic;

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

        GameObject lblSelectedGameMode;


        Button btnContinue;
        //GameObject highScore;

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
        }

        public MenuRoom(string name, Vector2 size, Vector2 position) : base(name, size, position)
        {
        }

        protected override void Initialize()
        {
            FullScreen = true;
            Background = Contents.Textures.BackgroundMenu;

            selectebleGameModes = availableGameModes.ToList(); // fill modes that i can select from 
            SelectedGameMode = selectebleGameModes[selectebleGameModes.FindIndex(mode => mode == Settings.Game.UserSettings.LastGameMode)];

            GameObject objToAdd;
            var padding = 30;
            var btnSize = new Vector2(Size.X - 2 * padding, 75);
            var btnTextSize = (int)(btnSize.Y * 0.45f);
            var btnPadding = 35;
            var btnCount = 4;
            var btnStartPosY = (Size.Y - ((btnCount * btnSize.Y) + ((btnCount - 1) * btnPadding))) / 2;

            //// Adding HighScore
            ////// ADDING LABEL FOR SCORE
            //highScore = new GameObject(this) // label for Score
            //{
            //    Position = new Vector2(padding, btnStartPosY - 35),
            //    Size = new Vector2(btnSize.X, 35),
            //    LayerDepth = 0.05f,
            //    Alpha = 1f,
            //    TextSpriteFont = Contents.Fonts.PlainTextFont,
            //    TextAlignment = TextAlignment.Left,
            //    TextHeight = 35,
            //    ShowText = true,
            //    TextColor = Color.White
            //};
            //Objects.Add(highScore);
            //ResetHighscoreText();

            ////// ADDING BUTTONS //////
            //// EXIT BUTTON ////
            var btnPauseSize = 100;
            objToAdd = Bank.Buttons.GetMenuButton(this);
            objToAdd.BackgroundImage = Contents.Textures.Button_exit;
            objToAdd.Size = new Vector2(btnPauseSize);
            objToAdd.Position = new Vector2(15, 15);
            (objToAdd as Button).Click += btnFinish_Click;
            Objects.Add(objToAdd);

            ////// PAUSE BUTTON ////
            //btnContinue = Bank.Buttons.GetMenuButton(this);
            //btnContinue.BackgroundImage = Contents.Textures.Button_pause;
            //btnContinue.Size = new Vector2(btnPauseSize);
            //btnContinue.Position = new Vector2(Size.X - btnPauseSize - 15, 15);
            //btnContinue.Enabled = false;
            //btnContinue.Click += btnContinue_Click;
            //Objects.Add(btnContinue);


            //// PLAY BUTTON ////
            var playButtonSize = new Vector2(280, 180);

            objToAdd = Bank.Buttons.GetMenuButton(this);
            objToAdd.BackgroundImage = Contents.Textures.Button_play;
            objToAdd.Size = playButtonSize;
            objToAdd.Position = new Vector2((Size.X - objToAdd.Size.X) / 2, Size.Y - objToAdd.Size.Y - padding);
            (objToAdd as Button).Click += btnPlayGame_Click;
            Objects.Add(objToAdd);

            var playButtonsPosition = objToAdd.Position;
            var sideButtonsSize = new Vector2(136, 112);

            //// SETTINGS BUTTON ////
            objToAdd = Bank.Buttons.GetMenuButton(this);
            objToAdd.BackgroundImage = Contents.Textures.Button_settings;
            objToAdd.Size = sideButtonsSize;
            objToAdd.Position = new Vector2((playButtonsPosition.X - sideButtonsSize.X) / 2, playButtonsPosition.Y + (playButtonSize.Y - sideButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnSettings_Click;
            Objects.Add(objToAdd);

            var offset = playButtonsPosition.X + playButtonSize.X;

            //// HIGHSCORE BUTTON ////
            objToAdd = Bank.Buttons.GetMenuButton(this);
            objToAdd.BackgroundImage = Contents.Textures.Button_highscore;
            objToAdd.Size = sideButtonsSize;
            objToAdd.Position = new Vector2(offset + (Size.X - offset - sideButtonsSize.X) / 2, playButtonsPosition.Y + (playButtonSize.Y - sideButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnHighscore_click;
            Objects.Add(objToAdd);

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
            objToAdd.Text = "- SELECT GAME -";
            Objects.Add(objToAdd);

            var arrowButtonsSize = new Vector2(84, 120);

            //// SELECT LEFT GAMEMODE BUTTON ////
            objToAdd = Bank.Buttons.GetMenuButton(this);
            objToAdd.BackgroundImage = Contents.Textures.Button_left;
            objToAdd.Size = arrowButtonsSize;
            objToAdd.Position = new Vector2(padding, lblSelectedGameMode.Position.Y + (lblSelectedGameMode.Size.Y - arrowButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnSelectLeft_Click;
            Objects.Add(objToAdd);

            //// SELECT RIGH GAMEMODE BUTTON ////
            objToAdd = Bank.Buttons.GetMenuButton(this);
            objToAdd.BackgroundImage = Contents.Textures.Button_right;
            objToAdd.Size = arrowButtonsSize;
            objToAdd.Position = new Vector2(Size.X - padding - arrowButtonsSize.X, lblSelectedGameMode.Position.Y + (lblSelectedGameMode.Size.Y - arrowButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnSelectRight_Click;
            Objects.Add(objToAdd);
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

        public override void AfterChangeEvent()
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
                SelectedGameMode = selectebleGameModes[selectebleGameModes.FindIndex(mode => mode == Settings.Game.UserSettings.LastGameMode)];
            }

            //ResetHighscoreText();
        }

        //private void ResetHighscoreText()
        //{ 
        //    if (Settings.Game.Highscores.Items.Count != 0)
        //    {
        //        var best = Settings.Game.Highscores.Items.First().Value.FirstOrDefault();
        //        highScore.Text = $"Highscore: { Utils.Strings.ScoreToLongString(best.Value) }";
        //    }
        //    else
        //    {
        //        highScore.Text = "Highscore: -----";
        //    }
        //}

        private void btnFinish_Click(object sender, EventArgs e)
        {
            RoomManager.Rooms.Clear();
            Game1.Activity.Finish();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var size = new Vector2(600, 350);
            new SettingsRoom(this, "Room_Settings", size, new Vector2((Size.X - size.X) / 2, (Size.Y - size.Y) / 2)).Show();
        }

        private void btnHighscore_click(object sender, EventArgs e)
        {
        }

        private void btnControls_Click(object sender, EventArgs e)
        {
        }

        //private void btnContinue_Click(object sender, EventArgs e)
        //{
        //    if (mainGame != null)
        //        mainGame.Show();
        //}

        private void btnPlayGame_Click(object sender, EventArgs e)
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
                        Settings.Game.UserSettings.LastGameMode = GameMode.Normal;
                        Settings.Game.ArenaSize = new Vector2(12, 20);
                        new GameRoom(this, "Room_Game", Settings.Game.WindowSize, new Vector2()).Show();
                        break;
                    }
                case GameMode.Extreme:
                    {
                        Settings.Game.UserSettings.LastGameMode = GameMode.Extreme;
                        Settings.Game.ArenaSize = new Vector2(18, 30);
                        new GameRoom(this, "Room_Game", Settings.Game.WindowSize, new Vector2()).Show();
                        break;
                    }
                case GameMode.Classic:
                    {
                        Settings.Game.UserSettings.LastGameMode = GameMode.Classic;
                        Settings.Game.ArenaSize = new Vector2(12, 20);
                        new GameRoom(this, "Room_Game", Settings.Game.WindowSize, new Vector2()).Show();
                        break;
                    }
            }
        }
    }
}