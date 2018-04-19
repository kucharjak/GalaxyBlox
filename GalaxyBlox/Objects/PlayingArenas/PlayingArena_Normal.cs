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

        //protected List<BonusType> availableBonuses = new List<BonusType>() { BonusType.CubesExplosion, BonusType.CancelLastCube, BonusType.Laser, BonusType.SwipeCubes, BonusType.TimeSlowdown };
        protected List<GameBonus> availableBonuses;
        protected Random bonusRandom;

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
            maxBonuses = 2;
            availableBonuses = new List<GameBonus>();
            AddGameBonuses();
            ResetBonuses();

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

            bonusRandom = new Random((int)DateTime.Now.Ticks);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsPaused)
                return;

            AddProgressToBonus(10);

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
                    case BonusType.CubesDropWithExplosion:
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

        public override void StartNewGame()
        {
            base.StartNewGame();

            RefreshBonuses();
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

        public void Control_ActivateBonus(GameBonus bonus, int? position = null)
        {
            ActivateBonus(bonus, position);
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

            AddProgressToBonus(fullLines.Count() * fullLines[0] / 2);

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
            var actorStartYPosition = gameMode == GameMode.Extreme ? -actor.Shape.GetLength(1) + 1 : 0;

            var positionsWithoutOtherActors = new List<Point>(); // i won't place new actor on other existing actor
            for (int i = 0; i < availibleArenaSize + 1 - actor.Shape.GetLength(0); i++)
            {
                actor.Position = new Point(i + newActorStartingPositionXPadding, actorStartYPosition);
                if (!ActorCollideActors(actor, actors))
                    positionsWithoutOtherActors.Add(actor.Position);
            }
            actor.Position = new Point();

            if (positionsWithoutOtherActors.Count > 0)
                return positionsWithoutOtherActors[Game1.Random.Next(0, positionsWithoutOtherActors.Count - 1)];
            else
                return new Point(Game1.Random.Next(0, availibleArenaSize + 1 - actor.Shape.GetLength(0) + 2), actorStartYPosition);
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


        protected int CountActorCubes(Actor actor)
        {
            var result = 0;

            for (int x = 0; x < actor.Shape.GetLength(0); x++)
            {
                for (int y = 0; y < actor.Shape.GetLength(1); y++)
                {
                    if (actor.Shape[x, y])
                        result++;
                }
            }

            return result;
        }

        protected override int InsertActorToPlayground(Actor actor)
        {
            LastPlayedActor = actor;
            return base.InsertActorToPlayground(actor);
        }

        protected override int RemoveActorFromPlayground(Actor actor)
        {
            LastPlayedActor = null;
            return base.RemoveActorFromPlayground(actor);
        }

        // private bonus activations

        protected virtual void AddProgressToBonus(int progress)
        {
            if (gameBonuses == null || gameBonuses.Count == 0 || gameBonuses.All(bns => bns.Progress == 100))
                return;

            GameBonus bonus;
            if (gameBonuses.Any(bns => bns.Progress > 0 && bns.Progress < 100))
            {
                bonus = gameBonuses.Where(bns => bns.Progress > 0 && bns.Progress < 100).First();
            }
            else
            {
                bonus = gameBonuses.Where(bns => bns.Progress < 100).First();
            }

            bonus.Progress += progress;
            if (bonus.Progress >= 100)
            {
                var bonusIndex = gameBonuses.FindIndex(bns => bns == bonus);
                var index = bonusRandom.Next(0, availableBonuses.Count);

                gameBonuses.RemoveAt(bonusIndex);
                gameBonuses.Insert(bonusIndex, availableBonuses[index]);
            }
            RefreshBonuses();
        }

        protected virtual void ActivateBonus(GameBonus bonus, int? position = null)
        {
            bonus.OnActivate();

            var insertIndex = 0;
            if (position.HasValue && gameBonuses.Count > position.Value && gameBonuses[position.Value] == bonus)
            {
                insertIndex = position.Value;
            }
             else
            {
                insertIndex = gameBonuses.IndexOf(bonus);
            }

            gameBonuses.RemoveAt(insertIndex);
            gameBonuses.Insert(insertIndex, new GameBonus(null, null, null, null)
            {
                Type = BonusType.None,
                Enabled = false,
                Name = "None",
                SpecialText = "None",
                Progress = 0
            });

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
            Vibrations.Vibrate(16 * 60);

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
                Type = BonusType.CubesDropWithExplosion,
                Name = "DropAndExplode",
                SpecialText = "DROP",
                Enabled = true,
                Progress = 100
            });
            //availableBonuses.Add(new GameBonus(Bonus_CancelLastCube_Activate, null, null, Bonus_CancelLastCube_Deactivate)
            //{
            //    Type = BonusType.CancelLastCube,
            //    Name = "Cancel last",
            //    SpecialText = "UNDO",
            //    Enabled = true,
            //    Progress = 100
            //});
            availableBonuses.Add(new GameBonus(Bonus_Laser_Activate, Bonus_Laser_MakeAction, null, Bonus_Laser_Deactivate)
            {
                Type = BonusType.Laser,
                Name = "Laser",
                SpecialText = "LASER",
                Enabled = true,
                Progress = 100
            });
            //availableBonuses.Add(new GameBonus(Bonus_Swipe_Activate, Bonus_Swipe_MakeAction, null, Bonus_Swipe_Deactivate)
            //{
            //    Type = BonusType.SwipeCubes,
            //    Name = "Swipe cubes",
            //    SpecialText = "SWIPE",
            //    Enabled = true,
            //    Progress = 100
            //});
            //availableBonuses.Add(new GameBonus(Bonus_SlowDown_Activate, null, null, Bonus_SlowDown_Deactivate)
            //{
            //    Type = BonusType.TimeSlowdown,
            //    Name = "Slowdown time",
            //    SpecialText = "SLOW",
            //    Enabled = true,
            //    Progress = 100
            //});
        }

        protected virtual void ResetBonuses()
        {
            gameBonuses = new List<GameBonus>();

            for (int i = 0; i < maxBonuses; i++)
            {
                gameBonuses.Add(new GameBonus(null, null, null, null)
                {
                    Type = BonusType.None,
                    Enabled = false,
                    Name = "None",
                    SpecialText = "None",
                    Progress = 0
                });
            }

            RefreshBonuses();
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
                            Score += (int)(CountActorCubes(actor) * baseCubeScore * (1f + MathHelper.Clamp((Level - 1), 0, maxLevel) * scoreLevelMultiplier));
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

                    var cubesBefore = CountActorCubes(actor);

                    actor.Position += startPosition;
                    actor.Shape = newShape;

                    var cubesAfter = CountActorCubes(actor);
                    Score += (int)((cubesBefore - cubesAfter) * baseCubeScore * (1f + MathHelper.Clamp((Level - 1), 0, maxLevel) * scoreLevelMultiplier));
                }
            }
        }

        protected virtual void LaserPlayground()
        {
            var overwritenCubes = 0;
            var laserRect = new Rectangle(laserPosition, new Point(laserWidth, playground.GetLength(1)));
            for (int x = laserRect.X; x < laserRect.X + laserRect.Width; x++)
            {
                for (int y = laserRect.Y; y < laserRect.Y + laserRect.Height; y++)
                {
                    if (playground[x, y] != 0)
                    {
                        playground[x, y] = 0;
                        overwritenCubes++;
                    }
                }
            }
            backgroundChanged = true;

            Score += (int)(overwritenCubes * baseCubeScore * (1f + MathHelper.Clamp((Level - 1), 0, maxLevel) * scoreLevelMultiplier));

            var laserWidthExtension = new Vector2(Size.X / arenaSize.X * 0.5f, 0);
            var laserCubesRect = GetCubesIngamePosition(new Point(laserRect.X, laserRect.Y), new Point(laserRect.X + laserRect.Width - 1, laserRect.Height));
            var laser = new GameObject(ParentRoom)
            {
                SpriteAnimation = Contents.Animations.Laser.Copy(true),
                Position = new Vector2(laserCubesRect.X, 0) - laserWidthExtension,
                Size = new Vector2(laserCubesRect.Width, ParentRoom.Size.Y) + 2 * laserWidthExtension,
                LayerDepth = 0.095f,
                //BaseColor = actor.Color
            };

            laser.SpriteAnimation.Parent = laser;
            laser.SpriteAnimation.AnimationEnd += Laser_AnimationEnd;
            laser.SpriteAnimation.AnimationNext += Laser_AnimationNext;
            ParentRoom.Objects.Add(laser);
            Pause();
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
                Vibrations.Vibrate(extraPower ? 350 : 150);

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
            
            if (ParentRoom != null)
            {
                var lowestX = actorCubes.Min(cb => cb.Item1);
                var lowestY = actorCubes.Min(cb => cb.Item2);
                var biggestX = actorCubes.Max(cb => cb.Item1);
                var biggestY = actorCubes.Max(cb => cb.Item2);

                var explosionRect = GetCubesIngamePosition(new Point(lowestX, lowestY), new Point(biggestX, biggestY));

                var explosionSizeExtension = new Vector2(Size.X / arenaSize.X * 0.5f);
                var explosion = new GameObject(ParentRoom)
                {
                    SpriteAnimation = Contents.Animations.Explosion.Copy(true),
                    Position = new Vector2(explosionRect.X, explosionRect.Y) - explosionSizeExtension,
                    Size = new Vector2(explosionRect.Width, explosionRect.Height) + 2 * explosionSizeExtension,
                    LayerDepth = 0.06f,
                    //BaseColor = actor.Color
                };

                explosion.SpriteAnimation.Parent = explosion;
                explosion.SpriteAnimation.AnimationEnd += Explosion_AnimationEnd;
                explosion.SpriteAnimation.AnimationNext += Explosion_AnimationNext;
                ParentRoom.Objects.Add(explosion);
                Pause();
            }

            backgroundChanged = true;

            var cubes = InsertBoxesToPlayground(actorCubes);
            Score += (int)(cubes * baseCubeScore * (1f + MathHelper.Clamp((Level - 1), 0, maxLevel) * scoreLevelMultiplier));
        }

        private void Explosion_AnimationNext(object sender, EventArgs e)
        {
            var animation = (sender as SpriteAnimation);
            if (animation.Position >= 2 && IsPaused)
                Resume();
        }

        private void Explosion_AnimationEnd(object sender, EventArgs e)
        {
            if (IsPaused)
                Resume();

            var animation = (sender as SpriteAnimation);
            animation.Parent.Destroyed = true;
        }

        private void Laser_AnimationNext(object sender, EventArgs e)
        {
            var animation = (sender as SpriteAnimation);
            if (animation.Position >= 12 && IsPaused)
                Resume();
        }

        private void Laser_AnimationEnd(object sender, EventArgs e)
        {
            if (IsPaused)
                Resume();

            var animation = (sender as SpriteAnimation);
            animation.Parent.Destroyed = true;
        }
    }
}