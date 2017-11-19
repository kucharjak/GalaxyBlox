using System;
using System.Collections.Generic;
using System.Linq;
using GalaxyBlox.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Android.Util;
using GalaxyBlox.EventArgsClasses;
using GalaxyBlox.Models;
using GalaxyBlox.Rooms;

namespace GalaxyBlox.Objects
{
    class PlayingArena : GameObject
    {
        public event EventHandler ActorsQueueChanged;
        protected virtual void OnActorsQueueChange(QueueChangeEventArgs e)
        {
            EventHandler handler = ActorsQueueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler ScoreChanged;
        protected virtual void OnScoreChange(EventArgs e)
        {
            EventHandler handler = ScoreChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private Size arenaSize;
        private string gameMode;

        private long score = 0;
        public long Score
        {
            get { return score; }
            set
            {
                score = value;
                UpdateLevel();
                OnScoreChange(new EventArgs());
            }
        }

        public int Level { get; set; } = 0;

        private bool[,] actor;
        private Color actorColor;
        private Point actorPosition;
        private List<Tuple<bool[,], Color>> actorsQueue;
        private int actorsQueueSize = 2;

        private int[,] playground;
        private List<Tuple<int, int, Color>> playgroundEffectsList = new List<Tuple<int, int, Color>>();
        private int playgroundInnerPadding;
        private int playgroundCubeSize;
        public int CubeSize { get { return playgroundCubeSize; } }
        private int playgroundCubeMargin;
        public int CubeMargin { get { return playgroundCubeMargin; } }
        
        private int gameSpeed; // move actor in 1000 ms (= 1 s)
        private int gameTimeElapsed = 0;
        private bool actorFalling;
        private int fallingPause = 0; // to avoid miss clicks

        private int moveTimer = 0;
        private int moveTimerSpeed = 0;
        private int moveTimerFastest = 50;
        private int moveTimerSlowest = 150;

        //private List<Point> playgroundChanges = new List<Point>();

        private bool backgroundChanged;
        private bool backgroundFirstDraw;
        private Color BackgroundColor;
        private Color BorderColor;
        private RenderTarget2D backgroundRenderTarget;
        private RenderTarget2D mainRenderTarget;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parentRoom"></param>
        /// <param name="size"></param>
        /// <param name="position"></param>
        public PlayingArena(Room parentRoom, Vector2 size, Vector2 position) : base(parentRoom)
        {
            gameMode = "Test";
            arenaSize = Settings.Game.ArenaSize;

            BackgroundColor = Contents.Colors.PlaygroundColor;
            BorderColor = Contents.Colors.PlaygroundBorderColor;
            Alpha = 1f;
            
            playgroundInnerPadding = 4;
            playgroundCubeMargin = 1;
            
            var spaceLeftForCubes = new Size(
                (int)(size.X - 2 * playgroundInnerPadding - (arenaSize.Width - 1) * playgroundCubeMargin),
                (int)(size.Y - 2 * playgroundInnerPadding - (arenaSize.Height - 1) * playgroundCubeMargin));

            if (spaceLeftForCubes.Width / (float)arenaSize.Width < spaceLeftForCubes.Height / (float)arenaSize.Height)
            { // WIDTH
                playgroundCubeSize = spaceLeftForCubes.Width / arenaSize.Width;
            }
            else
            { // HEIGHT
                playgroundCubeSize = spaceLeftForCubes.Height / arenaSize.Height;
            }

            Size = new Vector2(
                (arenaSize.Width - 1) * (playgroundCubeSize + playgroundCubeMargin) + playgroundCubeSize + 2 * playgroundInnerPadding,
                (arenaSize.Height - 1) * (playgroundCubeSize + playgroundCubeMargin) + playgroundCubeSize + 2 * playgroundInnerPadding);

            Position = new Vector2(
                0, //(size.X - Size.X) / 2,
                (position.Y + size.Y) - Size.Y);
            
            mainRenderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)Size.X, (int)Size.Y);
            backgroundRenderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)Size.X, (int)Size.Y);
            BackgroundImage = mainRenderTarget;

