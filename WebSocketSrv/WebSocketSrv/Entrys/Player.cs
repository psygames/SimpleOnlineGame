using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShitMan
{
    class Player : EntryBase
    {
        public string name;
        public float radius;
        public int level;

        public void UpdateLevel()
        {
            level = (int)Math.Floor(radius / GameConfig.PlayerLevelUpStepRaius);
        }

        public void IncreaseRaius(float tarRadius)
        {
            radius = (float)Math.Pow(radius * radius * radius + tarRadius * tarRadius * tarRadius, 1 / 3f);
            UpdateLevel();
        }

        public bool IsCollisionWith(Player player)
        {
            if (Vector3.Distance(this.pos, player.pos) <= this.radius + player.radius)
            {
                return true;
            }
            return false;
        }

        public bool IsCollisionWith(Food food)
        {
            if (Vector3.Distance(this.pos, food.pos) <= this.radius + food.radius)
            {
                return true;
            }
            return false;
        }
    }
}
