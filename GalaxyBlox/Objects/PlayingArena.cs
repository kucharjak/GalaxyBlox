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
using static GalaxyBlox.Static.Settings;
using static GalaxyBlox.Static.SettingOptions;

namespace GalaxyBlox.Objects
{
    class PlayingArena : GameObject
    {
        public event EventHandler ActiveBonusChanged;
        protected virtual void OnActiveBonusChange(ActiveBonusChangedEventArgs e)
        {
            EventHandler handler = ActiveBonusChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler AvailableBonusesChanged;
        protected virtual void OnAvailableBonusesChange(AvailableBonusesChangeEventArgs e)
        {
            EventHandler handler = AvailableBonusesChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

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

        private Vector2 arenaSize;
        private SettingOptions.GameMode gameMode;

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

        private Actor activeActor = null;
        private ActorMovement activeActorMovement;
        private List<Actor> actors;
        private int actorsMaxCount = 1;
        private List<Actor> actorsQueue;
        private int actorsQueueSize = 2;

        private int actorCreateTimer = 0;
        private int actorCreatePeriod = 1500;

        private int[,] playground;
        private HashSet<Tuple<int, int, Color>> playgroundEffectsList = new HashSet<Tuple<int, int, Color>>();
        private int playgroundInnerPadding;
        private int playgroundCubeSize;
        public int CubeSize { get { return playgroundCubeSize; } }
        private int playgroundCubeMargin;
        public int CubeMargin { get { return playgroundCubeMargin; } }

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

        private List<GameBonus> gameBonuses;
        private int maxBonuses = 3;
        private int timeSinceLastBonus = 0;
        private int timeUntilFreeBonus = freeBonusTimeLimit;
        private const int freeBonusTimeLimit = 1;

        private GameBonus activeBonus;
        /// <summary>
        /// Active bonus for game - assign value here if you wish to cause external change event
        /// </summary>
        public GameBonus ActiveBonus
        {
            get { return activeBonus; }
            protected set { activeBonus = value; OnActiveBonusChange(new ActiveBonusChangedEventArgs(value)); }
        }

        private int slowDownTimer;
        private int slowDownMultiplier = 5;

        private Point laserPosition;
        private int laserWidth = 2;
        private Actor lastActiveActor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parentRoom"></param>
        /// <param name="size"></param>
        /// <param name="position"></param>
        public PlayingArena(Room parentRoom, Vector2 size, Vector2 position, SettingOptions.GameMode gameMode, Vector2 arenaSize) : base(parentRoom)
        {
            this.gameMode = gameMode;
            this.arenaSize = arenaSize;

            BackgroundColor = Contents.Colors.PlaygroundColor;
            BorderColor = Contents.Colors.PlaygroundBorderColor;
            Alpha = 1f;

            playgroundInnerPadding = 4;
            playgroundCubeMargin = 1;

            var spaceLeftForCubes = new Vector2(
                (int)(size.X - 2 * playgroundInnerPadding - (arenaSize.X - 1) * playgroundCubeMargin),
                (int)(size.Y - 2 * playgroundInnerPadding - (arenaSize.Y - 1) * playgroundCubeMargin));

            if (spaceLeftForCubes.X / arenaSize.X < spaceLeftForCubes.Y / arenaSize.Y)
            { // WIDTH
                playgroundCubeSize = (int)(spaceLeftForCubes.X / arenaSize.X);
            }
            else
            { // HEIGHT
                playgroundCubeSize = (int)(spaceLeftForCubes.Y / arenaSize.Y);
            }

            Size = new Vector2(
                (arenaSize.X - 1) * (playgroundCubeSize + playgroundCubeMargin) + playgroundCubeSize + 2 * playgroundInnerPadding,
                (arenaSize.Y - 1) * (playgroundCubeSize + playgroundCubeMargin) + playgroundCubeSize + 2 * playgroundInnerPadding);

            Position = new Vector2(
                position.X + ((size.X - Size.X) / 2),
                position.Y + ((size.Y - Size.Y) / 2));  //(position.Y + size.Y) - Size.Y);

            mainRenderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)Size.X, (int)Size.Y);
            backgroundRenderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)Size.X, (int)Size.Y);
            BackgroundImage = mainRenderTarget;

            actors = new List<Actor>();
            actorsQueue = new List<Actor>();

            gameBonuses = new List<GameBonus>();

            if (gameMode == GameMode.Extreme)
                actorsMaxCount = 5;

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

