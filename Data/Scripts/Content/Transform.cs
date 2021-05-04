using System;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Transform
    {
        private Vector2 position;
        private Vector2 scale;
        private float rotation;

        public Vector2 Position
        {
            get => position;
            set { position = value; }
        }
        public Vector2 Scale
        {
            get => scale;
            set { scale = value; }
        }
        public float Rotation
        {
            get => rotation;
            set { rotation = value; }
        }

        public static Transform Default
        {
            get => new Transform(Vector2.Zero, Vector2.One);
        }

        public Transform(Vector2 position,
                         Vector2 scale,
                         float rotation = 0.0f)
        {
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
        }

        public Transform(Vector2 position)
        {
            this.position = position;
            this.scale = Vector2.One;
            this.rotation = 0.0f;
        }

    }
}
