using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;


namespace GamJam2k21.Interface
{
    class Settings
    {
        public Vector2 MouseLocation;
        private Button resButton360;
        private Button resButton480;
        private Button resButton720;
        private Button resButton1080;
        
        private Button fullScreen;

        private Icon background;

        private bool isFullScreen = false;


        private NativeWindow ns;
        public Settings(NativeWindow _ns)
        {
            ns = _ns;
            background = new Icon((-6f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_shop"), (12f, 8f)));
            resButton360 = new Button((-5.5f, 2f), (2, 1), "360p", TextType.white);
            resButton480 = new Button((-2.5f, 2f), (2, 1), "480p", TextType.white);
            resButton720 = new Button((0.5f, 2f), (2, 1), "720p", TextType.white);
            resButton1080 = new Button((3.5f, 2f), (2, 1), "1080p", TextType.white);
            fullScreen = new Button((-1.5f, 0f), (3, 11), "fullscreen", TextType.white);
        }

        public void Update()
        {
            resButton1080.Update(MouseLocation);
            resButton720.Update(MouseLocation);
            resButton480.Update(MouseLocation);
            resButton360.Update(MouseLocation);
            fullScreen.Update(MouseLocation);

            if (resButton1080.CanPerformAction())
                changeResolution(1920);
            if (resButton720.CanPerformAction())
                changeResolution(1280);
            if (resButton480.CanPerformAction())
                changeResolution(854);
            if (resButton360.CanPerformAction())
                changeResolution(480);
            if (fullScreen.CanPerformAction())
                switchFullScreen();

            
        }

        private void changeResolution(int _res)
        {
            Vector2i res = new Vector2i(_res, (_res * 9) / 16);

            ns.Size = res;
            GL.Viewport(0, 0, ns.Size.X, ns.Size.Y);
            ns.CenterWindow();
            Camera.WindowResolution = ns.Size;
            Camera.SetProjection();
        }

        private void switchFullScreen()
        {
            isFullScreen = !isFullScreen;
            ns.WindowState = isFullScreen ? WindowState.Fullscreen : WindowState.Normal;
            GL.Viewport(0, 0, ns.Size.X, ns.Size.Y);
            ns.CenterWindow();
            Camera.SetProjection();
        }


        public void Render()
        {
            background.Render(Camera.GetScreenCenter());
            resButton360.Render(Camera.GetScreenCenter());
            resButton480.Render(Camera.GetScreenCenter());
            resButton720.Render(Camera.GetScreenCenter());
            resButton1080.Render(Camera.GetScreenCenter());
            fullScreen.Render(Camera.GetScreenCenter());
        }
    }
}
