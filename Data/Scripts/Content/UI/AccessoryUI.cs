
namespace GamJam2k21.Interface
{
    public class AccessoryUI
    {
        private Player player;

        private Icon swapAccessory;
        private Icon accessory;
        private Icon secondAccessory;
        private bool accessoryIsBomb = true;
        private Text accessoryCount = new Text((3.6f, -4.4f), "99", TextType.tall_white_o, 0.3f);

        public AccessoryUI(Player player)
        {
            this.player = player;

            swapAccessory = new Icon((3.8f, -4.9f),
                Sprite.Single(ResourceManager.GetTexture("swap"), (2.0f, 2.0f)));
            accessory = new Icon((2.5f, -4.3f),
                Sprite.Single(ResourceManager.GetTexture("bomb_s"), (2.0f, 2.0f)));
            secondAccessory = new Icon((4.0f, -3.0f),
                Sprite.Single(ResourceManager.GetTexture("ladder"), (1.0f, 1.0f)));
            accessoryCount.UpdateText(player.BombCount.ToString());
        }

        public void Update()
        {
            updateAccessory();
            updateAccessoryCount();
        }

        private void updateAccessory()
        {
            if (accessoryIsBomb == player.IsHoldingBomb)
                return;
            accessoryIsBomb = player.IsHoldingBomb;
            string newAccTex = getAccessoryTexture();
            string newSecAccTex = getSecondAccessoryTexture();
            accessory.ChangeTexture(newAccTex);
            secondAccessory.ChangeTexture(newSecAccTex);
        }

        private string getAccessoryTexture()
        {
            if (player.IsHoldingBomb)
                return "bomb_s";
            return "ladder_s";
        }

        private string getSecondAccessoryTexture()
        {
            if (player.IsHoldingBomb)
                return "ladder";
            return "bomb";
        }

        private void updateAccessoryCount()
        {
            string result = "";
            if (player.IsHoldingBomb)
                result = player.BombCount.ToString();
            else
                result = player.LadderCount.ToString();
            accessoryCount.UpdateText(result);
        }

        public void Render()
        {
            secondAccessory.Render(Camera.GetLeftUpperCorner());
            accessory.Render(Camera.GetLeftUpperCorner());
            swapAccessory.Render(Camera.GetLeftUpperCorner());
            accessoryCount.Render(Camera.GetLeftUpperCorner());
        }

    }
}
