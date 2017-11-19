using System;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Android.Util;
using Microsoft.Xna.Framework;
using GalaxyBlox.Objects;
using System.Linq;
using GalaxyBlox.Static;

namespace GalaxyBlox.Rooms
{
    class MenuRoom : Room
    {
        GameRoom mainGame = null;
        Button btnContinue;
        GameObject highScore;

        public MenuRoom(Room parent, string name, Size size, Vector2 position) : base(parent, name, size, position)
        {
        }

        public MenuRoom(string name, Size size, Vector2 position) : base(name, size, position)
        {
        }

        protected override void Initialize()
        {
            FullScreen = true;
            Background = Contents.Textures.BackgroundMenu;

            GameObject objToAdd;
            var padding = 30;
            var btnSize = new Vector2(Size.Width - 2 * padding, 50);
            var btnCount = 4;
            var btnStartPosY = (Size.Height - (50 * btnCount + 10 * (btnCount - 1))) / 2;

            // Adding HighScore
            //// ADDING LABEL FOR SCORE
            highScore = new GameObject(this) // label for Score
            {
                Position = new Vector2(padding, btnStartPosY - 35),
                Size = new Vector2(btnSize.X, 35),
                LayerDepth = 0.05f,
                Alpha = 1f,
                TextAlignment = TextAlignment.Left,
                TextSpriteFont = Contents.Fonts.MenuButtonText,
                ShowText = true,
                TextColor = Color.White
            };
            Objects.Add(highScore);
            ResetHighScoreText();

            ////// ADDING BUTTONS //////
            btnContinue = Bank.Buttons.GetMenuButton(this);
                btnContinue.Size = btnSize;
                btnContinue.Position = new Vector2(padding, btnStartPosY);
                btnContinue.Text = "Pokračovat";
                btnContinue.Enabled = false;
            btnContinue.Click += btnContinue_Click;
            Objects.Add(btnContinue);

            objToAdd = Bank.Buttons.GetMenuButton(this);
                objToAdd.Size = btnSize;
                objToAdd.Position = new Vector2(padding, btnStartPosY + 65);
                objToAdd.Text = "Nová hra";
            (objToAdd as Button).Click += btnNewGame_Click;
            Objects.Add(objToAdd);

            //objToAdd = Bank.Buttons.GetMenuButton(this);
            //    objToAdd.Size = btnSize;
            //    objToAdd.Position = new Vector2(padding, btnStartPosY + 65 * 2);
            //    objToAdd.Text = "Ovládání";
            //(objToAdd as Button).Click += btnControls_Click;
            //Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetMenuButton(this);
                objToAdd.Size = btnSize;
                objToAdd.Position = new Vector2(padding, btnStartPosY + 65 * 2);
                objToAdd.Text = "Nastavení";
            (objToAdd as Button).Click += btnSettings_Click;
            Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetMenuButton(this);
                objToAdd.Size = btnSize;
                objToAdd.Position = new Vector2(padding, btnStartPosY + 65 * 3);
                objToAdd.Text = "Konec";
            (objToAdd as Button).Click += btnFinish_Click;
            Objects.Add(objToAdd);
        }

        public override void AfterChangeEvent()
        {
            if (mainGame != null)
                btnContinue.Enabled = true;
            ResetHighScoreText();
        }

        private void ResetHighScoreText()
        { 
            if (Settings.Game.User.HighScores.Count != 0)
            {
                var best = Settings.Game.User.HighScores.First().Value.FirstOrDefault();
                highScore.Text = $"Highscore: { best.ToString() }";
            }
            else
            {
                highScore.Text = "Highscore: 0";
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            Game1.Activity.Finish();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var size = new Size(400, 300);
            var settingsRoom = new SettingsRoom(this, "Settings_Room", size, new Vector2((Size.Width - size.Width) / 2, (Size.Height - size.Height) / 2));
            settingsRoom.Show();
        }

        private void btnControls_Click(object sender, EventArgs e)
        {
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (mainGame != null)
                mainGame.Show();
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            if (mainGame != null)
                mainGame.End();

            mainGame = new GameRoom(this, "Room_Game", Settings.Game.WindowSize, new Vector2());
            mainGame.Show();
        }
    }
}