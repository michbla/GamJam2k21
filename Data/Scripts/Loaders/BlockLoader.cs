using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace GamJam2k21.Loaders
{
    public class BlockLoader
    {
        public class BlockDataJson
        {
            public BlockData block { get; set; }
        }
        public class BlockData
        {
            public int id { get; set; }
            public string textureName { get; set; }
            public string name { get; set; }
            public int hardness { get; set; }
            public int endurance { get; set; }
            public int[] color { get; set; }
            public int exp { get; set; }
        }

        public void LoadBlocks()
        {
            string json = File.ReadAllText("Data/Instances/blocks.json");
            List<BlockDataJson> blocks = JsonSerializer.Deserialize<List<BlockDataJson>>(json);
            foreach (var blockData in blocks)
                loadBlock(blockData);
        }

        private void loadBlock(BlockDataJson blockData)
        {
            BlockData block = blockData.block;
            ResourceManager.AddBlock(
                block.id,
                Sprite.Single(ResourceManager.GetTexture(block.textureName),
                              Vector2.One),
                block.name,
                block.hardness,
                (float)block.endurance,
                (block.color[0] * 0.01f, block.color[1] * 0.01f, block.color[2] * 0.01f),
                (float)block.exp);
        }
    }
}
