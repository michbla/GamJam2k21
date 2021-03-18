using System;
using System.Diagnostics;
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
        private float cameraFollowSpeed = 5.0f;

        //Pozycja myszy na ekranie
        private Vector2 mousePos;
        //Pozycja myszy w swiecie
        private Vector2 mouseWorldPos;

        //Sprite renderer do rysowania obiektow
        private SpriteRenderer spriteRenderer;

        //Gracz
        private Player player;

        //Poziom gry
        private GameLevel level;

        //TEST::
        float scale = 2.0f;

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
            CenterWindow();

            //Pozycja widoku ustawiona tak, by lewy dolny rog mial wspolrzedne (0,0)
            viewPos = new Vector2(8.0f , 4.5f );
            //Generuje projekcje ortograficzna 16/9
            projection = Matrix4.CreateOrthographic(16 * scale, 9 * scale, -1.0f, 1.0f);
            //Generowanie ResourcesManagera
            ResourceManager.GetInstance();
            //LADOWANIE SHADEROW
            ResourceManager.LoadShader("Data/Resources/Shaders/spriteShader/spriteShader.vert", "Data/Resources/Shaders/spriteShader/spriteShader.frag", "sprite");
            ResourceManager.GetShader("sprite").Use();
            ResourceManager.GetShader("sprite").SetInt("texture0", 0);
            ResourceManager.GetShader("sprite").SetMatrix4("view", Matrix4.CreateTranslation(-viewPos.X, -viewPos.Y, 0.0f));
            ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);

            spriteRenderer = new SpriteRenderer(ResourceManager.GetShader("sprite"));
            //LADOWANIE TEKSTUR
            ResourceManager.LoadTexture("Data/Resources/Textures/dirt1.png", "dirt");
            ResourceManager.LoadTexture("Data/Resources/Textures/grass1.png", "grass");
            ResourceManager.LoadTexture("Data/Resources/Textures/stone1.png", "stone");
            ResourceManager.LoadTexture("Data/Resources/Textures/sky2.png", "sky");
            ResourceManager.LoadTexture("Data/Resources/Textures/char.png", "char");
            ResourceManager.LoadTexture("Data/Resources/Textures/cursor2.png", "cursor");

            //Gracz
            player = new Player((7.5f, 1.0f), (1.0f, 2.0f), ResourceManager.GetTexture("char"));
            //Poziom
            level = new GameLevel(64, 200);

            //TUTAJ KOD

            base.OnLoad();
        }
        //Obsluga logiki w kazdej klatce, dt - deltaTime (czas pomiedzy klatkami)
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //Wylaczenie widoku kursora, mamy wlasny
            CursorVisible = false;
            //Delta Czas
            float deltaTime = (float)e.Time;
            //Nie obsluguje logiki jesli okno dziala w tle
            if (!IsFocused)
                return;
            //TEMP:: [ESC] zamyka okno
            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            //Zmienne inputowe
            var mouseInput = MouseState;
            var input = KeyboardState;

            //Obliczanie pozycji myszy
            float mouseScale = (16.0f * scale) / Size.X;
            mousePos.X = mouseInput.Position.X * mouseScale;
            mousePos.Y = -(mouseInput.Position.Y - Size.Y) * mouseScale;
            mouseWorldPos = mousePos + viewPos - (16.0f * scale / 2.0f, 9.0f * scale/2.0f);

            //Kolizja TESTOWANA U GRACZA
            //Przekazuje bloki do gracza dla kolizji
            player.SetBlocks(ref level.currentBlocks);
            //Aktualizacja logiki gracza
            player.Update(input, deltaTime);

            //TEMP:: Testowe kopanie
            if (mouseInput.IsButtonDown(MouseButton.Left))
            {
                foreach (var block in level.currentBlocks)
                {
                    if (block.distanceToPlayer <= 2.0f && !block.isDestroyed)
                    {
                        double mPX = Math.Floor(mouseWorldPos.X);
                        double mPY= Math.Floor(mouseWorldPos.Y);
                        if(block.position.X == mPX && block.position.Y == mPY)
                        {
                            block.isDestroyed = true;
                        }
                    }
                }
            }

            //Plynne podazanie kamery za graczem
            float desiredViewX = MathHelper.Lerp(viewPos.X, player.position.X + player.size.X / 2.0f, deltaTime * cameraFollowSpeed);
            float desiredViewY = MathHelper.Lerp(viewPos.Y, player.position.Y + player.size.Y * 0.7f, deltaTime * cameraFollowSpeed);
            viewPos.X = desiredViewX;
            viewPos.Y = desiredViewY;

            //TEMP:: Ruch kamery przez przypadek dziala jak rozgladanie (feature?)
            if (KeyboardState.IsKeyDown(Keys.Down))
            {
                viewPos.Y -= deltaTime * 10;
            }
            else if (KeyboardState.IsKeyDown(Keys.Up))
            {
                viewPos.Y += deltaTime * 10;
            }
            if (KeyboardState.IsKeyDown(Keys.Left))
            {
                viewPos.X -= deltaTime * 10;
            }
            else if (KeyboardState.IsKeyDown(Keys.Right))
            {
                viewPos.X += deltaTime * 10;
            }

            //Aktualizacja poziomu (glownie chodzi o obliczanie dystansu od gracza dla kazdego bloku)
            level.Update(player.playerCenter);

            //TUTAJ KOD

            base.OnUpdateFrame(e);
        }
        //Obsluga rysowania klatek, kolejnosc ma znaczenie
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //Czyszczenie buffera
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //TEST
            //Rysowanie tla
            spriteRenderer.DrawSprite(ResourceManager.GetTexture("sky"), (8.0f * scale, 4.5f * scale), (0.0f, 0.0f), (16.0f * scale, 9.0f * scale), 0.0f);
            //Rysowanie poziomu
            level.Draw(spriteRenderer, viewPos);

            //Rysowanie gracza
            player.Draw(spriteRenderer, viewPos);

            //Rysowanie kursora
            spriteRenderer.DrawSprite(ResourceManager.GetTexture("cursor"), (8.0f * scale, 4.5f * scale), mousePos - (0.0f, 1.0f), (1.0f, 1.0f), 0.0f);

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
