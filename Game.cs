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
    public enum GameState
    {
        menu,
        active,
        paused,
        end
    }

    public class Game : GameWindow
    {
        public GameState state;

        private Matrix4 projection;
        private Vector2 viewPos;
        private readonly float cameraFollowSpeed = 5.0f;

        private Vector2 mousePos;
        private Vector2 mouseWorldPos;

        private SpriteRenderer spriteRenderer;
        private TextRenderer textRenderer;

        private Player player;
        private Vector2 playerSpawnPos = (64.0f, 1.0f);

        private GameLevel level;
        private readonly int LEVEL_WIDTH = 128;
        private readonly int LEVEL_DEPTH = 1000;

        private UI userInterface;
        private bool displayEq = false;

        private float renderScale = 1.0f;
        private readonly Vector2 screenSize = (24.0f, 13.5f);
        private bool isFullscreen = false;

        public DateTime time = new DateTime(1, 1, 1, 8, 0, 0);

        public Game(GameWindowSettings gWS, NativeWindowSettings nWS) : base(gWS, nWS)
        {
            state = GameState.active;
            WindowBorder = WindowBorder.Hidden;
        }

        protected override void OnLoad()
        {
            initScreenRender();
            projection = Matrix4.CreateOrthographic(screenSize.X * renderScale, screenSize.Y * renderScale, -1.0f, 1.0f);

            ResourceManager.GetInstance();
            loadShaders();
            initRenderers();
            loadTextures();
            loadBlocks();
            loadPickaxes();
         
            player = new Player(playerSpawnPos, (1.0f, 2.0f), ResourceManager.GetTexture("char"));
            level = new GameLevel(LEVEL_WIDTH, LEVEL_DEPTH);
            userInterface = new UI(spriteRenderer, textRenderer, player.PlayerStatistics, player, viewPos);
            viewPos = playerSpawnPos;

            //TUTAJ KOD

            userInterface.InitUI();
            base.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            CursorVisible = false;
            updateDayCycle();
            float deltaTime = (float)e.Time;
            if (!IsFocused)
                return;
            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            var mouseInput = MouseState;
            var input = KeyboardState;
            calculateMousePos(mouseInput);

            player.blocks = level.currentBlocks;
            player.UpdatePlayer(input, mouseInput, deltaTime, mouseWorldPos);
            setPlayerFlip(player);

            if (mouseInput.IsButtonDown(MouseButton.Button1))
                dig();

            updateView(deltaTime, input);

            level.Update(player.playerCenter, deltaTime);

            if (input.IsKeyPressed(Keys.F4))
                switchFullscreen();

            displayEq = input.IsKeyDown(Keys.E);

            //TUTAJ KOD

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            var color = setColorByTime();
            Console.WriteLine(color.X + " " + color.Y + " " + color.Z);
            if (state == GameState.active)
            {
                spriteRenderer.DrawSprite(ResourceManager.GetTexture("sky"), (screenSize.X / 2.0f * renderScale, screenSize.Y / 2.0f * renderScale), (0.0f, 0.0f), (screenSize.X * renderScale, screenSize.Y * renderScale), 0.0f, color);

                level.Draw(spriteRenderer, viewPos);
                player.Draw(spriteRenderer, viewPos);

                if (displayEq)
                    userInterface.DrawEQ();
                userInterface.DrawUI();

                //Console.WriteLine(time);
                
                spriteRenderer.DrawSprite(ResourceManager.GetTexture("cursor"), (screenSize.X / 2.0f * renderScale, screenSize.Y / 2.0f * renderScale), mousePos - (0.0f, 1.0f), (1.0f, 1.0f), 0.0f);
            }

            //TUTAJ KOD

            SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            ResourceManager.Clear();
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnResize(e);
        }

        private void switchFullscreen()
        {
            isFullscreen = !isFullscreen;
            WindowState = isFullscreen ? WindowState.Fullscreen : WindowState.Normal;
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        private void initScreenRender()
        {
            GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            CenterWindow();
        }

        private void loadShaders()
        {
            ResourceManager.LoadShader("Data/Resources/Shaders/spriteShader/spriteShader.vert", "Data/Resources/Shaders/spriteShader/spriteShader.frag", "sprite");
            ResourceManager.GetShader("sprite").Use();
            ResourceManager.GetShader("sprite").SetInt("texture0", 0);
            ResourceManager.GetShader("sprite").SetMatrix4("view", Matrix4.CreateTranslation(-viewPos.X, -viewPos.Y, 0.0f));
            ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);

            ResourceManager.LoadShader("Data/Resources/Shaders/particleShader/particleShader.vert", "Data/Resources/Shaders/particleShader/particleShader.frag", "particle");
            ResourceManager.GetShader("particle").Use();
            ResourceManager.GetShader("particle").SetInt("texture0", 0);
            ResourceManager.GetShader("sprite").SetMatrix4("view", Matrix4.CreateTranslation(-viewPos.X, -viewPos.Y, 0.0f));
            ResourceManager.GetShader("particle").SetMatrix4("projection", projection);
        }

        private void initRenderers()
        {
            spriteRenderer = new SpriteRenderer(ResourceManager.GetShader("sprite"));
            textRenderer = new TextRenderer(new SpriteRenderer(ResourceManager.GetShader("sprite"), new Vector2i(16, 8)));
        }

        private void loadTextures()
        {
            ResourceManager.LoadTexture("Data/Resources/Textures/dirt1.png", "dirt");
            ResourceManager.LoadTexture("Data/Resources/Textures/grass1.png", "grass");
            ResourceManager.LoadTexture("Data/Resources/Textures/stone1.png", "stone");

            ResourceManager.LoadTexture("Data/Resources/Textures/sky2.png", "sky");
            ResourceManager.LoadTexture("Data/Resources/Textures/bgDirt01.png", "backgroundDirt");

            ResourceManager.LoadTexture("Data/Resources/Textures/hero1.png", "char");
            ResourceManager.LoadTexture("Data/Resources/Textures/cursor2.png", "cursor");
            ResourceManager.LoadTexture("Data/Resources/Textures/hero_idle.png", "charIdle1");
            ResourceManager.LoadTexture("Data/Resources/Textures/text_bitmap.png", "textBitmap"); //xd
            ResourceManager.LoadTexture("Data/Resources/Textures/hero_walk1.png", "charWalk1");
            ResourceManager.LoadTexture("Data/Resources/Textures/hero_walk1.png", "charWalkBack1");
            ResourceManager.LoadTexture("Data/Resources/Textures/hero_arm1.png", "charArm1");

            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroTorso_idle.png", "heroTorso_idle");

            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroHead_idle.png", "heroHead_idle");

            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroArmBack_idle.png", "heroArmBack_idle");
            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroArmBack_walk.png", "heroArmBack_walk");

            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroArmFront_idle.png", "heroArmFront_idle");
            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroArmFront_walk.png", "heroArmFront_walk");
            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroArmFront_dig.png", "heroArmFront_dig");

            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroLegs_idle.png", "heroLegs_idle");
            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroLegs_walk.png", "heroLegs_walk");
            ResourceManager.LoadTexture("Data/Resources/Textures/hero/heroLegs_walkBack.png", "heroLegs_walkBack");

            ResourceManager.LoadTexture("Data/Resources/Textures/pickaxe0.png", "pickaxe0");
            ResourceManager.LoadTexture("Data/Resources/Textures/pickaxe1.png", "pickaxe1");
            ResourceManager.LoadTexture("Data/Resources/Textures/pickaxe2.png", "pickaxe2");
            ResourceManager.LoadTexture("Data/Resources/Textures/pickaxe3.png", "pickaxe3");
            ResourceManager.LoadTexture("Data/Resources/Textures/pickaxe4.png", "pickaxe4");
            ResourceManager.LoadTexture("Data/Resources/Textures/pickaxe5.png", "pickaxe5");

            ResourceManager.LoadTexture("Data/Resources/Textures/dest.png", "dest");
            ResourceManager.LoadTexture("Data/Resources/Textures/particle.png", "particle");
            ResourceManager.LoadTexture("Data/Resources/Textures/empty.png", "empty");
        }

        private void loadBlocks()
        {
            //ResourceManager.AddBlock(0, ResourceManager.GetTexture("air"), "Air", (1.0f, 1.0f, 1.0f), 0, 0);
            ResourceManager.AddBlock(1, ResourceManager.GetTexture("grass"), "Grass", (0.17f, 0.06f, 0.01f), 0, 100.0f);
            ResourceManager.AddBlock(2, ResourceManager.GetTexture("dirt"), "Dirt", (0.17f, 0.06f, 0.01f), 0, 100.0f);
            ResourceManager.AddBlock(3, ResourceManager.GetTexture("stone"), "Stone", (0.1f, 0.1f, 0.1f), 1, 150.0f);
        }

        private void loadPickaxes()
        {
            ResourceManager.AddPickaxe(0, ResourceManager.GetTexture("pickaxe0"), "Fists", 500.0f, 0, 10.0f);
            ResourceManager.AddPickaxe(1, ResourceManager.GetTexture("pickaxe1"), "Stone Pickaxe", 500.0f, 1, 15.0f);
            ResourceManager.AddPickaxe(2, ResourceManager.GetTexture("pickaxe2"), "Copper Pickaxe", 600.0f, 2, 25.0f);
            ResourceManager.AddPickaxe(3, ResourceManager.GetTexture("pickaxe3"), "Iron Pickaxe", 700.0f, 3, 40.0f);
            ResourceManager.AddPickaxe(4, ResourceManager.GetTexture("pickaxe4"), "GoldenPickaxe", 800.0f, 4, 60.0f);
            ResourceManager.AddPickaxe(5, ResourceManager.GetTexture("pickaxe5"), "Diamond Pickaxe", 900.0f, 5, 85.0f);
        }

        private void calculateMousePos(MouseState mouseInput)
        {
            float mouseScale = (screenSize.X * renderScale) / Size.X;
            mousePos.X = mouseInput.Position.X * mouseScale;
            mousePos.Y = -(mouseInput.Position.Y - Size.Y) * mouseScale;
            mouseWorldPos = mousePos + viewPos - (screenSize.X * renderScale / 2.0f, screenSize.Y * renderScale / 2.0f);
        }

        private void setPlayerFlip(Player player)
        {
            if (player.IsDigging())
                player.SetFlip(mouseWorldPos.X < player.playerCenter.X);
        }

        private void dig()
        {
            double mPX = Math.Floor(mouseWorldPos.X);
            double mPY = Math.Floor(mouseWorldPos.Y);
            var blockName = level.getBlockName((int)mPX, -(int)mPY);
            int dugBlock = level.getBlock((int)mPX, -(int)mPY);
            if (level.DamageBlock((int)mPX, -(int)mPY, player))
            {
                player.PlayerStatistics.SetBlocksDestroyed(blockName);
                player.eq.addToInventory(dugBlock);
                //Console.WriteLine("game " + dugBlock);
            }
        }

        private void updateView(float deltaTime, KeyboardState keyboard)
        {
            float desiredViewX = MathHelper.Lerp(viewPos.X, player.position.X + player.size.X / 2.0f, deltaTime * cameraFollowSpeed);
            float desiredViewY = MathHelper.Lerp(viewPos.Y, player.position.Y + player.size.Y * 0.7f, deltaTime * cameraFollowSpeed);
            viewPos.Y = desiredViewY;

            if (keyboard.IsKeyDown(Keys.S))
            {
                viewPos.Y -= deltaTime * 10;
            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                viewPos.Y += deltaTime * 10;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                desiredViewX -= deltaTime * 10;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                desiredViewX += deltaTime * 10;
            }
            viewPos.X = Math.Clamp(desiredViewX, 0.0f + screenSize.X / 2.0f, 128.0f - screenSize.X / 2.0f);
        }

        private void updateDayCycle()
        {
            if (time.Hour==18)
            {
                time = time.AddDays(1);
                time = time.AddHours(-10);
            }
            else if(time.Hour<18)
            {
                //Console.WriteLine(time);
                time = time.AddSeconds(30);
            }

        }

        private Vector3 setColorByTime()
        {
            double red, green, blue;
            Vector3 color; 
            red = Math.Sin((float)(time.Hour * 60 + time.Minute) / 1000);
            green = Math.Cos((float)(time.Hour * 60 + time.Minute) / 1000);
            blue = Math.Cos((float)(time.Hour * 60 + time.Minute) / 1000);
            color = new Vector3((float)red, (float)green, (float)blue);

            return color;
        }

    }
}
