namespace GamJam2k21
{
    public class Item
    {
        private readonly int id;
        private readonly string name;
        private readonly Sprite icon;
        private readonly float value;

        private int quantity = 1;

        public int Id
        {
            get => id;
        }

        public string Name
        {
            get => name;
        }

        public float Value
        {
            get => value;
        }

        public int Quantity
        {
            get => quantity;
            set { quantity = value; }
        }

        public Item(int id,
                    string name,
                    float value,
                    Sprite icon)
        {
            this.id = id;
            this.name = name;
            this.icon = icon;
            this.value = value;
        }

        public Item(Item copy)
        {
            id = copy.id;
            name = copy.name;
            icon = copy.icon;
            value = copy.value;
        }

        public void Render(Transform transform)
        {
            icon.RenderWithTransform(transform);
        }
    }
}
