using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{
    public class Pickaxe
    {
        public string name;

        public Texture sprite;

        public float speed;
        public int hardness;
        public float damage;

        public Pickaxe(string _name, Texture _texture, float _speed, int _hardness, float _damage)
        {
            name = _name;
            sprite = _texture;
            speed = _speed;
            hardness = _hardness;
            damage = _damage;
        }
    }
}
