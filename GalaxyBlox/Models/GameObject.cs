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
using Android.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyBlox.Models
{
    class GameObject
    {
        public string Name = "";
        public string Type = "";

        public Vector2 Size;
        public Vector2 Position;

        public string Text = "";
        public bool ShowText = false;
        public Color TextColor = Color.Black;
        public Point TextOffset = new Point();
        public SpriteFont TextSpriteFont = null;
        public Vector2 TextPosition { get { return new Vector2(Position.X + TextOffset.X, Position.Y + TextOffset.Y); } }
        public float TextScale = 1f;

        public Texture2D BackgroundImage = null;
        public Color Color { get { return BackgroundColor * Alpha; } }
        public Color BackgroundColor = Color.White;
        public float Alpha = 1f;

        public void CenterText(float roomScale)
        {
            var textSize = TextSpriteFont.MeasureString(Text);
            TextOffset = new Point((int)((Size.X - textSize.X / roomScale) / 2), (int)((Size.Y - textSize.Y / roomScale) / 2));
        }
    }
}