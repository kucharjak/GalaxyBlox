using System;
using System.Collections.Generic;
using System.Linq;
using GalaxyBlox.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GalaxyBlox.EventArgsClasses;
using GalaxyBlox.Models;
using GalaxyBlox.Rooms;
using static GalaxyBlox.Static.Settings;
using static GalaxyBlox.Static.SettingOptions;
using GalaxyBlox.Utils;

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

        public event EventHandler GameEnded;
        protected virtual void OnGameEnd(EventArgs e)
        {
            EventHandler handler = GameEnded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected Vector2 arenaSize;
        protected SettingOptions.GameMode gameMode;

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

        private int level = 0;
        public int Level
        {
            get { return level; }
            set
            {
                level = value;
                OnScoreChange(new EventArgs());
            }
        }

        public bool IsPaused;
        private int resumeTimer;

        protected Actor activeActor = null;
        protected ActorMovement activeActorMovement;
        protected List<Actor> actors;
        protected int actorsMaxCount = 1;
        protected List<Actor> actorsQueue;
        protected int actorsQueueSize = 2;

        protected int[,] playground;
        protected HashSet<Tuple<int, int, Color>> playgroundEffectsList = new HashSet<Tuple<int, int, Color>>();
        protected int playgroundInnerPadding;
        protected int playgroundCubeSize;
        public int CubeSize { get { return playgroundCubeSize; } }
        protected int playgroundCubeMargin;
        public int CubeMargin { get { return playgroundCubeMargin; } }

        protected int fallingPause = 0; // to avoid miss clicks

        protected int moveTimer = 0;
        protected int moveTimerSpeed = 0;
        protected const int moveTimerFastest = 50;
        protected const int moveTimerSlowest = 150;

        //private List<Point> playgroundChanges = new List<Point>();

        protected Vector2 backgroundSize;
        protected bool backgroundChanged;
        protected bool backgroundFirstDraw;
        protected Color BackgroundColor;
        protected Color BorderColor;
        protected RenderTarget2D backgroundRenderTarget;
        protected RenderTarget2D mainRenderTarget;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parentRoom"></param>
        /// <param name="size"></param>
        /// <param name="position"></param>
        public PlayingArena(Room parentRoom, Vector2 size, Vector2 position) : base(parentRoom)
        {
            InitializeArenaSettings();
            
            BackgroundColor = Contents.Colors.PlaygroundColor;
            BorderColor = Contents.Colors.PlaygroundBorderColor;
            Alpha = 1f;

            playgroundInnerPadding = 0;
            playgroundCubeMargin = 1;

            Size = size;
            Position = position;

            var displayRect = DisplayRect();

            var spaceLeftForCubes = new Vector2(
                (int)(displayRect.Size.X - 2 * playgroundInnerPadding - (arenaSize.X - 1) * playgroundCubeMargin),
                (int)(displayRect.Size.Y - 2 * playgroundInnerPadding - (arenaSize.Y - 1) * playgroundCubeMargin));

            if (spaceLeftForCubes.X / arenaSize.X < spaceLeftForCubes.Y / arenaSize.Y)
            { // WIDTH
                playgroundCubeSize = (int)(spaceLeftForCubes.X / arenaSize.X);
            }
            else
            { // HEIGHT
                playgroundCubeSize = (int)(spaceLeftForCubes.Y / arenaSize.Y);
            }

            backgroundSize = new Vector2(
                (arenaSize.X - 1) * (playgroundCubeSize + playgroundCubeMargin) + playgroundCubeSize + 2 * playgroundInnerPadding,
                (arenaSize.Y - 1) * (playgroundCubeSize + playgroundCubeMargin) + playgroundCubeSize + 2 * playgroundInnerPadding
                );

            Size = new Vector2(
                (float)Math.Ceiling(backgroundSize.X / ParentRoom.Scale),
                (float)Math.Ceiling(backgroundSize.Y / ParentRoom.Scale)
                );

            Position = new Vector2(
                (float)Math.Ceiling(position.X + ((size.X - Size.X) / 2)),
                (float)Math.Ceiling(position.Y + ((size.Y - Size.Y) / 2))
                );

            //mainRenderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)backgroundSize.X, (int)backgroundSize.Y);
            //backgroundRenderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)backgroundSize.X, (int)backgroundSize.Y);
            //BackgroundImage = mainRenderTarget;

            actors = new List<Actor>();
            actorsQueue = new List<Actor>();
        }

        protected virtual void InitializeArenaSettings()
        {
            arenaSize = new Vector2(12, 20);
        }

        public override void Update(GameTime gameTime)
        {
            if (resumeTimer > 0)
            {
                resumeTimer -= gameTime.ElapsedGameTime.Milliseconds;

                if (resumeTimer <= 0)
                    IsPaused = false;
            }

            if (IsPaused)
                return;

            base.Update(gameTime);

            if (fallingPause > 0)
                fallingPause -= gameTime.ElapsedGameTime.Milliseconds;

            if (fallingPause < 0)
                fallingPause = 0;

            if (moveTimer > 0)
                moveTimer -= gameTime.ElapsedGameTime.Milliseconds;

            if (moveTimer < 0)
                moveTimer = 0;

            UpdateEffectsArray();
        }

        public override void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (backgroundChanged)
            {
                if (backgroundRenderTarget == null || backgroundRenderTarget.IsContentLost || backgroundRenderTarget.IsDisposed)
                {
                    if (backgroundRenderTarget != null)
                    {
                        backgroundRenderTarget.Dispose();
                        backgroundRenderTarget = null;
                    }

                    backgroundRenderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)backgroundSize.X, (int)backgroundSize.Y);
                }

                graphicsDevice.SetRenderTarget(backgroundRenderTarget);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                graphicsDevice.Clear(BorderColor);

                spriteBatch.Draw(
                    Contents.Textures.Pix,
                    new Rectangle(playgroundInnerPadding - 2 * playgroundCubeMargin, playgroundInnerPadding - 2 * playgroundCubeMargin, (int)backgroundSize.X - 2 * (playgroundInnerPadding - 2 * playgroundCubeMargin), (int)backgroundSize.Y - 2 * (playgroundInnerPadding - 2 * playgroundCubeMargin)),
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

            if (mainRenderTarget == null || mainRenderTarget.IsContentLost || mainRenderTarget.IsDisposed)
            {
                if (mainRenderTarget != null)
                {
                    mainRenderTarget.Dispose();
                    mainRenderTarget = null;
                }
                
                mainRenderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)backgroundSize.X, (int)backgroundSize.Y);
                BackgroundImage = mainRenderTarget;
            }

            graphicsDevice.SetRenderTarget(mainRenderTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Draw(
                        backgroundRenderTarget,
                        new Rectangle(0, 0, (int)backgroundSize.X, (int)backgroundSize.Y),
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

        // Controls, events

        public virtual void ControlLeft_Down()
        {
            MoveActorLeft();
        }

        public virtual void ControlLeft_Up()
        {
            StopMovingActor();
        }

        public virtual void ControlRight_Down()
        {
            MoveActorRight();
        }

        public virtual void ControlRight_Up()
        {
            StopMovingActor();
        }

        public virtual void ControlDown_Click()
        {
            MakeActorFall();
        }

        public virtual void ControlDown_Down()
        {
            MakeActorSpeedup();
        }

        public virtual void ControlDown_Up()
        {
            SlowDownActor();
        }

        public virtual void ControlRotate_Click()
        {
            RotateActor();
        }

        public virtual void StartNewGame()
        {
            backgroundChanged = true;
            backgroundFirstDraw = true;
            playground = new int[(int)arenaSize.X, (int)arenaSize.Y];
            playgroundEffectsList.Clear();
            Score = 0;
            activeActor = null;
            actors.Clear();
            actorsQueue.Clear();
            CreateNewActor();
            IsPaused = false;
        }

        public virtual void Pause()
        {
            IsPaused = true;
        }

        public virtual void Resume(int resumeTimer = 0)
        {
            if (resumeTimer > 0)
            {
                this.resumeTimer = resumeTimer;
            }
            else
                IsPaused = false; 
        }

        protected virtual void GameOverRoom_Closed(object sender, EventArgs e)
        {
            OnGameEnd(new EventArgs());
        }

        // private/protected actions

        protected virtual void MoveActorRight()
        {
            activeActorMovement = ActorMovement.Right;
        }

        protected virtual void MoveActorLeft()
        {
            activeActorMovement = ActorMovement.Left;
        }

        protected virtual void StopMovingActor()
        {
            activeActorMovement = ActorMovement.None;
            moveTimer = 0;
            moveTimerSpeed = 0;
        }

        protected virtual void RotateActor()
        {
            if (activeActor == null)
                return;

            var rotatedActorShape = RotateActor(activeActor.Shape);
            var rotatedActorPosition = activeActor.Position;
            if (rotatedActorShape.GetLength(0) + activeActor.Position.X > playground.GetLength(0))
                rotatedActorPosition.X = playground.GetLength(0) - rotatedActorShape.GetLength(0);

            if (ActorCollideWithPlayground(rotatedActorPosition, rotatedActorShape))
                return;

            activeActor.Shape = rotatedActorShape;
            activeActor.Position = rotatedActorPosition;
        }

        protected virtual void MakeActorSpeedup()
        {
            if (activeActor == null || activeActor.IsFalling || fallingPause > 0)
                return;

            activeActor.FallingSpeed = GetGameSpeed(GameSpeed.Speedup);
        }

        protected virtual void SlowDownActor()
        {
            if (activeActor == null || activeActor.IsFalling)
                return;

            activeActor.FallingSpeed = GetGameSpeed(GameSpeed.Normal);
        }

        protected virtual void MakeActorFall()
        {
            if (activeActor == null || fallingPause > 0)
                return;

            activeActor.IsFalling = true;
            activeActor.FallingSpeed = GetGameSpeed(GameSpeed.Falling);
        }

        protected virtual bool[,] RotateActor(bool[,] actorArray, int nTimes, bool randomlyFlip = false)
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

        protected virtual bool[,] FlipActorVertically(bool[,] actorArray)
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

        protected virtual bool[,] FlipActorHorizontally(bool[,] actorArray)
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

        protected virtual bool[,] RotateActor(bool[,] actorArray)
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

        protected virtual void MoveActiveActorToSide()
        {
            if (activeActor == null || activeActorMovement == ActorMovement.None || moveTimer > 0)
                return;

            var newPos = activeActorMovement == ActorMovement.Left ? new Point(activeActor.Position.X - 1, activeActor.Position.Y) : new Point(activeActor.Position.X + 1, activeActor.Position.Y);
            var tmpActor = new Actor(activeActor.Shape, newPos, Color.White);
            if (ActorCollideWithPlayground(tmpActor))
                return;

            activeActor.Position = newPos;
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

        protected virtual void MoveActorDown(Actor actor)
        {
            var newPosition = new Point(actor.Position.X, actor.Position.Y + 1);

            if (!ActorCollideWithPlayground(newPosition, actor.Shape))
            {
                actor.Position = newPosition;
            }
            else
            {
                InsertActorToPlayground(actor);
                actors.Remove(actor);

                if (actor == activeActor)
                {
                    activeActor = actors.Count > 0 ? actors.First() : null;
                    var nextActorInQueue = actors.Count > 1 ? actors[1] : actorsQueue.FirstOrDefault();
                    if (nextActorInQueue != null)
                        OnActorsQueueChange(new QueueChangeEventArgs(nextActorInQueue));
                    else
                        OnActorsQueueChange(new QueueChangeEventArgs(null, Color.White));
                }

                CheckGameOver();

                int[] linesDestroyed;
                if (CheckPlaygroundForFullLines(out linesDestroyed))
                {
                    DestroyFullLines(linesDestroyed);
                    IncreaseScoreForLines(linesDestroyed.Count());
                    Vibrations.Vibrate(25 * linesDestroyed.Count());
                }

                if (actors.Count == 0)
                    CreateNewActor();
            }
        }

        protected virtual void CheckGameOver()
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

        protected virtual void GameOver()
        {
            var isNewHighscore = false;

            if (score > 0)
            {
                if (!Settings.Game.Highscores.Items.ContainsKey(gameMode))
                {
                    isNewHighscore = true;
                }
                else
                {
                    var highscores = Settings.Game.Highscores.Items[gameMode];
                    if (highscores.Count < Settings.Game.MaxHighscoresPerGameMod || highscores.Any(scr => scr.Value < score))
                    {
                        isNewHighscore = true;
                    }
                }
            }

            var gameOverRoom = new GameOverRoom(ParentRoom, "Room_GameOver", Vector2.Zero, Vector2.Zero, score, gameMode, isNewHighscore);
            gameOverRoom.Closed += GameOverRoom_Closed;
            gameOverRoom.Show();
        }

        protected virtual bool CheckPlaygroundForFullLines(out int[] fullLines)
        {
            var tmpfullLines = new List<int>();
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
                    tmpfullLines.Add(y);
            }
            fullLines = tmpfullLines.ToArray();
            return fullLines.Count() > 0;
        }

        protected virtual void DestroyFullLines(int[] fullLines)
        {
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

                for (int x = 0; x < playground.GetLength(0); x++)
                {
                    playground[x, playgroundPosY] = playground[x, y];

                    if (y != playgroundPosY) // i want to relocate cube, not to remove it
                        playground[x, y] = 0;
                }

                playgroundPosY--;
            }
            backgroundChanged = true; // need to indicate, that i changed backgrou, so game will redraw it
        }

        protected virtual void IncreaseScoreForLines(int linesDestroyed)
        {
            //var oldPlayground = (int[,])playground.Clone();
            // count score
            Score += (long)(Math.Pow(linesDestroyed, 3) * playground.GetLength(0)); // TODO: Test speed of score growt and decide which would be the best

            var newLevel = 0;
            var scoreCap = 0;
            while (scoreCap < Score)
            {
                newLevel++;
                scoreCap += newLevel * 50;
            }
            Level = newLevel;
        }

        protected virtual void UpdateLevel()
        {
        }
        
        protected virtual void RemoveActorFromPlayground(Actor actor)
        {
            var actorCubes = new List<Tuple<int, int, int>>();

            for (int x = 0; x < actor.Shape.GetLength(0); x++)
            {
                for (int y = 0; y < actor.Shape.GetLength(1); y++)
                {
                    if (actor.Shape[x, y])
                        actorCubes.Add(new Tuple<int, int, int>(x + actor.Position.X, y + actor.Position.Y, 0));
                }
            }

            InsertBoxesToPlayground(actorCubes);
        }

        protected virtual void InsertActorToPlayground(Actor actor)
        {
            var actorCubes = new List<Tuple<int, int, int>>();
            var actorColorPos = Contents.Colors.GameCubesColors.IndexOf(actor.Color);

            for (int x = 0; x < actor.Shape.GetLength(0); x++)
            {
                for (int y = 0; y < actor.Shape.GetLength(1); y++)
                {
                    if (actor.Shape[x, y])
                        actorCubes.Add(new Tuple<int, int, int>(x + actor.Position.X, y + actor.Position.Y, actorColorPos));
                }
            }

            InsertBoxesToPlayground(actorCubes);
        }

        /// <summary>
        /// Adds index, that represents color, to playground.
        /// </summary>
        /// <param name="cubes">List of cubes defined in tuple where values are - posX, posY, colorIndex</param>
        protected virtual void InsertBoxesToPlayground(List<Tuple<int, int, int>> cubes)
        {
            foreach (var cube in cubes)
            {
                if (cube.Item1 < playground.GetLength(0) && cube.Item1 >= 0
                    && cube.Item2 < playground.GetLength(1) && cube.Item2 >= 0)
                    playground[cube.Item1, cube.Item2] = cube.Item3;
            }
            backgroundChanged = true;
        }

        protected virtual void InsertBoxesToPlayground(Point boxesPosition, int[,] boxesArray, bool insertZeros = false)
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

        protected virtual bool ActorCollideWithPlayground(Actor actor)
        {
            return ActorCollideWithPlayground(actor.Position, actor.Shape);
        }

        protected virtual bool ActorCollideWithPlayground(Point actorPosition, bool[,] actorArray)
        {
            for (int x = 0; x < actorArray.GetLength(0); x++)
            {
                for (int y = 0; y < actorArray.GetLength(1); y++)
                {
                    if (actorArray[x, y])
                    {
                        var boxPosition = new Point(actorPosition.X + x, actorPosition.Y + y);

                        if (boxPosition.Y < 0 &&
                            boxPosition.X < playground.GetLength(0) &&
                            boxPosition.X >= 0)
                            continue;

                        if (boxPosition.X >= playground.GetLength(0) ||
                            boxPosition.Y >= playground.GetLength(1) ||
                            boxPosition.X < 0 ||
                            //boxPosition.Y < 0 ||
                            playground[boxPosition.X, boxPosition.Y] > 0)
                        { // collision happened
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected virtual void CreateNewActor()
        {
            if (actors.Count >= actorsMaxCount)
                return;

            if (actorsQueue.Count < actorsQueueSize)
            {
                var actorsToFullQueue = actorsQueueSize - actorsQueue.Count;
                for (int i = 0; i < actorsToFullQueue; i++)
                {
                    var newActorShape = GetRandomShape();
                    newActorShape = RotateActor(newActorShape, Game1.Random.Next(0, 3), true);
                    actorsQueue.Add(new Actor(newActorShape, new Point(), Contents.Colors.GameCubesColors[Game1.Random.Next(1, Contents.Colors.GameCubesColors.Count)]));
                }
            }

            var actor = actorsQueue.First();

            actorsQueue.Remove(actor);
            actor.Position = GetNewActorPosition(actor);
            actor.IsFalling = false;
            actor.FallingSpeed = GetGameSpeed(GameSpeed.Normal);
            actors.Add(actor);
            fallingPause = 150;

            if (activeActor == null)
                activeActor = actor;

            var nextActorInQueue = actors.Count > 1 ? actors[1] :actorsQueue.FirstOrDefault();
            if (nextActorInQueue != null)
                OnActorsQueueChange(new QueueChangeEventArgs(nextActorInQueue));
            else
                OnActorsQueueChange(new QueueChangeEventArgs(null, Color.White));
        }

        protected virtual bool[,] GetRandomShape()
        {
            return Contents.Shapes.GetRandomShape();
        }

        protected virtual Point GetNewActorPosition(Actor actor)
        {
            return new Point(Game1.Random.Next(0, playground.GetLength(0) + 1 - actor.Shape.GetLength(0)), 0);
        }

        protected virtual bool ActorCollideActors(Actor newActor, List<Actor> actorsList)
        {
            Actor tmpActor = null;
            return ActorCollideActors(newActor, actorsList, out tmpActor);
        }

        protected virtual bool ActorCollideActors(Actor newActor, List<Actor> actorsList, out Actor collidedActor)
        {
            collidedActor = null;
            var newRect = new Rectangle(newActor.Position, new Point(newActor.Shape.GetLength(1), newActor.Shape.GetLength(0)));
            foreach (var actor in actorsList)
            {
                var rect = new Rectangle(actor.Position, new Point(actor.Shape.GetLength(1), actor.Shape.GetLength(0)));
                if (newRect.Intersects(rect)) // easiest collision control - there is no reason for it to be much more complex
                {
                    collidedActor = actor;
                    return true;
                }
                    
            }
            return false;
        }

        /// <summary>
        /// Game speed is defined by game score
        /// </summary>
        protected virtual int GetGameSpeed(GameSpeed gameSpeedSetting)
        {
            return 0;
        }

        protected virtual void UpdateEffectsArray()
        {
            playgroundEffectsList.Clear();

            if (gameMode != SettingOptions.GameMode.Classic && Settings.Game.UserSettings.Indicator != SettingOptions.Indicator.None) // draw indicator if set
                DrawIndicator();

            foreach (var actor in actors)
            {
                if (actor != activeActor)
                    DrawActor(actor.Shape, actor.Position, Color.Lerp(actor.Color, Color.Black, Contents.Colors.NonActiveColorFactor));
            }

            if (activeActor != null)
                DrawActor(activeActor.Shape, activeActor.Position, activeActor.Color);
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
                    }
                }
            }
        }

        private void DrawIndicator()
        {
            if (activeActor == null)
                return;

            switch (Settings.Game.UserSettings.Indicator)
            {
                case SettingOptions.Indicator.Shadow:
                    {
                        for (int actorX = 0; actorX < activeActor.Shape.GetLength(0); actorX++)
                        {
                            var startPosition = new Point(activeActor.Position.X + actorX, activeActor.Position.Y);

                            bool foundActor = false;
                            for (int actorY = activeActor.Shape.GetLength(1) - 1; actorY >= 0; actorY--)
                            {
                                if (activeActor.Shape[actorX, actorY])
                                {
                                    foundActor = true;
                                    startPosition.Y = activeActor.Position.Y + (actorY + 1);
                                    break;
                                }
                            }
                            if (!foundActor)
                                continue;

                            if (startPosition.Y == activeActor.Position.Y)
                                startPosition.Y = activeActor.Position.Y + activeActor.Shape.GetLength(1);

                            for (int y = startPosition.Y; y < playground.GetLength(1); y++)
                            {
                                if (y < 0)
                                    continue;

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
                        var shadowPosition = activeActor.Position;

                        for (int y = shadowPosition.Y; y < playground.GetLength(1); y++)
                        {
                            shadowPosition.Y = y;
                            if (ActorCollideWithPlayground(shadowPosition, activeActor.Shape))
                            {
                                shadowPosition.Y--;
                                break;
                            }
                        }
                        if (shadowPosition.Y >= 0)
                            DrawActor(activeActor.Shape, shadowPosition, Contents.Colors.IndicatorColor);

                    } break;
            }
        }

        private Color GetCubeColor(int posX, int posY)
        {
            var result = Contents.Colors.GameCubesColors[playground[posX, posY]];
            return result;
        }

        protected enum ActorMovement
        {
            None,
            Right,
            Left
        }
    }

    class Actor
    {
        public bool[,] Shape;
        public Color Color;
        public Point Position;
        public Point Size { get { return new Point(Shape.GetLength(0), Shape.GetLength(1)); } }

        public int Timer;
        public int FallingSpeed;
        public bool IsFalling;

        public Actor(bool [,] shape, Point position, Color color, int fallingSpeed = 0)
        {
            Shape = shape;
            Position = position;
            Color = color;
            FallingSpeed = fallingSpeed;
            Timer = 0;
        }
    }
}