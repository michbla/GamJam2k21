using OpenTK.Mathematics;
using System;
using System.Text;

namespace GamJam2k21.Interface
{
    public class Shop
    {
        public Vector2 MouseLocation;
        private Player player;

        private Icon background;
        private Icon panel;

        private Button button1;
        private Button button2;
        private Button button3;

        private Icon pickaxeIcon = new Icon((-5.0f, -2.0f), Sprite.Single(ResourceManager.GetTexture("pickaxe_shop_1"), (3.0f, 5.0f)));
        private Icon bombIcon = new Icon((-1.5f, -2.0f), Sprite.Single(ResourceManager.GetTexture("bomb_shop"), (3.0f, 5.0f)));
        private Icon ladderIcon = new Icon((2.0f, -2.0f), Sprite.Single(ResourceManager.GetTexture("ladder_shop"), (3.0f, 5.0f)));

        public Text Gold = new Text((-2.0f, -4.25f), "     0", TextType.golden, 1.0f);
        private Text dolar = new Text((4.0f, -4.25f), "$", TextType.golden, 1.0f);

        private int pickaxeOnDisplayID = 1;

        private int nextPickaxePrice = 500;
        private int bombPrice = 300;
        private int ladderPrice = 150;

        private Text bombCount = new Text((0.0f, -1.8f), " 0", TextType.white, 0.6f);
        private Text ladderCount = new Text((3.5f, -1.8f), " 0", TextType.white, 0.6f);

        public Shop(Player player)
        {
            this.player = player;

            background = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_shop"), (12.0f, 8.0f)));
            panel = new Icon((-1.5f, -2.0f), Sprite.Single(ResourceManager.GetTexture("shopPanel"), (3.0f, 5.0f)));

            button1 = new Button((-5.0f, -3.0f), (3, 11), "999999", TextType.golden);
            button2 = new Button((-1.5f, -3.0f), (3, 11), "999999", TextType.golden);
            button3 = new Button((2.0f, -3.0f), (3, 11), "999999", TextType.golden);

            button1.UpdateText("   500");
            button2.UpdateText("   300");
            button3.UpdateText("   150");
        }

        public void Update()
        {
            button1.Update(MouseLocation);
            button2.Update(MouseLocation);
            button3.Update(MouseLocation);

            if (button1.CanPerformAction())
                tryBuingPickaxe();
            if (button2.CanPerformAction())
                tryBuingBomb();
            if (button3.CanPerformAction())
                tryBuingLadder();

            bombCount.UpdateText(countTotext(player.BombCount));
            ladderCount.UpdateText(countTotext(player.LadderCount));
        }

        private void tryBuingPickaxe()
        {
            if (player.Gold < nextPickaxePrice || pickaxeOnDisplayID > 10)
                return;
            player.Pay(nextPickaxePrice);
            player.EquipPickaxe(pickaxeOnDisplayID);
            pickaxeOnDisplayID++;
            pickaxeIcon.ChangeTexture("pickaxe_shop_" + pickaxeOnDisplayID);
            nextPickaxePrice = (int)Math.Pow(2, pickaxeOnDisplayID) * 500;
            button1.UpdateText(convertValueToString(nextPickaxePrice));
        }

        private void tryBuingBomb()
        {
            if (player.Gold < bombPrice)
                return;
            player.Pay(bombPrice);
            player.AddBomb();
            bombPrice += 100;
            button2.UpdateText(convertValueToString(bombPrice));
        }

        private void tryBuingLadder()
        {
            ladderCount.UpdateText(countTotext(player.LadderCount));
            if (player.Gold < ladderPrice)
                return;
            player.Pay(ladderPrice);
            player.AddLadder();
            ladderPrice += 50;
            button3.UpdateText(convertValueToString(ladderPrice));
        }

        public void Render()
        {
            background.Render(Camera.GetScreenCenter());
            panel.Render(Camera.GetScreenCenter());
            panel.Render(Camera.GetScreenCenter((-3.5f, 0.0f)));
            panel.Render(Camera.GetScreenCenter((3.5f, 0.0f)));
            button1.Render(Camera.GetScreenCenter());
            button2.Render(Camera.GetScreenCenter());
            button3.Render(Camera.GetScreenCenter());

            pickaxeIcon.Render(Camera.GetScreenCenter());
            bombIcon.Render(Camera.GetScreenCenter());
            ladderIcon.Render(Camera.GetScreenCenter());

            bombCount.Render(Camera.GetScreenCenter());
            ladderCount.Render(Camera.GetScreenCenter());

            Gold.Render(Camera.GetScreenCenter());
            dolar.Render(Camera.GetScreenCenter());
        }

        private string countTotext(int count)
        {
            string result = "";
            if (count < 10)
                result = " ";
            result += count;
            return result;
        }

        private string convertValueToString(int amount)
        {
            if (amount == 0)
                return "     0";
            var result = new StringBuilder();
            int value = amount;
            if (amount >= 1000000)
            {
                result.Append("M" + (value / 100000) % 10 + ".");
                value /= 1000000;
            }
            while (value != 0)
            {
                result.Append(value % 10);
                value /= 10;
            }
            int spaces = 6 - result.Length;
            for (int i = 0; i < spaces; i++)
                result.Append(" ");
            char[] charArray = result.ToString().ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
