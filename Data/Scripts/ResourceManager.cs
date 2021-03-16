using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa przechowujaca wszystkie wykorzystywane zasoby zasoby, tj.:
    /// -shadery
    /// -tekstury
    /// </summary>
    public class ResourceManager
    {
        //Slownik przechowujacy shadery
        private static Dictionary<string, Shader> shaders;
        //Slownik przechowujacy textury
        private static Dictionary<string, Texture> textures;
        //Singleton zapewniajacy, ze istnieje tylko jeden obiekt tej klasy
        #region Singleton
        private ResourceManager() { }
        private static ResourceManager instance;
        public static ResourceManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ResourceManager();
                shaders = new Dictionary<string, Shader>();
                textures = new Dictionary<string, Texture>();
            }
            return instance;
        }
        #endregion Singleton

        //Ladowanie shadera z plikow
        public static Shader LoadShader(string vertShaderPath, string fragShaderPath, string name)
        {
            shaders.Add(name, new Shader(vertShaderPath, fragShaderPath));
            return shaders[name];
        }
        //Pobieranie shadera ze slownika
        public static Shader GetShader(string name)
        {
            return shaders[name];
        }
        //Ladowanie textury z pliku
        public static Texture LoadTexture(string path, string name)
        {
            textures.Add(name, Texture.LoadFromFile(path));
            return textures[name];
        }
        //Pobieranie textury ze slownika
        public static Texture GetTexture(string name)
        {
            return textures[name];
        }
        //Czyszczenie zasobow
        public static void Clear()
        {
            if (shaders != null)
                foreach (var sha in shaders)
                    GL.DeleteProgram(sha.Value.handle);

            if (textures != null)
                foreach (var tex in textures)
                    GL.DeleteTexture(tex.Value._handle);
        }
    }
}
