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
        private bool newSingleColor;

        Button btnIndicator;
        Button btnVibration;
        Button btnExtendedShapes;
        Button btnSingleColor;
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

            this.Size = new Vector2(600, 735);
            CenterParent();

            newIndicator = Settings.UserSettings.Indicator;
            newVibration = Settings.UserSettings.Vibration;
            newUseExtendedShapesLibrary = Settings.UserSettings.UseExtendedShapeLibrary;
            newSingleColor = Settings.UserSettings.UseSingleColor;

            var margin = new { top = 129, left = 25, right = 25, bottom = 35 }; // anonymous type for margin
            
            var settingsItemSize = new Vector2(160, 80);
            var settingsItemTextHeight = (int)(settingsItemSize.Y * 0.3f);
            var itemPadding = 25;

            var btnDialogSize = new Vector2(160, 80);
            var btnDialogTextHeight = (int)(btnDialogSize.Y * 0.3f);

            var highscoreBackgroundSize = new Vector2(Size.X - margin.left - margin.right, 4 * settingsItemSize.Y + 5 * itemPadding);

            // ADDING BACKGROUND 
            GameObject obj = new DynamicBackgroundObject(this, Contents.Sprites.Dialog_inside, 4);
            obj.Position = new Vector2(margin.left, margin.top);
            obj.Size = highscoreBackgroundSize;
            obj.LayerDepth = 0.04f;
            Objects.Add(obj);

            // INDICATOR SETTINGS
            btnIndicator = Bank.Buttons.GetBasicButton(this);
            btnIndicator.Size = settingsItemSize;
            btnIndicator.TextHeight = settingsItemTextHeight;
            btnIndicator.Text = newIndicator.ToString().ToUpper();
            btnIndicator.Position = new Vector2(Size.X - btnIndicator.Size.X - margin.right - itemPadding, obj.Position.Y + itemPadding);
            btnIndicator.Click += BtnSetIndicator_Click;
            Objects.Add(btnIndicator);

            obj = Bank.Visuals.GetLabelLeft(this);
            obj.Text = Constants.Texts.Indicator;
            obj.Size = settingsItemSize;
            obj.TextHeight = settingsItemTextHeight;
            obj.Position = new Vector2(margin.left + itemPadding, btnIndicator.Position.Y);
            Objects.Add(obj);

            // VIBRATTION SETTINS
            btnVibration = Bank.Buttons.GetBasicButton(this);
            btnVibration.Size = settingsItemSize;
            btnVibration.TextHeight = settingsItemTextHeight;
            btnVibration.Text = Settings.UserSettings.Vibration ? Constants.Texts.Yes : Constants.Texts.CheekyNo;
            btnVibration.Position = new Vector2(Size.X - btnVibration.Size.X - margin.right - itemPadding, btnIndicator.Position.Y + btnIndicator.Size.Y + itemPadding);
            btnVibration.Click += BtnVibration_Click;
            Objects.Add(btnVibration);

            obj = Bank.Visuals.GetLabelLeft(this);
            obj.Text = Constants.Texts.Vibration;
            obj.Size = settingsItemSize;
            obj.TextHeight = settingsItemTextHeight;
            obj.Position = new Vector2(margin.left + itemPadding, btnVibration.Position.Y);
            Objects.Add(obj);

            // SHAPES SETTINGS
            btnExtendedShapes = Bank.Buttons.GetBasicButton(this);
            btnExtendedShapes.Size = settingsItemSize;
            btnExtendedShapes.TextHeight = settingsItemTextHeight;
            btnExtendedShapes.Text = Settings.UserSettings.UseExtendedShapeLibrary ? Constants.Texts.Yes : Constants.Texts.CheekyNo;
            btnExtendedShapes.Position = new Vector2(Size.X - btnExtendedShapes.Size.X - margin.right - itemPadding, btnVibration.Position.Y + btnVibration.Size.Y + itemPadding);
            btnExtendedShapes.Click += BtnExtendedShapes_Click;
            Objects.Add(btnExtendedShapes);

            obj = Bank.Visuals.GetLabelLeft(this);
            obj.Text = Constants.Texts.ExtendedShapesLibrary;
            obj.Size = settingsItemSize;
            obj.TextHeight = settingsItemTextHeight;
            obj.Position = new Vector2(margin.left + itemPadding, btnExtendedShapes.Position.Y);
            Objects.Add(obj);

            // SINGLE COLOR SETTINGS
            btnSingleColor = Bank.Buttons.GetBasicButton(this);
            btnSingleColor.Size = settingsItemSize;
            btnSingleColor.TextHeight = settingsItemTextHeight;
            btnSingleColor.Text = Settings.UserSettings.UseSingleColor ? Constants.Texts.Yes : Constants.Texts.CheekyNo;
            btnSingleColor.Position = new Vector2(Size.X - btnSingleColor.Size.X - margin.right - itemPadding, btnExtendedShapes.Position.Y + btnExtendedShapes.Size.Y + itemPadding);
            btnSingleColor.Click += BtnSingleColor_Click;
            Objects.Add(btnSingleColor);

            obj = Bank.Visuals.GetLabelLeft(this);
            obj.Text = Constants.Texts.SingleColor;
            obj.Size = settingsItemSize;
            obj.TextHeight = settingsItemTextHeight;
            obj.Position = new Vector2(margin.left + itemPadding, btnSingleColor.Position.Y);
            Objects.Add(obj);

            // OK AND CANCEL BUTTONS
            btnOK = Bank.Buttons.GetBasicButton(this);
            btnOK.Size = btnDialogSize;
            btnOK.Text = Constants.Texts.Save;
            btnOK.TextHeight = btnDialogTextHeight;
            btnOK.Click += BtnOK_Click;
            Objects.Add(btnOK);

            btnCancel = Bank.Buttons.GetBasicButton(this);
            btnCancel.Size = btnDialogSize;
            btnCancel.Text = Constants.Texts.Cancel;
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
            Settings.UserSettings.Indicator = newIndicator;
            Settings.UserSettings.Vibration = newVibration;
            Settings.UserSettings.UseExtendedShapeLibrary = newUseExtendedShapesLibrary;
            Settings.UserSettings.UseSingleColor = newSingleColor;

            Settings.SaveUserSettings();
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
            btnVibration.Text = newVibration ? Constants.Texts.Yes : Constants.Texts.CheekyNo;
        }

        private void BtnExtendedShapes_Click(object sender, EventArgs e)
        {
            newUseExtendedShapesLibrary = !newUseExtendedShapesLibrary;
            btnExtendedShapes.Text = newUseExtendedShapesLibrary ? Constants.Texts.Yes : Constants.Texts.CheekyNo;
        }

        private void BtnSingleColor_Click(object sender, EventArgs e)
        {
            newSingleColor = !newSingleColor;
            btnSingleColor.Text = newSingleColor ? Constants.Texts.Yes : Constants.Texts.CheekyNo;
        }
    }
}