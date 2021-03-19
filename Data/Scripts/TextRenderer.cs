using System;
using System.Collections.Generic;
using SharpFont;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


/// <summary>
/// ładuje .ttf i zapełnia słownik(??)
/// dalej już tak średnio
/// ważna kolejność instalacji nugetów
/// 1. sharpfont dependecies
/// 2. spacewizards.sharpfont
/// 3. w user/.nuget/packages/sharpfont.dependencies/2.6.0/build 
/// dać <Content Include="$(MSBuildThisFileDirectory)..\bin\msvc10\x64\freetype6.dll">
/// 4. w bin wyjebać wszystko poza msvc10/x64
/// </summary>

namespace GamJam2k21
{struct Character
        {
            public uint TextureID;
            public Vector2i Size;
            public Vector2i Bearing;
            public uint Advance;
            
        }
    public class TextRenderer
    {

        Dictionary<char, Character> Characters = new Dictionary<char, Character>();

        private int VAO;
        private int VBO;
        private Shader TextShader;

        public TextRenderer(uint width, uint height)
        {
            TextShader = ResourceManager.LoadShader("Data/Resources/Shaders/textShader/textShader.vert", "Data/Resources/Shaders/textShader/textShader.frag", "textShader");
            Matrix4 ortho = Matrix4.Identity;
            ortho *= Matrix4.CreateOrthographic(0f, width, height, 0);
            TextShader.SetMatrix4("projection", ortho);
            TextShader.SetInt("text", 0);
            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out VBO);
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, (IntPtr)null, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
        public void Load(string font, uint fontSize)
        {
            Characters.Clear();
            Library ft=new Library();

            Face face = new Face(ft, font);
            face.SetPixelSizes(0, fontSize);

            for (ushort c=0; c<128; c++)
            {
                face.LoadChar(c, LoadFlags.Render, LoadTarget.Mono);

                uint texture;
                GL.GenTextures(1, out texture);
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgb,
                    face.Glyph.Bitmap.Width,
                    face.Glyph.Bitmap.Rows,
                    0,
                    PixelFormat.Red,
                    PixelType.UnsignedByte,
                    face.Glyph.Bitmap.Buffer
                    );
                GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref texture);
                GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref texture);
                GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref texture);
                GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref texture);

                Character ch;
                ch.TextureID = texture;
                ch.Size = new Vector2i(face.Glyph.Bitmap.Width, face.Glyph.Bitmap.Rows);
                ch.Bearing = new Vector2i(face.Glyph.BitmapLeft, face.Glyph.BitmapTop);
                ch.Advance = (uint)face.Glyph.Advance.X.Value;
                
                Characters.Add((char)c, ch);
                
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
            face.Dispose();
            ft.Dispose();
        }

        public void RenderText(string text, float x, float y, float scale, Vector3 color)
        {
            TextShader.Use();
            TextShader.SetVector3("textColor", color);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            
            foreach (var c in text)
            {
               if (Characters.ContainsKey(c) == false)
                   continue;
                Character ch = Characters[c];
                //Console.WriteLine(c);
                float xpos = x + ch.Bearing.X * scale;
                float ypos = y + (Characters['H'].Bearing.Y-ch.Bearing.Y) * scale;

                float w = ch.Size.X * scale;
                float h = ch.Size.Y * scale;

                float[] verts =
                {
                     xpos, ypos + h, 0f, 1f,
                     xpos + w, ypos, 1f, 0f,
                     xpos, ypos, 0f, 0f,    
                     
                     xpos, ypos + h, 0f, 1f,
                     xpos + w, ypos + h, 1f, 1f,
                     xpos + w, ypos, 1f, 0f
                };
                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, verts.Length*sizeof(float), verts);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                x += (ch.Advance >> 6) * scale;
            }
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }


    }
}
