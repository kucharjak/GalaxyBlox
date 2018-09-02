using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GalaxyBlox.EventArgsClasses;

namespace GalaxyBlox.Models
{
    public static class RoomManager
    {
        private static List<Room> rooms = new List<Room>();
        /// <summary>
        /// Active rooms collection
        /// </summary>
        public static List<Room> Rooms { get { return rooms; } }
        public static Room ActiveRoom;

        private static FrameCounter fc = new FrameCounter();

        public static void Update(GameTime gameTime)
        {
            if (ActiveRoom != null)
                ActiveRoom.Update(gameTime);

            if (Static.Settings.ShowFPS)
                fc.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            // Clear graphics
            graphicsDevice.Clear(Color.Black);

            // Prepare visible rooms before drawing
            foreach (var room in rooms)
            {
                if (!room.IsVisible)
                    continue;

                room.Prepare(spriteBatch, graphicsDevice);
            }
            
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

            if (Static.Settings.ShowFPS)
            {
                spriteBatch.DrawString(Static.Contents.Fonts.PlainTextFont, "CUR: " + fc.CurrentFramesPerSecond, new Vector2(), Color.Red, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(Static.Contents.Fonts.PlainTextFont, "AVG: " + fc.AverageFramesPerSecond, new Vector2(0, Static.Contents.Fonts.PlainTextFont.LineSpacing), Color.Red, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
            }

            // Draw visible rooms
            foreach (var room in rooms)
            {
                if (!room.IsVisible)
                    continue;

                room.Draw(gameTime, spriteBatch);
            }

            // Draw separation between active room and other non active rooms
            spriteBatch.Draw(Static.Contents.Sprites.Pix.TextureRef, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Static.Contents.Sprites.Pix.SourceRectangle, Static.Contents.Colors.RoomsSeparateColor, 0f, new Vector2(), SpriteEffects.None, 0.8f);

            spriteBatch.End();
        }
        
        /// <summary>
        /// Method responsible for showing and pushing rooms to foreground.
        /// When called it adds room to rooms collection, push down curently active room and push up new room.
        /// </summary>
        /// <param name="room">Room to show</param>
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

        /// <summary>
        /// Method responsible for closing room or ending room.
        /// When called it closes/ends room and change focus to parent of that room.
        /// </summary>
        /// <param name="room">Room to close/end</param>
        /// <param name="endRoom">Indicates to delete room from collection.</param>
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

        /// <summary>
        /// Method responsible for ending room. 
        /// Calls CloseRoom method with endRoom parameter set to true.
        /// </summary>
        /// <param name="room">Room to end</param>
        public static void EndRoom(Room room)
        {
            CloseRoom(room, true);
        }

        /// <summary>
        /// Method responsible for recalculating room layer depth position.
        /// Currently supports 8 visible rooms at one moment.
        /// </summary>
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