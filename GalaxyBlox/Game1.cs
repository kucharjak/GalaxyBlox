using Android.Util;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GalaxyBlox
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static Game1 ActiveGame;
        public static ContentManager GameContent;

        public static Random Random;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
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

            Static.Settings.Game.LoadAll();

            GameContent = Content;
            Random = new Random(unchecked((int)DateTime.Now.Ticks));

            //new Rooms.MenuRoom("Room_Menu", Static.Settings.Game.WindowSize, new Vector2()).Show();
            new Rooms.GameRoom("Room_Game", Static.Settings.Game.WindowSize, new Vector2()).Show();
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
            Static.Contents.Fonts.PanelContentText = Content.Load<SpriteFont>("Fonts/PanelContentText");
            Static.Contents.Fonts.PanelHeaderText = Content.Load<SpriteFont>("Fonts/PanelHeaderText");
            Static.Contents.Textures.ControlButton_fall = Content.Load<Texture2D>("Sprites/ControlButton_down");
            Static.Contents.Textures.ControlButton_left = Content.Load<Texture2D>("Sprites/ControlButton_left");
            Static.Contents.Textures.ControlButton_right = Content.Load<Texture2D>("Sprites/ControlButton_right");
            Static.Contents.Textures.ControlButton_rotate = Content.Load<Texture2D>("Sprites/ControlButton_rotate");
            Static.Contents.Textures.ControlButton_pause = Content.Load<Texture2D>("Sprites/ControlButton_pause");
            Static.Contents.Textures.BackgroundGame = Content.Load<Texture2D>("Backgrounds/Background");
            Static.Contents.Textures.BackgroundMenu = Static.Contents.Textures.BackgroundGame; //Content.Load<Texture2D>("Backgrounds/menu");
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
            RoomManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            RoomManager.Draw(gameTime, spriteBatch, GraphicsDevice);
        }
    }
}
