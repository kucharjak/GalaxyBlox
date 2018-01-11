using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using GalaxyBlox.Static;
using Microsoft.Xna.Framework;
using static GalaxyBlox.Static.SettingOptions;
using GalaxyBlox.EventArgsClasses;
using GalaxyBlox.Utils;

namespace GalaxyBlox.Objects.PlayingArenas
{
    class PlayingArena_Normal : PlayingArena
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

        protected int actorCreateTimer = 0;
        protected int actorCreatePeriod;

        protected int newActorStartingPositionXPadding;

        protected List<GameBonus> gameBonuses;
        protected int maxBonuses;
        protected int timeSinceLastBonus = 0;
        protected int timeUntilFreeBonus;
        protected int freeBonusTimeLimit;

        //protected List<BonusType> availableBonuses = new List<BonusType>() { BonusType.CubesExplosion, BonusType.CancelLastCube, BonusType.Laser, BonusType.SwipeCubes, BonusType.TimeSlowdown };
        protected List<GameBonus> availableBonuses;

        protected GameBonus activeBonus;
        /// <summary>
        /// Active bonus for game - assign value here if you wish to cause external change event
        /// </summary>
        public GameBonus ActiveBonus
        {
            get { return activeBonus; }
            protected set { activeBonus = value; OnActiveBonusChange(new ActiveBonusChangedEventArgs(value)); }
        }

        protected int slowDownLimit;
        protected int slowDownTimer;
        protected int slowDownPower;

        protected Point laserPosition;
        protected int laserWidth;
        protected Actor lastActiveActor;

        protected Actor lastPlayedActor;
        public Actor LastPlayedActor
        {
            get { return lastPlayedActor; }
            protected set
            {
                var changed = (lastPlayedActor == null && value != null) || (lastPlayedActor != null && value == null);
                lastPlayedActor = value;

                if (changed)
                    RefreshBonuses();
            }
        }

        protected int cubesExplosionPower;
        protected int cubesExplosionExtraPower;
        protected int cubesExplosionExtraPowerProb;
        protected int cubesExplosionFallingSpeed;

        protected int baseCubeScore;
        protected float scoreLevelMultiplier;

        protected long[] scoreForLevelArray;
        protected int maxLevel;
        protected float baseFourLineLimit;
        protected float maxFourLineLimit;
        protected float increaseFourLineLimitPerLevel;

        protected int baseFallingSpeed;
        protected int minFallingSpeed;
        protected float fallingSpeedFactor;
        protected float increaseFallingSpeedLevelCount;

        protected bool bonusSwipeCubesLeft;

        public PlayingArena_Normal(Room parentRoom, Vector2 size, Vector2 position) : base(parentRoom, size, position)
        {
        }

