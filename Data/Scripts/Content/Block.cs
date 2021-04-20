using OpenTK.Mathematics;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa odpowiadajaca za bloki
    /// </summary>
    public class Block : GameObject
    {
        public string name;

        public int hardness = 1;

        public float baseEndurance = 100.0f;
        public float endurance = 100.0f;

        public float distanceToPlayer = 100.0f;

        private readonly static SpriteRenderer DESTRUCTION_RENDERER = new SpriteRenderer(ResourceManager.GetShader("sprite"), (10, 1));
        private readonly Texture DESTRUCTION_TEX = ResourceManager.GetTexture("dest");

        private Vector3 blockColor = (1.0f, 1.0f, 1.0f);

        private float regenCooldown = 0.0f;

        public Block(Vector2 pos, Texture sprite, string _name, Vector3 _color, int _hardness, float _endurance) : base(pos, (1.0f,1.0f), sprite)
        {
            name = _name;
            blockColor = _color;
            hardness = _hardness;
            baseEndurance = _endurance;
            endurance = baseEndurance;
        }
        
        public Block(Block copy, Vector2 pos) : base(pos,(1.0f,1.0f), copy.sprite)
        {
            name = copy.name;
            blockColor = copy.blockColor;
            hardness = copy.hardness;
            baseEndurance = copy.baseEndurance;
            endurance = baseEndurance;
        }
        
        public override void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            base.Draw(rend, viewPos);
            int destLevel = (int)((baseEndurance - endurance) / baseEndurance * 10);
            if(endurance < baseEndurance)
                DESTRUCTION_RENDERER.DrawSprite((destLevel, 0), DESTRUCTION_TEX, viewPos, position, size, rotation, blockColor);
        }
        
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (regenCooldown > 0.0f)
                regenCooldown -= deltaTime;
            else if (endurance < baseEndurance)
                Regenerate(deltaTime);
        }
        
        public bool Damage(Player player, int _hardness)
        {
            regenCooldown = 0.5f;
            if (player.isReadyToDamage && _hardness >= hardness)
            {
                endurance -= player.GetDamage();
                player.ResetCooldown();
                return true;
            }
            return false;
        }

        public Vector3 GetBlockColor()
        {
            return blockColor;
        }

        public bool IsDestroyed()
        {
            return endurance <= 0.0f;
        }

        public void Regenerate(float dt)
        {
            endurance += 200.0f * dt;
            if(endurance > baseEndurance)
                endurance = baseEndurance;
        }

    }
}
