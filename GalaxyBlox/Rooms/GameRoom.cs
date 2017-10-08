using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GalaxyBlox.Models;

namespace GalaxyBlox.Rooms
{
    class GameRoom : Room
    {
        public GameRoom(Size RealSize, Size GameSize) : base(RealSize, GameSize)
        {
        }
    }
}