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
                    Game1.Pix,
                    ScaleRect(new Rectangle((int)obj.Position.X, (int)obj.Position.Y, obj.Size.Width, obj.Size.Height)),
                    obj.Color);
            }

            if (touchPoint != null)
                spriteBatch.Draw(Game1.Pix,
                    new Rectangle(
                        touchPoint.Value.X - 25,
                        touchPoint.Value.Y - 25,
                        50,
                        50),
                    Color.Green);
        }

        public virtual void LoadContent(ContentManager content)
        {
        }

        protected virtual void HandleInput(TouchLocation input)
        {
            touchPoint = new Point((int)input.Position.X, (int)input.Position.Y);
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