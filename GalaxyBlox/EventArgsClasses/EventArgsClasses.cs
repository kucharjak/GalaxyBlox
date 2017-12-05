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
using static GalaxyBlox.Objects.SwipeArea;
using static GalaxyBlox.Static.SettingOptions;

namespace GalaxyBlox.EventArgsClasses
{
    class NewGameEventArgs : EventArgs
    {
        public GameMode GameMode;

        public NewGameEventArgs(GameMode gameMode)
        {
            GameMode = gameMode;
        }
    }

    class ChangerEventArgs : EventArgs
    {
        public Room ChangedRoom;

        public ChangerEventArgs(Room room)
        {
            ChangedRoom = room;
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

    class SwipeEventArgs : EventArgs
    {
        public SwipeDirection Direction;
        public float Force;

        public SwipeEventArgs(SwipeDirection direction, float force)
        {
            Direction = direction;
            Force = force;
        }
    }
}