using OpenTK.Mathematics;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa odpowiadajaca za bloki
    /// </summary>
    public class Block : GameObject
    {
        //Block name
        public string name;

        //Block hardness
        public int hardness = 1;

        //Block endurance
        public float baseEndurance = 100.0f;
        public float endurance = 100.0f;

        //Distance to player
        public float distanceToPlayer = 100.0f;

        //Destruction visualisation
        private static SpriteRenderer destRend = new SpriteRenderer(ResourceManager.GetShader("sprite"), (10, 1));
        private Texture destTex = ResourceManager.GetTexture("dest");

        //Particle and destruction color
        private Vector3 blockColor = (1.0f, 1.0f, 1.0f);

        private float regenCooldown = 0.0f;

        //Konstruktor
        public Block(Vector2 pos, Texture sprite, string _name, Vector3 _color, int _hardness, float _endurance) : base(pos, (1.0f,1.0f), sprite)
        {
            name = _name;
            blockColor = _color;
            hardness = _hardness;
            baseEndurance = _endurance;
            endurance = baseEndurance;
        }
        //Konstruktor kopiujacy
        public Block(Block copy, Vector2 pos) : base(pos,(1.0f,1.0f), copy.sprite)
        {
            this.name = copy.name;
            this.blockColor = copy.blockColor;
            this.hardness = copy.hardness;
            this.baseEndurance = copy.baseEndurance;
            endurance = baseEndurance;
        }
        //Rysowanie
        public override void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            base.Draw(rend, viewPos);
            int destLevel = (int)((baseEndurance - endurance) / baseEndurance * 10);
            if(endurance < baseEndurance)
                destRend.DrawSprite((destLevel, 0), destTex, viewPos, position, size, rotation, blockColor);
        }
        //Aktualizacja
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (regenCooldown > 0.0f)
                regenCooldown -= deltaTime;
            else
                Regenerate();
        }
        //Zadawanie obrazen
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

        public void Regenerate()
        {
            endurance = baseEndurance;
        }

    }
}
