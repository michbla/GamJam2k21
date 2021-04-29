using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;
using GamJam2k21.Loaders;

namespace GamJam2k21
{
    public class ResourceLoader
    {
        public string GameDirectory = Directory.GetCurrentDirectory();

        private BlockLoader blockLoader = new BlockLoader();
        private PickaxeLoader pickaxeLoader = new PickaxeLoader();
        private ItemLoader itemLoader = new ItemLoader();
        private OreLoader oreLoader = new OreLoader();

        public void LoadResources()
        {
            loadShaders();
            loadTextures();

            blockLoader.LoadBlocks();
            pickaxeLoader.LoadPickaxes();
            itemLoader.LoadItems();
            oreLoader.LoadOres();
        }

        private void loadShaders()
        {
            string shaderPath = "Data/Resources/Shaders/";
            string[] shaders = { "sprite", "particle" };
            foreach (var shader in shaders)
            {
                string name = shader + "Shader";
                ResourceManager.LoadShader(shaderPath + name + "/" + name + ".vert",
                                           shaderPath + name + "/" + name + ".frag",
                                           shader);
                ResourceManager.GetShader(shader).Use();
                ResourceManager.GetShader(shader).SetInt("texture0", 0);
                ResourceManager.GetShader(shader).SetMatrix4(
                    "view",
                    Matrix4.CreateTranslation(-Camera.Position.X, -Camera.Position.Y, 0.0f));
                ResourceManager.GetShader(shader).SetMatrix4("projection",
                                                             Camera.GetProjection());
            }
        }

        private void loadTextures()
        {
            string texturesDirectory = GameDirectory
                                     + @"\Data\Resources\Textures\";
            var textures = Directory.EnumerateFiles(
                texturesDirectory,
                "*.png",
                SearchOption.AllDirectories);

            foreach (var texture in textures)
            {
                string texturePath = texture.Replace("\\", "/").Substring(texture.IndexOf("Data"));
                string filename = Path.GetFileName(texture);
                filename = filename.Remove(filename.IndexOf('.'));
                ResourceManager.LoadTexture(texturePath, filename);
            }
        }
    }
}
