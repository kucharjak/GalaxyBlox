using Android.Util;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace GalaxyBlox
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static Game1 ActiveGame;
        public static Random Random;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //public static bool InMenu;
        Room menuRoom;
        Room gameRoom;
        RoomChanger roomChanger;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = GameWindow.Width;
            //graphics.PreferredBackBufferHeight = GameWindow.Height;
            ActiveGame = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Random = new Random(unchecked((int)DateTime.Now.Ticks));

            menuRoom = new Rooms.MenuRoom(new Size(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Static.Settings.GameSize);
            menuRoom.LoadContent(Content);
            menuRoom.IsVisible = true;
            menuRoom.IsPaused = false;
            gameRoom = new Rooms.GameRoom(new Size(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Static.Settings.GameSize);
            gameRoom.LoadContent(Content);
            gameRoom.IsVisible = false;
            gameRoom.IsPaused = true;

            roomChanger = new RoomChanger(menuRoom, gameRoom, new Size(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Static.Contents.Textures.Pix = Content.Load<Texture2D>("Sprites/pixel");
            Static.Contents.Fonts.MenuButtonText = Content.Load<SpriteFont>("Fonts/ButtonText");
            Static.Contents.Textures.ControlButton_down = Content.Load<Texture2D>("Sprites/ControlButton_down");
            Static.Contents.Textures.ControlButton_left = Content.Load<Texture2D>("Sprites/ControlButton_left");
            Static.Contents.Textures.ControlButton_right = Content.Load<Texture2D>("Sprites/ControlButton_right");
            Static.Contents.Textures.ControlButton_rotate = Content.Load<Texture2D>("Sprites/ControlButton_rotate");
            Static.Contents.Textures.ControlButton_pause = Content.Load<Texture2D>("Sprites/ControlButton_pause");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (roomChanger != null)
                roomChanger.Update(gameTime);

            menuRoom.Update(gameTime);
            gameRoom.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            menuRoom.Draw(gameTime, spriteBatch);
            gameRoom.Draw(gameTime, spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void ChangeRooms()
        {
            roomChanger.Change(true);
        }
    }
}
