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

        public Button(Room parentRoom) : base (parentRoom)
        {
            Type = "button";
            Origin = new Vector2(0.5f);
        }

        public virtual void Touch()
        {
            IsTouched = true;
            BackgroundColor = SelectedBackgroundColor;
        }

        public virtual void Release()
        {
            IsTouched = false;
            BackgroundColor = DefaultBackgroundColor;
        }

        public virtual void Press()
        {
            IsTouched = false;
            BackgroundColor = DefaultBackgroundColor;

            PressAction();
        }

        public virtual void PressAction()
        {

        }
    }
}