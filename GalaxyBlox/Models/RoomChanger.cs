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
using Android.Util;
using GalaxyBlox.EventArgsClasses;

namespace GalaxyBlox.Models
{
    public class RoomChanger
    {
        public event EventHandler ChangeEnded;
        protected virtual void OnChangeEnded(ChangerEventArgs e)
        {
            EventHandler handler = ChangeEnded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private Room room;
        private Vector2 startPosition;
        private Vector2 endPosition;

        public RoomChanger(Room room)
        {
            this.room = room;
            this.startPosition = new Vector2(Game1.ActiveGame.GraphicsDevice.Viewport.Width, Game1.ActiveGame.GraphicsDevice.Viewport.Height);
            this.endPosition = room.Position;
        }

        public virtual void Update(GameTime gameTime)
        {
            Swip();

            OnChangeEnded(new ChangerEventArgs(room));
        }

        public virtual void Swip()
        {
            room.Position = endPosition;
            RoomManager.ActiveRoom = room;
        }
    }
}