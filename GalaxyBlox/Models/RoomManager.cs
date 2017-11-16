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
    public class RoomManager
    {
        List<Room> rooms = new List<Room>();
        public Room ActiveRoom;
        public RoomChanger MenuGameRoomChanger;

        public RoomManager(GraphicsDevice graphicsDevice)
        {
            var menu = new Rooms.MenuRoom(this, "menu", new Size(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Static.Settings.GameSize);
            menu.LoadContent(Game1.GameContent);
            menu.IsVisible = true;
            menu.IsPaused = false;
            rooms.Add(menu);

            var game = new Rooms.GameRoom(this, "game", new Size(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Static.Settings.GameSize);
            game.LoadContent(Game1.GameContent);
            game.IsVisible = false;
            game.IsPaused = true;
            rooms.Add(game);

            MenuGameRoomChanger = new RoomChanger(menu, game, new Size(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height));
            MenuGameRoomChanger.AfterChange += MenuGameRoomChanger_AfterChange;
            ActiveRoom = menu;
        }

        private void MenuGameRoomChanger_AfterChange(object sender, EventArgs e)
        {
            var eventArgs = (e as ChangerEventArgs);
            ActiveRoom = eventArgs.ActiveRoom;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (MenuGameRoomChanger != null)
                MenuGameRoomChanger.Update(gameTime);

            if (ActiveRoom != null)
                ActiveRoom.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.Purple);

            foreach (var room in rooms)
                room.Prepare(spriteBatch, graphicsDevice);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            foreach (var room in rooms)
                room.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public void ChangeRooms(string args = "")
        {
            MenuGameRoomChanger.Change(args, true);
        }
    }
}