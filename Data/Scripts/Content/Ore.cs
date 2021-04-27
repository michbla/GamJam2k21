using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Ore
    {
        public readonly string name;
        public readonly int id;

        public readonly int hardness;
        public readonly float endurance;


        private readonly Texture SPRITE;
        public readonly Vector3 COLOR = (1.0f, 1.0f, 1.0f);

        public Item drop;

        public Ore(int _id, string _name, Texture _sprite, int _hardness, float _endurance, Item _drop, Vector3 _color)
        {
            id = _id;
            name = _name;
            SPRITE = _sprite;
            hardness = _hardness;
            endurance = _endurance;

            drop = _drop;
            COLOR = _color;
        }

        public void Draw(SpriteRenderer rend, Vector2 viewPos, Block parent)
        {
            rend.DrawSprite(SPRITE, viewPos, parent.position, parent.size, parent.rotation, parent.color);
        }

        public float getOreEndurance()
        {
            return endurance;
        }

    }
}
