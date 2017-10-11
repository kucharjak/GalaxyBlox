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
        protected Room ParentRoom;

        public Vector2 Size;
        public Vector2 Position;
        public Vector2 Origin;
        //protected float displayScale = 1f;
        public float Scale = 1f; // scale for drawing - for example when i need bigger object for a second (animation, or just to stand out like hovered button)

        public string Text
        {
            get { return text; }
            set {
                text = value;
                textSize = TextSpriteFont.MeasureString(Text);
                if (TextIsCentered)
                    CenterText();
            }
        }
        private string text;
        public bool TextIsCentered = false;
        public bool ShowText = false;
        public Color TextColor = Color.Black;
        public Point TextOffset = new Point();
        protected Vector2 textSize = new Vector2();
        public SpriteFont TextSpriteFont = null;
        public Vector2 TextPosition { get { return new Vector2(Position.X + TextOffset.X, Position.Y + TextOffset.Y); } }
        public float TextScale = 1f;

        public Texture2D BackgroundImage = null;
        public Color Color { get { return BackgroundColor * Alpha; } }
        public Color BackgroundColor = Color.White;
        public float Alpha = 1f;

        public Rectangle Rectangle { get { return new Rectangle((int)(Position.X), (int)(Position.Y), (int)Size.X, (int)Size.Y); } }

        public GameObject(Room parentRoom)
        {
            ParentRoom = parentRoom;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                    BackgroundImage,
                    DisplayRectWithScaleAndRoomPosition(), //ScaleRect(new Rectangle((int)obj.Position.X, (int)obj.Position.Y, (int)obj.Size.X, (int)obj.Size.Y)),
                    Color);

            if (ShowText)
                spriteBatch.DrawString(TextSpriteFont, Text, DisplayTextPositionWithScale() + ParentRoom.Position, TextColor, 0f, new Vector2(), Scale, SpriteEffects.None, 0f);
        }

        public void CenterText()
        {
            TextOffset = new Point((int)((Size.X - textSize.X / ParentRoom.Scale) / 2), (int)((Size.Y - textSize.Y / ParentRoom.Scale) / 2));
        }

        public Vector2 DisplayTextPositionWithScale()
        {
            var offsetX = Origin.X * (textSize.X * Scale - textSize.X);
            var offsetY = Origin.Y * (textSize.Y * Scale - textSize.Y);

            var resultVect = new Vector2(
                TextPosition.X * ParentRoom.Scale + ParentRoom.InGameOffsetX - offsetX,
                TextPosition.Y * ParentRoom.Scale + ParentRoom.InGameOffsetY - offsetY);
            return resultVect;
        }

        public Rectangle DisplayRect()
        {
            var resultRect = new Rectangle(
                (int)(Position.X * ParentRoom.Scale + ParentRoom.InGameOffsetX),
                (int)(Position.Y * ParentRoom.Scale + ParentRoom.InGameOffsetY),
                (int)(Size.X * ParentRoom.Scale),
                (int)(Size.Y * ParentRoom.Scale)
                );
            return resultRect;
        }

        public Rectangle DisplayRectWithScale()
        {
            var offsetX = Origin.X * (Size.X * Scale - Size.X);
            var offsetY = Origin.Y * (Size.Y * Scale - Size.Y);

            var resultRect = new Rectangle(
                (int)(Position.X * ParentRoom.Scale + ParentRoom.InGameOffsetX - offsetX),
                (int)(Position.Y * ParentRoom.Scale + ParentRoom.InGameOffsetY - offsetY),
                (int)(Size.X * ParentRoom.Scale * Scale),
                (int)(Size.Y * ParentRoom.Scale * Scale)
                );
            return resultRect;
        }

        public Rectangle DisplayRectWithScaleAndRoomPosition()
        {
            var offsetX = Origin.X * (Size.X * Scale - Size.X);
            var offsetY = Origin.Y * (Size.Y * Scale - Size.Y);

            var resultRect = new Rectangle(
                (int)(Position.X * ParentRoom.Scale + ParentRoom.InGameOffsetX - offsetX + ParentRoom.Position.X),
                (int)(Position.Y * ParentRoom.Scale + ParentRoom.InGameOffsetY - offsetY + ParentRoom.Position.Y),
                (int)(Size.X * ParentRoom.Scale * Scale),
                (int)(Size.Y * ParentRoom.Scale * Scale)
                );
            return resultRect;
        }
    }
}