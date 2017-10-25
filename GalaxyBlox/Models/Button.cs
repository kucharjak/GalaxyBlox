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

namespace GalaxyBlox.Models
{
    class Button : GameObject
    {
        public bool IsTouched;
        public int ButttonID = 0;
        public Color SelectedBackgroundColor;
        public Color DefaultBackgroundColor;
        public Color PressBackgroundColor;

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
            Clicked();
            OnClick(e);
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
            Released();
            OnRelease(e);
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
            Hovered();
            OnHover(e);
        }

        public Button(Room parentRoom) : base (parentRoom)
        {
            Type = "button";
            Origin = new Vector2(0.5f);
        }

        protected virtual void Hovered()
        {
            Scale = 1.05f;
            IsTouched = true;
            Color = SelectedBackgroundColor;
        }

        protected virtual void Released()
        {
            Scale = 1f;
            IsTouched = false;
            Color = DefaultBackgroundColor;
        }

        protected virtual void Clicked()
        {
            Scale = 1f;
            IsTouched = false;
            Color = DefaultBackgroundColor;
        }
    }
}