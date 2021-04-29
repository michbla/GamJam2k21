using System.Collections.Generic;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Sprite
    {
        private static readonly Dictionary<Vector2i, SpriteRenderer> renderers
            = new Dictionary<Vector2i, SpriteRenderer>();
        private static Shader spriteShader;

        private Texture texture;
        private Vector2 size;
        private Vector2i sheetSize;
        private Vector3 color;

        public Texture Texture
        {
            get => texture;
            set => texture = value;
        }
        public Vector2 Size { get => size; }
        public Vector3 Color
        {
            get => color;
            set { color = value; }
        }

        public static Sprite Single(Texture texture,
                             Vector2 size)
        {
            return new Sprite(texture, size, Vector2i.One, Vector3.One);
        }

        public static Sprite Sheet(Texture texture,
                            Vector2 singleSpriteSize,
                            Vector2i sheetSize)
        {
            return new Sprite(texture, singleSpriteSize, sheetSize, Vector3.One);
        }

        private Sprite(Texture texture,
                      Vector2 singleSpriteSize,
                      Vector2i sheetSize,
                      Vector3 color = default)
        {
            checkForSpriteShader();
            this.texture = texture;
            this.size = singleSpriteSize;
            this.color = color;
            this.sheetSize = sheetSize;
            makeRendererOfRatio(sheetSize);
        }

        private void checkForSpriteShader()
        {
            if (spriteShader == null)
                spriteShader = ResourceManager.GetShader("sprite");
        }

        private void makeRendererOfRatio(Vector2i sheetSize)
        {
            if (!renderers.ContainsKey(sheetSize))
            {
                SpriteRenderer renderer = new SpriteRenderer(spriteShader,
                                                             sheetSize);
                renderers.Add(sheetSize, renderer);
            }
        }

        private SpriteRenderer getRenderer()
        {
            return renderers[sheetSize];
        }

        public void RenderWithTransform(Transform transform,
                                        Vector2i spriteOffset = default,
                                        bool flipped = false)
        {
            getRenderer().DrawSprite(this, transform, spriteOffset, flipped);
        }
    }
}
