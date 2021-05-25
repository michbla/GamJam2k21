using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Block : GameObject
    {
        private readonly string name;

        private int hardness;

        private float endurance;
        private float baseEndurance;

        private Vector3 effectsColor;

        private float exp;

        private float distanceToPlayer;

        private Sprite destruction = Sprite.Sheet(
            ResourceManager.GetTexture("destruction"),
            Vector2.One,
            (10, 1));

        private float regenCooldown = 0.0f;

        private Ore ore = null;

        public bool IsCollidable = true;

        private BoxCollider collider;
        public string Name { get => name; }
        public int Hardness { get => hardness; }
        public Vector3 EffectsColor { get => effectsColor; }
        public float Exp { get => exp; }
        public float DistanceToPlayer
        {
            get => distanceToPlayer;
            set { distanceToPlayer = value; }
        }
        public Ore Ore { get => ore; }
        public BoxCollider Collider { get => collider; }

        public Block(Sprite sprite,
                     Transform transform,
                     string name,
                     int hardness,
                     float endurance,
                     Vector3 effectsColor,
                     float exp)
            : base(sprite, transform)
        {
            this.name = name;
            this.effectsColor = effectsColor;
            this.hardness = hardness;
            baseEndurance = endurance;
            this.endurance = baseEndurance;
            destruction.Color = effectsColor;
            this.exp = exp;
            collider = new BoxCollider(this.Transform, Vector2.One);
        }

        public Block(Block copy, Transform transform, Ore ore = null)
            : base(copy.Sprite, transform)
        {
            name = copy.name;
            effectsColor = copy.effectsColor;
            hardness = copy.hardness;
            baseEndurance = copy.baseEndurance;
            exp = copy.exp;
            if (ore != null)
                setOre(ore);
            endurance = baseEndurance;
            destruction.Color = effectsColor;
            collider = new BoxCollider(this.Transform, Vector2.One);
        }

        private void setOre(Ore ore)
        {
            this.ore = ore;
            baseEndurance += ore.Endurance;
            if (ore.Hardness > hardness)
                hardness = ore.Hardness;
            effectsColor = ore.Color;
            exp += ore.Exp;
        }

        public override void Render()
        {
            base.Render();
            if (HasOre())
                ore.Render(Transform);
            int destLevel = getDestructionLevel();
            if (destLevel > 0)
                destruction.RenderWithTransform(Transform,
                                                (destLevel, 0));
        }

        public bool HasOre()
        {
            return ore != null;
        }

        private int getDestructionLevel()
        {
            return (int)((baseEndurance - endurance) / baseEndurance * 10);
        }

        public override void Update()
        {
            base.Update();
            if (regenCooldown > 0.0f)
                regenCooldown -= Time.DeltaTime;
            else if (endurance < baseEndurance)
                regenerate();
        }

        private void regenerate()
        {
            endurance += 200.0f * Time.DeltaTime;
            if (endurance > baseEndurance)
                endurance = baseEndurance;
        }

        public bool CanBeDamaged(int hardness)
        {
            return hardness >= this.hardness;
        }

        public void Damage(float damageAmount)
        {
            regenCooldown = 0.5f;
            endurance -= damageAmount;
        }

        public bool IsDestroyed()
        {
            return endurance <= 0.0f;
        }

        public Item GetDrop()
        {
            return ore.Drop;
        }
    }
}
