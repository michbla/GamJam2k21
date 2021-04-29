﻿using System;
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

        private bool isFullscreen = false;

        public DateTime time = new DateTime(1, 1, 1, 8, 0, 0);

        public Game(GameWindowSettings gWS, NativeWindowSettings nWS) 
             : base(gWS, nWS)
        {
            state = GameState.active;
            WindowBorder = WindowBorder.Hidden;

            Time.GetInstance();
            Input.GetInstance();
            Input.SetInputs(MouseState, KeyboardState);
            ResourceManager.GetInstance();
            Camera.Initiate();
        }

        protected override void OnLoad()
        {
            initScreenRender();

            loader.LoadResources();

            player = new Player(Transform.Default, (1.0f, 2.0f));
            player.Position = spawnPosition;
            level = new GameLevel(128, 1000);
            UI = new UI(player);
            UI.Initiate();

            player.Level = level;

            Camera.Position = spawnPosition;
            Camera.SetTarget(player.Transform);
            Camera.WindowResolution = Size;

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
            if (Input.IsKeyDown(Keys.Escape))
                Close();
            if (Input.IsKeyPressed(Keys.F4))
                switchFullscreen();

            UI.Update();

            if (state == GameState.active)
            {
                Camera.Update();
                player.Update();
                level.Update(player.Center);
                //updateDayCycle();
            }
            if (state == GameState.summary)
            {
                //displaySummary = true;
                //if (Input.IsKeyPressed(Keys.Enter))
                //{
                //    state = GameState.active;
                //    displaySummary = false;
                //}
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
            handleAltTab();

            GL.Clear(ClearBufferMask.ColorBufferBit);
            Camera.RenderBackground();

            //var color = setColorByTime();
            if (state == GameState.active || state == GameState.summary)
            {
                level.Render();
                player.Render();
                //int day = getDay();
                // else if (displaySummary)
                //    userInterface.DrawDaySummary(day - 1);
                UI.Render();
            }

            //TUTAJ KOD

            SwapBuffers();
            base.OnRenderFrame(e);
        }

        private void handleAltTab()
        {
            if (wasUnFocused && isFullscreen)
            {
                refreshScreen();
                wasUnFocused = false;
            }
        }
        private void switchFullscreen()
        {
            isFullscreen = !isFullscreen;
            WindowState = isFullscreen ? WindowState.Fullscreen : WindowState.Normal;
            setNewAspectRatio();
        }

        private void setNewAspectRatio()
        {
            CenterWindow();
            GL.Viewport(0, 0, Size.X, Size.Y);
            Camera.WindowResolution = Size;
            Camera.SetProjection();
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

        private void resizeWindow(Vector2i newSize)
        {
            if (isFullscreen)
                return;
            Size = newSize;
            Camera.WindowResolution = Size;
            setNewAspectRatio();
        }

        private void refreshScreen()
        {
            WindowState = WindowState.Normal;
            CenterWindow();
            WindowState = WindowState.Fullscreen;
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        private float deltaSum = 0;
        private void updateDayCycle()
        {
            deltaSum += Time.DeltaTime;
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

    }
}
