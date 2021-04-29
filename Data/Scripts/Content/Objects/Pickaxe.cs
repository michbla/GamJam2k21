namespace GamJam2k21
{
    public class Pickaxe
    {
        private string name;

        private Sprite sprite;

        private int hardness;
        private float damage;

        public string Name { get => name; }
        public Sprite Sprite { get => sprite; }
        public int Hardness { get => hardness; }
        public float Damage { get => damage; }

        public Pickaxe(string name, Sprite sprite, int hardness, float damage)
        {
            this.name = name;
            this.sprite = sprite;
            this.hardness = hardness;
            this.damage = damage;
        }

        public void Render(Transform transform)
        {
            sprite.RenderWithTransform(transform);
        }
    }
}
