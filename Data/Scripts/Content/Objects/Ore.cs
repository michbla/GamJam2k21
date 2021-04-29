using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Ore
    {
        private Sprite sprite;

        private readonly int id;
        private readonly string name;
        private readonly int hardness;
        private readonly float endurance;
        private Vector3 color;

        private Item drop;

        public int Id
        {
            get => id;
        }

        public string Name
        {
            get => name;
        }

        public int Hardness
        {
            get => hardness;
        }

        public float Endurance
        {
            get => endurance;
        }

        public Vector3 Color
        {
            get => color;
        }

        public Item Drop
        {
            get => drop;
        }

        public Ore(int id,
                   Sprite sprite,
                   string name,
                   int hardness,
                   float endurance,
                   Item drop,
                   Vector3 color)
        {
            this.id = id;
            this.sprite = sprite;
            this.name = name;
            this.hardness = hardness;
            this.endurance = endurance;
            this.drop = drop;
            this.color = color;
        }

        public void Render(Transform transform)
        {
            sprite.RenderWithTransform(transform);
        }

    }
}
