using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GalaxyBlox.Objects;
using GalaxyBlox.Static;
using static GalaxyBlox.Static.SettingOptions;
using static GalaxyBlox.Static.Settings;
using static GalaxyBlox.Static.SettingClasses;

namespace GalaxyBlox.Rooms
{
    class HighscoresRoom : Room
    {
        readonly private List<GameMode> selectebleGameModes = new List<GameMode>() { GameMode.Normal, GameMode.Extreme, GameMode.Classic };

        private SettingOptions.GameMode selectedMode;
        public GameMode SelectedMode
        {
            get { return selectedMode; }
            set
            {
                selectedMode = value;
                if (lblSelectedGameMode != null)
                    lblSelectedGameMode.Text = selectedMode.ToString().ToUpper();

                UpdateHighscoreList();
            }
        }

        Button btnOK;
        GameObject lblSelectedGameMode;
        GameObject highscoreBackground;
        List<GameObject> highscoresData;

        public HighscoresRoom(Room parent, string name, Vector2 size, Vector2 position) : base(parent, name, size, position)
        {
            Initialize();
        }

        protected override void Initialize()
        {
            DialogBackground = Contents.Sprites.Dialog_background;
            DialogIcon = Contents.Sprites.Dialog_icon_highscore;
            DialogBackgroundScale = 4;
            IsDialog = true;

            this.Size = new Vector2(600, 710);
            CenterParent();

            var margin = new { top = 129, left = 25, right = 25, bottom = 35 }; // anonymous type for margin

            var btnDialogSize = new Vector2(160, 80);
            var btnDialogTextHeight = (int)(btnDialogSize.Y * 0.3f);

            var lblSize = 45;

            //// GAMEMODE LABEL ////
            lblSelectedGameMode = new GameObject(this);
            lblSelectedGameMode.Size = new Vector2(Size.X, lblSize);
            lblSelectedGameMode.Position = new Vector2(0, margin.top);
            lblSelectedGameMode.LayerDepth = 0.04f;
            lblSelectedGameMode.Alpha = 1f;
            lblSelectedGameMode.TextSpriteFont = Contents.Fonts.PixelArtTextFont;
            lblSelectedGameMode.TextAlignment = TextAlignment.Center;
            lblSelectedGameMode.TextHeight = lblSize;
            lblSelectedGameMode.ShowText = true;
            lblSelectedGameMode.TextColor = Color.White;
            lblSelectedGameMode.Text = selectedMode.ToString().ToUpper();
            Objects.Add(lblSelectedGameMode);

            var arrowButtonsSize = new Vector2(40, 60);

            //// SELECT LEFT GAMEMODE BUTTON ////
            GameObject objToAdd = Bank.Buttons.GetPlainButtonWithText(this);
            objToAdd.SpriteImage = Contents.Sprites.Button_left_small;
            objToAdd.Size = arrowButtonsSize;
            objToAdd.Position = new Vector2(margin.left, lblSelectedGameMode.Position.Y + (lblSelectedGameMode.Size.Y - arrowButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnSelectLeft_Click;
            Objects.Add(objToAdd);

            //// SELECT RIGH GAMEMODE BUTTON ////
            objToAdd = Bank.Buttons.GetPlainButtonWithText(this);
            objToAdd.SpriteImage = Contents.Sprites.Button_right_small;
            objToAdd.Size = arrowButtonsSize;
            objToAdd.Position = new Vector2(Size.X - margin.right - arrowButtonsSize.X, lblSelectedGameMode.Position.Y + (lblSelectedGameMode.Size.Y - arrowButtonsSize.Y) / 2);
            (objToAdd as Button).Click += btnSelectRight_Click;
            Objects.Add(objToAdd);

            var itemPadding = 40;
            var highscoreBackgroundSize = new Vector2(550, 350);

            highscoreBackground = new DynamicBackgroundObject(this, Contents.Sprites.Dialog_inside, 4);
            highscoreBackground.Position = new Vector2(margin.left, lblSelectedGameMode.Position.Y + lblSelectedGameMode.Size.Y + itemPadding);
            highscoreBackground.Size = highscoreBackgroundSize;
            highscoreBackground.LayerDepth = 0.04f;
            Objects.Add(highscoreBackground);

            //GameObject obj = Bank.Visuals.GetSettingsLabel(this);
            //obj.Text = "INDICATOR:";
            //obj.Size = settingsItemSize;
            //obj.TextHeight = settingsItemTextHeight;
            //obj.Position = new Vector2(margin.left, btnIndicator.Position.Y);
            //Objects.Add(obj);

            btnOK = Bank.Buttons.GetBasicButton(this);
            btnOK.Size = btnDialogSize;
            btnOK.Position = new Vector2((Size.X - btnOK.Size.X) / 2, Size.Y - btnOK.Size.Y - margin.bottom);
            btnOK.Text = "NICE";
            btnOK.TextHeight = btnDialogTextHeight;
            btnOK.Click += BtnOK_Click;
            Objects.Add(btnOK);

            SelectedMode = Settings.UserSettings.LastGameMode;
        }

        private void UpdateHighscoreList()
        {
            if (highscoresData == null)
                highscoresData = new List<GameObject>();
            else
            {
                foreach (var data in highscoresData)
                    Objects.Remove(data);
                highscoresData.Clear();
            }

            var highscoresCount = 5;
            var mode = Settings.Highscores.Items.Where(gameMode => gameMode.Key == SelectedMode).FirstOrDefault();

            var highscores = new List<Score>();
            if (mode.Value != null)
                highscores = mode.Value;

            var highscoreMargin = 15;
            var highscoreTextSize = 25;
            var highscoreLineHeigth = (int)((highscoreBackground.Size.Y /*- 2 * highscoreMargin*/) / highscoresCount);

            for (int i = 0; i < highscoresCount; i++)
            {
                var nameText = (i + 1).ToString() + ". ---";
                var scoreText = "---";

                if (highscores.Count() > i)
                {
                    nameText = (i + 1).ToString() + ". " + highscores[i].Name;
                    scoreText = highscores[i].Value.ToString();
                }

                var posY = highscoreBackground.Position.Y + /*highscoreMargin*/ + highscoreLineHeigth * i;

                var obj = new GameObject(this);
                obj.Size = new Vector2(highscoreBackground.Size.X / 2, highscoreLineHeigth);
                obj.Position = new Vector2(highscoreBackground.Position.X + highscoreMargin, posY);
                obj.LayerDepth = 0.05f;
                obj.Alpha = 1f;
                obj.TextSpriteFont = Contents.Fonts.PixelArtTextFont;
                obj.TextAlignment = TextAlignment.Left;
                obj.TextHeight = highscoreTextSize;
                obj.ShowText = true;
                obj.TextColor = Color.White;
                obj.Text = nameText;
                Objects.Add(obj);
                highscoresData.Add(obj);

                obj = new GameObject(this);
                obj.Size = new Vector2(highscoreBackground.Size.X / 2, highscoreLineHeigth);
                obj.Position = new Vector2(highscoreBackground.Position.X + highscoreBackground.Size.X - obj.Size.X - highscoreMargin, posY);
                obj.LayerDepth = 0.05f;
                obj.Alpha = 1f;
                obj.TextSpriteFont = Contents.Fonts.PixelArtTextFont;
                obj.TextAlignment = TextAlignment.Right;
                obj.TextHeight = highscoreTextSize;
                obj.ShowText = true;
                obj.TextColor = Color.White;
                obj.Text = scoreText;
                Objects.Add(obj);
                highscoresData.Add(obj);
            }
        }

        private void btnSelectRight_Click(object sender, EventArgs e)
        {
            var index = selectebleGameModes.FindIndex(mode => mode == SelectedMode) + 1;
            if (index >= selectebleGameModes.Count)
                index = 0;

            SelectedMode = selectebleGameModes[index];
        }

        private void btnSelectLeft_Click(object sender, EventArgs e)
        {
            var index = selectebleGameModes.FindIndex(mode => mode == SelectedMode) - 1;
            if (index < 0)
                index = selectebleGameModes.Count - 1;

            SelectedMode = selectebleGameModes[index];
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            End();
        }

        private void BtnSetIndicator_Click(object sender, EventArgs e)
        {
            var newValue = Enum.GetValues(typeof(SettingOptions.GameMode)).Cast<SettingOptions.GameMode>().SkipWhile(val => val != selectedMode).Skip(1).FirstOrDefault();
            selectedMode = newValue;
        }
    }
}