            actorsQueue = new List<Tuple<bool[,], Color>>();

            StartNewGame();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (fallingPause > 0)
                fallingPause -= gameTime.ElapsedGameTime.Milliseconds;

            if (fallingPause < 0)
                fallingPause = 0;

            if (moveTimer > 0)
                moveTimer -= gameTime.ElapsedGameTime.Milliseconds;

            if (moveTimer < 0)
                moveTimer = 0;

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
            if (backgroundChanged)
            {
                graphicsDevice.SetRenderTarget(backgroundRenderTarget);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                graphicsDevice.Clear(BorderColor);

                spriteBatch.Draw(
                    Contents.Textures.Pix,
                    new Rectangle(playgroundInnerPadding - 2 * playgroundCubeMargin, playgroundInnerPadding - 2 * playgroundCubeMargin, (int)Size.X - 2 * (playgroundInnerPadding - 2 * playgroundCubeMargin), (int)Size.Y - 2 * (playgroundInnerPadding - 2 * playgroundCubeMargin)),
                    BackgroundColor);

                for (int x = 0; x < playground.GetLength(0); x++)
                {
                    for (int y = 0; y < playground.GetLength(1); y++)
                    {
                        spriteBatch.Draw(
                            Contents.Textures.Pix,
                            new Rectangle(playgroundInnerPadding + x * (playgroundCubeSize + playgroundCubeMargin), playgroundInnerPadding + y * (playgroundCubeSize + playgroundCubeMargin), playgroundCubeSize, playgroundCubeSize),
                            GetCubeColor(x, y));
                    }
                }

                spriteBatch.End();
                graphicsDevice.SetRenderTarget(null);
                backgroundChanged = false;
            }

            //    if (backgroundFirstDraw || backgroundRenderTarget.IsContentLost)
            //    {
            //        graphicsDevice.SetRenderTarget(backgroundRenderTarget);
            //        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            //        graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            //        graphicsDevice.Clear(BorderColor);

            //        spriteBatch.Draw(
            //            Contents.Textures.Pix,
            //            new Rectangle(playgroundInnerPadding - 2 * playgroundCubeMargin, playgroundInnerPadding - 2 * playgroundCubeMargin, (int)Size.X - 2 * (playgroundInnerPadding - 2 * playgroundCubeMargin), (int)Size.Y - 2 * (playgroundInnerPadding - 2 * playgroundCubeMargin)),
            //            BackgroundColor);

            //        for (int x = 0; x < playground.GetLength(0); x++)
            //        {
            //            for (int y = 0; y < playground.GetLength(1); y++)
            //            {
            //                spriteBatch.Draw(
            //                    Contents.Textures.Pix,
            //                    new Rectangle(playgroundInnerPadding + x * (playgroundCubeSize + playgroundCubeMargin), playgroundInnerPadding + y * (playgroundCubeSize + playgroundCubeMargin), playgroundCubeSize, playgroundCubeSize),
            //                    GetCubeColor(x, y));
            //            }
            //        }

            //        spriteBatch.End();
            //        graphicsDevice.SetRenderTarget(null);
            //        backgroundChanged = false;

            //        backgroundFirstDraw = false;
            //    }
            //    else
            //    {
            //        using (var tempRenderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)Size.X, (int)Size.Y))
            //        {
            //            graphicsDevice.SetRenderTarget(tempRenderTarget);
            //            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            //            spriteBatch.Draw(backgroundRenderTarget, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), null, Color.White);
            //            spriteBatch.End();

            //            graphicsDevice.SetRenderTarget(backgroundRenderTarget);
            //            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque); //, BlendState.AlphaBlend

            //            spriteBatch.Draw(tempRenderTarget, new Rectangle(0, 0, (int)Size.X, (int)Size.Y), Color.White);

