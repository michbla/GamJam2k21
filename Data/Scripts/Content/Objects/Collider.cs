using OpenTK.Mathematics;

namespace GamJam2k21
{
    public enum Direction
    {
        up,
        right,
        down,
        left
    }
    public abstract class Collider
    {
        private Transform parent;

        private Vector2 position;
        private Vector2 offset;

        public Vector2 Position { get => position; }

        public void SetPosition(Vector2 newPosition)
        {
            parent.Position = newPosition;
        }

        public Collider(Transform parent, Vector2 offset = default)
        {
            this.parent = parent;
            position = parent.Position + offset;
            this.offset = offset;
        }

        public void Update()
        {
            followParent();
        }

        private void followParent()
        {
            position = parent.Position + offset;
        }

        public abstract (bool, Direction, Vector2) CheckCollision(BoxCollider collider);

        protected static Direction VectorDirection(Vector2 target)
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

    public class BoxCollider : Collider
    {
        private Vector2 size;

        public Vector2 Size
        {
            get => size;
        }

        public BoxCollider(Transform parent, Vector2 size, Vector2 offset = default)
            : base(parent, offset)
        {
            this.size = size;
        }

        public override (bool, Direction, Vector2) CheckCollision(BoxCollider collider)
        {
            bool collisionX = Position.X + size.X >= collider.Position.X &&
                collider.Position.X + collider.Size.X >= Position.X;
            bool collisionY = Position.Y + Size.Y >= collider.Position.Y &&
                collider.Position.Y + collider.Size.Y >= Position.Y;
            if (!collisionX || !collisionY)
                return (false, Direction.up, (0.0f, 0.0f));
            Vector2 center = new Vector2(Position.X + Size.X / 2.0f, Position.Y + Size.Y / 2.0f);
            Vector2 collCenter = new Vector2(
                collider.Position.X + collider.Size.X / 2.0f,
                collider.Position.Y + collider.Size.Y / 2.0f);
            Vector2 difference = center - collCenter;
            return (true, VectorDirection(difference), difference);
        }
    }
    public class CircleCollider : Collider
    {
        private float radius;

        public float Radius
        {
            get => radius;
        }

        public CircleCollider(Transform parent, float radius, Vector2 offset = default)
            : base(parent, offset)
        {
            this.radius = radius;
        }

        public override (bool, Direction, Vector2) CheckCollision(BoxCollider collider)
        {
            Vector2 center = new Vector2(Position.X, Position.Y);
            Vector2 aabbHalfExtents = new Vector2(collider.Size.X / 2f, collider.Size.Y / 2f);
            Vector2 aabbCenter = new Vector2(
                collider.Position.X + aabbHalfExtents.X,
                collider.Position.Y + aabbHalfExtents.Y);
            Vector2 difference = center - aabbCenter;
            Vector2 clamped = new Vector2(
                MathHelper.Clamp(difference.X, -aabbHalfExtents.X, aabbHalfExtents.X),
                MathHelper.Clamp(difference.Y, -aabbHalfExtents.Y, aabbHalfExtents.Y));
            Vector2 closest = aabbCenter + clamped;
            difference = closest - center;
            if (difference.Length < radius)
                return (true, alterDirection(VectorDirection(difference)), difference);
            else
                return (false, Direction.up, (0.0f, 0.0f));
        }

        private Direction alterDirection(Direction original)
        {
            Direction result = Direction.up;
            switch (original)
            {
                case Direction.up:
                    result = Direction.down;
                    break;
                case Direction.down:
                    result = Direction.up;
                    break;
                case Direction.left:
                    result = Direction.right;
                    break;
                case Direction.right:
                    result = Direction.left;
                    break;
            }
            return result;
        }
    }
}
