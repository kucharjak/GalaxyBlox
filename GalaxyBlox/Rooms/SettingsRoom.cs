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
    class SettingsRoom : Room
    {
        private SettingOptions.Indicator newIndicator;
        private bool newVibration;
        private bool newUseExtendedShapesLibrary;

        Button btnIndicator;
        Button btnVibration;
        Button btnExtendedShapes;
        Button btnOK;
        Button btnCancel;

        public SettingsRoom(Room parent, string name, Vector2 size, Vector2 position) : base(parent, name, size, position)
        {
            Initialize();
        }

        protected override void Initialize()
        {
            DialogBackground = Contents.Sprites.Dialog_background;
            DialogIcon = Contents.Sprites.Dialog_icon_settings;
            DialogBackgroundScale = 4;
            IsDialog = true;

            this.Size = new Vector2(600, 630);
            CenterParent();

            newIndicator = Settings.Game.UserSettings.Indicator;
            newVibration = Settings.Game.UserSettings.Vibration;
            newUseExtendedShapesLibrary = Settings.Game.UserSettings.UseExtendedShapeLibrary;

            var margin = new { top = 129, left = 25, right = 25, bottom = 35 }; // anonymous type for margin
            
            var settingsItemSize = new Vector2(160, 80);
            var settingsItemTextHeight = (int)(settingsItemSize.Y * 0.3f);
            var itemPadding = 25;

            var btnDialogSize = new Vector2(160, 80);
            var btnDialogTextHeight = (int)(btnDialogSize.Y * 0.3f);

            var highscoreBackgroundSize = new Vector2(Size.X - margin.left - margin.right, 3 * settingsItemSize.Y + 4 * itemPadding);

            GameObject obj = new DynamicBackgroundObject(this, Contents.Sprites.Dialog_inside, 4);
            obj.Position = new Vector2(margin.left, margin.top);
            obj.Size = highscoreBackgroundSize;
            obj.LayerDepth = 0.04f;
            Objects.Add(obj);

            btnIndicator = Bank.Buttons.GetEmptyButton(this);
            btnIndicator.Size = settingsItemSize;
            btnIndicator.TextHeight = settingsItemTextHeight;
            btnIndicator.Text = newIndicator.ToString().ToUpper();
            btnIndicator.Position = new Vector2(Size.X - btnIndicator.Size.X - margin.right - itemPadding, obj.Position.Y + itemPadding);
            btnIndicator.Click += BtnSetIndicator_Click;
            Objects.Add(btnIndicator);

            obj = Bank.Visuals.GetSettingsLabel(this);
            obj.Text = "INDICATOR:";
            obj.Size = settingsItemSize;
            obj.TextHeight = settingsItemTextHeight;
            obj.Position = new Vector2(margin.left + itemPadding, btnIndicator.Position.Y);
            Objects.Add(obj);

            btnVibration = Bank.Buttons.GetEmptyButton(this);
            btnVibration.Size = settingsItemSize;
            btnVibration.TextHeight = settingsItemTextHeight;
            btnVibration.Text = Settings.Game.UserSettings.Vibration ? "YES" : "NO";
            btnVibration.Position = new Vector2(Size.X - btnVibration.Size.X - margin.right - itemPadding, btnIndicator.Position.Y + btnIndicator.Size.Y + itemPadding);
            btnVibration.Click += BtnVibration_Click;
            Objects.Add(btnVibration);

            obj = Bank.Visuals.GetSettingsLabel(this);
            obj.Text = "VIBRATION:";
            obj.Size = settingsItemSize;
            obj.TextHeight = settingsItemTextHeight;
            obj.Position = new Vector2(margin.left + itemPadding, btnVibration.Position.Y);
            Objects.Add(obj);

            btnExtendedShapes = Bank.Buttons.GetEmptyButton(this);
            btnExtendedShapes.Size = settingsItemSize;
            btnExtendedShapes.TextHeight = settingsItemTextHeight;
            btnExtendedShapes.Text = Settings.Game.UserSettings.UseExtendedShapeLibrary ? "YES" : "NOPE";
            btnExtendedShapes.Position = new Vector2(Size.X - btnExtendedShapes.Size.X - margin.right - itemPadding, btnVibration.Position.Y + btnVibration.Size.Y + itemPadding);
            btnExtendedShapes.Click += BtnExtendedShapes_Click;
            Objects.Add(btnExtendedShapes);

            obj = Bank.Visuals.GetSettingsLabel(this);
            obj.Text = "MORE BLOCKs:";
            obj.Size = settingsItemSize;
            obj.TextHeight = settingsItemTextHeight;
            obj.Position = new Vector2(margin.left + itemPadding, btnExtendedShapes.Position.Y);
            Objects.Add(obj);

            btnOK = Bank.Buttons.GetEmptyButton(this);
            btnOK.Size = btnDialogSize;
            btnOK.Text = "SAVE";
            btnOK.TextHeight = btnDialogTextHeight;
            btnOK.Click += BtnOK_Click;
            Objects.Add(btnOK);

            btnCancel = Bank.Buttons.GetEmptyButton(this);
            btnCancel.Size = btnDialogSize;
            btnCancel.Text = "CANCEL";
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
            Settings.Game.UserSettings.Indicator = newIndicator;
            Settings.Game.UserSettings.Vibration = newVibration;
            Settings.Game.UserSettings.UseExtendedShapeLibrary = newUseExtendedShapesLibrary;

            Settings.Game.SaveUserSettings();
            End();
        }

        private void BtnSetIndicator_Click(object sender, EventArgs e)
        {
            var newValue = Enum.GetValues(typeof(SettingOptions.Indicator)).Cast<SettingOptions.Indicator>().SkipWhile(val => val != newIndicator).Skip(1).FirstOrDefault();
            newIndicator = newValue;
            btnIndicator.Text = newValue.ToString().ToUpper();
        }

        private void BtnVibration_Click(object sender, EventArgs e)
        {
            newVibration = !newVibration;
            btnVibration.Text = newVibration ? "YES" : "NO";
        }

        private void BtnExtendedShapes_Click(object sender, EventArgs e)
        {
            newUseExtendedShapesLibrary = !newUseExtendedShapesLibrary;
            btnExtendedShapes.Text = newUseExtendedShapesLibrary ? "YES" : "NO";
        }
    }
}