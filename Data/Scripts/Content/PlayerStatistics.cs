using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{

    /// <summary>
    /// Klasa zlicza doświadczenie, ilość zniszczonych bloków i max głębokość
    /// </summary>
    /// 
    public class PlayerStatistics
    {
        private int levelReached = 0;
        private int blocksDestroyed = 0;
        private float exp = 0;
        private const int BLOCK_EXP = 10;

        public PlayerStatistics(int levelReached, int blocksDestroyed, float exp)
        {
            this.levelReached = levelReached;
            this.blocksDestroyed = blocksDestroyed;
            this.exp = exp;
        }

        public void SetBlocksDestroyed()
        {
            blocksDestroyed += 1;
            exp += BLOCK_EXP;
        }

        public void addLevelReached()
        {
            levelReached += 1; 
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
