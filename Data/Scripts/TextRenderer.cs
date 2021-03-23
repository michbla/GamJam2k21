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
    public class TextRenderer
    {
        private SpriteRenderer charRenderer;
        

        public TextRenderer(SpriteRenderer cr)
        {
            charRenderer = cr;
        }

        public void PrintText(string text, Texture tex, Vector2 vPos, Vector2 tPos, Vector2 textSize, Vector3 color)
        {
            int i = 0;
            foreach (var t in text)
            {
                int x, y;
                float scale = (textSize.X + textSize.Y) / 2f;
                y = (t / 16);
                x = (t % 16) - 1;
                if (x == -1)
                {
                    x = 15;
                    y -= 1;
                }
                charRenderer.DrawSprite((x, y), tex, vPos, ((tPos.X+(i*scale)) , tPos.Y), textSize, 0f, color);
                i++;
            }
        }
    }
}
