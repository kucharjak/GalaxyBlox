using System;
using Microsoft.Xna.Framework;
using GalaxyBlox.Models;

namespace GalaxyBlox.Objects
{
    class Button : GameObject
    {
        public bool IsTouched;
        public Color SelectedBackgroundColor;
        public Color DefaultBackgroundColor;
        public Color PressBackgroundColor;
        public ButtonState State = ButtonState.None;

        public int HoverTime = 0;

        public event EventHandler Click;
        protected virtual void OnClick(EventArgs e)
        {
            EventHandler handler = Click;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void RaiseClick(EventArgs e)
        {
            if (Enabled && State != ButtonState.Clicked)
            {
                State = ButtonState.Clicked;
                Clicked();
                OnClick(e);
            }
        }

        public event EventHandler Release;
        protected virtual void OnRelease(EventArgs e)
        {
            EventHandler handler = Release;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void RaiseRelease(EventArgs e)
        {
            if (Enabled && State != ButtonState.Released)
            {
                State = ButtonState.Released;
                Released();
                OnRelease(e);
            }
        }

        public event EventHandler Hover;
        protected virtual void OnHover(EventArgs e)
        {
            EventHandler handler = Hover;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void RaiseHover(EventArgs e)
        {
            if (Enabled /*&& State != ButtonState.Hovered*/)
            {
                State = ButtonState.Hovered;
                Hovered();
                OnHover(e);
            }
        }

        public Button(Room parentRoom) : base (parentRoom)
        {
            Type = "button";
            Origin = new Vector2(0.5f);
            HoverTime = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsTouched)
                HoverTime += gameTime.ElapsedGameTime.Milliseconds;
        }

        protected virtual void Hovered()
        {
            Scale = 1.05f;
            IsTouched = true;
            BaseColor = SelectedBackgroundColor;
        }

        protected virtual void Released()
        {
            Scale = 1f;
            IsTouched = false;
            BaseColor = DefaultBackgroundColor;
            HoverTime = 0;
        }

        protected virtual void Clicked()
        {
            Scale = 1f;
            IsTouched = false;
            BaseColor = DefaultBackgroundColor;
        }

        public enum ButtonState
        {
            None,
            Clicked,
            Hovered,
            Released
        }
    }
}