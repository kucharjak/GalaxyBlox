using GalaxyBlox.Models;
using GalaxyBlox.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static GalaxyBlox.Static.Contents;

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

            var displaySize = new Vector2(
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);

            Static.Settings.LoadAll(displaySize);


            GameContent = Content;
            Random = new Random(unchecked((int)DateTime.Now.Ticks));

            new Rooms.MenuRoom("Room_Menu", Static.Settings.WindowSize, new Vector2()).Show();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ///// LOADING TEXTURES /////

            Fonts.PlainTextFont = Content.Load<SpriteFont>("Fonts/PlainText");
            Fonts.PixelArtTextFont = Content.Load<SpriteFont>("Fonts/PixelArtText");

            Textures.Pix = Content.Load<Texture2D>("Sprites/pixel");
            Textures.Buttons = Content.Load<Texture2D>("Sprites/buttons");
            Textures.Stars = Content.Load<Texture2D>("Sprites/stars");
            Textures.Dialog_Icons = Content.Load<Texture2D>("Sprites/dialogs_icon");
            Textures.Logo = Content.Load<Texture2D>("Sprites/logo");
            Textures.FrozenStars = Content.Load<Texture2D>("Sprites/frozen_stars");

            Textures.Dialog_Backgrounds = Content.Load<Texture2D>("Backgrounds/dialogs");
            Textures.GameUI_Backgrounds = Content.Load<Texture2D>("Backgrounds/gameUI");
            Textures.Game_Background = Content.Load<Texture2D>("Backgrounds/background");

            Textures.Animations = Content.Load<Texture2D>("Animations/animations");

            ///// CREATING SPRITES /////

            Sprites.Pix = new Sprite(Textures.Pix, Textures.Pix.GetRectangle());

            Sprites.ControlButton_fall = new Sprite(Textures.Buttons, new Rectangle(0, 0, 38, 28));
            Sprites.ControlButton_left = new Sprite(Textures.Buttons, new Rectangle(39, 0, 38, 28));
            Sprites.ControlButton_right = new Sprite(Textures.Buttons, new Rectangle(78, 0, 38, 28));
            Sprites.ControlButton_rotate = new Sprite(Textures.Buttons, new Rectangle(117, 0, 38, 28));
            Sprites.Button_pause = new Sprite(Textures.Buttons, new Rectangle(112, 61, 29, 25));
            Sprites.Button_exit = new Sprite(Textures.Buttons, new Rectangle(70, 29, 25, 25));
            Sprites.Button_play = new Sprite(Textures.Buttons, new Rectangle(0, 58, 70, 45));
            Sprites.Button_settings = new Sprite(Textures.Buttons, new Rectangle(35, 29, 34, 28));
            Sprites.Button_highscore = new Sprite(Textures.Buttons, new Rectangle(0, 29, 34, 28));
            Sprites.Button_left = new Sprite(Textures.Buttons, new Rectangle(96, 29, 15, 28));
            Sprites.Button_right = new Sprite(Textures.Buttons, new Rectangle(112, 29, 15, 28));
            Sprites.Button_left_small = new Sprite(Textures.Buttons, new Rectangle(112, 87, 10, 15));
            Sprites.Button_right_small = new Sprite(Textures.Buttons, new Rectangle(123, 87, 10, 15));
            Sprites.Button_up_medium = new Sprite(Textures.Buttons, new Rectangle(128, 29, 25, 15));
            Sprites.Button_down_medium = new Sprite(Textures.Buttons, new Rectangle(128, 45, 25, 15));
            Sprites.Button_empty = new Sprite(Textures.Buttons, new Rectangle(71, 58, 40, 20));
            Sprites.Button_bonus = new Sprite(Textures.Buttons, new Rectangle(71, 79, 40, 15));

            Sprites.Game_Background = new Sprite(Textures.Game_Background, Textures.Game_Background.GetRectangle());

            Sprites.Dialog_background = new Sprite(Textures.Dialog_Backgrounds, new Rectangle(0, 0, 96, 96));
            Sprites.Dialog_inside = new Sprite(Textures.Dialog_Backgrounds, new Rectangle(97, 0, 30, 30));

            Sprites.GameUI_top_background = new Sprite(Textures.GameUI_Backgrounds, new Rectangle(0, 28, 178, 33));
            Sprites.GameUI_bottom_classic_background = new Sprite(Textures.GameUI_Backgrounds, new Rectangle(0, 62, 178, 36));
            Sprites.GameUI_bottom_normal_background = new Sprite(Textures.GameUI_Backgrounds, new Rectangle(0, 99, 178, 56));
            Sprites.GameUI_playingArena_border = new Sprite(Textures.GameUI_Backgrounds, new Rectangle(0, 0, 27, 27));

            Sprites.Dialog_icon_highscore = new Sprite(Textures.Dialog_Icons, new Rectangle(0, 0, 18, 22));
            Sprites.Dialog_icon_questionMark = new Sprite(Textures.Dialog_Icons, new Rectangle(19, 0, 18, 22));
            Sprites.Dialog_icon_settings = new Sprite(Textures.Dialog_Icons, new Rectangle(38, 0, 18, 22));

            Sprites.Star_small = new Sprite(Textures.Stars, new Rectangle(0, 0, 11, 11));
            Sprites.Star_medium_01 = new Sprite(Textures.Stars, new Rectangle(12, 0, 9, 9));
            Sprites.Star_medium_02 = new Sprite(Textures.Stars, new Rectangle(22, 0, 7, 7));
            Sprites.Star_big = new Sprite(Textures.Stars, new Rectangle(30, 0, 3, 3));

            Sprites.Logo = new Sprite(Textures.Logo, Textures.Logo.GetRectangle());

            Sprites.FrozenStars = new Sprite(Textures.FrozenStars, Textures.FrozenStars.GetRectangle());

            ///// CREATING ANIMATIONS //////

            var tmpSprites = new List<Sprite>();
            for (int i = 0; i < 10; i++)
                tmpSprites.Add(new Sprite(Textures.Animations, new Rectangle(i * 32, 0, 32, 32)));
            Animations.Explosion = new SpriteAnimation(tmpSprites, 20, true) { Loop = false };

            tmpSprites = new List<Sprite>();
            for (int i = 0; i < 18; i++)
                tmpSprites.Add(new Sprite(Textures.Animations, new Rectangle(i * 32, 32, 32, 64)));
            Animations.Laser = new SpriteAnimation(tmpSprites, 20, true) { Loop = false };
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
