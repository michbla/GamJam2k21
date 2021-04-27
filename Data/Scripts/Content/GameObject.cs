using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa odpowiadajaca za wszelkie obiekty bedace czescia swiata gry
    /// </summary>
    public class GameObject
    {
        public Vector2 position = Vector2.Zero;
        public Vector2 size = Vector2.One;
        public Vector2 velocity = Vector2.Zero;
        public float rotation = 0.0f;
        public Vector3 color = Vector3.One;

        public bool isSolid = false;
        public bool isDestroyed = false;

        public Texture sprite = null;

        public GameObject(Vector2 position = default,
                          Vector2 size = default,
                          Texture sprite = null,
                          Vector3 color = default)
        {
            this.position = position;
            this.size = size;
            this.sprite = sprite;
            this.color = color;
        }

        public virtual void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            if (isInRenderDistance(viewPos))
                rend.DrawSprite(sprite, viewPos, position, size, rotation, color);
        }

        private bool isInRenderDistance(Vector2 viewPos)
        {
            float renderDistance = 9.0f;
            return position.Y + size.Y > viewPos.Y - renderDistance
                && position.Y < viewPos.Y + renderDistance
                && position.X + size.X > viewPos.X - renderDistance * 2.0f
                && position.X < viewPos.X + renderDistance * 2.0f;
        }

        public virtual void Update(KeyboardState input, MouseState mouseInput, float deltaTime)
        {

        }

        public virtual void Update(float deltaTime)
        {

        }
    }
}
