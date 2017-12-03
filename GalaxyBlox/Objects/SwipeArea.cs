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
using GalaxyBlox.Models;
using GalaxyBlox.EventArgsClasses;
using Microsoft.Xna.Framework;
using Android.Util;

namespace GalaxyBlox.Objects
{
    class SwipeArea : GameObject
    {
        public event EventHandler Swipe;
        protected virtual void OnSwipe(SwipeEventArgs e)
        {
            EventHandler handler = Swipe;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool swiping;
        private Vector2 pressLocation;
        private int timer;

        private float widthSwipeTolerance;
        private float heightSwipeTolerance;

        public SwipeArea(Room parentRoom, Vector2 position, Vector2 size) : base(parentRoom)
        {
            Type = "swipe_area";
            this.Position = position;
            this.Size = size;

            var realSize = DisplayRect();
            widthSwipeTolerance = realSize.X * 0.10f;
            heightSwipeTolerance = realSize.Y * 0.10f;
        }

        public void StartSwipe(Vector2 pressLocation)
        {
            swiping = true;
            this.pressLocation = pressLocation;
            timer = 0;
        }

        public bool FinishSwipe(Vector2 releaseLocation)
        {
            if (!swiping)
            {
                EndSwipe();
                return false;
            }
            
            var result = RaiseSwipe(pressLocation, releaseLocation);
            swiping = false;
            return result;
        }

        private bool RaiseSwipe(Vector2 prevLocation, Vector2 location)
        {
            var vect = prevLocation - location;
            if (!swiping)
                return false;

            var rotation = MathHelper.ToDegrees((float)Math.Atan2(vect.X, vect.Y));
            var direction = SwipeDirection.None;
            var lenght = vect.Length();
            if (rotation < -45f && rotation > -135f)
            {
                if (lenght < widthSwipeTolerance)
                    return false;

                direction = SwipeDirection.Right;
            } else if (rotation > 45f && rotation < 135f)
            {
                if (lenght < widthSwipeTolerance)
                    return false;

                direction = SwipeDirection.Left;

            } else if (rotation > -45f && rotation < 45f)
            {
                if (lenght < heightSwipeTolerance)
                    return false;

                direction = SwipeDirection.Up;
            } else if (rotation > 135f || rotation < -135f )
            {
                if (lenght < heightSwipeTolerance)
                    return false;

                direction = SwipeDirection.Down;
            }

            if (direction == SwipeDirection.None)
                return false;

            OnSwipe(new SwipeEventArgs(direction, vect.Length()));
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!swiping)
                return;

            timer += gameTime.ElapsedGameTime.Milliseconds;
            if (timer > 500)
                EndSwipe();
        }

        private void EndSwipe()
        {
            swiping = false;
            timer = 0;
        }

        public enum SwipeDirection
        {
            None,
            Left,
            Right,
            Up,
            Down
        }
    }
}