            //            foreach (var change in playgroundChanges)
            //            { 
            //                    spriteBatch.Draw(
            //                        Contents.Textures.Pix,
            //                        new Rectangle(playgroundInnerPadding + change.X * (playgroundCubeSize + playgroundCubeMargin) - playgroundCubeMargin, playgroundInnerPadding + change.Y * (playgroundCubeSize + playgroundCubeMargin) - playgroundCubeMargin, playgroundCubeSize + 2* playgroundCubeMargin, playgroundCubeSize + 2 * playgroundCubeMargin),
            //                        BackgroundColor);
            //                    spriteBatch.Draw(
            //                        Contents.Textures.Pix,
            //                        new Rectangle(playgroundInnerPadding + change.X * (playgroundCubeSize + playgroundCubeMargin), playgroundInnerPadding + change.Y * (playgroundCubeSize + playgroundCubeMargin), playgroundCubeSize, playgroundCubeSize),
            //                        Contents.Colors.GameCubesColors[playground[change.X, change.Y]]);
            //            }
            //            playgroundChanges.Clear();

            //            spriteBatch.End();
            //            graphicsDevice.SetRenderTarget(null);
            //            backgroundChanged = false;
            //        }
            //    }
            //}

            graphicsDevice.SetRenderTarget(mainRenderTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Draw(
                        backgroundRenderTarget,
                        new Rectangle(0, 0, (int)Size.X, (int)Size.Y),
                        Color.White);

            foreach (var effect in playgroundEffectsList)
            {
                spriteBatch.Draw(
                        Contents.Textures.Pix,
                        new Rectangle(playgroundInnerPadding + effect.Item1 * (playgroundCubeSize + playgroundCubeMargin), playgroundInnerPadding + effect.Item2 * (playgroundCubeSize + playgroundCubeMargin), playgroundCubeSize, playgroundCubeSize),
                        effect.Item3);
            }

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        public void StartNewGame()
        {
            backgroundChanged = true;
            backgroundFirstDraw = true;
            playground = new int[arenaSize.Width, arenaSize.Height];
            playgroundEffectsList.Clear();
            Score = 0;
            actorsQueue = new List<Tuple<bool[,], Color>>();
            CreateNewActor();
        }

        public void SlowDownActor()
        {
            if (actorFalling)
                return;

            SetGameSpeed(GameSpeed.Normal);
        }

        public void MakeActorSpeedup()
        {
            if (actorFalling || fallingPause > 0)
                return;

            SetGameSpeed(GameSpeed.Speedup);
        }

        public void MakeActorFall()
        {
            if (fallingPause > 0)
                return;

            actorFalling = true;
            SetGameSpeed(GameSpeed.Falling);
        }

        public void MoveRight()
        {
            if (moveTimer > 0)
                return;

            var newPosition = new Point(actorPosition.X + 1, actorPosition.Y);
            if (ActorCollide(newPosition, actor))
                return;

            actorPosition = newPosition;
            if (moveTimerSpeed == 0)
            {
                moveTimer = moveTimerSlowest;
                moveTimerSpeed = moveTimerSlowest;
            }
            else
            {
                var newSpeed = moveTimerSpeed - ((moveTimerSlowest - moveTimerFastest) / 4);
                moveTimer = newSpeed > moveTimerFastest ? newSpeed : moveTimerSpeed;
                moveTimerSpeed = newSpeed > moveTimerFastest ? newSpeed : moveTimerSpeed;
            }
            
        }

        public void StopMovingRight()
        {
            moveTimer = 0;
            moveTimerSpeed = 0;
        }

        public void MoveLeft()
        {
            if (moveTimer > 0)
                return;

            var newPosition = new Point(actorPosition.X - 1, actorPosition.Y);
            if (ActorCollide(newPosition, actor))
                return;

            actorPosition = newPosition;
            if (moveTimerSpeed == 0)
            {
                moveTimer = moveTimerSlowest;
                moveTimerSpeed = moveTimerSlowest;
            }
            else
            {
                var newSpeed = moveTimerSpeed - ((moveTimerSlowest - moveTimerFastest) / 4);
                moveTimer = newSpeed > moveTimerFastest ? newSpeed : moveTimerSpeed;
                moveTimerSpeed = newSpeed > moveTimerFastest ? newSpeed : moveTimerSpeed;
            }
        }

        public void StopMovingLeft()
        {
            moveTimer = 0;
            moveTimerSpeed = 0;
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

        private bool[,] RotateActor(bool[,] actorArray, int nTimes, bool randomlyFlip = false)
        {
            var resultActor = new bool[actorArray.GetLength(0), actorArray.GetLength(1)];
            Array.Copy(actorArray, resultActor, actorArray.GetLength(0) * actorArray.GetLength(1));

            if (randomlyFlip)
            {
                if (Game1.Random.Next(1, 100) % 2 == 0)
                {
                    resultActor = FlipActorVertically(resultActor);
                }
            }

            for (int times = 0; times < nTimes; times++)
            {
                resultActor = RotateActor(resultActor);
            }

            return resultActor;
        }

        private bool[,] FlipActorVertically(bool[,] actorArray)
        {
            var resultArray = new bool[actorArray.GetLength(0), actorArray.GetLength(1)];
            var actorArrayHeight = actorArray.GetLength(1) - 1;

            for (int x = 0; x < actorArray.GetLength(0); x++)
            {
                for (int y = 0; y < actorArray.GetLength(1); y++)
                {
                    resultArray[x, y] = actorArray[x, actorArrayHeight - y];
                }
            }

            return resultArray;
        }

        private bool[,] FlipActorHorizontally(bool[,] actorArray)
        {
            var resultArray = new bool[actorArray.GetLength(0), actorArray.GetLength(1)];
            var actorArrayWidth = actorArray.GetLength(0) - 1;

            for (int x = 0; x < actorArray.GetLength(0); x++)
            {
                for (int y = 0; y < actorArray.GetLength(1); y++)
                {
                    resultArray[x, y] = actorArray[actorArrayWidth - x, y];
                }
            }

            return resultArray;
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
                    GameOver();
                    break;
                }
            }
        }

