using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;

namespace GamJam
{
    public enum GameState
    {
        game_active,
        game_paused,
        game_end
    }

    public class Game : GameWindow
    {
        //Stan gry
        public GameState state;
        //Lista poziomow
        private List<GameLevel> levels;
        //Aktualny poziom
        private int currentLevel;

        //Obiekt gracza
        private GameObject player;
        //Rozmiar gracza
        private readonly Vector2 playerSize = new Vector2(100.0f, 20.0f);
        //Predkosc gracza
        private readonly float playerVelocity = 500.0f;

        //Obiekt pilki
        private BallObject ball;
        //Poczatkowa predkosc pilki
        private readonly Vector3 initialBallVel = new Vector3(100.0f, 350.0f,0.0f);
        //Promien pilki
        private readonly float ballRadius = 12.5f;

        ParticleEmmiter particles;
        PostProcessor effects;

        //Konstruktor okna
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
             : base(gameWindowSettings, nativeWindowSettings)
        {
            state = GameState.game_active;
        }
        ~Game() { }

        //Sprite-renderer do rysowania obiektow
        private SpriteRenderer rend;
        private Matrix4 projection;

        //Wywolywana przy wlaczeniu
        protected override void OnLoad()
        {
            GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
            //GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            //Projekcja ortograficzna
            projection = Matrix4.CreateOrthographic(Size.X, Size.Y, -1.0f, 1.0f);
            //Inicjalizacja sprite-renderera
            ResourceManager.GetInstance();
            ResourceManager.LoadShader("Shaders/spriteShader.vert", "Shaders/spriteShader.frag", "sprite");
            ResourceManager.GetShader("sprite").Use();
            ResourceManager.GetShader("sprite").SetInt("texture0", 0);
            ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);

            ResourceManager.LoadShader("Shaders/particleShader.vert", "Shaders/particleShader.frag", "particle");
            ResourceManager.GetShader("particle").Use();
            ResourceManager.GetShader("particle").SetInt("texture0",0);
            ResourceManager.GetShader("particle").SetMatrix4("projection",projection);

            ResourceManager.LoadShader("Shaders/postProcessing.vert", "Shaders/postProcessing.frag", "postprocessing");


            rend = new SpriteRenderer(ResourceManager.GetShader("sprite"));
            //LADOWANIE TEKSTUR
            ResourceManager.LoadTexture("Textures/background.png", "background");
            ResourceManager.LoadTexture("Textures/awesomeface.png", "face");
            ResourceManager.LoadTexture("Textures/block.png", "block");
            ResourceManager.LoadTexture("Textures/block_solid.png", "block_solid");
            ResourceManager.LoadTexture("Textures/paddle.png", "paddle");
            ResourceManager.LoadTexture("Textures/particle.png", "particle");
            //LADOWANIE POZIOMOW
            levels = new List<GameLevel>();
            GameLevel one = new GameLevel(); one.Load("Levels/one.lvl", (uint)Size.X, (uint)Size.Y / 2);
            GameLevel two = new GameLevel(); two.Load("Levels/two.lvl", (uint)Size.X, (uint)Size.Y / 2);
            GameLevel three = new GameLevel(); three.Load("Levels/three.lvl", (uint)Size.X, (uint)Size.Y / 2);
            GameLevel four = new GameLevel(); four.Load("Levels/four.lvl", (uint)Size.X, (uint)Size.Y / 2);
            levels.Add(one);
            levels.Add(two);
            levels.Add(three);
            levels.Add(four);
            //Aktualny poziom
            currentLevel = 0;
            //Inicjalizacja gracza
            Vector2 playerPos = new Vector2(-playerSize.X / 2.0f, -Size.Y / 2);
            player = new GameObject((playerPos.X, playerPos.Y, 0.0f), playerSize, ResourceManager.GetTexture("paddle"));

            //Inicjalizacja pilki
            Vector2 ballPos = playerPos + new Vector2(playerSize.X/2 - ballRadius/2, playerSize.Y);
            ball = new BallObject((ballPos.X,ballPos.Y,0.1f), ballRadius, initialBallVel, ResourceManager.GetTexture("face"));

            particles = new ParticleEmmiter(ResourceManager.GetShader("particle"),ResourceManager.GetTexture("particle"),500);
            effects = new PostProcessor(ResourceManager.GetShader("postprocessing"),Size.X,Size.Y);
            //TUTAJ KOD

