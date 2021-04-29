using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{
    public class PlayerStatistics
    {
        
        private int levelReached = 0;
        private int blocksDestroyed = 0;
        private float exp = 0;

        private static Dictionary<Ore, int> expList= new Dictionary<Ore, int>();

        //niepotrzebne???
        //private static Dictionary<string, int> blocksDug = new Dictionary<string, int>();

        public PlayerStatistics(int levelReached, int blocksDestroyed, float exp)
        {
            this.levelReached = levelReached;
            this.blocksDestroyed = blocksDestroyed;
            this.exp = exp;
            addToExpList();
        }

        public void SetOresDestroyed(Ore name)
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
            int oreCount = ResourceManager.getOreListCount();
            for (int i=1;i<=oreCount;i++)
            {
                var ore = ResourceManager.GetOreByID(i);
                expList.Add(ore, (int)ore.Drop.Value);
            }
        }
      /*  private void addDugBlock(string block)
        {
            blocksDug[block]++;
        } niepotrzebne*/
        

    }


    
}
