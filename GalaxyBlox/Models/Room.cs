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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Android.Util;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;

namespace GalaxyBlox.Models
{
    class Room
    {
        protected List<GameObject> objects = new List<GameObject>();
        public Texture2D Background;
        public Size RoomSize;

        protected float scale = 1f;
        protected float offsetX = 0f;
        protected float offsetY = 0f;

        protected int padding = 20;

        private Point? touchPoint = null;

        public Room(Size RealSize, Size GameSize)
        {
            RoomSize = GameSize;

            if (RealSize.Width - GameSize.Width > RealSize.Height - GameSize.Height)
            { // HEIGHT + X offset
                scale = RealSize.Height / (GameSize.Height * 1f);
                offsetX = (int)((RealSize.Width - (GameSize.Width * scale)) / 2);
            }
            else
            { // WIDTH  + Y offset
                scale = RealSize.Width / (GameSize.Width * 1f);
                offsetY = (int)((RealSize.Height - (GameSize.Height * scale)) / 2);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            var input = TouchPanel.GetState().Where(tch => tch.State != TouchLocationState.Invalid).FirstOrDefault();
            if (input != null)
            {
                HandleInput(input);
            }
            else
            {
                touchPoint = null;
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Background != null)
                spriteBatch.Draw(Background, new Rectangle(0,0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);

            foreach (var obj in objects)
            {
                spriteBatch.Draw(
                    Game1.Contents.Pix,
                    ScaleRect(new Rectangle((int)obj.Position.X, (int)obj.Position.Y, (int)obj.Size.X, (int)obj.Size.Y)),
                    obj.Color);

                if (obj.ShowText)
                    spriteBatch.DrawString(obj.TextSpriteFont, obj.Text, ScaleVector2(obj.TextPosition), obj.TextColor, 0f, new Vector2(), 1f, SpriteEffects.None, 0f);
            }

            //if (touchPoint != null)
            //    spriteBatch.Draw(Game1.Contents.Pix,
            //        new Rectangle(
            //            touchPoint.Value.X - 25,
            //            touchPoint.Value.Y - 25,
            //            50,
            //            50),
            //        Color.Green);
        }

        public virtual void LoadContent(ContentManager content)
        {
        }

        protected virtual void HandleInput(TouchLocation input)
        {
            var vectInput = UnScaleVector(input.Position);
            var rectInput = new Rectangle((int)vectInput.X, (int)vectInput.Y, 1, 1);
            //touchPoint = new Point((int)input.Position.X, (int)input.Position.Y);
            var buttons = objects.Where(btn => btn.Type == "button").ToArray();
            if (buttons.Count() == 0)
                return;

            switch (input.State)
            {
                case TouchLocationState.Moved:
                case TouchLocationState.Pressed:
                    {
                        var touchedButton = buttons.Where(btn => btn.Rectangle.Intersects(rectInput)).FirstOrDefault();
                        if (touchedButton != null)
                            (touchedButton as Button).Touch();

                        var releasedButtons = buttons.Where(btn => !btn.Rectangle.Intersects(rectInput) && btn != touchedButton);
                        foreach (var btn in releasedButtons)
                            (btn as Button).Release();
                    }
                    break;
                case TouchLocationState.Released:
                    {
                        var pressedButton = buttons.Where(btn => btn.Rectangle.Intersects(rectInput)).FirstOrDefault();
                        if (pressedButton != null)
                            (pressedButton as Button).Press();
                    }
                    break;
            }

            //var touchedButton = buttons.Where(btn => btn.Rectangle.Intersects(rectInput) && !(btn as Button).IsTouched).FirstOrDefault();
            //if (touchedButton != null)
            //    (touchedButton as Button).Touch();

            //var pressedButtons = buttons.Where(btn => (btn as Button).IsTouched && !btn.Rectangle.Intersects(rectInput));
            //foreach (var btn in pressedButtons)
            //    (btn as Button).Press();

            //var releasedButtons = buttons.Where(btn => !(btn as Button).IsTouched && !btn.Rectangle.Intersects(rectInput) && !pressedButtons.Contains(btn));
            //foreach (var btn in releasedButtons)
            //    (btn as Button).Release();

        }

        private Vector2 UnScaleVector(Vector2 vect)
        {
            var resultVect = new Vector2(vect.X / scale - offsetX, vect.Y / scale - offsetY);
            return resultVect;
        }

        private Vector2 ScaleVector2(Vector2 vect)
        {
            var resultVect = new Vector2(vect.X * scale + offsetX, vect.Y * scale + offsetY);
            return resultVect;
        }

        private Rectangle ScaleRect(Rectangle rect)
        {
            var resultRect = new Rectangle(
                (int)(rect.X * scale + offsetX),
                (int)(rect.Y * scale + offsetY),
                (int)(rect.Width * scale),
                (int)(rect.Height * scale)
                );
            return resultRect;
        }
    }
}