        protected override void InitializeArenaSettings() 
        {
            gameMode = GameMode.Normal;
            arenaSize = new Vector2(12, 20);

            actorCreatePeriod = 1;
            newActorStartingPositionXPadding = 2;

            // init bonuses

            availableBonuses = new List<GameBonus>();
            AddGameBonuses();
            gameBonuses = new List<GameBonus>();

            maxBonuses = 3;
            freeBonusTimeLimit = 1; // low for testing
            timeUntilFreeBonus = freeBonusTimeLimit;

            slowDownLimit = 7500;
            slowDownPower = 5;

            laserWidth = 2;

            cubesExplosionPower = 2;
            cubesExplosionExtraPower = 4;
            cubesExplosionExtraPowerProb = 100;
            cubesExplosionFallingSpeed = 6;

            // init score and level vars
            baseCubeScore = 5;
            scoreLevelMultiplier = 0.5f;

            maxLevel = 99;
            baseFourLineLimit = 1f;
            maxFourLineLimit = 5f;
            increaseFourLineLimitPerLevel = 0.2f;

            baseFallingSpeed = 800;
            minFallingSpeed = 16;
            fallingSpeedFactor = 0.08f;
            increaseFallingSpeedLevelCount = 2;

            CalculateScoreForLevelArray();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (gameBonuses.Count == 0 && ActiveBonus == null)
                timeSinceLastBonus += gameTime.ElapsedGameTime.Milliseconds;

            if (timeSinceLastBonus >= timeUntilFreeBonus)
            {
                AddBonus();
                timeUntilFreeBonus = freeBonusTimeLimit;
            }

            switch (ActiveBonus?.Type)
            {
                case BonusType.TimeSlowdown:
                    {
                        slowDownTimer -= gameTime.ElapsedGameTime.Milliseconds;

                        if (slowDownTimer <= 0)
                            DeactivateBonus();
                    }
                    break;
            }

            if (ActiveBonus == null || ActiveBonus.Type == BonusType.None || ActiveBonus.Type == BonusType.TimeSlowdown)
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
            }
            else
            {
                switch (ActiveBonus?.Type)
                {
                    case BonusType.Laser:
                        {
                            MoveLaser();
                        }
                        break;
                    case BonusType.CubesExplosion:
                        {
                            if (activeActor == null)
                                DeactivateBonus();
                            else
                            {
                                activeActor.Timer += gameTime.ElapsedGameTime.Milliseconds;
                                if (activeActor.Timer > activeActor.FallingSpeed)
                                {
                                    activeActor.Timer = 0;
                                    MoveActorTowardsExplosion(activeActor);
                                }
                            }
                        }
                        break;
                }
            }
        }

        // controls

        public override void ControlLeft_Down()
        {
            if (ActiveBonus?.Type == BonusType.SwipeCubes)
            {
                bonusSwipeCubesLeft = true;
                Bonus_Swipe_MakeAction();
                return;
            }

            base.ControlLeft_Down();
        }

        public override void ControlRight_Down()
        {
            if (ActiveBonus?.Type == BonusType.SwipeCubes)
            { 
                bonusSwipeCubesLeft = false;
                Bonus_Swipe_MakeAction();
                return;
            }

            base.ControlRight_Down();
        }

        public override void ControlDown_Click()
        {
            if (ActiveBonus?.Type == BonusType.Laser)
            {
                Bonus_Laser_MakeAction();
                return;
            }

            base.ControlDown_Click();
        }

        public void Control_ActivateBonus(GameBonus bonus)
        {
            ActivateBonus(bonus);
        }

        // Private methods

        protected void CalculateScoreForLevelArray()
        {
            int playgroundXSize = (int)arenaSize.X;
            var scores = new List<long>();
            long lastScore = 0;
            for (int i = 0; i <= maxLevel; i++)
            {
                var fourLineCount = MathHelper.Clamp((4 + 4 * i * increaseFourLineLimitPerLevel), 4, maxFourLineLimit * 4);
                scores.Add((long)(fourLineCount * baseCubeScore * playgroundXSize * (1f + i * scoreLevelMultiplier)) + lastScore);
                lastScore = scores.Last();
            }
            scoreForLevelArray = scores.ToArray();
        }

        protected override void DestroyFullLines(int[] fullLines)
        {
            base.DestroyFullLines(fullLines);

            if (fullLines.Count() >= 4)
            {
                AddBonus();
            }

            LastPlayedActor = null;
        }

        protected override void IncreaseScoreForLines(int linesDestroyed) 
        {
            var newScore = linesDestroyed * baseCubeScore * (int)arenaSize.X * (1f + MathHelper.Clamp((Level - 1), 0, maxLevel) * scoreLevelMultiplier);
            Score += (int)newScore;
        }

        protected override void UpdateLevel()
        {
            int newLevel = 0;
            foreach(var requiredScoreForLevel in scoreForLevelArray)
            {
                if (requiredScoreForLevel > Score)
                    break;
                newLevel++;
            }
            if (Level != newLevel)
                Level = newLevel;
        }

        protected override int GetGameSpeed(SettingOptions.GameSpeed gameSpeedSetting)
        {
            var levelIndex = (int)Math.Floor(Level / increaseFallingSpeedLevelCount);
            var fallingSpeed = baseFallingSpeed;
            var normalFallingSpeed = MathHelper.Clamp((int)(baseFallingSpeed * (1f - fallingSpeedFactor * levelIndex)), minFallingSpeed, baseFallingSpeed);
            switch (gameSpeedSetting)
            {
                case GameSpeed.Normal:
                    fallingSpeed = normalFallingSpeed;

                    if (ActiveBonus?.Type == BonusType.TimeSlowdown)
                        fallingSpeed = fallingSpeed * slowDownPower; // if is slowdown activated return slower speeds

                    break;
                case GameSpeed.Speedup:
                    fallingSpeed = MathHelper.Min(normalFallingSpeed, 50); // since i don't want to fall slower than normal speed is, i return slowest speed
                    break;
                case GameSpeed.Falling:
                    fallingSpeed = 5; break;
            }

            return fallingSpeed;
        } 

        protected override Point GetNewActorPosition(Actor actor)
        {
            var availibleArenaSize = (int)arenaSize.X - 2 * newActorStartingPositionXPadding;

            var positionsWithoutOtherActors = new List<Point>(); // i won't place new actor on other existing actor
            for (int i = 0; i < availibleArenaSize + 1 - actor.Shape.GetLength(0); i++)
            {
                actor.Position = new Point(i + newActorStartingPositionXPadding, 0);
                if (!ActorCollideActors(actor, actors))
                    positionsWithoutOtherActors.Add(actor.Position);
            }
            actor.Position = new Point();

            if (positionsWithoutOtherActors.Count > 0)
                return positionsWithoutOtherActors[Game1.Random.Next(0, positionsWithoutOtherActors.Count - 1)];
            else
                return new Point(Game1.Random.Next(0, availibleArenaSize + 1 - actor.Shape.GetLength(0) + 2), 0);
        }

        protected override void UpdateEffectsArray()
        {
            base.UpdateEffectsArray();

            if (ActiveBonus?.Type == BonusType.Laser)
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

        protected override void InsertActorToPlayground(Actor actor)
        {
            base.InsertActorToPlayground(actor);
            LastPlayedActor = actor;
        }

        protected override void RemoveActorFromPlayground(Actor actor)
        {
            base.RemoveActorFromPlayground(actor);
            LastPlayedActor = null;
        }

        // private bonus activations

        protected virtual void AddBonus()
        {
            if (gameBonuses.Count < maxBonuses)
            {
                availableBonuses.Shuffle(); // before every pick i shuffle available bonuses for more random chance
                var bonusIndex = Game1.Random.Next(0, availableBonuses.Count - 1);
                gameBonuses.Add(availableBonuses[bonusIndex]);
                timeSinceLastBonus = 0;
                RefreshBonuses();
            }
        }

        protected virtual void ActivateBonus(GameBonus bonus)
        {
            bonus.OnActivate();
            gameBonuses.Remove(bonus);
            RefreshBonuses();
        }

        protected virtual void DeactivateBonus()
        {
            if (ActiveBonus != null)
                ActiveBonus.OnDeactivate();

            RefreshBonuses();
        }

        protected virtual void RefreshBonuses()
        {
            if (ActiveBonus == null || ActiveBonus.Type == BonusType.None)
            {
                gameBonuses.ForEach(gmb => gmb.Enabled = true);
                gameBonuses.Where(gmb => gmb.Type == BonusType.CancelLastCube).ToList().ForEach(gmb => gmb.Enabled = LastPlayedActor != null); // if there is no last actor to cancel, disable cancel bonus
            }
            else
            {
                gameBonuses.ForEach(gmb => gmb.Enabled = false);
            }

            OnAvailableBonusesChange(new AvailableBonusesChangeEventArgs(gameBonuses));
        }

        protected virtual void Bonus_CancelLastCube_Activate(GameBonus bonus)
        {
            ActiveBonus = bonus;

            if (LastPlayedActor != null)
                RemoveActorFromPlayground(LastPlayedActor);

            DeactivateBonus();
        }

        protected virtual void Bonus_CancelLastCube_Deactivate()
        {
            ActiveBonus = null;
        }

        protected virtual void Bonus_SlowDown_Activate(GameBonus bonus)
        {
            ActiveBonus = bonus;
            foreach (var actor in actors)
            {
                actor.FallingSpeed = actor.FallingSpeed * slowDownPower; // make actor x times slower from their previous speed TODO: optimaze speed
            }

            slowDownTimer = slowDownLimit;

            BorderColor = Color.Red;
            backgroundChanged = true;
        }

        protected virtual void Bonus_SlowDown_Deactivate()
        {
            ActiveBonus = null;
            foreach (var actor in actors)
            {
                if (!actor.IsFalling)
                    actor.FallingSpeed = GetGameSpeed(GameSpeed.Normal);
            }
            slowDownTimer = 0;

            BorderColor = Contents.Colors.PlaygroundBorderColor;
            backgroundChanged = true;
        }

        protected virtual void Bonus_Laser_Activate(GameBonus bonus)
        {
            ActiveBonus = bonus;

            laserPosition = new Point((playground.GetLength(0) - laserWidth) / 2, 0);
            lastActiveActor = activeActor;
            activeActor = null;
        }

        protected virtual void Bonus_Laser_MakeAction()
        {
            LaserActors();
            LaserPlayground();
            backgroundChanged = true;

            DeactivateBonus();
        }

        protected virtual void Bonus_Laser_Deactivate()
        {
            ActiveBonus = null;

            if (actors.Contains(lastActiveActor))
                activeActor = lastActiveActor;
            else
            {
                activeActor = actors.FirstOrDefault();

                if (activeActor == null)
                    CreateNewActor();
            }
        }

        protected virtual void Bonus_Swipe_Activate(GameBonus bonus)
        {
            ActiveBonus = bonus;

            actorsQueue.InsertRange(0, actors);
            OnActorsQueueChange(new QueueChangeEventArgs(actorsQueue.First()));
            actors.Clear();
            activeActor = null;
        }

        protected virtual void Bonus_Swipe_MakeAction()
        {
            if (bonusSwipeCubesLeft)
            {
                for (int y = 0; y < playground.GetLength(1); y++)
                {
                    var lastEmptyX = 0;
                    for (int x = lastEmptyX; x < playground.GetLength(0); x++)
                    {
                        if (playground[x, y] > 0)
                        {
                            if (x != lastEmptyX)
                            {
                                playground[lastEmptyX, y] = playground[x, y];
                                playground[x, y] = 0;
                            }
                            lastEmptyX++;
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < playground.GetLength(1); y++)
                {
                    var lastEmptyX = playground.GetLength(0) - 1;
                    for (int x = lastEmptyX; x >= 0; x--)
                    {
                        if (playground[x, y] > 0)
                        {
                            if (x != lastEmptyX)
                            {
                                playground[lastEmptyX, y] = playground[x, y];
                                playground[x, y] = 0;
                            }
                            lastEmptyX--;
                        }
                    }
                }
            }
            backgroundChanged = true;
            CreateNewActor();
            DeactivateBonus();
        }

        protected virtual void Bonus_Swipe_Deactivate()
        {
            ActiveBonus = null;
        }

        protected virtual void Bonus_CubesExplosion_Activate(GameBonus bonus)
        {
            ActiveBonus = bonus;
            activeActor.FallingSpeed = cubesExplosionFallingSpeed;
        }

        protected virtual void Bonus_CubesExplosion_Deactivate()
        {
            ActiveBonus = null;

            activeActor = actors.FirstOrDefault();

            if (activeActor == null)
                CreateNewActor();
        }

        protected virtual void AddGameBonuses()
        {
            availableBonuses.Add(new GameBonus(Bonus_CubesExplosion_Activate, null, null, Bonus_CubesExplosion_Deactivate)
            {
                Type = BonusType.CubesExplosion,
                Name = "Explosion",
                SpecialText = "EXP",
                Enabled = true,
            });
            availableBonuses.Add(new GameBonus(Bonus_CancelLastCube_Activate, null, null, Bonus_CancelLastCube_Deactivate)
            {
                Type = BonusType.CancelLastCube,
                Name = "Cancel last",
                SpecialText = "CNL",
                Enabled = true
            });
            availableBonuses.Add(new GameBonus(Bonus_Laser_Activate, Bonus_Laser_MakeAction, null, Bonus_Laser_Deactivate)
            {
                Type = BonusType.Laser,
                Name = "Laser",
                SpecialText = "LSR",
                Enabled = true
            });
            availableBonuses.Add(new GameBonus(Bonus_Swipe_Activate, Bonus_Swipe_MakeAction, null, Bonus_Swipe_Deactivate)
            {
                Type = BonusType.SwipeCubes,
                Name = "Swipe cubes",
                SpecialText = "SWP",
                Enabled = true
            });
            availableBonuses.Add(new GameBonus(Bonus_SlowDown_Activate, null, null, Bonus_SlowDown_Deactivate)
            {
                Type = BonusType.TimeSlowdown,
                Name = "Slowdown time",
                SpecialText = "SLW",
                Enabled = true
            });
        }

        protected virtual void MoveLaser()
        {
            if (activeActorMovement == ActorMovement.None || moveTimer > 0)
                return;

            if (activeActorMovement == ActorMovement.Left)
            {
                if (laserPosition.X - 1 >= 0)
                    laserPosition.X--;
            }
            else
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

        protected virtual void LaserActors()
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

        protected virtual void LaserPlayground()
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

        protected virtual void MoveActorTowardsExplosion(Actor actor)
        {
            var newPosition = new Point(actor.Position.X, actor.Position.Y + 1);

            if (!ActorCollideWithPlayground(newPosition, actor.Shape))
            {
                actor.Position = newPosition;
            }
            else
            {
                var extraPower = Game1.Random.Next(0, cubesExplosionExtraPowerProb) == cubesExplosionExtraPowerProb;
                ExplodeActor(actor, !extraPower ? cubesExplosionPower : cubesExplosionExtraPower);

                if (actor == activeActor)
                    activeActor = null;
                actors.Remove(actor);
            }
        }

        protected virtual void ExplodeActor(Actor actor, int power)
        {
            var actorCubes = new List<Tuple<int, int, int>>();

            for (int x = 0; x < actor.Shape.GetLength(0); x++)
            {
                for (int y = 0; y < actor.Shape.GetLength(1); y++)
                {
                    if (actor.Shape[x, y])
                    {
                        for (int expX = x - power; expX <= x + power; expX++)
                        {
                            for (int expY = y - power; expY <= y + power; expY++)
                            {
                                if (expX + actor.Position.X >= 0 && expX + actor.Position.X < playground.GetLength(0) && expY + actor.Position.Y >= 0 && expY + actor.Position.Y < playground.GetLength(1))
                                    actorCubes.Add(new Tuple<int, int, int>(expX + actor.Position.X, expY + actor.Position.Y, 0));
                            }
                        }
                    }
                }
            }
            InsertBoxesToPlayground(actorCubes);
            backgroundChanged = true;
        }
    }
}