            base.OnLoad();
        }
        //Obsluga wejscia i logiki
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!IsFocused)
                return;
            //KOD LOGIKI:
            //Przechowuje czas miedzy klatkami
            float deltaTime = (float)e.Time;
            //Przechowuje status klawiatury
            var input = KeyboardState;

            //Esc zamyka aplikacje
            if (input.IsKeyDown(Keys.Escape))
                Close();

            if (state == GameState.game_active)
            {
                //Predkosc wymnozona przez delta czas
                float velocity = playerVelocity * deltaTime;

                //Ruch gracza <Lewy,Prawy>
                if (input.IsKeyDown(Keys.Left))
                {
                    player.Position.X -= velocity;
                }
                if (input.IsKeyDown(Keys.Right))
                {
                    player.Position.X += velocity;
                }
                //Ograniczam ruch w ramach szerokosci ekranu
                player.Position.X = MathHelper.Clamp(player.Position.X, -Size.X / 2, Size.X / 2 - player.Size.X);
                if(ball.stuck == true)
                {
                    ball.Position.X = player.Position.X + playerSize.X / 2 - ballRadius;
                }
                //Spacja wprawia pilke w ruch
                if (input.IsKeyDown(Keys.Space))
                {
                    ball.stuck = false;
                }
            }
            //Ruch pilki
            ball.Move(deltaTime, Size.X, Size.Y);
            //Wykonywanie kolicji
            DoCollisions();
            particles.Update(deltaTime,ball,2,(ballRadius/2.0f,ballRadius/2.0f));

            //Sprawdzenie czy pilka wyszla za dolna krawedz okna
            if(ball.Position.Y <= -Size.Y)
            {
                //Przegrana = reset
                ResetLevel();
                ResetPlayer();
            }

            if(shakeTime > 0.0f)
            {
                shakeTime -= deltaTime;
                if (shakeTime <= 0.0f)
                    effects.isShake = false;
            }

            //KONIEC KODU
            base.OnUpdateFrame(e);
        }
        //Obsluga renderingu
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //Czyszczenie bufferow
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.Clear(ClearBufferMask.ColorBufferBit);
            //KOD RENDEROWANIA:

            //rend.DrawSprite(ResourceManager.GetTexture("container"), new Vector2(0,0), new Vector2(512, 512), 45, new Vector3(1, 1,1));

            if (state == GameState.game_active)
            {
                effects.BeginRender();

                //Rysuje tlo
                rend.DrawSprite(ResourceManager.GetTexture("background"), (-Size.X / 2, -Size.Y / 2, 0.0f), (Size.X, Size.Y), 0f, (1f, 1f, 1f));
                //Rysuje bloki poziomu
                levels[currentLevel].Draw(rend);
                //Rysuje gracza
                player.Draw(rend);
                //Rysuje czasteczki
                particles.Draw();
                //Rysuje pilke
                ball.Draw(rend);

                effects.EndRender();
                effects.Render(GLFW.GetTime());
            }

            //KONIEC KODU
            SwapBuffers();
            base.OnRenderFrame(e);
        }
        //Dopasowywanie do rozmiaru okna
        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnResize(e);
        }
        //Wywolywana przy wylaczeniu
        protected override void OnUnload()
        {
            //Czyszczenie bufferow
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            //Czyszczenie zasobow
            ResourceManager.Clear();

            base.OnUnload();
        }
        //Resetowanie poziomu
        private void ResetLevel()
        {
            if (currentLevel == 0)
                levels[0].Load("Levels/one.lvl", (uint)Size.X, (uint)Size.Y / 2);
            else if(currentLevel == 1)
                levels[1].Load("Levels/two.lvl", (uint)Size.X, (uint)Size.Y / 2);
            else if (currentLevel == 2)
                levels[2].Load("Levels/three.lvl", (uint)Size.X, (uint)Size.Y / 2);
            else if (currentLevel == 3)
                levels[3].Load("Levels/four.lvl", (uint)Size.X, (uint)Size.Y / 2);
        }
        //Resetowanie gracza na poczatkowa pozycje
        private void ResetPlayer()
        {
            player.Size = playerSize;
            player.Position = (-playerSize.X / 2.0f, -Size.Y / 2.0f, 0.0f);
            ball.Reset((player.Position.X + playerSize.X / 2 - ballRadius / 2, player.Position.Y + playerSize.Y, 0.0f), initialBallVel);
        }

        #region KOLIZJE
        //KOLIZJE AABB

        //Kolizja rect
        //private bool CheckCollision(GameObject a, GameObject b)
        //{
        //    bool collisionX = a.Position.X + a.Size.X >= b.Position.X &&
        //        b.Position.X + b.Size.X >= a.Position.X;
        //    bool collisionY = a.Position.Y + a.Size.Y >= b.Position.Y &&
        //        b.Position.Y + b.Size.Y >= a.Position.Y;
        //    return collisionX && collisionY;
        //}

        //private (bool, Direction, Vector2) Collision;

        //Kolizja circle
        private (bool, Direction, Vector2) CheckCollision(BallObject a, GameObject b)
        {
            Vector2 center = new Vector2(a.Position.X + a.radius, a.Position.Y + a.radius);
            Vector2 aabbHalfExtents = new Vector2(b.Size.X / 2f, b.Size.Y / 2f);
            Vector2 aabbCenter = new Vector2(
                b.Position.X + aabbHalfExtents.X,
                b.Position.Y + aabbHalfExtents.Y
                );
            Vector2 difference = center - aabbCenter;
            Vector2 clamped = new Vector2(MathHelper.Clamp(difference.X, -aabbHalfExtents.X, aabbHalfExtents.X), MathHelper.Clamp(difference.Y, -aabbHalfExtents.Y, aabbHalfExtents.Y));
            Vector2 closest = aabbCenter + clamped;
            difference = closest - center;
            if (difference.Length < a.radius)
                return (true, VectorDirection(difference), difference);
            else
                return (false, Direction.up, (0.0f, 0.0f));
        }
        //Wykonywanie kolizji, zmiana predkosci pilki
        private float shakeTime = 0.0f;
        private void DoCollisions()
        {
            //Kolizja z blokami
            foreach(GameObject box in levels[currentLevel].Bricks){
                if (!box.Destroyed)
                {
                    (bool, Direction, Vector2) coll = CheckCollision(ball, box);
                    if (coll.Item1 == true)
                    {
                        if (!box.IsSolid)
                        {
                            box.Destroyed = true;
                        }
                        else
                        {
                            shakeTime = 0.05f;
                            effects.isShake = true;
                        }
                        Direction dir = coll.Item2;
                        Vector2 diffVector = coll.Item3;
                        if(dir == Direction.left || dir == Direction.right)
                        {
                            ball.Velocity.X *= -1;
                            float pen = ball.radius - Math.Abs(diffVector.X);
                            if (dir == Direction.left)
                                ball.Position.X += pen;
                            else
                                ball.Position.X -= pen;
                        }
                        else
                        {
                            ball.Velocity.Y *= -1;
                            float pen = ball.radius - Math.Abs(diffVector.Y);
                            if (dir == Direction.up)
                                ball.Position.Y -= pen;
                            else
                                ball.Position.Y += pen;
                        }
                        return;
                    }
                }
            }
            //Kolizja z graczem
            (bool, Direction, Vector2) result = CheckCollision(ball, player);
            if(!ball.stuck && result.Item1 == true)
            {
                float centerBoard = player.Position.X + player.Size.X / 2f;
                float distance = (ball.Position.X + ball.radius) - centerBoard;
                float percentage = distance / (player.Size.X / 2f);
                float strength = 2.0f;
                Vector3 oldVel = ball.Velocity;
                ball.Velocity.X = initialBallVel.X * percentage * strength;
                ball.Velocity.Y = Math.Abs(ball.Velocity.Y);
                ball.Velocity = Vector3.Normalize(ball.Velocity) * oldVel.Length;
            }

        }

        enum Direction
        {
            up,
            right,
            down,
            left
        }
        //Obliczanie kierunku uderzenia
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
            for(var i = 0; i < 4; i++)
            {
                float dotProduct = Vector2.Dot(Vector2.Normalize(target), compass[i]);
                if(dotProduct > max)
                {
                    max = dotProduct;
                    bestMatch = i;
                }
            }
            return (Direction)bestMatch;
        }
        #endregion KOLIZJE 
    }
}
