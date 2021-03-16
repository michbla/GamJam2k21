using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace GamJam2k21
{
    //Typ wyliczeniowy reprezentujacy stany gry
    public enum GameState
    {
        active,
        paused,
        end
    }
    //Klasa okna gry
    public class Game : GameWindow
    {
        //Aktualny stan gry
        public GameState state;

        //WIDOK (Kamera):
        private Matrix4 projection;
        private Vector2 viewPos;

        //Sprite renderer do rysowania obiektow
        private SpriteRenderer spriteRenderer;

        //Konstruktor okna gry
        public Game(GameWindowSettings gWS, NativeWindowSettings nWS) : base(gWS, nWS)
        {
            state = GameState.active;
        }

        //Metoda inicjalizujaca
        protected override void OnLoad()
        {
            //Ustawianie kolor tla
            GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
            //Wlaczanie mieszania dla tekstur z alpha
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            //Pozycja widoku ustawiona tak, by lewy dolny rog mial wspolrzedne (0,0)
            viewPos = new Vector2(-8.0f, -4.5f);
            //Generuje projekcje ortograficzna 16/9
            projection = Matrix4.CreateOrthographic(16, 9, -1.0f, 1.0f);
            //Generowanie ResourcesManagera
            ResourceManager.GetInstance();
            //LADOWANIE SHADEROW
            ResourceManager.LoadShader("Data/Resources/Shaders/spriteShader/spriteShader.vert", "Data/Resources/Shaders/spriteShader/spriteShader.frag", "sprite");
            ResourceManager.GetShader("sprite").Use();
            ResourceManager.GetShader("sprite").SetInt("texture0", 0);
            ResourceManager.GetShader("sprite").SetMatrix4("view", Matrix4.CreateTranslation(viewPos.X,viewPos.Y,0.0f));
            ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);

            spriteRenderer = new SpriteRenderer(ResourceManager.GetShader("sprite"));
            //LADOWANIE TEKSTUR
            ResourceManager.LoadTexture("Data/Resources/Textures/dirt.png", "dirt");
            ResourceManager.LoadTexture("Data/Resources/Textures/grass_side.png", "grass");
            ResourceManager.LoadTexture("Data/Resources/Textures/sky.png", "sky");


            //TUTAJ KOD

            base.OnLoad();
        }
        //Obsluga logiki w kazdej klatce, dt - deltaTime (czas pomiedzy klatkami)
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //Nie obsluguje logiki jesli okno dziala w tle
            if (!IsFocused)
                return;
            //TEMP:: [ESC] zamyka okno
            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            if (KeyboardState.IsKeyDown(Keys.Down))
            {
                viewPos.Y += (float)e.Time * 100;
            }
            else if (KeyboardState.IsKeyDown(Keys.Up))
            {
                viewPos.Y -= (float)e.Time * 100;
            }
            //TUTAJ KOD

            base.OnUpdateFrame(e);
        }
        //Obsluga rysowania klatek, kolejnosc ma znaczenie
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //Czyszczenie buffera
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //TEST
            //ResourceManager.GetShader("sprite").SetMatrix4("view", Matrix4.CreateTranslation(viewPos));
            spriteRenderer.DrawSprite(ResourceManager.GetTexture("sky"), (-8.0f, -4.5f), (0, 0), (16, 9), 0);
            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 16; j++)
                {
                    if (i == 0)
                    {
                        spriteRenderer.DrawSprite(ResourceManager.GetTexture("grass"), viewPos, (j, -i), (1, 1), 0);
                    }
                    else
                    {
                        spriteRenderer.DrawSprite(ResourceManager.GetTexture("dirt"), viewPos, (j, -i), (1, 1), 0);
                    }
                }
            }
            //spriteRenderer.DrawSprite(ResourceManager.GetTexture("dirt"), viewPos, (0, 0, 0), (32, 32), 0);

            //TUTAJ KOD

            SwapBuffers();
            base.OnRenderFrame(e);
        }
        //Metoda finalizujaca dzialanie okna
        protected override void OnUnload()
        {
            //Czyszczenie pamieci itp.
            //Czyszczenie bufferow
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            //Czyszczenie zasobow
            ResourceManager.Clear();
            base.OnUnload();
        }
        //Obsluga zmiany rozmiaru okna
        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnResize(e);
        }
    }
}
