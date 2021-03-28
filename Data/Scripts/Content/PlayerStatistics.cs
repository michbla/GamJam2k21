using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{

    /// <summary>
    /// Klasa zlicza doswiadczenie, ilosc zniszczonych blokow i max glebokosc
    /// TODO: przekazac liste blokow do addToExpList i wyliczac expo na podstawie hardnessa
    /// </summary>
    /// 
    public class PlayerStatistics
    {
        private int levelReached = 0;
        private int blocksDestroyed = 0;
        private float exp = 0;
        Dictionary<string, int> expList= new Dictionary<string, int>();
        

        public PlayerStatistics(int levelReached, int blocksDestroyed, float exp)
        {
            this.levelReached = levelReached;
            this.blocksDestroyed = blocksDestroyed;
            this.exp = exp;
            addToExpList();
        }

        public void SetBlocksDestroyed(string name)
        {
            blocksDestroyed += 1;
            exp += expList[name];
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

        public void addToExpList()
        {
            expList.Add("Grass",10  );
            expList.Add("Dirt",10  );
            expList.Add("Stone",15  );
            
        }
    }


    
}
