using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa obiektu gracza
    /// </summary>
    public class Player : GameObject
    {
        //Predkosc gracza
        public float playerSpeed = 8f;

        public float jumpForce = 10.0f;
        public float jumpingTime = 0.0f;

        private float fallMultiplier = 0.01f;
        private float lowJumpMultiplier = 0.5f;

        //Flagi
        public bool canMove = true;
        public bool isGrounded = true;

        public bool cDown = false;
        public bool cUp = false;
        public bool cLeft = false;
        public bool cRight = false;

        //Rozmiar collidera gracza
        private Vector2 colliderSize = (0.9f, 1.8f);

        //Grawitacja
        private float gravity = 30.0f;

        private Vector2 lastPlayerPos;

        public Player(Vector2 pos, Vector2 size, Texture sprite) : base(pos, size, sprite)
        {
            lastPlayerPos = pos;
            velocity = (0.0f, 0.0f);
        }

        //Logika gracza
        public override void Update(KeyboardState input, float deltaTime)
        {

            if (canMove)
            {
                float vel = playerSpeed * deltaTime;
                if (!isGrounded)
                    vel = vel * 0.8f;

                if (input.IsKeyDown(Keys.A))
                {
                    //MOVE LEFT
                    position.X -= vel;
                }
                else if (input.IsKeyDown(Keys.D))
                {
                    //MOVE RIGHT
                    position.X += vel;
                }

                if (isGrounded && input.IsKeyDown(Keys.Space))
                {
                    velocity.Y = 1.0f * jumpForce;
                }
            }
            if (!isGrounded)
            {
                velocity.Y -= gravity * deltaTime;

                if (velocity.Y < 0.0f)
                {
                    velocity.Y += gravity * (fallMultiplier - 1) * deltaTime;
                }
                else if (velocity.Y > 0.0f && !input.IsKeyDown(Keys.Space))
                {
                    velocity.Y += gravity * (lowJumpMultiplier - 1) * deltaTime;
                }
            }
            position.Y += velocity.Y * deltaTime;

            lastPlayerPos = position;
        }
        //Sprawdzenie kolizji z obiektem <collider>
        //Tymczasowo tylko na graczu, poniewaz tylko gracz moze sie ruszac
        public void CheckCollision(GameObject collider)
        {
            bool collisionX = position.X + colliderSize.X >= collider.position.X &&
                collider.position.X + collider.size.X >= position.X + 1.0f - colliderSize.X;
            bool collisionY = position.Y + colliderSize.Y >= collider.position.Y &&
                collider.position.Y + collider.size.Y >= position.Y;
            if (collisionY && collisionX)
            {
                Vector2 center = new Vector2(position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);
                Vector2 collCenter = new Vector2(collider.position.X + collider.size.X / 2.0f, collider.position.Y + collider.size.Y / 2.0f);

                Vector2 difference = center - collCenter;

                Direction dir = VectorDirection(difference);
                if (dir == Direction.up && center.X >= collider.position.X && center.X <= collider.position.X + size.X)
                {
                    isGrounded = true;
                    float diff = collider.position.Y + collider.size.Y - position.Y;
                    position.Y = position.Y + diff;
                }
                else if (dir == Direction.down && center.X >= collider.position.X && center.X <= collider.position.X + size.X)
                {
                    float diff = position.Y + size.Y - collider.position.Y;
                    position.Y = position.Y - diff - size.Y + colliderSize.Y;
                }
                else if (dir == Direction.left && center.Y >= collider.position.Y && center.Y <= collider.position.Y + size.Y)
                {
                    float diff = position.X + size.X - collider.position.X;
                    position.X = position.X - diff + size.X - colliderSize.X;
                }
                else if (dir == Direction.right && center.Y >= collider.position.Y && center.Y <= collider.position.Y + size.Y)
                {
                    float diff = collider.position.X + collider.size.X - position.X;
                    position.X = position.X + diff - size.X + colliderSize.X;
                }
            }
        }

        enum Direction
        {
            up,
            right,
            down,
            left
        }
        //Obliczanie kierunku kolizji
        private Direction VectorDirection(Vector2 target)
        {
            Vector2[] compass =
            {
                (0.0f,1.0f),//up
                (1.0f,0.0f),//right
                (0.0f,-1.0f),//down
                (-1.0f,0.0f),//left
            };
            float max = 0.0f;
            int bestMatch = -1;
            for (var i = 0; i < 4; i++)
            {
                float dotProduct = Vector2.Dot(Vector2.Normalize(target), compass[i]);
                if (dotProduct > max)
                {
                    max = dotProduct;
                    bestMatch = i;
                }
            }
            return (Direction)bestMatch;
        }
    }
}
