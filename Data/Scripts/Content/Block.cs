using OpenTK.Mathematics;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa odpowiadajaca za bloki
    /// </summary>
    public class Block : GameObject
    {
        public string name;

        public int hardness;

        public float endurance;

        public float distanceToPlayer;

        public Block(Vector2 pos, Texture sprite, string _name) : base(pos, (1.0f,1.0f), sprite)
        {
            distanceToPlayer = 100.0f;
            name = _name;
        }

    }
}
