using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class GameObject
    {
        private static readonly float RENDER_DISTANCE = 9.0f;

        private Sprite sprite;
        private Transform transform;

        public bool IsSolid = false;

        public Sprite Sprite { get => sprite; }
        public Texture Texture { set => sprite.Texture = value; }
        public Vector2 Size { get => sprite.Size; }
        public Vector2 Position
        {
            get => transform.Position;
            set { transform.Position = value; }
        }
        public Vector3 Color
        {
            get => sprite.Color;
            set { sprite.Color = value; }
        }
        public Transform Transform { get => transform; }
        public Vector2 Scale
        {
            get => transform.Scale;
            set { transform.Scale = value; }
        }
        public float Rotation
        {
            get => transform.Rotation;
            set { transform.Rotation = value; }
        }

        public GameObject(Sprite sprite,
                          Transform transform)
        {
            this.transform = transform;
            this.sprite = sprite;
        }

        public virtual void Render()
        {
            if (isInRenderDistance())
                sprite.RenderWithTransform(transform);
        }

        private bool isInRenderDistance()
        {
            return Position.Y + Size.Y > Camera.Position.Y - RENDER_DISTANCE
                && Position.Y < Camera.Position.Y + RENDER_DISTANCE
                && Position.X + Size.X > Camera.Position.X - RENDER_DISTANCE * 2.0f
                && Position.X < Camera.Position.X + RENDER_DISTANCE * 2.0f;
        }

        public virtual void Update()
        {

        }
    }
}
