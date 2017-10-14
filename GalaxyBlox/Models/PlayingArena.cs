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
using GalaxyBlox.Static;

namespace GalaxyBlox.Models
{
    class PlayingArena : GameObject
    {
        public PlayingArena(Room parentRoom) : base(parentRoom)
        {
            BackgroundImage = Contents.Textures.Pix;
        }
    }
}