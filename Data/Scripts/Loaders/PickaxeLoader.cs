using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace GamJam2k21.Loaders
{
    public class PickaxeLoader
    {
        public class PickaxeDataJson
        {
            public PickaxeData pickaxe { get; set; }
        }
        public class PickaxeData
        {
            public int id { get; set; }
            public string textureName { get; set; }
            public string name { get; set; }
            public int hardness { get; set; }
            public int damage { get; set; }
        }

        public void LoadPickaxes()
        {
            string json = File.ReadAllText("Data/Instances/pickaxes.json");
            List<PickaxeDataJson> pickaxes = JsonSerializer.Deserialize<List<PickaxeDataJson>>(json);
            foreach (var pickaxeData in pickaxes)
                loadPickaxe(pickaxeData);
        }

        private void loadPickaxe(PickaxeDataJson pickaxeData)
        {
            PickaxeData pickaxe = pickaxeData.pickaxe;
            ResourceManager.AddPickaxe(
                pickaxe.id,
                Sprite.Single(ResourceManager.GetTexture(pickaxe.textureName),
                              Vector2.One * 4.0f),
                pickaxe.name,
                pickaxe.hardness,
                (float)pickaxe.damage);
        }
    }
}
