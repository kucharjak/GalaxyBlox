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
    class SettingsRoom : Room
    {
        private SettingOptions.Indicator newIndicator;
        Button btnIndicator;
        Button btnOK;
        Button btnCancel;

        public SettingsRoom(Room parent, string name, Vector2 size, Vector2 position) : base(parent, name, size, position)
        {
        }

        protected override void Initialize()
        {
            Background = Contents.Textures.Pix;
            BaseColor = Color.DarkBlue;
            newIndicator = Settings.Game.UserSettings.Indicator;

            var margin = 40;
            var padding = 20;
            var posY = margin;
            GameObject obj = Bank.Visuals.GetSettingsLabel(this);
            obj.Text = "Zaměřovač:";
            obj.Size = new Vector2(100, 45);
            obj.Position = new Vector2(margin, posY);
            Objects.Add(obj);

            btnIndicator = Bank.Buttons.GetSettingsButton(this);
            btnIndicator.Size = new Vector2(100, 45);
            btnIndicator.Position = new Vector2(Size.X - btnIndicator.Size.X - margin, posY);
            btnIndicator.Click += BtnSetIndicator_Click;
            Objects.Add(btnIndicator);
            SetBtnIndicatorText();

            btnOK = Bank.Buttons.GetSettingsButton(this);
            btnOK.Size = new Vector2(150, 45);
            btnOK.Text = "Uložit";
            btnOK.Click += BtnOK_Click;
            Objects.Add(btnOK);

            btnCancel = Bank.Buttons.GetSettingsButton(this);
            btnCancel.Size = new Vector2(150, 45);
            btnCancel.Text = "Zrušit";
            btnCancel.Click += BtnCancel_Click;
            Objects.Add(btnCancel);

            var btnPosX = (Size.X - (btnOK.Size.X + btnCancel.Size.X + padding)) / 2;
            btnOK.Position = new Vector2(btnPosX, Size.Y - btnOK.Size.Y - margin);
            btnPosX += btnOK.Size.X + padding;
            btnCancel.Position = new Vector2(btnPosX, Size.Y - btnCancel.Size.Y - margin);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            End();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Settings.Game.UserSettings.Indicator = newIndicator;
            Settings.Game.SaveUserSettings();
            End();
        }

        private void BtnSetIndicator_Click(object sender, EventArgs e)
        {
            var newValue = Enum.GetValues(typeof(SettingOptions.Indicator)).Cast<SettingOptions.Indicator>().SkipWhile(val => val != newIndicator).Skip(1).FirstOrDefault();
            newIndicator = newValue;
            SetBtnIndicatorText();
        }

        private void SetBtnIndicatorText()
        {
            var text = "";
            switch (newIndicator)
            {
                case SettingOptions.Indicator.None: text = "žádný"; break;
                case SettingOptions.Indicator.Shadow: text = "stín"; break;
                case SettingOptions.Indicator.Shape: text = "tvar"; break;
            }
            btnIndicator.Text = text;
        }
    }
}