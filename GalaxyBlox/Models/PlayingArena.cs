using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Android.Util;

namespace GalaxyBlox.Models
{
    class PlayingArena : GameObject
    {
        private bool[,] actor;
        private Color actorColor;
        private Point actorPosition;

        private int[,] playground;
        private Color?[,] playgroundEffectsArray;
        private int playgroundInnerPadding;
        private int playgroundCubeSize;
        private int playgroundCubeMargin;

        private int gameSpeed = 1000; // move actor in 1000 ms = 1 s
        private int gameTimeElapsed = 0;

        private Color BackgroundColor;
        private Color BorderColor;
        private RenderTarget2D renderTarget;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parentRoom"></param>
        /// <param name="size"></param>
        /// <param name="position"></param>
        public PlayingArena(Room parentRoom, Vector2 size, Vector2 position) : base(parentRoom)
        {
            playground = new int[Settings.GameArenaSize.Width, Settings.GameArenaSize.Height];
            BackgroundColor = Contents.Colors.PlaygroundColor;
            BorderColor = Contents.Colors.PlaygroundColor;
            Alpha = 1f;
            
            playgroundInnerPadding = 4;
            playgroundCubeMargin = 1;

            var spaceLeftForCubes = new Size(
                (int)(size.X - 2 * playgroundInnerPadding - (Settings.GameArenaSize.Width - 1) * playgroundCubeMargin),
                (int)(size.Y - 2 * playgroundInnerPadding - (Settings.GameArenaSize.Height - 1) * playgroundCubeMargin));

            if (spaceLeftForCubes.Width / (float)Settings.GameArenaSize.Width < spaceLeftForCubes.Height / (float)Settings.GameArenaSize.Height)
            { // WIDTH
                playgroundCubeSize = spaceLeftForCubes.Width / Settings.GameArenaSize.Width;
            }
            else
            { // HEIGHT
                playgroundCubeSize = spaceLeftForCubes.Height / Settings.GameArenaSize.Height;
            }

            Size = new Vector2(
                (Settings.GameArenaSize.Width - 1) * (playgroundCubeSize + playgroundCubeMargin) + playgroundCubeSize + 2 * playgroundInnerPadding,
                (Settings.GameArenaSize.Height - 1) * (playgroundCubeSize + playgroundCubeMargin) + playgroundCubeSize + 2 * playgroundInnerPadding);

            Position = new Vector2(
                (size.X - Size.X) / 2,
                (position.Y + size.Y) - Size.Y);

            CreateActor();

            renderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)Size.X, (int)Size.Y);
            BackgroundImage = renderTarget;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (actor != null)
            {
                gameTimeElapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (gameTimeElapsed > gameSpeed)
                {
                    //MoveActorDown();
                    gameTimeElapsed = 0;
                } 
            }

            UpdateEffectsArray();
        }

        public override void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            graphicsDevice.Clear(BorderColor);

            spriteBatch.Draw(
                Contents.Textures.Pix,
                new Rectangle(playgroundInnerPadding - 2 * playgroundCubeMargin, playgroundInnerPadding - 2 * playgroundCubeMargin, (int)Size.X  - 2 * (playgroundInnerPadding - 2 * playgroundCubeMargin), (int)Size.Y - 2 * (playgroundInnerPadding - 2 * playgroundCubeMargin)),
                BackgroundColor);

            for (int x = 0; x < playground.GetLength(0); x++)
            {
                for (int y = 0; y < playground.GetLength(1); y++)
                {
                    spriteBatch.Draw(
                        Contents.Textures.Pix,
                        new Rectangle(playgroundInnerPadding + x * (playgroundCubeSize + playgroundCubeMargin), playgroundInnerPadding + y * (playgroundCubeSize + playgroundCubeMargin), playgroundCubeSize, playgroundCubeSize), 
                        GetCubeColor(x,y));
                }
            }

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public void MoveDown()
        {
        }

        public void MoveRight()
        {
        }

        public void MoveLeft()
        {
        }

        public void Rotate()
        {
            CreateActor();
        }

        private void MoveActorDown()
        {
            actorPosition.Y++;
        }

        private void CreateActor()
        {
            actor = new bool[3, 2];
            actor[0, 0] = true;
            actor[1, 0] = true;
            actor[2, 0] = true;
            actor[0, 1] = false;
            actor[1, 1] = true;
            actor[2, 1] = false;

            actorColor = Contents.Colors.GameCubesColors[Game1.Random.Next(1, Contents.Colors.GameCubesColors.Count - 1)];
            actorPosition = new Point(Game1.Random.Next(0, playground.GetLength(0) - actor.GetLength(0)), 0);
            gameTimeElapsed = 0;
        }

        private void UpdateEffectsArray()
        {
            playgroundEffectsArray = new Color?[Settings.GameArenaSize.Width, Settings.GameArenaSize.Height];
            for (int x = 0; x < actor.GetLength(0); x++)
            {
                for (int y = 0; y < actor.GetLength(1); y++)
                {
                    if (actor[x,y])
                    {
                        playgroundEffectsArray[actorPosition.X + x, actorPosition.Y + y] = actorColor;
                    }
                }
            }
        }

        private Color GetCubeColor(int posX, int posY)
        {
            var result = Contents.Colors.GameCubesColors[playground[posX, posY]];
            //if (playground[posX, posY] == 0)
            //    result *= 0.5f;

            if (playgroundEffectsArray != null && playgroundEffectsArray[posX, posY].HasValue)
                result = playgroundEffectsArray[posX, posY].Value;

            return result;
        }
    }
}