using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{

    /// <summary>
    /// Klasa zlicza doswiadczenie, ilosc zniszczonych blokow i max glebokosc
    /// 
    /// </summary>
    /// 
    public class PlayerStatistics
    {
        
        private int levelReached = 0;
        private int blocksDestroyed = 0;
        private float exp = 0;

        private static Dictionary<string, int> expList= new Dictionary<string, int>();

        private static Dictionary<string, int> blocksDug = new Dictionary<string, int>();

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
            addDugBlock(name);
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
            Dictionary<int, Block> blocklist= new Dictionary<int, Block>();
            for (int i=1;i<=ResourceManager.GetBlockListSize();i++)
            {
                blocklist.Add(i, ResourceManager.GetBlockByID(i));
                expList.Add(blocklist[i].name, (int)blocklist[i].endurance / 10);
                blocksDug.Add(blocklist[i].name, 0);
            }
            blocklist.Clear();
        }
        private void addDugBlock(string block)
        {
            blocksDug[block]++;
        }
        
        public int getDugBlocks(string name)
        {
            return blocksDug[name];
        }

    }


    
}
