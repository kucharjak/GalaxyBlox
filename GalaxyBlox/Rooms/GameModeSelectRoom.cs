using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using Microsoft.Xna.Framework;
using GalaxyBlox.Objects;
using static GalaxyBlox.Static.SettingOptions;
using GalaxyBlox.EventArgsClasses;

namespace GalaxyBlox.Rooms
{
    class GameModeSelectRoom : Room
    {
        private Button btnClassicMode;
        private Button btnNormalMode;
        private Button btnExtremeMode;

        public GameModeSelectRoom(Room parent, string name, Vector2 size, Vector2 position) : base(parent, name, size, position)
        {
        }

        public GameModeSelectRoom(string name, Vector2 size, Vector2 position) : base(name, size, position)
        {
        }

        protected override void Initialize()
        {
            Background = Static.Contents.Textures.Pix;
            BaseColor = Color.DarkBlue;

            var btnCount = 3;
            var btnSize = new Vector2(320, 100);
            var btnTextSize = (int)(btnSize.Y * 0.65f);
            var pading = 25;
            var posX = (Size.X - btnSize.X) / 2;
            var posY = (Size.Y - (btnSize.Y * btnCount + pading * (btnCount - 1))) / 2;            

            btnClassicMode = Bank.Buttons.GetSettingsButton(this);
            btnClassicMode.Position = new Vector2(posX, posY);
            btnClassicMode.Size = btnSize;
            btnClassicMode.Text = "Clasický";
            btnClassicMode.TextHeight = btnTextSize;
            btnClassicMode.Click += BtnClassicMode_Click;
            Objects.Add(btnClassicMode);
            posY += (int)btnClassicMode.Size.Y + pading;

            btnNormalMode = Bank.Buttons.GetSettingsButton(this);
            btnNormalMode.Position = new Vector2(posX, posY);
            btnNormalMode.Size = btnSize;
            btnNormalMode.Text = "Normální";
            btnNormalMode.TextHeight = btnTextSize;
            btnNormalMode.Click += BtnNormalMode_Click;
            Objects.Add(btnNormalMode);
            posY += (int)btnNormalMode.Size.Y + pading;

            btnExtremeMode = Bank.Buttons.GetSettingsButton(this);
            btnExtremeMode.Position = new Vector2(posX, posY);
            btnExtremeMode.Size = btnSize;
            btnExtremeMode.Text = "Extrémní";
            btnExtremeMode.TextHeight = btnTextSize;
            btnExtremeMode.Click += BtnExtremeMode_Click;
            Objects.Add(btnExtremeMode);
            posY += (int)btnExtremeMode.Size.Y + pading;
        }

        private void BtnExtremeMode_Click(object sender, EventArgs e)
        {
            Static.Settings.Game.Mode = GameMode.Extreme;
            Static.Settings.Game.ArenaSize = new Vector2(18, 30);
            StartGame();
        }

        private void BtnNormalMode_Click(object sender, EventArgs e)
        {
            Static.Settings.Game.Mode = GameMode.Normal;
            Static.Settings.Game.ArenaSize = new Vector2(12, 20);
            StartGame();
        }

        private void BtnClassicMode_Click(object sender, EventArgs e)
        {
            Static.Settings.Game.Mode = GameMode.Classic;
            Static.Settings.Game.ArenaSize = new Vector2(12, 20);
            StartGame();
        }

        private void StartGame()
        {
            new GameRoom(Parent, "Room_Game", Static.Settings.Game.WindowSize, new Vector2()).Show();
            End();
        }
    }
}