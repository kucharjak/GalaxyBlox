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

        public GameOverRoom(Room parent, string name, Size size, Vector2 position, long highscore) : base(parent, name, size, position)
        {
            this.highscore = highscore;
        }

        protected override void Initialize()
        {
            Background = Contents.Textures.Pix;
            BaseColor = Color.DarkBlue;

            var margin = 40;
            var posY = margin;
            GameObject obj = Bank.Visuals.GetGameOverLabel(this);
            obj.Text = "- Konec hry -";
            obj.Size = new Vector2(Size.Width, 45);
            obj.Position = new Vector2(0, posY);
            Objects.Add(obj);

            posY += margin;
            obj = Bank.Visuals.GetGameOverLabel(this);
            obj.Text = $"Highscore: { Utils.Strings.ScoreToLongString(highscore) }";
            obj.Size = new Vector2(Size.Width, 45);
            obj.Position = new Vector2(0, posY);
            Objects.Add(obj);

            btnOK = Bank.Buttons.GetSettingsButton(this);
            btnOK.Size = new Vector2(100, 45);
            btnOK.Position = new Vector2((Size.Width - btnOK.Size.X) / 2, Size.Height - btnOK.Size.Y - margin);
            btnOK.Text = "OK";
            btnOK.Click += BtnOK_Click;
            Objects.Add(btnOK);
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            End();
        }
    }
}