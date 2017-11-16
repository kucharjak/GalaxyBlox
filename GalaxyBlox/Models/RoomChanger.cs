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
        public event EventHandler AfterChange;
        protected virtual void OnAfterChange(ChangerEventArgs e)
        {
            EventHandler handler = AfterChange;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected bool changing = false;
        protected bool deactivateRooms = false;
        protected string args;

        protected Size displaySize;
        protected Room firstRoom;
        protected Room secondRoom;

        public RoomChanger(Room firstRoom, Room secondRoom, Size displaySize)
        {
            this.displaySize = displaySize;
            this.firstRoom = firstRoom;
            this.secondRoom = secondRoom;

            this.firstRoom.Position = Vector2.Zero;
            this.secondRoom.Position = new Vector2(this.displaySize.Width, 0);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!changing)
                return;
            
            Swip();

            OnAfterChange(new ChangerEventArgs(firstRoom, secondRoom));
            firstRoom.AfterChangeEvent(args);
            secondRoom.AfterChangeEvent(args);
        }

        public virtual void Swip()
        {
            firstRoom.Position = new Vector2(displaySize.Width / 2, 0);
            secondRoom.Position = Vector2.Zero;

            var tmpRoom = firstRoom;
            firstRoom = secondRoom;
            secondRoom = tmpRoom;

            firstRoom.IsPaused = false;
            firstRoom.IsVisible = true;
            secondRoom.IsPaused = true;
            secondRoom.IsVisible = false;

            changing = false;
        }

        public void Change(string args = "", bool deactivate = false)
        {
            this.args = args;
            deactivateRooms = deactivate;
            changing = true;
        }
    }
}