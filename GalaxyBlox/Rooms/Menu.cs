using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Android.Util;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Rooms
{
    class Menu : Room
    {
        public Menu(Size RealSize, Size GameSize) : base(RealSize, GameSize)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            Background = content.Load<Texture2D>("Backgrounds/menu");
            loadObjects();
        }

        private void loadObjects()
        {
            padding = 30;
            //objects.Add(new GameObject()
            //{
            //    Size = new Android.Util.Size(RoomSize.Width - 2 * padding, RoomSize.Height - 2 * padding),
            //    Position = new Microsoft.Xna.Framework.Vector2(padding, padding)
            //});
            var btnSize = new Vector2(RoomSize.Width - 2 * padding, 50);
            var btnCount = 4;
            var btnStartPosY = (RoomSize.Height - (50 * btnCount + 10 * (btnCount - 1))) / 2;
            objects.Add(new GameObject()
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY),
                Alpha = 0.5f,
                BackgroundColor = new Color(121, 140, 170),
                Text = "Nová hra",
                ShowText = true,
                TextColor = Color.White,
                TextSpriteFont = Game1.Contents.ButtonText
            });
            objects.Last().CenterText(scale);
            objects.Add(new GameObject()
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65),
                Alpha = 0.5f,
                BackgroundColor = new Color(121, 140, 170),
                Text = "Ovládání",
                ShowText = true,
                TextColor = Color.White,
                TextSpriteFont = Game1.Contents.ButtonText
            });
            objects.Last().CenterText(scale);
            objects.Add(new GameObject()
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65 * 2),
                Alpha = 0.5f,
                BackgroundColor = new Color(121, 140, 170),
                Text = "Nastavení",
                ShowText = true,
                TextColor = Color.White,
                TextSpriteFont = Game1.Contents.ButtonText
            });
            objects.Last().CenterText(scale);
            objects.Add(new GameObject()
            {
                Size = btnSize,
                Position = new Vector2(padding, btnStartPosY + 65 * 3),
                Alpha = 0.5f,
                BackgroundColor = new Color(121, 140, 170),
                Text = "Konec",
                ShowText = true,
                TextColor = Color.White,
                TextSpriteFont = Game1.Contents.ButtonText
            });
            objects.Last().CenterText(scale);
        }
    }
}