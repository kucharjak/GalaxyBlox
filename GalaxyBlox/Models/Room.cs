using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Android.Util;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using GalaxyBlox.Objects;

namespace GalaxyBlox.Models
{
    public class Room
    {
        public Room Parent { get; private set; }

        protected List<GameObject> Objects { get; set; } = new List<GameObject>();
        public Texture2D Background;
        public Size RoomSize;
        public Size RealSize;

        public bool IsPaused = false;
        public bool IsVisible = false;

        public Vector2 Position = new Vector2();
        public float Scale = 1f;
        public float InGameOffsetX = 0f;
        public float InGameOffsetY = 0f;

        public string Name = "";
        public float LayerDepth = 0f;
        //protected int padding = 20;

        public Room(Room parent, string name, Size realSize, Size gameSize) : this(name, realSize, gameSize)
        {
            Parent = parent;
        }

        public Room(string name, Size realSize, Size gameSize)
        {
            Name = name;
            RoomSize = gameSize;
            RealSize = realSize;

            if (realSize.Width - gameSize.Width > realSize.Height - gameSize.Height)
            { // HEIGHT + X offset
                Scale = realSize.Height / (gameSize.Height * 1f);
                InGameOffsetX = (int)((realSize.Width - (gameSize.Width * Scale)) / 2);
            }
            else
            { // WIDTH  + Y offset
                Scale = realSize.Width / (gameSize.Width * 1f);
                InGameOffsetY = (int)((realSize.Height - (gameSize.Height * Scale)) / 2);
            }

            Position = new Vector2();

            LoadContent(Game1.GameContent);
        }

        public void Show()
        {
            IsVisible = true;
            RoomManager.ShowRoom(this);
        }

        public void Close()
        {
            Hide();
            RoomManager.CloseRoom(this);
        }

        public void End()
        {
            RoomManager.EndRoom(this);
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsPaused || !IsVisible)
                return;

            var input = TouchPanel.GetState().Where(tch => tch.State != TouchLocationState.Invalid).FirstOrDefault();
            HandleInput(input);

            foreach (var obj in Objects)
                obj.Update(gameTime);
        }

        public virtual void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            foreach (var obj in Objects)
                obj.Prepare(spriteBatch, graphicsDevice);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Background != null)
                spriteBatch.Draw(Background, new Rectangle((int)Position.X, (int)Position.Y, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), null, Color.White, 0, new Vector2(), SpriteEffects.None, LayerDepth);

            foreach (var obj in Objects)
                obj.Draw(spriteBatch);
        }

        public virtual void LoadContent(ContentManager content)
        {
            Initialize();
        }
        
        public virtual void AfterChangeEvent()
        {
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void HandleInput(TouchLocation input)
        {
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
                HandleBackButton();

            if (input == null || input.State == TouchLocationState.Invalid)
                return;

            var vectInput = input.Position; //UnScaleVector(input.Position);
            var rectInput = new Rectangle((int)vectInput.X, (int)vectInput.Y, 1, 1);
            var buttons = Objects.Where(btn => btn.Type == "button").ToArray();
            if (buttons.Count() > 0)
            {
                Button touchedButton = null;
                switch (input.State)
                {
                    case TouchLocationState.Moved:
                    case TouchLocationState.Pressed:
                        {
                            var touch = buttons.Where(btn => btn.DisplayRectWithScaleAndRoomPosition().Intersects(rectInput)).FirstOrDefault();
                            if (touch != null)
                            {
                                touchedButton = (touch as Button);
                                touchedButton.RaiseHover(new EventArgs());
                            }
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

                var releasedButtons = buttons.Where(btn => btn != touchedButton);
                foreach (var btn in releasedButtons)
                    (btn as Button).RaiseRelease(new EventArgs());
            }
        }

        protected virtual void HandleBackButton()
        {
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