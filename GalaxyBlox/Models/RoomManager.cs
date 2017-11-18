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
using Android.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GalaxyBlox.EventArgsClasses;

namespace GalaxyBlox.Models
{
    public static class RoomManager
    {
        private static List<Room> rooms = new List<Room>();
        public static Room ActiveRoom;
        private static RoomChanger RoomChanger = null;

        public static void Update(GameTime gameTime)
        {
            if (RoomChanger != null)
                RoomChanger.Update(gameTime);

            if (ActiveRoom != null)
                ActiveRoom.Update(gameTime);
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Purple);
            
            foreach (var room in rooms)
            {
                if (!room.IsVisible)
                    continue;

                room.Prepare(spriteBatch, graphicsDevice);
            }

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            foreach (var room in rooms)
            {
                if (!room.IsVisible)
                    continue;

                room.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
        }
        
        public static void ShowRoom(Room room)
        {
            if (!rooms.Contains(room))
                AddRoom(room);

            if (ActiveRoom == null)
            {
                ActiveRoom = room;
            }
            else
            {
                RoomChanger = new RoomChanger(room);
                RoomChanger.ChangeEnded += RoomChanger_ChangeEnded;
            }
        }

        public static void CloseRoom(Room room)
        {
            if (ActiveRoom == room)
            {
                if (ActiveRoom.Parent != null)
                {
                    RoomChanger = new RoomChanger(room.Parent);
                    RoomChanger.ChangeEnded += RoomChanger_ChangeEnded;
                }
                else
                {
                    ActiveRoom = null;
                }
            }
        }

        public static void EndRoom(Room room)
        {
            CloseRoom(room);
            DeleteRoom(room);
        }

        private static void AddRoom(Room room)
        {
            rooms.Add(room);
        }

        private static void DeleteRoom(Room room)
        {
            if (rooms.Contains(room))
                rooms.Remove(room);
        }
        
        private static void RoomChanger_ChangeEnded(object sender, EventArgs e)
        {
            (e as ChangerEventArgs).ChangedRoom.AfterChangeEvent();
        }
    }
}