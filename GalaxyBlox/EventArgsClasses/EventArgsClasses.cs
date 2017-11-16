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
using Microsoft.Xna.Framework;

namespace GalaxyBlox.EventArgsClasses
{
    public class ChangerEventArgs : EventArgs
    {
        public Room ActiveRoom;
        public Room OtherRoom;

        public ChangerEventArgs(Room activeRoom, Room noLongerActiveRoom)
        {
            ActiveRoom = activeRoom;
            OtherRoom = noLongerActiveRoom;
        }
    }

    class QueueChangeEventArgs : EventArgs
    {
        public bool[,] NewActor;
        public Color NewActorsColor;

        public QueueChangeEventArgs(bool[,] newActor, Color newActorsColor)
        {
            NewActor = newActor;
            NewActorsColor = newActorsColor;
        }
    }
}