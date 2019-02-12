using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GalaxyBlox.Objects;
using GalaxyBlox.Static;

namespace GalaxyBlox.Rooms
{
    class CloseGameRoom : Room
    {
        Button btnOK;
        Button btnCancel;

        public CloseGameRoom(Room parent, string name, Vector2 size, Vector2 position) : base(parent, name, size, position)
        {
            Initialize();
        }

        protected override void Initialize()
        {
            DialogBackground = Contents.Sprites.Dialog_background;
            DialogIcon = Contents.Sprites.Dialog_icon_questionMark;
            DialogBackgroundScale = 4;
            IsDialog = true;

            this.Size = new Vector2(600, 360);
            CenterParent();

            var margin = new { top = 140, left = 25, right = 25, bottom = 35 }; // anonymous type for margin

            var settingsItemSize = new Vector2(160, 40);
            var settingsItemTextHeight = (int)(80 * 0.3f);

            var btnDialogSize = new Vector2(160, 80);
            var btnDialogTextHeight = (int)(btnDialogSize.Y * 0.3f);
            
            GameObject obj = Bank.Visuals.GetLabelLeft(this);
            obj.TextHeight = settingsItemTextHeight;
            obj.Text = Constants.Texts.ExitMsg;
            obj.TextAlignment = Models.TextAlignment.Left;
            obj.Size = settingsItemSize;
            obj.Position = new Vector2(margin.left, margin.top);
            Objects.Add(obj);

            btnOK = Bank.Buttons.GetBasicButton(this);
            btnOK.Size = btnDialogSize;
            btnOK.Text = Constants.Texts.CheekyYes;
            btnOK.TextHeight = btnDialogTextHeight;
            btnOK.Click += BtnOK_Click;
            Objects.Add(btnOK);

            btnCancel = Bank.Buttons.GetBasicButton(this);
            btnCancel.Size = btnDialogSize;
            btnCancel.Text = Constants.Texts.CheekyNo;
            btnCancel.TextHeight = btnDialogTextHeight;
            btnCancel.Click += BtnCancel_Click;
            Objects.Add(btnCancel);

            var btnPosX = (Size.X - (btnOK.Size.X + btnCancel.Size.X + margin.left)) / 2;
            btnOK.Position = new Vector2(btnPosX, Size.Y - btnOK.Size.Y - margin.bottom);
            btnPosX += btnOK.Size.X + margin.left;
            btnCancel.Position = new Vector2(btnPosX, Size.Y - btnCancel.Size.Y - margin.bottom);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            End();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Game1.Activity.Finish();
        }
    }
}