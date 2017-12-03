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
        Button btnContinue;
        GameObject highScore;

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

            GameObject objToAdd;
            var padding = 30;
            var btnSize = new Vector2(Size.X - 2 * padding, 75);
            var btnPadding = 35;
            var btnCount = 4;
            var btnStartPosY = (Size.Y - ((btnCount * btnSize.Y) + ((btnCount - 1) * btnPadding))) / 2;

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
            ResetHighscoreText();

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
                objToAdd.Position = new Vector2(padding, btnStartPosY + (btnSize.Y + btnPadding));
                objToAdd.Text = "Nová hra";
            (objToAdd as Button).Click += btnNewGame_Click;
            Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetMenuButton(this);
                objToAdd.Size = btnSize;
                objToAdd.Position = new Vector2(padding, btnStartPosY + (btnSize.Y + btnPadding) * 2);
                objToAdd.Text = "Nastavení";
            (objToAdd as Button).Click += btnSettings_Click;
            Objects.Add(objToAdd);

            objToAdd = Bank.Buttons.GetMenuButton(this);
                objToAdd.Size = btnSize;
                objToAdd.Position = new Vector2(padding, btnStartPosY + (btnSize.Y + btnPadding) * 3);
                objToAdd.Text = "Konec";
            (objToAdd as Button).Click += btnFinish_Click;
            Objects.Add(objToAdd);
        }

        public override void AfterChangeEvent()
        {
            if (btnContinue == null)
                return;

            if (mainGame != null)
                btnContinue.Enabled = true;
            else
                btnContinue.Enabled = false;

            ResetHighscoreText();
        }

        private void ResetHighscoreText()
        { 
            if (Settings.Game.Highscores.Items.Count != 0)
            {
                var best = Settings.Game.Highscores.Items.First().Value.FirstOrDefault();
                highScore.Text = $"Highscore: { Utils.Strings.ScoreToLongString(best.Value) }";
            }
            else
            {
                highScore.Text = "Highscore: -----";
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            Game1.Activity.Finish();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var size = new Vector2(600, 350);
            new SettingsRoom(this, "Room_Settings", size, new Vector2((Size.X - size.X) / 2, (Size.Y - size.Y) / 2)).Show();
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

            new GameRoom(this, "Room_Game", Settings.Game.WindowSize, new Vector2()).Show();
        }
    }
}