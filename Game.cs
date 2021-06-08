using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace GamJam2k21
{
    public enum GameState
    {
        menu,
        active,
        paused,
        postgame,
        end
    }

    public class Game : GameWindow
    {
        public static GameState state;

        ResourceLoader loader = new ResourceLoader();

        private Player player;
        private Vector2 spawnPosition = (64.0f, 1.0f);
        private GameLevel level;
        private UI UI;

        NativeWindow nativeWindow;
        public DateTime time = new DateTime(1, 1, 1, 8, 0, 0);

        public Game(GameWindowSettings gWS, NativeWindowSettings nWS)
             : base(gWS, nWS)
        {
            WindowBorder = WindowBorder.Fixed;
            Time.GetInstance();
            Input.GetInstance();
            Input.SetInputs(MouseState, KeyboardState);
            ResourceManager.GetInstance();
            Camera.Initiate(Size);
            nativeWindow = this;
        }

        protected override void OnLoad()
        {
            initScreenRender();
            loader.LoadResources();

            initGame();

            base.OnLoad();
        }

        private void initScreenRender()
        {
            GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha,
                         BlendingFactor.OneMinusSrcAlpha);
            CenterWindow();
        }
        
        private void initGame()
        {
            state = GameState.menu;

            player = new Player(Transform.Default, (1.0f, 2.0f));
            player.Position = spawnPosition;
            level = new GameLevel(player, 128, 1000);
            UI = new UI(player, nativeWindow);
            UI.Initiate();

            player.GameLevel = level;

            Camera.Position = spawnPosition;
            Camera.SetTarget(player.Transform);
            Camera.WindowResolution = Size;

            SoundManager.StartBackgroundMusic();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            CursorVisible = false;
            Time.DeltaTime = (float)e.Time;
            if (!IsFocused)
                return;
            Console.WriteLine(state);
            UI.Update();
            if (state == GameState.postgame)
            {
                SoundManager.StopBackgroundMusic();
                UI.Update();
                initGame();
            } 

            if (state == GameState.menu)
            {
                SoundManager.Update();
                Camera.Update();
                level.Update();
            }
            if (state == GameState.active)
            {
                if (player.stats.getLevelReached() >= 1000)
                    state = GameState.end;

                SoundManager.Update();
                Camera.Update();
                player.Update();
                level.Update();
                
                Time.UpdateInGameTime();
                Time.GetTime();
            }
            if (state == GameState.end)
            {
                Camera.Update();
                UI.Update();
            }
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Camera.RenderBackground();

            level.Render();
            
            if (state == GameState.active)
                player.Render();

            UI.Render();

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
    }
}
