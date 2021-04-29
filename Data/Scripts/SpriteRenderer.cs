using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class SpriteRenderer
    {
        private Shader shader;

        private int quadVAO;

        private Vector2i sheetSize;

        public SpriteRenderer(Shader shader, Vector2i sheetSize)
        {
            this.shader = shader;
            this.sheetSize = sheetSize;
            InitRenderData();
        }

        private void InitRenderData()
        {
            float[] vertPos = generateVertexQuad();
            float[] texPos = generateTextureQuad(getSingleSpriteSize());

            bindVertexArrayObject(vertPos, texPos);
        }

        private float[] generateVertexQuad()
        {
            return new float[] {
            //pos //tex
            0.0f, 1.0f,//lewy gorny
            1.0f, 0.0f,//prawy dolny
            0.0f, 0.0f,//lewy dolny

            0.0f, 1.0f,//lewy gorny
            1.0f, 1.0f,//prawy gorny
            1.0f, 0.0f//prawy dolny
            };
        }

        private float[] generateTextureQuad(Vector2 singleSpriteSize)
        {
            return new float[] {
            //pos                   //tex
            0.0f,                   0.0f, //lewy gorny
            singleSpriteSize.X,     singleSpriteSize.Y, //prawy dolny
            0.0f,                   singleSpriteSize.Y, //lewy dolny

            0.0f,                   0.0f, //lewy gorny
            singleSpriteSize.X,     0.0f, //prawy gorny
            singleSpriteSize.X,     singleSpriteSize.Y  //prawy dolny
            };
        }

        private Vector2 getSingleSpriteSize()
        {
            return new Vector2(1.0f / sheetSize.X, 1.0f / sheetSize.Y);
        }

        private void bindVertexArrayObject(float[] vertPos, float[] texPos)
        {
            int vbo = GL.GenBuffer();
            int texB = GL.GenBuffer();
            quadVAO = GL.GenVertexArray();

            GL.BindVertexArray(quadVAO);
            bindVertexBuffer(vbo, vertPos);
            bindTextureBuffer(texB, texPos);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void bindVertexBuffer(int vbo, float[] vertPos)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                vertPos.Length * sizeof(float),
                vertPos,
                BufferUsageHint.StaticDraw);
            enableVertexAttribArray(0);
        }

        private void bindTextureBuffer(int texB, float[] texPos)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, texB);
            GL.BufferData(BufferTarget.ArrayBuffer,
                texPos.Length * sizeof(float),
                texPos,
                BufferUsageHint.StaticDraw);
            enableVertexAttribArray(1);
        }

        private void enableVertexAttribArray(int index)
        {
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index,
                2,
                VertexAttribPointerType.Float,
                false,
                2 * sizeof(float),
                0);
        }

        public void DrawSprite(Sprite sprite,
                               Transform transform,
                               Vector2i spriteOffset = default,
                               bool isFlipped = false)
        {
            shader.Use();

            Matrix4 model = Matrix4.Identity;

            Vector2 size = (sprite.Size.X * transform.Scale.X,
                            sprite.Size.Y * transform.Scale.Y);

            model = scaleModel(model, size);
            model = moveToCenter(model, size);
            if (isFlipped)
                model = flip(model);
            model = rotate(model, transform.Rotation);
            model = moveBack(model, size);
            model = setPosition(model, transform.Position);

            setShaderProperties(model, sprite.Color, spriteOffset);

            sprite.Texture.Use(TextureUnit.Texture0);

            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }

        private Matrix4 scaleModel(Matrix4 model, Vector2 scale)
        {
            return model * Matrix4.CreateScale(scale.X, scale.Y, 1.0f);
        }

        private Matrix4 moveToCenter(Matrix4 model, Vector2 size)
        {
            return model * Matrix4.CreateTranslation(-0.5f * size.X, -0.5f * size.Y, 0.0f);
        }

        private Matrix4 flip(Matrix4 model)
        {
            return model * Matrix4.CreateScale(-1.0f, 1.0f, 1.0f);
        }

        private Matrix4 rotate(Matrix4 model, float degrees)
        {
            float radians = (float)MathHelper.DegreesToRadians(degrees);
            return model * Matrix4.CreateRotationZ(radians);
        }

        private Matrix4 moveBack(Matrix4 model, Vector2 size)
        {
            return model * Matrix4.CreateTranslation(0.5f * size.X, 0.5f * size.Y, 0.0f);
        }

        private Matrix4 setPosition(Matrix4 model, Vector2 position)
        {
            return model * Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        }

        private void setShaderProperties(Matrix4 model, Vector3 color, Vector2i offset)
        {
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", Matrix4.CreateTranslation(-Camera.Position.X, -Camera.Position.Y, 0.0f));
            shader.SetVector3("spriteColor", color);

            Vector2 singleSpriteSize = getSingleSpriteSize();
            shader.SetVector2("texOffset", (offset.X * singleSpriteSize.X, offset.Y * singleSpriteSize.Y));
        }

        ~SpriteRenderer()
        {
            GL.DeleteVertexArray(quadVAO);
        }
    }
}
