using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using GalaxyBlox.Models;
using Android.Util;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GalaxyBlox.Objects;
using GalaxyBlox.Static;

namespace GalaxyBlox.Rooms
{
    class GameOverRoom : Room
    {
        private long highscore;
        private bool isNewHighscore;
        
        Button btnOK;

        private List<GameObject> charactersList;

        public GameOverRoom(Room parent, string name, Vector2 size, Vector2 position, long highscore, bool isNewHighscore) : base(parent, name, size, position)
        {
            this.highscore = highscore;
            this.isNewHighscore = isNewHighscore;
        }

        protected override void Initialize()
        {
            DialogBackground = Contents.Textures.Dialog_background;
            DialogIcon = Contents.Textures.Dialog_icon_settings;
            IsDialog = true;

            Size = isNewHighscore ? new Vector2(600, 750) : new Vector2(600, 550); 

            //var itemsHeight = 45;
            //var itemsTextHeight = itemsHeight;
            
            var margin = new { top = 129, left = 25, right = 25, bottom = 35 }; // anonymous type for margin
            var backgroundMargin = new { top = 30, bottom = 25 };

            var btnDialogSize = new Vector2(160, 80);
            var btnDialogTextHeight = (int)(btnDialogSize.Y * 0.3f);

            var itemPadding = 50;
            
            GameObject obj = Bank.Visuals.GetGameOverLabel(this);
            obj.Size = new Vector2(Size.X, 60);
            obj.Position = new Vector2(0, margin.top + backgroundMargin.top);
            obj.Text = "GAME OVER";
            obj.TextHeight = (int)obj.Size.Y;
            Objects.Add(obj);

            var lastObj = obj;
            obj = Bank.Visuals.GetGameOverLabel(this);
            obj.Size = new Vector2(Size.X, 30);
            obj.Position = new Vector2(0, lastObj.Position.Y + lastObj.Size.Y +  itemPadding);
            obj.Text = isNewHighscore? "HIGHSCORE!" : "SCORE";
            obj.TextHeight = (int)obj.Size.Y;
            Objects.Add(obj);

            lastObj = obj;
            obj = Bank.Visuals.GetGameOverLabel(this);
            obj.Size = new Vector2(Size.X, 40);
            obj.Position = new Vector2(0, lastObj.Position.Y + lastObj.Size.Y + itemPadding);
            obj.Text = Utils.Strings.ScoreToLongString(highscore);
            obj.TextHeight = (int)obj.Size.Y;
            Objects.Add(obj);

            lastObj = obj;

            if (isNewHighscore)
            {
                lastObj = obj;
                obj = Bank.Visuals.GetGameOverLabel(this);
                obj.Size = new Vector2(Size.X, 30);
                obj.Position = new Vector2(0, lastObj.Position.Y + lastObj.Size.Y + itemPadding);
                obj.Text = "SELECT NAME";
                obj.TextHeight = (int)obj.Size.Y;
                Objects.Add(obj);

                lastObj = obj;
                var btnArrowSize = new Vector2(60, 40);
                var charItemSize = new Vector2(60, 60);
                var charItemPadding = 20;

                charactersList = new List<GameObject>();
                var maxCharacters = 4;
                var posX = (Size.X - (maxCharacters - 1) * charItemPadding - charItemSize.X * maxCharacters) / 2;
                var posY = lastObj.Position.Y + lastObj.Size.Y + itemPadding;
                
                for (int i = 0; i < maxCharacters; i++)
                {
                    var arrUp = Bank.Buttons.GetMenuButton(this);
                    arrUp.BackgroundImage = Contents.Textures.Button_up_small;
                    arrUp.Size = btnArrowSize;
                    arrUp.Position = new Vector2(posX + i * (btnArrowSize.X + charItemPadding), posY);
                    Objects.Add(arrUp);

                    var character = new GameObject(this);
                    character.Size = charItemSize;
                    character.Position = new Vector2(posX + i * (charItemSize.X + charItemPadding), arrUp.Position.Y + arrUp.Size.Y + charItemPadding);
                    character.ShowText = true;
                    character.TextAlignment = Models.TextAlignment.Center;
                    character.TextSpriteFont = Contents.Fonts.PixelArtTextFont;
                    character.TextHeight = (int)(charItemSize.Y * 0.8f);
                    character.Text = Contents.Constants.AvailableNameChars.First().ToString();
                    character.TextColor = Color.White;
                    character.BackgroundImage = Contents.Textures.Pix;
                    character.BaseColor = Color.Red;
                    character.LayerDepth = 0.05f;
                    Objects.Add(character);
                    charactersList.Add(character);

                    var arrDown = Bank.Buttons.GetMenuButton(this);
                    arrDown.BackgroundImage = Contents.Textures.Button_down_small;
                    arrDown.Size = btnArrowSize;
                    arrDown.Position = new Vector2(posX + i * (btnArrowSize.X + charItemPadding), character.Position.Y + character.Size.Y + charItemPadding);
                    Objects.Add(arrDown);

                    arrUp.Click += delegate 
                    {
                        var c = character.Text.First();
                        var selection = Contents.Constants.AvailableNameChars;
                        var index = selection.IndexOf(c, 0);
                        index++;
                        if (index >= selection.Count())
                            index = 0;

                        character.Text = selection.ToString();
                        RefreshOkButton();
                    };
                    arrDown.Click += delegate 
                    {
                        var c = character.Text.First();
                        var selection = Contents.Constants.AvailableNameChars;
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
            
            btnOK = Bank.Buttons.GetEmptyButton(this);
            btnOK.Size = btnDialogSize;
            btnOK.Position = new Vector2((Size.X - btnOK.Size.X) / 2, Size.Y - btnOK.Size.Y - btnPadding);
            btnOK.Text = isNewHighscore ? "NICE" : "OK";
            btnOK.TextHeight = btnDialogTextHeight;
            btnOK.Click += BtnOK_Click;
            Objects.Add(btnOK);

            var highscoreBackgroundSize = new Vector2(Size.X - margin.left - margin.right, Size.Y - margin.top - (Size.Y - btnOK.Position.Y)- btnPadding);

            obj =  new DynamicBackgroundObject(this, Contents.Textures.Dialog_inside);
            obj.Position = new Vector2(margin.left, margin.top);
            obj.Size = highscoreBackgroundSize;
            obj.LayerDepth = 0.04f;
            Objects.Add(obj);

            //posY += (int)obj.Size.Y + margin;
            //score = Bank.Visuals.GetGameOverLabel(this);
            //score.TextHeight = itemsTextHeight;
            //score.Size = new Vector2(Size.X, itemsHeight);
            //score.Position = new Vector2(0, posY);
            //Objects.Add(score);

            RefreshOkButton();
            CenterParent();
        }

        private void RefreshOkButton()
        {
            if (charactersList == null)
                return;

            if (charactersList.All(c => c.Text == Contents.Constants.AvailableNameChars.First().ToString()))
                btnOK.Enabled = false;
            else
                btnOK.Enabled = true;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            End();
        }
    }
}