using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

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
        //Slownik przechowujacy bloki
        private static Dictionary<int, Block> blocks;
        //Slownik przechowujacy kilofy
        private static Dictionary<int, Pickaxe> pickaxes;
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
                blocks = new Dictionary<int, Block>();
                pickaxes = new Dictionary<int, Pickaxe>();
            }
            return instance;
        }
        #endregion Singleton
        //Dodawanie bloku do slownika
        public static void AddPickaxe(int id, Texture _sprite, string _name, float _speed, int _hardness, float _damage)
        {
            pickaxes.Add(id, new Pickaxe(_name, _sprite, _speed, _hardness, _damage));
        }
        //Pobieranie bloku przez ID
        public static Pickaxe GetPickaxeByID(int id)
        {
            return pickaxes[id];
        }
        //Dodawanie bloku do slownika
        public static void AddBlock(int id, Texture _sprite, string _name,Vector3 _color, int _hardness, float _endurance)
        {
            blocks.Add(id, new Block((0.0f, 0.0f), _sprite, _name, _color,_hardness,_endurance));
        }
        //Pobieranie bloku przez ID
        public static Block GetBlockByID(int id)
        {
            return blocks[id];
        }
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
