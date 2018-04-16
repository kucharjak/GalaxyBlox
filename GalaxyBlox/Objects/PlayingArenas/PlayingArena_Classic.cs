using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using GalaxyBlox.Static;
using Microsoft.Xna.Framework;
using static GalaxyBlox.Static.SettingOptions;

namespace GalaxyBlox.Objects.PlayingArenas
{
    class PlayingArena_Classic : PlayingArena
    {
        private int linesDestroyed;

        private Dictionary<int, int> LevelsToSpeedDic;
        private Dictionary<int, int> ScoreForLines;

        public PlayingArena_Classic(Room parentRoom, Vector2 size, Vector2 position) : base(parentRoom, size, position)
        {

        }

        protected override void InitializeArenaSettings() // TODO: ADD INFO
        {
            gameMode = GameMode.Classic;
            arenaSize = new Vector2(12, 20);

            ScoreForLines = new Dictionary<int, int>()
            {
                { 1, 40 },
                { 2, 100 },
                { 3, 300 },
                { 4, 1200 }
            };

            LevelsToSpeedDic = new Dictionary<int, int>()
            {
                { 0,800 },
                { 1,716 },
                { 2,633 },
                { 3,550 },
                { 4,466 },
                { 5,383 },
                { 6,300 },
                { 7,216 },
                { 8,133 },
                { 9,100 },
                { 10,83 },
                { 11,83 },
                { 12,83 },
                { 13,66 },
                { 14,66 },
                { 15,66 },
                { 16,50 },
                { 17,50 },
                { 18,50 },
                { 19,33 },
                { 20,33 },
                { 21,33 },
                { 22,33 },
                { 23,33 },
                { 24,33 },
                { 25,33 },
                { 26,33 },
                { 27,33 },
                { 28,33 },
                { 29,16 }
            };
        }

        // public methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsPaused)
                return;

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

        // private methods

        protected override void IncreaseScoreForLines(int linesDestroyed)
        {
            if (linesDestroyed > 4 || linesDestroyed < 1)
                return;

            this.linesDestroyed += linesDestroyed;
            
            Score += ScoreForLines[linesDestroyed] * (Level + 1);
        }

        protected override void UpdateLevel()
        {
            Level = (int)Math.Floor(linesDestroyed / 10D);
        }

        protected override int GetGameSpeed(SettingOptions.GameSpeed gameSpeedSetting)
        {
            var fallingSpeed = 16;
            switch (gameSpeedSetting)
            {
                case GameSpeed.Normal:
                    if (LevelsToSpeedDic.ContainsKey(Level))
                        fallingSpeed = LevelsToSpeedDic[Level];
                    break;
                case GameSpeed.Speedup:
                    fallingSpeed = 50; break;
                case GameSpeed.Falling:
                    fallingSpeed = 5; break;
            }

            return fallingSpeed;
        }

        protected override Point GetNewActorPosition(Actor actor)
        {
            return new Point((playground.GetLength(0) - actor.Shape.GetLength(0)) / 2, 0);
        }
    }
}