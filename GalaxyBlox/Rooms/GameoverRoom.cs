using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using GalaxyBlox.Models;
using GalaxyBlox.Objects;
using GalaxyBlox.Static;
using static GalaxyBlox.Static.SettingOptions;
using static GalaxyBlox.Static.SettingClasses;

namespace GalaxyBlox.Rooms
{
    class GameOverRoom : Room
    {
        private long score;
        private GameMode gameMode;
        private bool isNewHighscore;
        
        Button btnOK;

        private List<GameObject> charactersList;

        public GameOverRoom(Room parent, string name, Vector2 size, Vector2 position, long highscore, GameMode gameMode, bool isNewHighscore) : base(parent, name, size, position)
        {
            this.score = highscore;
            this.gameMode = gameMode;
            this.isNewHighscore = isNewHighscore;

            Initialize();
        }

        protected override void Initialize()
        {
            DialogBackground = Contents.Sprites.Dialog_background;
            DialogIcon = Contents.Sprites.Dialog_icon_highscore;
            DialogBackgroundScale = 4;
            IsDialog = true;

            Size = isNewHighscore ? new Vector2(600, 750) : new Vector2(600, 550); 
            
            var margin = new { top = 129, left = 25, right = 25, bottom = 35 }; // anonymous type for margin
            var backgroundMargin = new { top = 30, bottom = 25 };

            var btnDialogSize = new Vector2(160, 80);
            var btnDialogTextHeight = (int)(btnDialogSize.Y * 0.3f);

            var itemPadding = 50;
            
            GameObject obj = Bank.Visuals.GetLabelCenter(this);
            obj.Size = new Vector2(Size.X, 60);
            obj.Position = new Vector2(0, margin.top + backgroundMargin.top);
            obj.Text = Constants.Texts.GameOver;
            obj.TextHeight = (int)obj.Size.Y;
            Objects.Add(obj);

            var lastObj = obj;
            obj = Bank.Visuals.GetLabelCenter(this);
            obj.Size = new Vector2(Size.X, 30);
            obj.Position = new Vector2(0, lastObj.Position.Y + lastObj.Size.Y +  itemPadding);
            obj.Text = isNewHighscore? Constants.Texts.NewHighscore : Constants.Texts.Score;
            obj.TextHeight = (int)obj.Size.Y;
            Objects.Add(obj);

            lastObj = obj;
            obj = Bank.Visuals.GetLabelCenter(this);
            obj.Size = new Vector2(Size.X, 40);
            obj.Position = new Vector2(0, lastObj.Position.Y + lastObj.Size.Y + itemPadding);
            obj.Text = Utils.Strings.ScoreToLongString(score);
            obj.TextHeight = (int)obj.Size.Y;
            Objects.Add(obj);

            lastObj = obj;

            if (isNewHighscore)
            {
                lastObj = obj;
                obj = Bank.Visuals.GetLabelCenter(this);
                obj.Size = new Vector2(Size.X, 30);
                obj.Position = new Vector2(0, lastObj.Position.Y + lastObj.Size.Y + itemPadding);
                obj.Text = Constants.Texts.SelectName;
                obj.TextHeight = (int)obj.Size.Y;
                Objects.Add(obj);

                lastObj = obj;
                var btnArrowSize = new Vector2(100, 60);
                var charItemSize = new Vector2(100, 80);
                var charItemPadding = 30;

                charactersList = new List<GameObject>();
                var maxCharacters = 3;
                var posX = (Size.X - (maxCharacters - 1) * charItemPadding - charItemSize.X * maxCharacters) / 2;
                var posY = lastObj.Position.Y + lastObj.Size.Y + itemPadding;
                var lastName = Settings.UserSettings.LastName;

                for (int i = 0; i < maxCharacters; i++)
                {
                    var arrUp = Bank.Buttons.GetPlainButtonWithText(this);
                    arrUp.SpriteImage = Contents.Sprites.Button_up_medium;
                    arrUp.Size = btnArrowSize;
                    arrUp.Position = new Vector2(posX + i * (btnArrowSize.X + charItemPadding), posY);
                    Objects.Add(arrUp);

                    var character = new GameObject(this);
                    character.Size = charItemSize;  
                    character.Position = new Vector2(posX + i * (charItemSize.X + charItemPadding), arrUp.Position.Y + arrUp.Size.Y + charItemPadding);
                    character.ShowText = true;
                    character.TextAlignment = TextAlignment.Center;
                    character.TextSpriteFont = Contents.Fonts.PixelArtTextFont;
                    character.TextHeight = (int)(charItemSize.Y * 0.8f);
                    character.Text = Settings.UseLastHighscoreName && !String.IsNullOrEmpty(lastName) && lastName.Count() > i ? lastName[i].ToString() : Constants.AvailableNameChars.First().ToString();
                    character.TextColor = Color.White;
                    character.LayerDepth = 0.05f;
                    Objects.Add(character);
                    charactersList.Add(character);

                    var arrDown = Bank.Buttons.GetPlainButtonWithText(this);
                    arrDown.SpriteImage = Contents.Sprites.Button_down_medium;
                    arrDown.Size = btnArrowSize;
                    arrDown.Position = new Vector2(posX + i * (btnArrowSize.X + charItemPadding), character.Position.Y + character.Size.Y + charItemPadding);
                    Objects.Add(arrDown);

                    arrDown.Click += delegate
                    {
                        var c = character.Text.First();
                        var selection = Constants.AvailableNameChars;
                        var index = selection.IndexOf(c, 0);
                        index++;
                        if (index >= selection.Count())
                            index = 0;

                        character.Text = selection[index].ToString();
                        RefreshOkButton();
                    };
                    arrUp.Click += delegate 
                    {
                        var c = character.Text.First();
                        var selection = Constants.AvailableNameChars;
                        var index = selection.IndexOf(c, 0);
                        index--;
                        if (index < 0)
                            index = selection.Count() - 1;

                        character.Text = selection[index].ToString();
                        RefreshOkButton();
                    };

                    lastObj = arrDown;
                }
            }

            var btnPadding = 30;

            Size = new Vector2(Size.X, lastObj.Position.Y + lastObj.Size.Y + btnDialogSize.Y + 2 * btnPadding + backgroundMargin.bottom);
            
            btnOK = Bank.Buttons.GetBasicButton(this);
            btnOK.Size = btnDialogSize;
            btnOK.Position = new Vector2((Size.X - btnOK.Size.X) / 2, Size.Y - btnOK.Size.Y - btnPadding);
            btnOK.Text = isNewHighscore ? Constants.Texts.CheekyOk : Constants.Texts.CheekyCancel;
            btnOK.TextHeight = btnDialogTextHeight;
            btnOK.Click += BtnOK_Click;
            Objects.Add(btnOK);

            var highscoreBackgroundSize = new Vector2(Size.X - margin.left - margin.right, Size.Y - margin.top - (Size.Y - btnOK.Position.Y)- btnPadding);

            obj =  new DynamicBackgroundObject(this, Contents.Sprites.Dialog_inside, 4);
            obj.Position = new Vector2(margin.left, margin.top);
            obj.Size = highscoreBackgroundSize;
            obj.LayerDepth = 0.04f;
            Objects.Add(obj);

            RefreshOkButton();
            CenterParent();
        }

        private void RefreshOkButton()
        {
            if (charactersList == null)
                return;

            if (charactersList.Any(c => c.Text == Constants.AvailableNameChars.First().ToString()))
                btnOK.Enabled = false;
            else
                btnOK.Enabled = true;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (isNewHighscore)
            {
                var name = String.Join("", charactersList.Select(character => character.Text));
                Settings.UserSettings.LastName = name;

                var highscores = Settings.Highscores.Items.ContainsKey(gameMode) ? Settings.Highscores.Items[gameMode] : new List<Score>();
                highscores.Add(new Score(name, score));

                highscores = highscores.OrderByDescending(scr => scr.Value).ToList();
                while (highscores.Count > Settings.MaxHighscoresPerGameMod)
                {
                    highscores.RemoveAt(highscores.Count - 1);
                }

                if (Settings.Highscores.Items.ContainsKey(gameMode))
                    Settings.Highscores.Items.Remove(gameMode);
                Settings.Highscores.Items.Add(gameMode, highscores);
                Settings.SaveAll();
            }

            End();
        }
    }
}