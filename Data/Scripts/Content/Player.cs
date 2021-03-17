using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa obiektu gracza
    /// </summary>
    public class Player : GameObject
    {
        public float playerSpeed = 5.0f;
        //Flagi
        public bool canMove = true;

        public Player(Vector2 pos, Vector2 size, Texture sprite) : base(pos, size, sprite)
        {
            
        }

        //Logika gracza
        public override void Update(KeyboardState input, float deltaTime)
        {
            if (canMove)
            {
                float velocity = playerSpeed * deltaTime;

                if (input.IsKeyDown(Keys.A))
                {
                    //MOVE LEFT
                    position.X -= velocity;
                }
                else if (input.IsKeyDown(Keys.D))
                {
                    //MOVE RIGHT
                    position.X += velocity;
                }
            }
        }
    }
}
