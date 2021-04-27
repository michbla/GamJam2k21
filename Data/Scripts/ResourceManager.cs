using System.Collections.Generic;
using System;
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
        private static Dictionary<string, Shader> shaders;
        private static Dictionary<string, Texture> textures;
        private static Dictionary<int, Block> blocks;
        private static Dictionary<int, Pickaxe> pickaxes;
        private static Dictionary<int, Ore> ores;
        private static Dictionary<int, Item> items;
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
                ores = new Dictionary<int, Ore>();
                items = new Dictionary<int, Item>();
            }
            return instance;
        }
        #endregion Singleton
        public static void AddItem(int id, string _name, Texture _icon)
        {
            items.Add(id, new Item(id, _name, _icon));
        }
        public static Item GetItemByID(int id)
        {
            return items[id];
        }
        
        public static void AddOre(int id, string _name, Texture _sprite, int _hardness, float _endurance, float _value, Item _drop, Vector3 _color)
        {
            ores.Add(id, new Ore(id,_name, _sprite, _hardness, _endurance, _value, _drop, _color));
        }
        
        public static int getOreListCount()
        {
            return ores.Count;
        }

        public static Ore GetOreByID(int id)
        {
            if (id == 0)
                return null;
            return ores[id];
        }

        
        
        public static void AddPickaxe(int id, Texture _sprite, string _name, float _speed, int _hardness, float _damage)
        {
            pickaxes.Add(id, new Pickaxe(_name, _sprite, _speed, _hardness, _damage));
        }
        public static Pickaxe GetPickaxeByID(int id)
        {
            return pickaxes[id];
        }
        public static void AddBlock(int id, Texture _sprite, string _name,Vector3 _color, int _hardness, float _endurance)
        {
            blocks.Add(id, new Block((0.0f, 0.0f), _sprite, _name, _color,_hardness,_endurance));
        }
        public static Block GetBlockByID(int id)
        {
            return blocks[id];
        }

        public static int GetBlockID(Block block)
        {
            foreach (KeyValuePair<int, Block> entry in blocks)
            {
                //Console.WriteLine("rm2 " + entry.Value.name);
                if (entry.Value.name==block.name)
                    {
                        //Console.WriteLine("rm - " + entry.Value.name);
                        return entry.Key;
                    }
            }
            return 1;
        }


        public static Shader LoadShader(string vertShaderPath, string fragShaderPath, string name)
        {
            shaders.Add(name, new Shader(vertShaderPath, fragShaderPath));
            return shaders[name];
        }
        public static Shader GetShader(string name)
        {
            return shaders[name];
        }
        public static Texture LoadTexture(string path, string name)
        {
            textures.Add(name, Texture.LoadFromFile(path));
            return textures[name];
        }
        public static Texture GetTexture(string name)
        {
            return textures[name];
        }
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
