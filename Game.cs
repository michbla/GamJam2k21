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
        summary,
        end
    }

    public class Game : GameWindow
    {
        public GameState state;

        private Matrix4 projection;
        private Vector2 viewPos;
        private readonly float CAMERA_FOLLOW_SPEED = 5.0f;

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
        private bool displaySummary = false;

        private readonly float renderScale = 1.0f;
        private readonly Vector2 SCREEN_SIZE = (24.0f, 13.5f);
        private bool isFullscreen = false;

        public DateTime time = new DateTime(1, 1, 1, 8, 0, 0);

        private GameObject blockSelection;
        private bool showSelection = false;

        public Game(GameWindowSettings gWS, NativeWindowSettings nWS) : base(gWS, nWS)
        {
            state = GameState.active;
            WindowBorder = WindowBorder.Hidden;
        }

        protected override void OnLoad()
        {
            initScreenRender();
            projection = Matrix4.CreateOrthographic(SCREEN_SIZE.X * renderScale, SCREEN_SIZE.Y * renderScale, -1.0f, 1.0f);

            ResourceManager.GetInstance();
            loadShaders();
            initRenderers();
            loadTextures();
            loadBlocks();
            loadPickaxes();
            loadItems();
            loadOres();

            player = new Player(playerSpawnPos, (1.0f, 2.0f), ResourceManager.GetTexture("empty"));
            level = new GameLevel(LEVEL_WIDTH, LEVEL_DEPTH);
            userInterface = new UI(spriteRenderer, textRenderer, player.PlayerStatistics, player, viewPos);
            viewPos = playerSpawnPos;
            blockSelection = new GameObject((0, 0), (1.0f, 1.0f), ResourceManager.GetTexture("blockSelection"), Vector3.One);

            //TUTAJ KOD

            userInterface.InitUI();
            base.OnLoad();
        }
        private float cursorScale = 0.0f;
        private float cursorSize = 1.0f;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            CursorVisible = false;
            float deltaTime = (float)e.Time;
            if (!IsFocused)
                return;
            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            var mouseInput = MouseState;
            var input = KeyboardState;
            calculateMousePos(mouseInput);

            if (state == GameState.active)
            {
                player.blocks = level.currentBlocks;
                player.UpdatePlayer(input, mouseInput, deltaTime, mouseWorldPos);
                setPlayerFlip(player);

                dig(mouseInput.IsButtonDown(MouseButton.Button1));

                updateView(deltaTime, input);

                level.Update(player.playerCenter, deltaTime);

                if (input.IsKeyPressed(Keys.F4))
                    switchFullscreen();

                displayEq = input.IsKeyDown(Keys.E);
                updateDayCycle(deltaTime);
                cursorScale += 3 * deltaTime;
                cursorSize = 1.0f + (float)Math.Sin(cursorScale) / 50.0f;
            }
            if (state == GameState.summary)
            {
                showSelection = false;
                displaySummary = true;
                if (input.IsKeyPressed(Keys.Enter))
                {
                    state = GameState.active;
                    displaySummary = false;
                }
            }

            //TUTAJ KOD
            base.OnUpdateFrame(e);
        }
        private bool wasUnFocused = false;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (!IsFocused)
            {
                wasUnFocused = true;
                return;
            }
            if (wasUnFocused && isFullscreen)
            {
                refreshScreen();
                wasUnFocused = false;
            }
            GL.Clear(ClearBufferMask.ColorBufferBit);
            var color = setColorByTime();
            if (state == GameState.active || state == GameState.summary)
            {
                spriteRenderer.DrawSprite(ResourceManager.GetTexture("sky"),
                    (SCREEN_SIZE.X / 2.0f * renderScale, SCREEN_SIZE.Y / 2.0f * renderScale),
                    (0.0f, 0.0f),
                    (SCREEN_SIZE.X * renderScale, SCREEN_SIZE.Y * renderScale),
                    0.0f,
                    color);

                level.Draw(spriteRenderer, viewPos);
                if (showSelection)
                    blockSelection.Draw(spriteRenderer, viewPos);
                player.Draw(spriteRenderer, viewPos);
                int day = getDay();
                if (displayEq)
                    userInterface.DrawEQ();
                else if (displaySummary)
                    userInterface.DrawDaySummary(day - 1);
                userInterface.DrawUI();

                //Console.WriteLine(time);

                drawCursor();
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

        protected override void OnMinimized(MinimizedEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnMinimized(e);
        }

        protected override void OnMaximized(MaximizedEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);
            base.OnMaximized(e);
        }

        private void switchFullscreen()
        {
            isFullscreen = !isFullscreen;
            WindowState = isFullscreen ? WindowState.Fullscreen : WindowState.Normal;
            CenterWindow();
            GL.Viewport(0, 0, Size.X, Size.Y);
            projection = Matrix4.CreateOrthographic(SCREEN_SIZE.X * renderScale, SCREEN_SIZE.Y * renderScale, -1.0f, 1.0f);
        }

        private void resizeWindow(Vector2i newSize)
        {
            if (isFullscreen)
                return;
            Size = newSize;
            CenterWindow();
            GL.Viewport(0, 0, Size.X, Size.Y);
            projection = Matrix4.CreateOrthographic(SCREEN_SIZE.X * renderScale, SCREEN_SIZE.Y * renderScale, -1.0f, 1.0f);
        }

        private void refreshScreen()
        {
            WindowState = WindowState.Normal;
            CenterWindow();
            WindowState = WindowState.Fullscreen;
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
            string shaderPath = "Data/Resources/Shaders/";
            ResourceManager.LoadShader(shaderPath + "spriteShader/spriteShader.vert", shaderPath + "spriteShader/spriteShader.frag", "sprite");
            ResourceManager.GetShader("sprite").Use();
            ResourceManager.GetShader("sprite").SetInt("texture0", 0);
            ResourceManager.GetShader("sprite").SetMatrix4("view", Matrix4.CreateTranslation(-viewPos.X, -viewPos.Y, 0.0f));
            ResourceManager.GetShader("sprite").SetMatrix4("projection", projection);

            ResourceManager.LoadShader(shaderPath + "particleShader/particleShader.vert", shaderPath + "particleShader/particleShader.frag", "particle");
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
            string texturePath = "Data/Resources/Textures/";
            ResourceManager.LoadTexture(texturePath + "dirt1.png", "dirt");
            ResourceManager.LoadTexture(texturePath + "grass1.png", "grass");
            ResourceManager.LoadTexture(texturePath + "stone1.png", "stone");

            ResourceManager.LoadTexture(texturePath + "sky2.png", "sky");
            ResourceManager.LoadTexture(texturePath + "bgDirt02.png", "backgroundDirt");
            ResourceManager.LoadTexture(texturePath + "bgStone01.png", "backgroundStone");

            ResourceManager.LoadTexture(texturePath + "cursor2.png", "cursor");
            ResourceManager.LoadTexture(texturePath + "cursor_pick.png", "cursorPick");
            ResourceManager.LoadTexture(texturePath + "text_bitmap_bold.png", "textBitmap");
            ResourceManager.LoadTexture(texturePath + "text_bitmap_bold_white.png", "textBitmapWhite");
            ResourceManager.LoadTexture(texturePath + "text_bitmap_bold.png", "textBitmapBold");

            string heroTexturePath = texturePath + "hero/";
            ResourceManager.LoadTexture(heroTexturePath + "heroTorso_idle.png", "heroTorso_idle");

            ResourceManager.LoadTexture(heroTexturePath + "heroHead_idle.png", "heroHead_idle");

            ResourceManager.LoadTexture(heroTexturePath + "heroArmBack_idle.png", "heroArmBack_idle");
            ResourceManager.LoadTexture(heroTexturePath + "heroArmBack_walk.png", "heroArmBack_walk");

            ResourceManager.LoadTexture(heroTexturePath + "heroArmFront_idle.png", "heroArmFront_idle");
            ResourceManager.LoadTexture(heroTexturePath + "heroArmFront_walk.png", "heroArmFront_walk");
            ResourceManager.LoadTexture(heroTexturePath + "heroArmFront_dig.png", "heroArmFront_dig");

            ResourceManager.LoadTexture(heroTexturePath + "heroLegs_idle.png", "heroLegs_idle");
            ResourceManager.LoadTexture(heroTexturePath + "heroLegs_walk.png", "heroLegs_walk");
            ResourceManager.LoadTexture(heroTexturePath + "heroLegs_walkBack.png", "heroLegs_walkBack");

            ResourceManager.LoadTexture(texturePath + "pickaxe0.png", "pickaxe0");
            ResourceManager.LoadTexture(texturePath + "pickaxe1.png", "pickaxe1");
            ResourceManager.LoadTexture(texturePath + "pickaxe2.png", "pickaxe2");
            ResourceManager.LoadTexture(texturePath + "pickaxe3.png", "pickaxe3");
            ResourceManager.LoadTexture(texturePath + "pickaxe4.png", "pickaxe4");
            ResourceManager.LoadTexture(texturePath + "pickaxe5.png", "pickaxe5");

            ResourceManager.LoadTexture(texturePath + "dest.png", "dest");
            ResourceManager.LoadTexture(texturePath + "particle.png", "particle");
            ResourceManager.LoadTexture(texturePath + "empty.png", "empty");
            ResourceManager.LoadTexture(texturePath + "blockSelection.png", "blockSelection");

            string oreTexturePath = texturePath + "ores/";
            ResourceManager.LoadTexture(oreTexturePath + "coalOre.png", "coalOre");

            string itemsTexturePath = texturePath + "items/";
            ResourceManager.LoadTexture(itemsTexturePath + "coal.png", "coal");
        }

        private void loadBlocks()
        {
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

        private void loadItems()
        {
            ResourceManager.AddItem(1, "Coal", ResourceManager.GetTexture("coal"));
        }

        private void loadOres()
        {
            ResourceManager.AddOre(1, "Coal", ResourceManager.GetTexture("coalOre"), 1, 100.0f, 5f, ResourceManager.GetItemByID(1), (0.08f, 0.065f, 0.07f));
        }

        private void calculateMousePos(MouseState mouseInput)
        {
            float mouseScale = (SCREEN_SIZE.X * renderScale) / Size.X;
            mousePos.X = mouseInput.Position.X * mouseScale;
            mousePos.Y = -(mouseInput.Position.Y - Size.Y) * mouseScale;
            mouseWorldPos = mousePos + viewPos - (SCREEN_SIZE.X * renderScale / 2.0f, SCREEN_SIZE.Y * renderScale / 2.0f);
        }

        private void setPlayerFlip(Player player)
        {
            if (player.IsDigging())
                player.SetFlip(mouseWorldPos.X < player.playerCenter.X);
        }

        private void dig(bool isMouseDown)
        {
            double mPX = Math.Floor(mouseWorldPos.X);
            double mPY = Math.Floor(mouseWorldPos.Y);
            Block block = level.GetBlock((int)mPX, -(int)mPY);
            showSelection = false;
            if (block == null || block.hardness > player.GetHardness())
                return;
            blockSelection.position = ((int)mPX, (int)mPY);
            showSelection = true;
            if (isMouseDown && level.DamageBlock((int)mPX, -(int)mPY, player))
            {

                if (block.hasOre())
                    if (player.eq.addToInventory(new Item(block.GetDrop())))
                    {
                        var ore = block.getOre();
                        player.PlayerStatistics.SetOresDestroyed(ore);
                        //Succesfully added item to inventory
                    }
                    else
                    {
                        userInterface.DrawInventoryFull();
                        //Inventory full
                    }
            }
        }

        private void updateView(float deltaTime, KeyboardState keyboard)
        {
            float desiredViewX = MathHelper.Lerp(viewPos.X, player.position.X + player.size.X / 2.0f, deltaTime * CAMERA_FOLLOW_SPEED);
            float desiredViewY = MathHelper.Lerp(viewPos.Y, player.position.Y + player.size.Y * 0.7f, deltaTime * CAMERA_FOLLOW_SPEED);
            viewPos.Y = desiredViewY;

            if (keyboard.IsKeyDown(Keys.S))
                viewPos.Y -= deltaTime * 10;
            if (keyboard.IsKeyDown(Keys.W))
                viewPos.Y += deltaTime * 10;
            if (keyboard.IsKeyDown(Keys.A))
                desiredViewX -= deltaTime * 10;
            if (keyboard.IsKeyDown(Keys.D))
                desiredViewX += deltaTime * 10;
            viewPos.X = Math.Clamp(desiredViewX, 0.0f + SCREEN_SIZE.X / 2.0f, 128.0f - SCREEN_SIZE.X / 2.0f);
        }

        private float deltaSum = 0;
        private void updateDayCycle(float dt)
        {
            deltaSum += dt;
            if (deltaSum >= 1)
            {
                time = time.AddMinutes(60);
                deltaSum = 0;
            }
            if (time.Hour >= 18)
            {
                state = GameState.summary;
                time = time.AddDays(1);
                time = time.AddHours(-10);
            }

        }

        private int getDay()
        {
            int day = 0;
            day = time.Day + (time.Month - 1) * 30 + (time.Year - 1) * 365;
            return day;
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

        private void drawCursor()
        {
            float cursorRotation = 0.0f;
            String cursorTexture = "cursor";
            if (showSelection)
            {
                cursorTexture = "cursorPick";
                cursorRotation = 0.0f + (float)Math.Sin(cursorScale) * 5.0f;
            }

            spriteRenderer.DrawSprite(ResourceManager.GetTexture(cursorTexture),
                            (SCREEN_SIZE.X / 2.0f * renderScale, SCREEN_SIZE.Y / 2.0f * renderScale),
                            mousePos - (0.0f, 1.0f), (cursorSize, cursorSize), cursorRotation);
        }

    }
}