        private void GameOver()
        {
            if (!Settings.Game.User.HighScores.ContainsKey(gameMode))
            {
                Settings.Game.User.SaveHighScore(gameMode, new List<long>() { score });
                Settings.Game.SaveUser();
            }
            else
            {
                var highscores = Settings.Game.User.HighScores[gameMode];
                if (highscores.Any(scr => scr < score))
                {
                    highscores.Add(score);
                    highscores = highscores.OrderByDescending(scr => scr).ToList();
                    
                    while(highscores.Count > Settings.Game.MaxHighscoresPerGameMod)
                    {
                        highscores.RemoveAt(highscores.Count - 1);
                    }
                    Settings.Game.User.SaveHighScore(gameMode, highscores);
                    Settings.Game.SaveUser();
                }
            }

            var size = new Size(400, 250);
            var gameOverRoom = new GameOverRoom(ParentRoom, "Gameover_Room", size, new Vector2((ParentRoom.Size.Width - size.Width) / 2, (ParentRoom.Size.Height - size.Height) / 2), score);
            gameOverRoom.Show();
            gameOverRoom.Closed += GameOverRoom_Closed;
        }

        private void GameOverRoom_Closed(object sender, EventArgs e)
        {
            StartNewGame();
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
                //var oldPlayground = (int[,])playground.Clone();
                // count score
                Score += (long)(Math.Pow(fullLines.Count, 3) * playground.GetLength(0)); // TODO: Test speed of score growt and decide which would be the best

                // refil playground with-out fullLines
                var playgroundPosY = playground.GetLength(1) - 1;
                for (int y = playground.GetLength(1) - 1; y >= 0; y--)
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

                        if (y != playgroundPosY) // i want to relocate cube, not to remove it
                            playground[x, y] = 0;
                    }