            if (gameMode != GameMode.Classic)
            {
                if (gameBonuses.Count == 0 && ActiveBonus == GameBonus.None)
                    timeSinceLastBonus += gameTime.ElapsedGameTime.Milliseconds;

                if (timeSinceLastBonus >= timeUntilFreeBonus)
                {
                    AddBonus();
                    timeUntilFreeBonus = freeBonusTimeLimit;
                }

                switch (ActiveBonus)
                {
                    case GameBonus.TimeSlowdown:
                        {
                            slowDownTimer -= gameTime.ElapsedGameTime.Milliseconds;

                            if (slowDownTimer <= 0)
                                DeactivateBonus();
                        }
                        break;

                }
            }

            if (ActiveBonus == GameBonus.None || ActiveBonus == GameBonus.TimeSlowdown)
            {
                actorCreateTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (actorCreateTimer > actorCreatePeriod)
                {
                    CreateNewActor();
                    actorCreateTimer = 0;
                }

                MoveActiveActorToSide();

                foreach (var actor in actors.ToArray())
                {
                    actor.Timer += gameTime.ElapsedGameTime.Milliseconds;
                    if (actor.Timer > actor.FallingSpeed)
                    {
                        MoveActorDown(actor);
                        actor.Timer = 0;
                    }
                }
            } else
            {
                switch (ActiveBonus)
                {
                    case GameBonus.Laser:
                        {
                            MoveLaser();
                        }
                        break;
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
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
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

        // Controls

        public void ControlLeft_Down()
        {
            MoveActorLeft();
        }

        public void ControlLeft_Up()
        {
            StopMovingActor();
        }

        public void ControlRight_Down()
        {
            MoveActorRight();
        }

        public void ControlRight_Up()
        {
            StopMovingActor();
        }

        public void ControlDown_Click()
        {
            if (ActiveBonus != GameBonus.Laser)
                MakeActorFall();
            else
                Bonus_Laser_MakeAction();
        }

        public void ControlDown_Down()
        {
            MakeActorSpeedup();
        }

        public void ControlDown_Up()
        {
            SlowDownActor();
        }

        public void ControlRotate_Click()
        {
            RotateActor();
        }

        public void Control_ActivateBonus(GameBonus bonusToActivate)
        {
            ActivateBonus(bonusToActivate);
        }

        public void StartNewGame()
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
        }

        // Private actions

        private void MoveActorRight()
        {
            activeActorMovement = ActorMovement.Right;
        }

        private void MoveActorLeft()
        {
            activeActorMovement = ActorMovement.Left;
        }

        private void StopMovingActor()
        {
            activeActorMovement = ActorMovement.None;
            moveTimer = 0;
            moveTimerSpeed = 0;
        }
        
        private void RotateActor()
        {
            if (activeActor == null)
                return;

            var rotatedActorShape = RotateActor(activeActor.Shape);
            var rotatedActorPosition = activeActor.Position;
            if (rotatedActorShape.GetLength(0) + activeActor.Position.X > playground.GetLength(0))
                rotatedActorPosition.X = playground.GetLength(0) - rotatedActorShape.GetLength(0);

            if (ActorCollide(rotatedActorPosition, rotatedActorShape))
                return;

            activeActor.Shape = rotatedActorShape;
            activeActor.Position = rotatedActorPosition;
        }

        private void MakeActorSpeedup()
        {
            if (activeActor == null || activeActor.IsFalling || fallingPause > 0)
                return;

            activeActor.FallingSpeed = GetGameSpeed(GameSpeed.Speedup);
        }

        private void SlowDownActor()
        {
            if (activeActor == null || activeActor.IsFalling)
                return;

            activeActor.FallingSpeed = GetGameSpeed(GameSpeed.Normal);
        }

        private void MakeActorFall()
        {
            if (activeActor == null || fallingPause > 0)
                return;

            activeActor.IsFalling = true;
            activeActor.FallingSpeed = GetGameSpeed(GameSpeed.Falling);
        }

        // private bonus activations

        private void Bonus_SlowDown_Activate()
        {
            ActiveBonus = GameBonus.TimeSlowdown;
            foreach (var actor in actors)
            {
                actor.FallingSpeed = actor.FallingSpeed * slowDownMultiplier; // make actor x times slower from their previous speed TODO: optimaze speed
            }

            slowDownTimer = 5000;

            BorderColor = Color.Red;
            backgroundChanged = true;
        }

        private void Bonus_SlowDown_Deactivate()
        {
            ActiveBonus = GameBonus.None;
            foreach (var actor in actors)
            {
                if (!actor.IsFalling)
                    actor.FallingSpeed = GetGameSpeed(GameSpeed.Normal);
            }
            slowDownTimer = 0;

            BorderColor = Contents.Colors.PlaygroundBorderColor;
            backgroundChanged = true;
        }

        private void Bonus_Laser_Activate()
        {
            ActiveBonus = GameBonus.Laser;

            laserPosition = new Point((playground.GetLength(0) - laserWidth) / 2, 0);
            lastActiveActor = activeActor;
            activeActor = null;
        }

        private void Bonus_Laser_MakeAction()
        {
            LaserActors();
            LaserPlayground();
            backgroundChanged = true;

            DeactivateBonus();
        }

        private void Bonus_Laser_Deactivate()
        {
            ActiveBonus = GameBonus.None;

            if (actors.Contains(lastActiveActor))
                activeActor = lastActiveActor;
            else
            {
                activeActor = actors.FirstOrDefault();

                if (activeActor == null)
                    CreateNewActor();
            }
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

        private void MoveActiveActorToSide()
        {
            if (activeActor == null || activeActorMovement == ActorMovement.None || moveTimer > 0)
                return;

            var newPos = activeActorMovement == ActorMovement.Left ? new Point(activeActor.Position.X - 1, activeActor.Position.Y) : new Point(activeActor.Position.X + 1, activeActor.Position.Y);
            var tmpActor = new Actor(activeActor.Shape, newPos, Color.White);
            if (ActorCollideWithPlayground(tmpActor))
                return;

            //var nonActiveActors = actors.Where(act => act != activeActor).ToList();
            //Actor collidedActor;
            //if (ActorsCollide(tmpActor, nonActiveActors, out collidedActor))
            //{ // if collide - try to find new position for that actor
            //    var tmpNonActiveActor = new Actor(activeActor.Shape, collidedActor.Position, Color.White);
            //    var tmpActors = actors.Where(act => act != collidedActor).ToList();
            //    bool found = false;

            //    for (int x = 0; x < tmpActor.Shape.GetLength(0) + 1; x++)
            //    {
            //        if (activeActorMovement == ActorMovement.Left)
            //            tmpNonActiveActor.Position.X = tmpActor.Position.X + x;
            //        else
            //            tmpNonActiveActor.Position.X = tmpActor.Position.X + tmpActor.Shape.GetLength(0) - tmpNonActiveActor.Shape.GetLength(0) - x;

            //        if (!ActorsCollide(tmpNonActiveActor, tmpActors))
            //        {
            //            collidedActor.Position = tmpNonActiveActor.Position;
            //            found = true;
            //            break;
            //        }
            //    }

            //    if (!found)
            //        return;
            //}

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

        private void MoveActorDown(Actor actor)
        {
            var newPosition = new Point(actor.Position.X, actor.Position.Y + 1);

            if (!ActorCollide(newPosition, actor.Shape))
            {
                actor.Position = newPosition;
            }
            else
            {
                InsertActorToPlayground(actor);
                actors.Remove(actor);

                if (actor == activeActor)
                    activeActor = actors.Count > 0 ? actors.First() : null;
                
                CheckGameOver();
                CheckPlaygroundForFullLines();

                if (actors.Count == 0)
                    CreateNewActor();
            }
        }

        private void MoveLaser()
        {
            if (activeActorMovement == ActorMovement.None ||  moveTimer > 0)
                return;

            if (activeActorMovement == ActorMovement.Left)
            {
                if (laserPosition.X - 1 >= 0)
                    laserPosition.X--;
            } else
            {
                if (laserPosition.X + laserWidth + 1 <= playground.GetLength(0))
                    laserPosition.X++;
            }

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

        private void LaserActors()
        {
            var laserRect = new Rectangle(laserPosition, new Point(laserWidth, playground.GetLength(1)));
            foreach (var actor in actors.ToList())
            {
                var rect = new Rectangle(actor.Position, actor.Size);
                if (rect.Intersects(laserRect))
                {
                    if (rect.X >= laserPosition.X)
                    {
                        var IsDestroyed = rect.X + rect.Width - laserPosition.X <= laserWidth;
                        if (IsDestroyed)
                        {
                            actors.Remove(actor);
                            continue;
                        }
                    }

                    Point newSize;
                    bool[,] newShape;
                    Point startPosition;
                    if (laserPosition.X > rect.X)
                    {
                        newSize = new Point(laserPosition.X - rect.X, rect.Height);
                        startPosition = new Point(0, 0);
                    }
                    else
                    {
                        newSize = new Point((rect.X + rect.Width) - (laserPosition.X + laserWidth), rect.Height);
                        startPosition = new Point(laserWidth - (rect.X - laserPosition.X), 0);
                    }

                    newShape = new bool[newSize.X, newSize.Y];
                    for (int x = 0; x < newSize.X; x++)
                    {
                        for (int y = 0; y < newSize.Y; y++)
                        {
                            newShape[x, y] = actor.Shape[startPosition.X + x, startPosition.Y + y];
                        }
                    }

                    actor.Position += startPosition;
                    actor.Shape = newShape;
                }
            }
        }

        private void LaserPlayground()
        {
            var laserRect = new Rectangle(laserPosition, new Point(laserWidth, playground.GetLength(1)));
            for (int x = laserRect.X; x < laserRect.X + laserRect.Width; x++)
            {
                for (int y = laserRect.Y; y < laserRect.Y + laserRect.Height; y++)
                {
                    playground[x, y] = 0;
                }
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
            if (!Settings.Game.Highscores.Items.ContainsKey(gameMode))
            {
                Settings.Game.Highscores.SaveHighScore(gameMode, new List<Score>() { new Score("Test", score) });
            }
            else
            {
                var highscores = Settings.Game.Highscores.Items[gameMode];
                if (highscores.Any(scr => scr.Value < score))
                {
                    highscores.Add(new Score("Test", score));
                    highscores = highscores.OrderByDescending(scr => scr.Value).ToList();

                    while (highscores.Count > Settings.Game.MaxHighscoresPerGameMod)
                    {
                        highscores.RemoveAt(highscores.Count - 1);
                    }
                    Settings.Game.Highscores.SaveHighScore(gameMode, highscores);
                }
            }
            Settings.Game.SaveHighscores();

            var size = new Vector2(600, 350);
            var gameOverRoom = new GameOverRoom(ParentRoom, "Room_GameOver", size, new Vector2((ParentRoom.Size.X - size.X) / 2, (ParentRoom.Size.Y - size.Y) / 2), score);
            gameOverRoom.Show();
            gameOverRoom.Closed += GameOverRoom_Closed;
        }

        private void GameOverRoom_Closed(object sender, EventArgs e)
        {
            OnGameEnd(new EventArgs());
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

                    for (int x = 0; x < playground.GetLength(0); x++)
                    {
                        playground[x, playgroundPosY] = playground[x, y];

                        if (y != playgroundPosY) // i want to relocate cube, not to remove it
                            playground[x, y] = 0;
                    }

                    playgroundPosY--;
                }
                backgroundChanged = true; // need to indicate, that i changed backgrou, so game will redraw it
                //LogPlaygroundChanges(oldPlayground, playground);

                if (fullLines.Count >= 4)
                {
                    AddBonus();
                }
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

        private void InsertActorToPlayground(Actor actor)
        {
            var actorBoxes = new int[actor.Shape.GetLength(0), actor.Shape.GetLength(1)];
            var actorColorPos = Contents.Colors.GameCubesColors.IndexOf(actor.Color);
            if (actorColorPos < 1)
                actorColorPos = 1;

            for (int x = 0; x < actor.Shape.GetLength(0); x++)
            {
                for (int y = 0; y < actor.Shape.GetLength(1); y++)
                {
                    if (actor.Shape[x, y])
                    {
                        actorBoxes[x, y] = actorColorPos;
                    }
                }
            }

            InsertBoxesToPlayground(actor.Position, actorBoxes);
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

        private bool ActorCollideWithPlayground(Actor actor)
        {
            return ActorCollide(actor.Position, actor.Shape);
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
            if (actors.Count >= actorsMaxCount)
                return;

            if (actorsQueue.Count < actorsQueueSize)
            {
                var actorsToFullQueue = actorsQueueSize - actorsQueue.Count;
                for (int i = 0; i < actorsToFullQueue; i++)
                {
                    var newActorShape = Contents.Shapes.GetRandomShape();
                    newActorShape = RotateActor(newActorShape, Game1.Random.Next(0, 3), true);
                    actorsQueue.Add(new Actor(newActorShape, new Point(), Contents.Colors.GameCubesColors[Game1.Random.Next(1, Contents.Colors.GameCubesColors.Count)]));
                }
            }
            
            var actor = actorsQueue.First();

            var positionsWithoutOtherActors = new List<Point>(); // i won't place new actor on other existing actor
            for (int i = 0; i < playground.GetLength(0) + 1 - actor.Shape.GetLength(0); i++)
            {
                actor.Position = new Point(i, 0);
                if (!ActorsCollide(actor, actors))
                    positionsWithoutOtherActors.Add(actor.Position);
            }
            actor.Position = new Point();

            if (positionsWithoutOtherActors.Count > 0)
            {
                actorsQueue.Remove(actor);
                actor.Position = positionsWithoutOtherActors[Game1.Random.Next(0, positionsWithoutOtherActors.Count - 1)];
                actor.IsFalling = false;
                actor.FallingSpeed = GetGameSpeed(GameSpeed.Normal);
                actors.Add(actor);
                fallingPause = 150;

                if (activeActor == null)
                    activeActor = actor;

                var nextActorInQueue = actorsQueue.FirstOrDefault();
                if (nextActorInQueue != null)
                    OnActorsQueueChange(new QueueChangeEventArgs(nextActorInQueue.Shape, nextActorInQueue.Color));
                else
                    OnActorsQueueChange(new QueueChangeEventArgs(null, Color.White));
            }
        }

        private bool ActorsCollide(Actor newActor, List<Actor> actorsList)
        {
            Actor tmpActor = null;
            return ActorsCollide(newActor, actorsList, out tmpActor);
        }

        private bool ActorsCollide(Actor newActor, List<Actor> actorsList, out Actor collidedActor)
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

        private void UpdateLevel()
        {
            var level = 0;
            var scoreCap = 0;
            while (scoreCap < Score)
            {
                level++;
                scoreCap += level * 50;
            }
            Level = level;
        }

        /// <summary>
        /// Game speed is defined by game score
        /// </summary>
        private int GetGameSpeed(GameSpeed gameSpeedSetting)
        {
            var maxFallingSpeed = 250;
            var fallingSpeed = 1000;
            switch (gameSpeedSetting)
            {
                case GameSpeed.Normal:
                    fallingSpeed = (int)(1000 - Math.Pow(Level, 2)); // TODO test more speeds
                    if (fallingSpeed < maxFallingSpeed)
                        fallingSpeed = maxFallingSpeed;

                    if (ActiveBonus == GameBonus.TimeSlowdown)
                        fallingSpeed = fallingSpeed * slowDownMultiplier; // if is slowdown activated return slower speeds

                    break;
                case GameSpeed.Speedup:
                    fallingSpeed = 50; break;
                case GameSpeed.Falling:
                    fallingSpeed = 5; break;
            }

            return fallingSpeed;
        }

        private void AddBonus()
        {
            if (gameBonuses.Count < maxBonuses)
            {
                //var bonusIndex = Game1.Random.Next(1, Enum.GetValues(typeof(GameBonus)).Length);
                //gameBonuses.Add((GameBonus)bonusIndex);
                gameBonuses.Add(GameBonus.Laser);
                timeSinceLastBonus = 0;
                RefreshBonuses();
            }
        }

        private void ActivateBonus(GameBonus bonusToActivate)
        {
            var bonus = gameBonuses.Where(bns => bns == bonusToActivate);
            if (bonus.Count() == 0)
            {
                RefreshBonuses();
                return;
            }
            
            switch (bonusToActivate)
            {
                case GameBonus.TimeSlowdown:
                    {
                        Bonus_SlowDown_Activate();
                    } break;
                case GameBonus.Laser:
                    {
                        Bonus_Laser_Activate();
                    } break;
            }

            gameBonuses.Remove(bonus.First());
            RefreshBonuses();
        }

        private void DeactivateBonus()
        {
            switch (ActiveBonus)
            {
                case GameBonus.TimeSlowdown:
                    {
                        Bonus_SlowDown_Deactivate();
                    }
                    break;
                case GameBonus.Laser:
                    {
                        Bonus_Laser_Deactivate();
                    } break;
            }
            RefreshBonuses();
        }

        private void RefreshBonuses()
        {
            OnAvailableBonusesChange(new AvailableBonusesChangeEventArgs(gameBonuses, ActiveBonus == GameBonus.None));
        }

        private void UpdateEffectsArray()
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

            if (ActiveBonus == GameBonus.Laser)
            {
                for (int x = laserPosition.X; x < laserPosition.X + laserWidth; x++)
                {
                    for (int y = laserPosition.Y; y < playground.GetLength(1); y++)
                    {
                        playgroundEffectsList.Add(new Tuple<int, int, Color>(x, y, Color.Red * 0.35f));
                    }
                }
            }
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
                            if (ActorCollide(shadowPosition, activeActor.Shape))
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

        enum ActorMovement
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