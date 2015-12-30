using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShitMan
{
    class GameConfig
    {
        public const int MaxFoodCount = 30;
        public const int FoodGeneratePerDurationCount = 3;
        public const int MinFoodCount = 10;
        public const float FoodGenerateDuration = 3f;

        public const int PlayerCanBeEatLevelDist = 1;
        public const float PlayerLevelUpStepRaius = 0.1f;

        public const int StateQueueMaxLength = 5;

        public const float PlayerMoveSyncInterval = 0.2f;

        public const float maxFoodAliveTime = 180f;
    }
}
