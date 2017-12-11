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
using GalaxyBlox.Objects;

namespace GalaxyBlox.EventArgsClasses
{
    class ActiveBonusChangedEventArgs : EventArgs
    {
        public GameBonus ActiveBonus;

        public ActiveBonusChangedEventArgs(GameBonus activeBonus)
        {
            ActiveBonus = activeBonus;
        }
    }

    class AvailableBonusesChangeEventArgs : EventArgs
    {
        public List<GameBonus> GameBonuses;
        public bool EnableBonusButtons;

        public AvailableBonusesChangeEventArgs(List<GameBonus> gameBonuses, bool enableBonusButtons = true)
        {
            GameBonuses = gameBonuses;
            EnableBonusButtons = enableBonusButtons;
        }
    }

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

        public QueueChangeEventArgs(Actor actor)
        {
            NewActor = actor.Shape;
            NewActorsColor = actor.Color;
        }

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