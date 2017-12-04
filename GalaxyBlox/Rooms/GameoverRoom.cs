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
        Button btnOK;
        GameObject score;

        public GameOverRoom(Room parent, string name, Vector2 size, Vector2 position, long highscore) : base(parent, name, size, position)
        {
            this.highscore = highscore;
            score.Text = $"Skóre: { Utils.Strings.ScoreToLongString(highscore) }";
        }

        protected override void Initialize()
        {
            Background = Contents.Textures.Pix;
            BaseColor = Color.DarkBlue;

            var margin = 20;
            var posY = margin + 30;
            var itemsHeight = 45;
            var itemsTextHeight = itemsHeight;
            GameObject obj = Bank.Visuals.GetGameOverLabel(this);
            obj.Text = "- Konec hry -";
            obj.TextHeight = itemsTextHeight;
            obj.Size = new Vector2(Size.X, itemsHeight);
            obj.Position = new Vector2(0, posY);
            Objects.Add(obj);

            posY += (int)obj.Size.Y + margin;
            score = Bank.Visuals.GetGameOverLabel(this);
            score.TextHeight = itemsTextHeight;
            score.Size = new Vector2(Size.X, itemsHeight);
            score.Position = new Vector2(0, posY);
            Objects.Add(score);

            btnOK = Bank.Buttons.GetSettingsButton(this);
            btnOK.Size = new Vector2(250, 120);
            btnOK.Position = new Vector2((Size.X - btnOK.Size.X) / 2, Size.Y - btnOK.Size.Y - margin);
            btnOK.Text = "OK";
            btnOK.TextHeight = 60;
            btnOK.Click += BtnOK_Click;
            Objects.Add(btnOK);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            End();
        }
    }
}