using System;
using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    /// <summary>
    /// Program opisujacy wlasciwosci pikseli
    /// </summary>
    public class Shader
    {
        //ID programu
        public readonly int handle;
        //Slownik lokalizacji vertexow
        private readonly Dictionary<string, int> _uniformLocations;
        //Kostruktor przyjmujacy vertex shader i fragment shader
        public Shader(string vertexPath, string fragmentPath)
        {

            var shaderSource = File.ReadAllText(vertexPath);
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSource);
            CompileShader(vertexShader);

            shaderSource = File.ReadAllText(fragmentPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);

            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            LinkProgram(handle);

            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(handle, i, out _, out _);

                var location = GL.GetUniformLocation(handle, key);
                _uniformLocations.Add(key, location);
            }

        }
        //Kompilator programu
        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Blad kompilacji Shadera ({shader}).\n\n{infoLog}");
            }
        }
        //Metoda podlaczajaca program
        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Blad podpinania Programu ({program})");
            }
        }
        //Metoda uzywajaca program
        public void Use()
        {
            GL.UseProgram(handle);
        }
        //Pobieranie lokalizacji vertexow
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(handle, attribName);
        }
        //Settery do uniform wartosci w shaderze (jesli jakiegos brakuje - dorobic wedlug wzoru)
        public void SetInt(string name, int data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetFLoat(string name, float data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        public void SetVector4(string name, Vector4 data)
        {
            GL.UseProgram(handle);
            GL.Uniform4(_uniformLocations[name], data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(handle);
            GL.Uniform3(_uniformLocations[name], data);
        }

        public void SetVector2(string name, Vector2 data)
        {
            GL.UseProgram(handle);
            GL.Uniform2(_uniformLocations[name], data);
        }

        public void SetBool(string name, bool data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(_uniformLocations[name], data ? 1 : 0);
        }
    }
}
