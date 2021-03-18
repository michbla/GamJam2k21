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

    /// <summary>
    /// Obiekt kolizji przyczepiony do GameObjectu
    /// Stworzony do uproszczenia kalkulacji kolizji
    /// </summary>
    public class Collider
    {
        //Rodzic collidera
        private GameObject parent;

        //Pozycja i offset do podazania za rodzicem
        //Przy boxie oznacza lewy dolny rog,
        //a przy kole oznacza srodek kola
        public Vector2 position;
        private Vector2 offset;

        public Collider(GameObject p, Vector2 offset)
        {
            parent = p;
            position = parent.position + offset;
            this.offset = offset;
        }

        public void Update()
        {
            position = parent.position + offset;
        }

        public static (bool, Direction, Vector2) CheckCircleCollision(CircleCollider circle, GameObject collider)
        {
            Vector2 center = new Vector2(circle.position.X, circle.position.Y);
            Vector2 aabbHalfExtents = new Vector2(collider.size.X / 2f, collider.size.Y / 2f);
            Vector2 aabbCenter = new Vector2(
                collider.position.X + aabbHalfExtents.X,
                collider.position.Y + aabbHalfExtents.Y
                );
            Vector2 difference = center - aabbCenter;
            Vector2 clamped = new Vector2(MathHelper.Clamp(difference.X, -aabbHalfExtents.X, aabbHalfExtents.X), MathHelper.Clamp(difference.Y, -aabbHalfExtents.Y, aabbHalfExtents.Y));
            Vector2 closest = aabbCenter + clamped;
            difference = closest - center;
            if (difference.Length < circle.radius)
                return (true, VectorDirection(difference), difference);
            else
                return (false, Direction.up, (0.0f, 0.0f));
        }
        public static (bool, Direction, Vector2) CheckBoxCollision(BoxCollider box, GameObject collider)
        {
            bool collisionX = box.position.X + box.size.X >= collider.position.X &&
                collider.position.X + collider.size.X >= box.position.X;
            bool collisionY = box.position.Y + box.size.Y >= collider.position.Y &&
                collider.position.Y + collider.size.Y >= box.position.Y;
            if (!collisionX || !collisionY)
                return (false, Direction.up, (0.0f, 0.0f));
            Vector2 center = new Vector2(box.position.X + box.size.X / 2.0f, box.position.Y + box.size.Y / 2.0f);
            Vector2 collCenter = new Vector2(collider.position.X + collider.size.X / 2.0f, collider.position.Y + collider.size.Y / 2.0f);
            Vector2 difference = center - collCenter;
            return (true, VectorDirection(difference), difference);
        }
        //Obliczanie kierunku kolizji
        private static Direction VectorDirection(Vector2 target)
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
        public Vector2 size;
        public BoxCollider(GameObject p, Vector2 offset, Vector2 size) : base(p,offset)
        {
            this.size = size;
        }
    }
    public class CircleCollider : Collider
    {
        public float radius;
        public CircleCollider(GameObject p, Vector2 offset, float radius) : base(p, offset)
        {
            this.radius = radius;
        }
    }
}
