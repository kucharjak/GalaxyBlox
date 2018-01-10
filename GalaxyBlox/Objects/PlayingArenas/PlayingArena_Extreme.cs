using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using Microsoft.Xna.Framework;
using static GalaxyBlox.Static.SettingOptions;

namespace GalaxyBlox.Objects.PlayingArenas
{
    class PlayingArena_Extreme : PlayingArena_Normal
    {
        public PlayingArena_Extreme(Room parentRoom, Vector2 size, Vector2 position) : base(parentRoom, size, position)
        {
        }

        protected override void InitializeArenaSettings() // TODO: ADD INFO
        {
            gameMode = GameMode.Extreme;
            arenaSize = new Vector2(18, 30);

            actorsMaxCount = 5; // TODO: GRADUAL UPDATE PER LEVEL
            actorCreatePeriod = 2000;
            newActorStartingPositionXPadding = 3;

            availableBonuses = new List<GameBonus>();
            AddGameBonuses();
            gameBonuses = new List<GameBonus>();

            maxBonuses = 4;
            freeBonusTimeLimit = 1; // low for testing
            timeUntilFreeBonus = freeBonusTimeLimit;

            slowDownLimit = 10000;
            slowDownPower = 5;

            laserWidth = 3;

            cubesExplosionPower = 3;
            cubesExplosionExtraPower = 5;
            cubesExplosionExtraPowerProb = 100;
            cubesExplosionFallingSpeed = 6;

            // init score and level vars
            baseCubeScore = 10;
            scoreLevelMultiplier = 0.75f;

            maxLevel = 99;
            baseFourLineLimit = 1f;
            maxFourLineLimit = 8f;
            increaseFourLineLimitPerLevel = 0.3f;

            baseFallingSpeed = 1200;
            minFallingSpeed = 50;
            fallingSpeedFactor = 0.04f;
            increaseFourLineLimitPerLevel = 1.5f;

            CalculateScoreForLevelArray();
        }
    }
}