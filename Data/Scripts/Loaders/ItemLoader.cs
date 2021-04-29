using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace GamJam2k21.Loaders
{
    public class ItemLoader
    {
        public class ItemDataJson
        {
            public ItemData item { get; set; }
        }
        public class ItemData
        {
            public int id { get; set; }
            public string name { get; set; }
            public int value { get; set; }
            public string textureName { get; set; }
        }

        public void LoadItems()
        {
            string json = File.ReadAllText("Data/Instances/items.json");
            List<ItemDataJson> items = JsonSerializer.Deserialize<List<ItemDataJson>>(json);
            foreach (var itemData in items)
                loadItem(itemData);
        }

        private void loadItem(ItemDataJson itemData)
        {
            ItemData item = itemData.item;
            ResourceManager.AddItem(
                item.id,
                item.name,
                item.value,
                Sprite.Single(ResourceManager.GetTexture(item.textureName),
                              Vector2.One));
        }
    }
}