                    playgroundPosY--;
                }
                backgroundChanged = true; // need to indicate, that i changed backgrou, so game will redraw it
                //LogPlaygroundChanges(oldPlayground, playground);
            }
        }

        //private void LogPlaygroundChanges(int[,] oldPlayground, int[,] newPlayground)
        //{
        //    for (int x = 0; x < oldPlayground.GetLength(0); x++)
        //    {
        //        for (int y = 0; y < oldPlayground.GetLength(1); y++)
        //        {
        //            if (oldPlayground[x, y] != newPlayground[x, y])
        //                playgroundChanges.Add(new Point(x, y));
        //        }
        //    }
        //}

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
                            //playgroundChanges.Add(new Point(boxPosition.X, boxPosition.Y)); // log change for redraw
                        }
                    }
                }
            }
            backgroundChanged = true; // indicating for background redraw
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
            if (actorsQueue.Count < actorsQueueSize)
            {
                var actorsToFullQueue = actorsQueueSize - actorsQueue.Count;
                for (int i = 0; i < actorsToFullQueue; i++)
                {
                    var nextActor = Contents.Shapes.GetRandomShape();
                    nextActor = RotateActor(nextActor, Game1.Random.Next(0, 3), true);
                    actorsQueue.Add(new Tuple<bool[,], Color>(nextActor, Contents.Colors.GameCubesColors[Game1.Random.Next(1, Contents.Colors.GameCubesColors.Count)]));
                }
            }

            //actor = Contents.Shapes.GetRandomShape();
            //actor = RotateActor(actor, Game1.Random.Next(0, 3)); // rotate actor randomly for variation and funzies
            //actorColor = Contents.Colors.GameCubesColors[Game1.Random.Next(1, Contents.Colors.GameCubesColors.Count)];

            var actorFromQueue= actorsQueue.First();
            actor = actorFromQueue.Item1;
            actorColor = actorFromQueue.Item2;
            actorPosition = new Point(Game1.Random.Next(0, playground.GetLength(0) - actor.GetLength(0) + 1), 0);
            actorsQueue.Remove(actorFromQueue);
            OnActorsQueueChange(new QueueChangeEventArgs(actorsQueue.FirstOrDefault()?.Item1, actorsQueue.FirstOrDefault() != null ? actorsQueue.FirstOrDefault().Item2 : Color.White));

            gameTimeElapsed = 0;
            actorFalling = false;
            SetGameSpeed(GameSpeed.Normal);
            fallingPause = 150;
        }

        private void UpdateLevel()
        {
            var level = 0;
            var scoreCap = 0;
            while (scoreCap  < Score)
            {
                level++;
                scoreCap += level * 50;
            }
            Level = level;
        }

        /// <summary>
        /// Game speed is defined by game score
        /// </summary>
        private void SetGameSpeed(GameSpeed gameSpeedSetting)
        {
            var maxFallingSpeed = 250;
            switch(gameSpeedSetting)
            {
                case GameSpeed.Normal:
                    gameSpeed = (int)(1000 - Math.Pow(Level, 2)); // TODO test more speeds
                    if (gameSpeed < maxFallingSpeed)
                        gameSpeed = maxFallingSpeed;
                    break;
                case GameSpeed.Speedup:
                    gameSpeed = 50; break;
                case GameSpeed.Falling:
                    gameSpeed = 1; break;
            }
        }

        private void UpdateEffectsArray()
        {
            //playgroundEffectsArray = new Color?[Settings.GameArenaSize.Width, Settings.GameArenaSize.Height]; // create new effects array
            playgroundEffectsList.Clear();

            if (Settings.Game.User.Indicator != SettingOptions.Indicator.None) // draw indicator if set
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
                        playgroundEffectsList.Add(new Tuple<int, int, Color>(positionToDraw.X + x, positionToDraw.Y + y, colorToDraw));
                        //playgroundEffectsArray[positionToDraw.X + x, positionToDraw.Y + y] = colorToDraw;
                    }
                }
            }
        }

        private void DrawIndicator()
        {
            switch(Settings.Game.User.Indicator)
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

                            for (int y = startPosition.Y; y < playground.GetLength(1); y++)
                            {
                                if (playground[startPosition.X, y] == 0)
                                {
                                    playgroundEffectsList.Add(new Tuple<int, int, Color>(startPosition.X, y, Contents.Colors.IndicatorColor));
                                    //playgroundEffectsArray[startPosition.X, y] = Contents.Colors.IndicatorColor;
                                }
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
            return result;
        }
        
        public enum GameSpeed
        {
            Normal,
            Speedup,
            Falling
        }
    }
}