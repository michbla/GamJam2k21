using System;
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

        ResourceLoader loader = new ResourceLoader();

        private Player player;
        private Vector2 spawnPosition = (64.0f, 1.0f);
        private GameLevel level;
        private UI UI;

        NativeWindow nw;
        public DateTime time = new DateTime(1, 1, 1, 8, 0, 0);

        public Game(GameWindowSettings gWS, NativeWindowSettings nWS) 
             : base(gWS, nWS)
        {
            state = GameState.active;
            WindowBorder = WindowBorder.Fixed;
            Time.GetInstance();
            Input.GetInstance();
            Input.SetInputs(MouseState, KeyboardState);
            ResourceManager.GetInstance();
            Camera.Initiate();
            nw = this;
        }

        protected override void OnLoad()
        {
            initScreenRender();
            
            loader.LoadResources();

            player = new Player(Transform.Default, (1.0f, 2.0f));
            player.Position = spawnPosition;
            level = new GameLevel(player, 128, 1000);
            UI = new UI(player, nw);
            UI.Initiate();

            player.GameLevel = level;

            Camera.Position = spawnPosition;
            Camera.SetTarget(player.Transform);
            Camera.WindowResolution = Size;

            SoundManager.StartBackgroundMusic();

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

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            CursorVisible = false;
            Time.DeltaTime = (float)e.Time;
            if (!IsFocused)
                return;
            if (player.stats.getLevelReached()==999)
            {
                state = GameState.end;
                
            }

            SoundManager.Update();
            
            if (state == GameState.active)
            {
                Camera.Update();
                player.Update();
                level.Update();
                UI.Update(state);
                Time.UpdateInGameTime();
                Time.GetTime();
            }
            if (state == GameState.end)
            {
                Camera.Update();
                UI.Update(state);
            }
            base.OnUpdateFrame(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Camera.RenderBackground();

            if (state == GameState.active || state == GameState.summary)
            {
                level.Render();
                player.Render();
                UI.Render();
            }

            if (state == GameState.end)
            {
                level.Render();
                UI.Render();
            }


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
