using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{
    public class Inventory
    {
        private readonly List<Item> itemList = new List<Item>();
        public readonly int CAPACITY = 20;

        private readonly int STACK_SIZE = 64;

        public Inventory()
        {

        }

        public bool addToInventory(Item newItem)
        {
            foreach(var item in itemList)
                if (item.id == newItem.id && item.quantity < STACK_SIZE)
                {
                    item.quantity++;
                    return true;
                }
            if (itemList.Count >= CAPACITY)
                return false;
            itemList.Add(newItem);
            return true;
        }

        public List<Item> getInventory()
        {
            return itemList;
        }
    }
}
