using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class ResourceManager
    {
        private readonly static Dictionary<string, Shader> shaders
            = new Dictionary<string, Shader>();

        private readonly static Dictionary<string, Texture> textures
            = new Dictionary<string, Texture>();

        private readonly static Dictionary<int, Block> blocks
            = new Dictionary<int, Block>();

        private readonly static Dictionary<int, Pickaxe> pickaxes
            = new Dictionary<int, Pickaxe>();

        private readonly static Dictionary<int, Ore> ores
            = new Dictionary<int, Ore>();

        private readonly static Dictionary<int, Item> items
            = new Dictionary<int, Item>();

        #region Singleton
        private ResourceManager() { }
        private static ResourceManager instance;
        public static ResourceManager GetInstance()
        {
            if (instance == null)
                instance = new ResourceManager();
            return instance;
        }
        #endregion Singleton
        public static void AddItem(int id, string name, int value, Sprite icon)
        {
            items.Add(id, new Item(id, name, value, icon));
        }

        public static Item GetItemByID(int id)
        {
            return items[id];
        }

        public static void AddOre(int id,
                                  Sprite sprite,
                                  string name,
                                  int hardness,
                                  float endurance,
                                  Item drop,
                                  Vector3 color)
        {
            ores.Add(id, new Ore(id, sprite, name, hardness, endurance, drop, color));
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

        public static void AddPickaxe(int id, Sprite sprite, string name, int hardness, float damage)
        {
            pickaxes.Add(id, new Pickaxe(name, sprite, hardness, damage));
        }

        public static Pickaxe GetPickaxeByID(int id)
        {
            return pickaxes[id];
        }

        public static void AddBlock(int id,
                                    Sprite sprite,
                                    string name,
                                    int hardness,
                                    float endurance,
                                    Vector3 effectsColor)
        {
            blocks.Add(id, new Block(sprite, Transform.Default, name, hardness, endurance, effectsColor));
        }

        public static Block GetBlockByID(int id)
        {
            return blocks[id];
        }

        public static int GetBlockID(Block block)
        {
            foreach (var entry in blocks)
                if (entry.Value.Name == block.Name)
                    return entry.Key;
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
