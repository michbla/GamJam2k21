using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{
    public class Item
    {
        public readonly int id;
        public readonly string name;
        public readonly Texture icon;

        public int quantity = 1;

        public Item(int _id, string _name, Texture _icon)
        {
            id = _id;
            name = _name;
            icon = _icon;
        }

        public Item(Item copy)
        {
            id = copy.id;
            name = copy.name;
            icon = copy.icon;
        }
    }
}
