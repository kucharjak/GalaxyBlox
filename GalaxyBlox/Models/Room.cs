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
using Microsoft.Xna.Framework.Input;

namespace GalaxyBlox.Models
{
    class Room
    {
        protected List<GameObject> objects = new List<GameObject>();
        public Texture2D Background;
        public Size RoomSize;

        public bool IsPaused = false;
        public bool IsVisible = false;

        public Vector2 Position = new Vector2();
        public float Scale = 1f;
        public float InGameOffsetX = 0f;
        public float InGameOffsetY = 0f;

        //protected int padding = 20;

        public Room(Size RealSize, Size GameSize)
        {
            RoomSize = GameSize;

            if (RealSize.Width - GameSize.Width > RealSize.Height - GameSize.Height)
            { // HEIGHT + X offset
                Scale = RealSize.Height / (GameSize.Height * 1f);
                InGameOffsetX = (int)((RealSize.Width - (GameSize.Width * Scale)) / 2);
            }
            else
            { // WIDTH  + Y offset
                Scale = RealSize.Width / (GameSize.Width * 1f);
                InGameOffsetY = (int)((RealSize.Height - (GameSize.Height * Scale)) / 2);
            }

            Position = new Vector2(200);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsPaused || !IsVisible)
                return;

            var input = TouchPanel.GetState().Where(tch => tch.State != TouchLocationState.Invalid).FirstOrDefault();
            if (input != null)
                HandleInput(input);

            foreach (var obj in objects)
                obj.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            if (Background != null)
                spriteBatch.Draw(Background, new Rectangle((int)Position.X, (int)Position.Y, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);

            foreach (var obj in objects)
            {
                obj.Draw(spriteBatch);
            }
        }

        public virtual void LoadContent(ContentManager content)
        {
            AddObjects();
        }

        protected virtual void AddObjects()
        {
        }

        protected virtual void HandleInput(TouchLocation input)
        {
            var vectInput = input.Position; //UnScaleVector(input.Position);
            var rectInput = new Rectangle((int)vectInput.X, (int)vectInput.Y, 1, 1);
            var buttons = objects.Where(btn => btn.Type == "button").ToArray();
            if (buttons.Count() > 0)
            {
                switch (input.State)
                {
                    case TouchLocationState.Moved:
                    case TouchLocationState.Pressed:
                        {
                            var touchedButton = buttons.Where(btn => btn.DisplayRectWithScaleAndRoomPosition().Intersects(rectInput)).FirstOrDefault();
                            if (touchedButton != null)
                                (touchedButton as Button).RaiseHover(new EventArgs());

                            var releasedButtons = buttons.Where(btn => !btn.DisplayRectWithScaleAndRoomPosition().Intersects(rectInput) && btn != touchedButton);
                            foreach (var btn in releasedButtons)
                                (btn as Button).RaiseRelease(new EventArgs());
                        }
                        break;
                    case TouchLocationState.Released:
                        {
                            var pressedButton = buttons.Where(btn => btn.DisplayRectWithScaleAndRoomPosition().Intersects(rectInput)).FirstOrDefault();
                            if (pressedButton != null)
                                (pressedButton as Button).RaiseClick(new EventArgs());
                        }
                        break;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Back))
                Game1.ActiveGame.ChangeRooms();
                //Game1.InMenu = true;
        }

        private Vector2 UnScaleVector(Vector2 vect)
        {
            var resultVect = new Vector2(vect.X / Scale - InGameOffsetX, vect.Y / Scale - InGameOffsetY);
            return resultVect;
        }

        private Vector2 ScaleVector2(Vector2 vect)
        {
            var resultVect = new Vector2(vect.X * Scale + InGameOffsetX, vect.Y * Scale + InGameOffsetY);
            return resultVect;
        }
    }
}