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

        public float Scale = 1f;
        public float OffsetX = 0f;
        public float OffsetY = 0f;

        protected int padding = 20;

        private Point? touchPoint = null;

        public Room(Size RealSize, Size GameSize)
        {
            RoomSize = GameSize;

            if (RealSize.Width - GameSize.Width > RealSize.Height - GameSize.Height)
            { // HEIGHT + X offset
                Scale = RealSize.Height / (GameSize.Height * 1f);
                OffsetX = (int)((RealSize.Width - (GameSize.Width * Scale)) / 2);
            }
            else
            { // WIDTH  + Y offset
                Scale = RealSize.Width / (GameSize.Width * 1f);
                OffsetY = (int)((RealSize.Height - (GameSize.Height * Scale)) / 2);
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
                obj.Draw(spriteBatch);
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
        }

        private Vector2 UnScaleVector(Vector2 vect)
        {
            var resultVect = new Vector2(vect.X / Scale - OffsetX, vect.Y / Scale - OffsetY);
            return resultVect;
        }

        private Vector2 ScaleVector2(Vector2 vect)
        {
            var resultVect = new Vector2(vect.X * Scale + OffsetX, vect.Y * Scale + OffsetY);
            return resultVect;
        }
    }
}