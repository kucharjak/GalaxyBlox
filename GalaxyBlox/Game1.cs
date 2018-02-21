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

            new Rooms.MenuRoom("Room_Menu", Static.Settings.Game.WindowSize, new Vector2()).Show();
            //new Rooms.GameRoom("Room_Game", Static.Settings.Game.WindowSize, new Vector2()).Show();
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
            Static.Contents.Fonts.PlainTextFont = Content.Load<SpriteFont>("Fonts/PlainText");
            Static.Contents.Fonts.PixelArtTextFont = Content.Load<SpriteFont>("Fonts/PixelArtText");

            Static.Contents.Textures.ControlButton_fall = Content.Load<Texture2D>("Sprites/ControlButton_down");
            Static.Contents.Textures.ControlButton_left = Content.Load<Texture2D>("Sprites/ControlButton_left");
            Static.Contents.Textures.ControlButton_right = Content.Load<Texture2D>("Sprites/ControlButton_right");
            Static.Contents.Textures.ControlButton_rotate = Content.Load<Texture2D>("Sprites/ControlButton_rotate");

            Static.Contents.Textures.Button_pause = Content.Load<Texture2D>("Sprites/btn_pause");
            Static.Contents.Textures.Button_exit = Content.Load<Texture2D>("Sprites/btn_exit");
            Static.Contents.Textures.Button_play = Content.Load<Texture2D>("Sprites/btn_play");
            Static.Contents.Textures.Button_settings= Content.Load<Texture2D>("Sprites/btn_settings");
            Static.Contents.Textures.Button_highscore= Content.Load<Texture2D>("Sprites/btn_highscore");
            Static.Contents.Textures.Button_left= Content.Load<Texture2D>("Sprites/btn_left");
            Static.Contents.Textures.Button_right = Content.Load<Texture2D>("Sprites/btn_right");
            Static.Contents.Textures.Button_left_small = Content.Load<Texture2D>("Sprites/btn_left_small");
            Static.Contents.Textures.Button_right_small = Content.Load<Texture2D>("Sprites/btn_right_small");
            Static.Contents.Textures.Button_up_medium = Content.Load<Texture2D>("Sprites/btn_up_medium");
            Static.Contents.Textures.Button_down_medium = Content.Load<Texture2D>("Sprites/btn_down_medium");
            Static.Contents.Textures.Button_empty = Content.Load<Texture2D>("Sprites/btn_empty");
            
            Static.Contents.Textures.BackgroundGame = Content.Load<Texture2D>("Backgrounds/Background");
            Static.Contents.Textures.BackgroundMenu = Static.Contents.Textures.BackgroundGame;
            Static.Contents.Textures.BorderedButtonBackground = Content.Load<Texture2D>("Sprites/BorderedButtonBackground");
            Static.Contents.Textures.Dialog_background = Content.Load<Texture2D>("Backgrounds/dialog_background");
            Static.Contents.Textures.Dialog_inside = Content.Load<Texture2D>("Backgrounds/dialog_inside");

            Static.Contents.Textures.Dialog_icon_questionMark = Content.Load<Texture2D>("Sprites/dialog_icon_questionMark");
            Static.Contents.Textures.Dialog_icon_settings = Content.Load<Texture2D>("Sprites/dialog_icon_settings");
            Static.Contents.Textures.Dialog_icon_highscore = Content.Load<Texture2D>("Sprites/dialog_icon_highscore");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //Static.Contents.Textures.Pix.Dispose();
            //Static.Contents.Textures.Pix = null;
            //Static.Contents.Textures.ControlButton_fall.Dispose();
            //Static.Contents.Textures.ControlButton_fall = null;
            //Static.Contents.Textures.ControlButton_left.Dispose();
            //Static.Contents.Textures.ControlButton_left = null;
            //Static.Contents.Textures.ControlButton_right.Dispose();
            //Static.Contents.Textures.ControlButton_right = null;
            //Static.Contents.Textures.ControlButton_rotate.Dispose();
            //Static.Contents.Textures.ControlButton_rotate = null;
            //Static.Contents.Textures.ControlButton_pause.Dispose();
            //Static.Contents.Textures.ControlButton_pause = null;
            //Static.Contents.Textures.BackgroundGame.Dispose();
            //Static.Contents.Textures.BackgroundGame = null;
            //Static.Contents.Textures.BackgroundMenu.Dispose();
            //Static.Contents.Textures.BackgroundMenu = null;
            //Static.Contents.Textures.BorderedButtonBackground.Dispose();
            //Static.Contents.Textures.BorderedButtonBackground = null;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            RoomManager.Update(gameTime);
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
