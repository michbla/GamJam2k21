using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa obiektu gracza
    /// </summary>
    public class Player : GameObject
    {
        public Vector2 playerCenter;
        //Predkosc gracza
        private float playerSpeed = 8.0f;
        //Sila skoku
        private float jumpForce = 8.0f;
        //Zmienne do plynnego skoku
        private float fallMultiplier = 0.01f;
        private float lowJumpMultiplier = 0.5f;

        private float rememberGrounded = 0.0f;

        //Flagi
        public bool canMove = true;
        public bool isGrounded = true;

        //Collidery
        private CircleCollider groundChecker;
        private CircleCollider ceilingChecker;

        private float minY = float.MinValue;
        private float maxY = float.MaxValue;

        private float minX = float.MinValue;
        private float maxX = float.MaxValue;

        //Grawitacja
        private float gravity = 30.0f;
        //Ostatnia pozycja gracza
        private Vector2 lastPlayerPos;
        //Kostruktor
        public Player(Vector2 pos, Vector2 size, Texture sprite) : base(pos, size, sprite)
        {
            lastPlayerPos = pos;
            velocity = (0.0f, 0.0f);
            playerCenter = new Vector2(position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);
            groundChecker = new CircleCollider(this, new Vector2(this.size.X / 2.0f, 0.2f), 0.25f);
            ceilingChecker = new CircleCollider(this, new Vector2(this.size.X / 2.0f, 1.8f), 0.25f);
        }
        //Logika gracza
        public override void Update(KeyboardState input, float deltaTime)
        {
            playerCenter = (position.X + size.X / 2.0f, position.Y + size.Y / 2.0f);

            groundChecker.Update();
            ceilingChecker.Update();

            if (canMove)
            {
                float vel = playerSpeed * deltaTime;
                if (!isGrounded)
                {
                    vel *= 0.8f;
                    if (rememberGrounded > 0.0f)
                        rememberGrounded -= deltaTime;
                }
                else
                {
                    rememberGrounded = 0.1f;
                }

                if (input.IsKeyDown(Keys.A))
                {
                    position.X -= vel;
                }
                else if (input.IsKeyDown(Keys.D))
                {
                    position.X += vel;
                }

                if (rememberGrounded > 0.0f && input.IsKeyDown(Keys.Space))
                {
                    velocity.Y = 1.0f * jumpForce;
                }
            }
            //Grawitacja tylko jesli w powietrzu
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

            position.Y = MathHelper.Clamp(position.Y, minY, maxY);

            //groundChecker.Update();

            lastPlayerPos = position;
        }

        public void ResetBounds()
        {
            minY = float.MinValue;
            maxY = float.MaxValue;
            minX = float.MinValue;
            maxX = float.MaxValue;
            isGrounded = false;
        }

        //Sprawdzenie kolizji z obiektem <collider>
        //Tymczasowo tylko na graczu, poniewaz tylko gracz moze sie ruszac
        //BUG:: Kolizje maja jakis problem ze soba
        //Pomysl na rozwiazanie:
        //-policzyc pozycje gracza w nastepnej klatce
        //-sprawdzic jej kolicje
        //-jesli koliduje z obiektem
        //-zabronic graczowi sie ruszac w tym kierunku za pomoca booli albo czegos w tym stylu
        public void CheckCollision(GameObject collider)
        {
            (bool, Direction, Vector2) groundRes = Collider.CheckCircleCollision(groundChecker, collider);
            if (groundRes.Item1 == true)
            {
                //Is grounded
                isGrounded = true;
                minY = collider.position.Y + collider.size.Y;
            }
            (bool, Direction, Vector2) ceilRes = Collider.CheckCircleCollision(ceilingChecker, collider);
            if (ceilRes.Item1 == true)
            {
                maxY = collider.position.Y - size.Y;
            }
        }
        /*public void CheckCollision(GameObject collider)
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
        }*/
        /*
        public (bool, Direction, Vector2) CheckCircleCollision(Vector2 circleCenter, float radius, GameObject collider)
        {
            Vector2 center = new Vector2(circleCenter.X + radius, circleCenter.Y + radius);
            Vector2 aabbHalfExtents = new Vector2(collider.size.X / 2f, collider.size.Y / 2f);
            Vector2 aabbCenter = new Vector2(
                collider.position.X + aabbHalfExtents.X,
                collider.position.Y + aabbHalfExtents.Y
                );
            Vector2 difference = center - aabbCenter;
            Vector2 clamped = new Vector2(MathHelper.Clamp(difference.X, -aabbHalfExtents.X, aabbHalfExtents.X), MathHelper.Clamp(difference.Y, -aabbHalfExtents.Y, aabbHalfExtents.Y));
            Vector2 closest = aabbCenter + clamped;
            difference = closest - center;
            if (difference.Length < radius)
                return (true, VectorDirection(difference), difference);
            else
                return (false, Direction.up, (0.0f, 0.0f));
        }
        public (bool, Direction, Vector2) CheckBoxCollision(Vector2 boxPos, Vector2 boxSize, GameObject collider)
        {
            bool collisionX = boxPos.X + boxSize.X >= collider.position.X &&
                collider.position.X + collider.size.X >= boxPos.X + boxSize.X;
            bool collisionY = boxPos.Y + boxSize.Y >= collider.position.Y &&
                collider.position.Y + collider.size.Y >= boxPos.Y;
            if(!collisionX || !collisionY)
                return (false, Direction.up, (0.0f, 0.0f));
            Vector2 center = new Vector2(boxPos.X + boxSize.X / 2.0f, boxPos.Y + boxSize.Y / 2.0f);
            Vector2 collCenter = new Vector2(collider.position.X + collider.size.X / 2.0f, collider.position.Y + collider.size.Y / 2.0f);
            Vector2 difference = center - collCenter;
            return (true, VectorDirection(difference), difference);
        }
        public enum Direction
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
        }*/
    }
}
