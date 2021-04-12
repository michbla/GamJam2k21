using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{
    public class Inventory
    {
        //posiadany blok i jego ilosc
        private Dictionary<Block, int> itemList= new Dictionary<Block, int>();

        public Inventory()
        {

        }

        public void addToInventory(int go)
        {
            Block block = ResourceManager.GetBlockByID(go);
            if (itemList.ContainsKey(block))
            {
                if (itemList[block] < 99)
                    itemList[block]++;
                else
                {
                    itemList.TryAdd(block, 1);
                }
            }
            else itemList.Add(block, 1);
        }

        public Dictionary<Block, int> getInventory()
        {
            return itemList;
        }
    }
}
