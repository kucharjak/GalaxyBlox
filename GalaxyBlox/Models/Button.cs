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

        public Button(float scale) : base (scale)
        {
            Type = "button";
        }

        public virtual void Touch()
        {
            IsTouched = true;
        }

        public virtual void Release()
        {
            IsTouched = false;
        }

        public virtual void Press()
        {
            IsTouched = false;

            PressAction();
        }

        public virtual void PressAction()
        {

        }
    }
}