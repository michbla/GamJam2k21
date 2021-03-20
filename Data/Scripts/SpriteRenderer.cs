using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa renderujaca sprite'y 2D.
    /// Moze byc uzywana dla roznych obiektow wykorzystujacych ten sam shader.
    /// </summary>
    public class SpriteRenderer
    {
        //Shader
        private Shader shader;
        //Buffer prostokata
        private int quadVAO;

        //Ile spriteow jest w kazdym wymiarze tekstury
        Vector2i dim;

        //Wstepna inicjalizacja
        private void InitRenderData()
        {
            //Kazdy sprite jest prostokatem
            //wiec nie musi posiadac informacji
            //o ksztalcie bryly
            int vbo;
            int texB;

            float[] vertPos = {
            //pos       //tex
            0.0f, 1.0f,//lewy gorny
            1.0f, 0.0f,//prawy dolny
            0.0f, 0.0f,//lewy dolny

            0.0f, 1.0f,//lewy gorny
            1.0f, 1.0f,//prawy gorny
            1.0f, 0.0f//prawy dolny
            };

            float sizeX = 1.0f / dim.X;
            float sizeY = 1.0f / dim.Y;

            float[] texPos = {
            //pos       //tex
            0.0f, 0.0f, //lewy gorny
            sizeX, sizeY, //prawy dolny
            0.0f, sizeY, //lewy dolny

            0.0f, 0.0f, //lewy gorny
            sizeX, 0.0f, //prawy gorny
            sizeX, sizeY  //prawy dolny
            };

            quadVAO = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            texB = GL.GenBuffer();

            GL.BindVertexArray(quadVAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertPos.Length * sizeof(float), vertPos, BufferUsageHint.StaticDraw);


            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, texB);
            GL.BufferData(BufferTarget.ArrayBuffer, texPos.Length * sizeof(float), texPos, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        //Konstuktor
        public SpriteRenderer(Shader sha, Vector2i d)
        {
            shader = sha;
            dim = d;
            InitRenderData();
        }
        public SpriteRenderer(Shader sha) : this(sha, new Vector2i(1, 1)) { }
        //Destruktor
        ~SpriteRenderer()
        {
            GL.DeleteVertexArray(quadVAO);
        }
        //Rysowanie sprite'a
        public void DrawSprite(Vector2i offset, Texture tex, Vector2 viewPos, Vector2 pos, Vector2 size, float rotate, Vector3 color)
        {
            shader.Use();
            Matrix4 model = Matrix4.Identity;
            //Skalowanie
            model *= Matrix4.CreateScale(size.X, size.Y, 1.0f);
            //Obracanie wzgledem srodka
            model *= Matrix4.CreateTranslation(-0.5f * size.X, -0.5f * size.Y, 0.0f);
            model *= Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(-rotate));
            model *= Matrix4.CreateTranslation(0.5f * size.X, 0.5f * size.Y, 0.0f);
            //Przemieszczanie
            model *= Matrix4.CreateTranslation(pos.X, pos.Y, 0.0f);
            //Ustawianie wartosci shadera
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", Matrix4.CreateTranslation(-viewPos.X, -viewPos.Y, 0.0f));
            shader.SetVector3("spriteColor", color);
            shader.SetVector2("texOffset", (offset.X*(1.0f/dim.X), offset.Y*(1.0f/dim.Y)));
            //Uzywanie tekstury
            tex.Use(TextureUnit.Texture0);
            //Rysowanie geometrii
            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
        }
        //Rysowanie ze standardowym kolorem
        public void DrawSprite(Texture tex, Vector2 viewPos, Vector2 pos, Vector2 size, float rotate) { DrawSprite((0, 0), tex, viewPos, pos, size, rotate, (1.0f, 1.0f, 1.0f)); }
        public void DrawSprite(Texture tex, Vector2 viewPos, Vector2 pos, Vector2 size, float rotate,Vector3 color) { DrawSprite((0, 0), tex, viewPos, pos, size, rotate, color); }
    }
}
