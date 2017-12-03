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
        public event EventHandler Closed;
        protected virtual void OnClosed(EventArgs e)
        {
            EventHandler handler = Closed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public Room Parent { get; protected set; }

        protected List<GameObject> Objects { get; set; } = new List<GameObject>();
        public Texture2D Background;
        public Color BackgroundColor { get { return BaseColor * Alpha; } }
        public Color BaseColor = Color.White;
        public bool FullScreen = false;
        public float Alpha = 1f;
        public Size Size;

        public bool IsPaused = false;
        public bool IsVisible = false;

        public Vector2 Position;
        public float Scale = 1f;
        public float InGameOffsetX = 0f;
        public float InGameOffsetY = 0f;

        public string Name = "";
        public float LayerDepth = 0.1f;

        public Room(Room parent, string name, Size size, Vector2 position) : this(name, size, position)
        {
            Parent = parent;
        }

        public Room(string name, Size size, Vector2 position)
        {
            Position = position;
            Name = name;
            Size = size;

            var realSize = new Size(Game1.ActiveGame.GraphicsDevice.Viewport.Width, Game1.ActiveGame.GraphicsDevice.Viewport.Height);
            var windowSize = Static.Settings.Game.WindowSize;
            if (realSize.Width - windowSize.Width > realSize.Height - windowSize.Height)
            { // HEIGHT + X offset
                Scale = realSize.Height / (windowSize.Height * 1f);
                InGameOffsetX = (int)((realSize.Width - (windowSize.Width * Scale)) / 2);
            }
            else
            { // WIDTH  + Y offset
                Scale = realSize.Width / (windowSize.Width * 1f);
                InGameOffsetY = (int)((realSize.Height - (windowSize.Height * Scale)) / 2);
            }

            Initialize();
        }

        public void Show()
        {
            RoomManager.ShowRoom(this);
        }

        public void Close()
        {
            RoomManager.CloseRoom(this);
            OnClosed(new EventArgs());
        }

        public void End()
        {
            RoomManager.EndRoom(this);
            OnClosed(new EventArgs());
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsPaused || !IsVisible)
                return;

            var input = TouchPanel.GetState().Where(tch => tch.State != TouchLocationState.Invalid);
            if (input.Count() > 0)
                HandleInput(input.FirstOrDefault());

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
            {
                if (FullScreen)
                    spriteBatch.Draw(Background, new Rectangle(0, 0, Game1.ActiveGame.GraphicsDevice.Viewport.Width, Game1.ActiveGame.GraphicsDevice.Viewport.Height), null, BackgroundColor, 0, new Vector2(), SpriteEffects.None, LayerDepth);
                else
                    spriteBatch.Draw(Background, DisplayRectWithScale(), null, BackgroundColor, 0, new Vector2(), SpriteEffects.None, LayerDepth);
            }

            foreach (var obj in Objects)
                obj.Draw(spriteBatch);
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

            var rectInput = new Rectangle((int)input.Position.X, (int)input.Position.Y, 1, 1);
            var swipe = Objects.Where(area => area.Type == "swipe_area").ToArray();
            bool swiped = false;
            if (swipe.Count() > 0)
            {
                if (input.State == TouchLocationState.Pressed || input.State == TouchLocationState.Released)
                {
                    var touch = swipe.Where(btn => btn.DisplayRect().Intersects(rectInput));
                    foreach (var area in touch)
                    {
                        if (input.State == TouchLocationState.Pressed)
                            (area as SwipeArea).StartSwipe(input.Position);
                        else
                            if ((area as SwipeArea).FinishSwipe(input.Position))
                                swiped = true;
                    }
                }
            }

            var buttons = Objects.Where(btn => btn.Type == "button").ToArray();
            if (buttons.Count() > 0)
            {
                Button touchedButton = null;

                if (!swiped)
                {
                    switch (input.State)
                    {
                        case TouchLocationState.Moved:
                        case TouchLocationState.Pressed:
                            {
                                var touch = buttons.Where(btn => btn.DisplayRect().Intersects(rectInput)).FirstOrDefault();
                                if (touch != null)
                                {
                                    touchedButton = (touch as Button);
                                    touchedButton.RaiseHover(new EventArgs());
                                }
                            }
                            break;
                        case TouchLocationState.Released:
                            {
                                var pressedButton = buttons.Where(btn => btn.DisplayRect().Intersects(rectInput)).FirstOrDefault();
                                if (pressedButton != null)
                                    (pressedButton as Button).RaiseClick(new EventArgs());
                            }
                            break;
                    }
                }
                var releasedButtons = buttons.Where(btn => btn != touchedButton);
                foreach (var btn in releasedButtons)
                    (btn as Button).RaiseRelease(new EventArgs());
            }
        }

        protected virtual void HandleBackButton()
        {
        }

        public Rectangle DisplayRectWithScale()
        {
            var resultRect = new Rectangle(
                (int)(Position.X * Scale + InGameOffsetX),
                (int)(Position.Y * Scale + InGameOffsetY),
                (int)(Size.Width * Scale),
                (int)(Size.Height * Scale));
            return resultRect;
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