using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa odpowiadajaca za wszelkie obiekty bedace czescia swiata gry
    /// </summary>
    public class GameObject
    {
        public Vector2 position;
        public Vector2 size;
        public Vector2 velocity;
        public float rotation;
        public Vector3 color;

        public bool isSolid = false;
        public bool isDestroyed = false;

        public Texture sprite;

        public GameObject() : this((0.0f, 0.0f), (1.0f, 1.0f), null, (1.0f, 1.0f, 1.0f), (0.0f, 0.0f)) { }
        
        public GameObject(Vector2 pos, Vector2 size, Texture sprite) : this(pos, size, sprite, (1.0f, 1.0f, 1.0f), (0.0f, 0.0f)) { }
        
        public GameObject(Vector2 pos, Vector2 size, Texture sprite, Vector3 col) : this(pos, size, sprite, col, (0.0f, 0.0f)) { }
        
        public GameObject(Vector2 pos, Vector2 size, Texture sprite, Vector3 col, Vector2 vel)
        {
            this.position = pos;
            this.size = size;
            this.sprite = sprite;
            this.color = col;
            this.velocity = vel;
        }
        
        public virtual void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            if (position.Y + size.Y < viewPos.Y - 9f || position.Y > viewPos.Y + 9f || position.X + size.X < viewPos.X - 18f || position.X > viewPos.X + 18f)
                return;
            rend.DrawSprite(sprite, viewPos, position, size, rotation, color);
        }
        
        public virtual void Update(KeyboardState input, MouseState mouseInput, float deltaTime)
        {

        }

        public virtual void Update(float deltaTime)
        {

        }
    }
}
