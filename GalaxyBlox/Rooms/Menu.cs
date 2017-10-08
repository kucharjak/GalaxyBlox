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
            //objects.Add(new GameObject()
            //{
            //    Size = new Android.Util.Size(RoomSize.Width - 2 * padding, RoomSize.Height - 2 * padding),
            //    Position = new Microsoft.Xna.Framework.Vector2(padding, padding)
            //});


        }
    }
}