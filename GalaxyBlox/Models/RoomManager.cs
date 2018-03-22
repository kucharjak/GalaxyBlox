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
        public static List<Room> Rooms { get { return rooms; } }
        public static Room ActiveRoom;

        private static FrameCounter fc = new FrameCounter();

        public static void Update(GameTime gameTime)
        {
            if (ActiveRoom != null)
                ActiveRoom.Update(gameTime);

            fc.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Black);

            foreach (var room in rooms)
            {
                if (!room.IsVisible)
                    continue;

                room.Prepare(spriteBatch, graphicsDevice);
            }

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

            if (Static.Settings.Game.ShowFPS)
            {
                spriteBatch.DrawString(Static.Contents.Fonts.PlainTextFont, "CUR: " + fc.CurrentFramesPerSecond, new Vector2(), Color.Red, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(Static.Contents.Fonts.PlainTextFont, "AVG: " + fc.AverageFramesPerSecond, new Vector2(0, Static.Contents.Fonts.PlainTextFont.LineSpacing), Color.Red, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
            }

            foreach (var room in rooms)
            {
                if (!room.IsVisible)
                    continue;

                room.Draw(gameTime, spriteBatch);
            }

            // draw separation
            spriteBatch.Draw(Static.Contents.Textures.Pix, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), null, Static.Contents.Colors.RoomsSeparateColor, 0f, new Vector2(), SpriteEffects.None, 0.8f);

            spriteBatch.End();
        }
        
        public static void ShowRoom(Room room)
        {
            Room previousRoom = null;

            if (!rooms.Contains(room))
                AddRoom(room);

            if (ActiveRoom != null)
            {
                ActiveRoom.LayerDepth = 0.8f;
                previousRoom = ActiveRoom;
            }
                
            
            ActiveRoom = room;
            ActiveRoom.LayerDepth = 0.9f;
            RecalculateRoomDepth();

            ActiveRoom.IsVisible = true;
            ActiveRoom.AfterChangeEvent(previousRoom);
        }

        public static void CloseRoom(Room room, bool endRoom = false)
        {
            if (ActiveRoom == room)
            {
                if (room.Parent != null)
                {
                    room.LayerDepth = 0.8f;
                    room.IsVisible = false;
                    ActiveRoom = room.Parent;
                    if (endRoom)
                        DeleteRoom(room);

                    ActiveRoom.AfterChangeEvent(room);
                    ActiveRoom.LayerDepth = 0.9f;
                    ActiveRoom.IsVisible = true;
                }
                else
                {
                    var newRoom = rooms.Where(rm => rm != room && rm.IsVisible); // try to find new room
                    if (newRoom.Count() != 0)
                    {
                        ActiveRoom = newRoom.First();
                        if (endRoom)
                            DeleteRoom(room);

                        ActiveRoom.AfterChangeEvent(room);
                    }
                    else
                        throw new Exception("Active room can't be null");
                }
            }
            else
            {
                if (endRoom)
                {
                    DeleteRoom(room);
                }
                else
                {
                    room.IsVisible = false;
                }
            }
            RecalculateRoomDepth();
        }

        public static void EndRoom(Room room)
        {
            CloseRoom(room, true);
        }

        private static void RecalculateRoomDepth()
        {
            var layerDepth = 0.0f;
            foreach (var rm in rooms.Where(rum => rum != ActiveRoom).OrderBy(rum => rum.LayerDepth))
            {
                layerDepth += 0.1f;
                rm.LayerDepth = layerDepth;
            }
        }

        private static void AddRoom(Room room)
        {
            rooms.Add(room);
        }

        private static void DeleteRoom(Room room)
        {
            if (rooms.Contains(room))
            {
                rooms.Remove(room);
            }
        }
    }
}