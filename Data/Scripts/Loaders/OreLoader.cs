using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace GamJam2k21.Loaders
{
    public class OreLoader
    {
        public class OreDataJson
        {
            public OreData ore { get; set; }
        }
        public class OreData
        {
            public int id { get; set; }
            public string textureName { get; set; }
            public string name { get; set; }
            public int hardness { get; set; }
            public int endurance { get; set; }
            public int dropId { get; set; }
            public int[] color { get; set; }
            public int exp { get; set; }
        }

        public void LoadOres()
        {
            string json = File.ReadAllText("Data/Instances/ores.json");
            List<OreDataJson> ores = JsonSerializer.Deserialize<List<OreDataJson>>(json);
            foreach (var oreData in ores)
            {
                loadOre(oreData);
            }
        }

        private void loadOre(OreDataJson oreData)
        {
            OreData ore = oreData.ore;
            ResourceManager.AddOre(
                ore.id,
                Sprite.Single(ResourceManager.GetTexture(ore.textureName),
                              Vector2.One),
                ore.name,
                ore.hardness,
                (float)ore.endurance,
                ResourceManager.GetItemByID(ore.dropId),
                (ore.color[0] * 0.01f, ore.color[1] * 0.01f, ore.color[2] * 0.01f),
                (float)ore.exp);
        }
    }
}
