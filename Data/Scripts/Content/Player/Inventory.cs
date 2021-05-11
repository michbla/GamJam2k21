using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{
    public class Inventory
    {
        private readonly List<Item> itemList = new List<Item>();

        private readonly int CAPACITY = 20;
        private readonly int STACK_SIZE = 64;

        public Inventory()
        {
            
        }

        public bool HasFreeSpaceForItem(Item newItem)
        {
            return findItem(newItem) != -1;
        }

        private int findItem(Item item)
        {
            for(int i = 0; i < itemList.Count; i++)
                if (itemList[i].Id == item.Id && itemList[i].Quantity < STACK_SIZE)
                    return i;
            if (itemList.Count >= CAPACITY)
                return -1;
            return CAPACITY + 1;
        }

        public void AddToInventory(Item newItem)
        {
            int itemIndex = findItem(newItem);
            if(itemIndex > CAPACITY)
            {
                itemList.Add(newItem);
                return;
            }
            itemList[itemIndex].Quantity++;
        }

        public List<Item> GetInventoryItems()
        {
            return itemList;
        }
    }
}
