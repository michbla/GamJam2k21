using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21.PlayerElements
{
    public class PlayerStatistics
    {
        private Player player;

        private int levelReached = 0;

        public int ExpLevel = 0;
        private readonly int maxLevel = 40;
        private float exp = 0;
        private float expToNextLevel = 100;

        public PlayerStatistics(Player player)
        {
            this.player = player;
        }

        public void AddExp(float value)
        {
            if (ExpLevel == maxLevel)
                return;
            exp += value;
            checkIfNextLevel();
        }

        private void checkIfNextLevel()
        {
            while(exp >= expToNextLevel)
            {
                exp -= expToNextLevel;
                ExpLevel++;
                expToNextLevel = calculateExpToNextLevel();
                player.AddAttributePoint();
            }
        }

        private float calculateExpToNextLevel()
        {
            return expToNextLevel * 2;
        }

        public void SetLevelReached(int newLevel)
        {
            levelReached = newLevel; 
        }

        public float getExp()
        {
            return exp;
        }
        public int getLevelReached()
        {
            return levelReached;
        }
    }
}
