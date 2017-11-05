﻿using System;
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
        private long score = 0;
        public long Score
        {
            get { return score; }
            set { score = value; }
        }

        private bool[,] actor;
        private Color actorColor;
        private Point actorPosition;

        private int[,] playground;
        private Color?[,] playgroundEffectsArray;
        private int playgroundInnerPadding;
        private int playgroundCubeSize;
        private int playgroundCubeMargin;

        private int gameSpeed; // move actor in 1000 ms (= 1 s)
        private int gameTimeElapsed = 0;
        private bool actorFalling;

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
            
            renderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)Size.X, (int)Size.Y);
            BackgroundImage = renderTarget;

            StartNewGame();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (actor != null)
            {
                gameTimeElapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (gameTimeElapsed > gameSpeed)
                {
                    MoveActorDown();
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

        public void StartNewGame()
        {
            playground = new int[Settings.GameArenaSize.Width, Settings.GameArenaSize.Height];
            playgroundEffectsArray = new Color?[Settings.GameArenaSize.Width, Settings.GameArenaSize.Height];
            CreateNewActor();
        }

        public void SlowDownActor()
        {
            if (actorFalling)
                return;

            SetGameSpeed(SettingOptions.GameSpeed.Normal);
        }

        public void MakeActorSpeedup()
        {
            if (actorFalling)
                return;

            SetGameSpeed(SettingOptions.GameSpeed.Speedup);
        }

        public void MakeActorFall()
        {
            actorFalling = true;
            SetGameSpeed(SettingOptions.GameSpeed.Falling);
        }

        public void MoveRight()
        {
            var newPosition = new Point(actorPosition.X + 1, actorPosition.Y);
            if (ActorCollide(newPosition, actor))
                return;

            actorPosition = newPosition;
        }

        public void MoveLeft()
        {
            var newPosition = new Point(actorPosition.X - 1, actorPosition.Y);
            if (ActorCollide(newPosition, actor))
                return;

            actorPosition = newPosition;
        }

        public void Rotate()
        {
            var newActor = RotateActor(actor);
            var newPosition = actorPosition;
            if (newActor.GetLength(0) + actorPosition.X > playground.GetLength(0))
                newPosition.X = playground.GetLength(0) - newActor.GetLength(0);

            if (ActorCollide(newPosition, newActor))
                return;

            actor = newActor;
            actorPosition = newPosition;
        }

        // Private methods

        private bool[,] RotateActor(bool[,] actorArray, int nTimes)
        {
            var resultActor = new bool[actorArray.GetLength(0), actorArray.GetLength(1)];
            Array.Copy(actorArray, resultActor, actorArray.GetLength(0) * actorArray.GetLength(1));

            for (int times = 0; times < nTimes; times++)
            {
                resultActor = RotateActor(resultActor);
            }

            return resultActor;
        }

        private bool[,] RotateActor(bool[,] actorArray)
        {
            var resultArray = new bool[actorArray.GetLength(1), actorArray.GetLength(0)];

            for (int x = 0; x < actorArray.GetLength(0); x++)
            {
                for (int y = 0; y < actorArray.GetLength(1); y++)
                {
                    resultArray[(actorArray.GetLength(1) - 1) - y, x] = actorArray[x, y];
                }
            }

            return resultArray;
        }

        private void MoveActorDown()
        {
            var newPosition = new Point(actorPosition.X, actorPosition.Y + 1);

            if (!ActorCollide(newPosition, actor))
            {
                actorPosition = newPosition;
            }
            else
            {
                InsertActorToPlayground();
                CheckGameOver();
                CheckPlaygroundForFullLines();
                CreateNewActor();
            }
        }

        private void CheckGameOver()
        {
            for (int x = 0; x < playground.GetLength(0); x++)
            {
                if (playground[x, 0] > 0)
                { // call game over
                    StartNewGame();
                }
            }
        }

        private void CheckPlaygroundForFullLines()
        {
            var fullLines = new List<int>();
            for (int y = 0; y < playground.GetLength(1); y++)
            {
                var fullLine = true;
                for (int x = 0; x < playground.GetLength(0); x++)
                {
                    if (playground[x, y] == 0)
                    {
                        fullLine = false;
                        break;
                    }
                }

                if (fullLine)
                    fullLines.Add(y);
            }

            if (fullLines.Count > 0)
            {
                // count score
                // TODO

                // refil playground with-out fullLines
                var playgroundPosY = playground.GetLength(1) - 1;
                for (int y = playground.GetLength(1) - 1; y >= fullLines.Count; y--)
                {
                    if (fullLines.Contains(y))
                    {
                        for (int x = 0; x < playground.GetLength(0); x++)
                        {
                            playground[x, y] = 0;
                        }

                        continue;
                    }

                    for(int x = 0; x < playground.GetLength(0); x++)
                    {
                        playground[x, playgroundPosY] = playground[x, y];
                    }

                    playgroundPosY--;
                }
            }
        }

        private void InsertActorToPlayground()
        {
            var actorBoxes = new int[actor.GetLength(0), actor.GetLength(1)];
            var actorColorPos = Contents.Colors.GameCubesColors.IndexOf(actorColor);
            if (actorColorPos < 1)
                actorColorPos = 1;

            for (int x = 0; x < actor.GetLength(0); x++)
            {
                for (int y = 0; y < actor.GetLength(1); y++)
                {
                    if (actor[x, y])
                    {
                        actorBoxes[x, y] = actorColorPos;
                    }
                }
            }

            InsertBoxesToPlayground(actorPosition, actorBoxes);
        }

        private void InsertBoxesToPlayground(Point boxesPosition, int[,] boxesArray, bool insertZeros = false)
        {
            for (int x = 0; x < boxesArray.GetLength(0); x++)
            {
                for (int y = 0; y < boxesArray.GetLength(1); y++)
                {
                    if (boxesArray[x, y] > 0 || insertZeros)
                    {
                        var boxPosition = new Point(boxesPosition.X + x, boxesPosition.Y + y);
                        if (boxPosition.X < playground.GetLength(0) && boxPosition.Y < playground.GetLength(1) && boxPosition.X >= 0 && boxPosition.Y >= 0) 
                        { 
                            playground[boxPosition.X, boxPosition.Y] = boxesArray[x, y];
                        }
                    }
                }
            }
        }
        
        private bool ActorCollide(Point actorPosition, bool[,] actorArray)
        {
            for (int x = 0; x < actorArray.GetLength(0); x++)
            {
                for (int y = 0; y < actorArray.GetLength(1); y++)
                {
                    if (actorArray[x, y])
                    {
                        var boxPosition = new Point(actorPosition.X + x, actorPosition.Y + y);
                        if (boxPosition.X >= playground.GetLength(0) || 
                            boxPosition.Y >= playground.GetLength(1) || 
                            boxPosition.X < 0 ||
                            boxPosition.Y < 0 ||
                            playground[boxPosition.X, boxPosition.Y] > 0)
                        { // collision happened
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void CreateNewActor()
        {
            actor = Contents.Shapes.GetRandomShape();
            actor = RotateActor(actor, Game1.Random.Next(0, 3)); // rotate actor randomly for variation and funzies
            actorColor = Contents.Colors.GameCubesColors[Game1.Random.Next(1, Contents.Colors.GameCubesColors.Count)];
            actorPosition = new Point(Game1.Random.Next(0, playground.GetLength(0) - actor.GetLength(0) + 1), 0);
            
            gameTimeElapsed = 0;
            actorFalling = false;
            SetGameSpeed(SettingOptions.GameSpeed.Normal);
        }

        /// <summary>
        /// Game speed is defined by game score
        /// </summary>
        private void SetGameSpeed(SettingOptions.GameSpeed gameSpeedSetting)
        {
            switch(gameSpeedSetting)
            {
                case SettingOptions.GameSpeed.Normal:  gameSpeed = 1000; break;
                case SettingOptions.GameSpeed.Speedup: gameSpeed = 50; break;
                case SettingOptions.GameSpeed.Falling: gameSpeed = 1; break;
            }
        }

        private void UpdateEffectsArray()
        {
            playgroundEffectsArray = new Color?[Settings.GameArenaSize.Width, Settings.GameArenaSize.Height]; // create new effects array

            if (Settings.Indicator != SettingOptions.Indicator.None) // draw indicator if set
                DrawIndicator();

            DrawActor(actor, actorPosition, actorColor);
        }

        private void DrawActor(bool[,] actorToDraw, Point positionToDraw, Color colorToDraw)
        {
            for (int x = 0; x < actorToDraw.GetLength(0); x++)
            {
                for (int y = 0; y < actorToDraw.GetLength(1); y++)
                {
                    if (actorToDraw[x, y])
                    {
                        playgroundEffectsArray[positionToDraw.X + x, positionToDraw.Y + y] = colorToDraw;
                    }
                }
            }
        }

        private void DrawIndicator()
        {
            switch(Settings.Indicator)
            {
                case SettingOptions.Indicator.Shadow:
                    {
                        for (int actorX = 0; actorX < actor.GetLength(0); actorX++)
                        {
                            var startPosition = new Point(actorPosition.X + actorX, actorPosition.Y);

                            bool foundActor = false;
                            for (int actorY = actor.GetLength(1) - 1; actorY >= 0; actorY--)
                            {
                                if (actor[actorX, actorY])
                                {
                                    foundActor = true;
                                    startPosition.Y = actorPosition.Y + (actorY + 1);
                                    break;
                                }
                            }
                            if (!foundActor)
                                continue;

                            if (startPosition.Y == actorPosition.Y)
                                startPosition.Y = actorPosition.Y + actor.GetLength(1);

                            for (int y = startPosition.Y; y < playgroundEffectsArray.GetLength(1); y++)
                            {
                                if (playground[startPosition.X, y] == 0)
                                    playgroundEffectsArray[startPosition.X, y] = Contents.Colors.IndicatorColor;
                                else
                                    break;
                            }
                        }
                    } break;
                case SettingOptions.Indicator.Shape:
                    {
                        var shadowPosition = actorPosition;

                        for (int y = shadowPosition.Y; y < playground.GetLength(1); y++)
                        {
                            shadowPosition.Y = y;
                            if (ActorCollide(shadowPosition, actor))
                            {
                                shadowPosition.Y--;
                                break;
                            }
                        }
                        if (shadowPosition.Y >= 0)
                            DrawActor(actor, shadowPosition, Contents.Colors.IndicatorColor);

                    } break;
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