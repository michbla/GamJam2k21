using System;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using GamJam2k21.Interface;
using System.Text;

namespace GamJam2k21
{
    public class UI
    {
        private List<UI_Element> ui_elements = new List<UI_Element>();

        private Player player;
        private Cursor cursor;

        private bool displayMenu = false;

        private GameObject blockSelection;

        private Icon dimmBackground;

        private Text WIP = new Text((-6.4f, 0.0f), "WORK IN PROGRESS", TextType.white, 0.4f);
        private UI_Element PickaxeFrame;
        private Button buttonTest;
        private Button subtractButton;
        private ProgressBar barTest;

        private Icon accessory;
        private bool accessoryIsBomb = true;

        private Icon UIbackgound;
        private Icon OPT_back;
        private Button CHAR_button;
        private Button SHOP_button;
        private Button OPT_button;

        private string UI_state = "basic";

        private Icon Coin;
        private Text Gold = new Text((-8.75f, -2.0f), "     0", TextType.golden, 1.0f);
        private Text playTime;

        private Shop shop;

        private CharacterMenu charMenu;

        public UI(Player player)
        {
            this.player = player;
        }

        public void Initiate()
        {
            dimmBackground = new Icon((-12.0f, -6.75f),
                                      Sprite.Single(ResourceManager.GetTexture("dimmBackground"),
                                                    (24.0f, 13.5f)));

            UIbackgound = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back"), (12.0f, 8.0f)));

            OPT_back = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_options"), (12.0f, 8.0f)));

            CHAR_button = new Button((-5.0f, 3.5f), (3, 1), "A", TextType.ui_icon);
            SHOP_button = new Button((-1.5f, 3.5f), (3, 1), "B", TextType.ui_icon);
            OPT_button = new Button((2.0f, 3.5f), (3, 1), "C", TextType.ui_icon);

            Coin = new Icon((-2.5f, -2.5f), Sprite.Single(ResourceManager.GetTexture("coin"), (2.0f, 2.0f)));

            PickaxeFrame = new Icon((0.0f, -1.0f), Sprite.Single(ResourceManager.GetTexture("canvas"), Vector2.One));

            accessory = new Icon((3.0f, -3.0f), Sprite.Single(ResourceManager.GetTexture("bomb"), Vector2.One));

            ui_elements.Add(CHAR_button);
            ui_elements.Add(SHOP_button);
            ui_elements.Add(OPT_button);

            shop = new Shop(player);
            charMenu = new CharacterMenu(player);

            //
            buttonTest = new Button((3.0f, 2.0f), (2, 1), "+");
            subtractButton = new Button((1.0f, 2.0f), (1, 1), "-");
            barTest = new ProgressBar((3.0f, 1.0f), 5);
            barTest.Colorize((1, 0, 0));
            //

            Sprite selection = Sprite.Single(ResourceManager.GetTexture("blockSelection"),
                                             Vector2.One);
            blockSelection = new GameObject(selection, Transform.Default);
            cursor = new Cursor();

            Time.GetInstance();
            playTime = new Text((7.4f, -2.2f), Time.GetTime(), TextType.tall_white_o, 1f);
        }
        private int lastGold = 0;
        public void Update()
        {
            cursor.Update();
            blockSelection.Position = cursor.OnGridPos;
            player.Animator.GiveMouseLocation(cursor.InWorldPos);
            player.CursorOnGridPosition = cursor.OnGridPos;

            if (player.Gold != lastGold)
                setGold(player.Gold);
            lastGold = player.Gold;
            playTime.UpdateText(Time.GetTime());

            if (!isInMenu())
            {
                cursor.DisplayPickaxe = player.HasSelectedBlock;
                player.CanBeControlled = true;
                Camera.CanLookAround = true;
                updateAccessory();
            }
            else
            {
                cursor.DisplayPickaxe = false;
                player.CanBeControlled = false;
                Camera.CanLookAround = false;

                shop.MouseLocation = cursor.InWorldPos;
                shop.Update();

                charMenu.MouseLocation = cursor.InWorldPos;
                charMenu.Update();

                if (CHAR_button.CanPerformAction())
                    UI_state = "char";
                if (SHOP_button.CanPerformAction())
                    UI_state = "shop";
                if (OPT_button.CanPerformAction())
                    UI_state = "options";
            }

            if (Input.IsKeyPressed(Keys.E))
                switchMenu();

            foreach (var e in ui_elements)
                e.Update(cursor.InWorldPos);

            //
            buttonTest.Update(cursor.InWorldPos);
            barTest.Update(cursor.InWorldPos);
            subtractButton.Update(cursor.InWorldPos);


            if (buttonTest.CanPerformAction())
                barTest.SetValue(barTest.value + 0.1f);
            if (subtractButton.CanPerformAction())
                barTest.SetValue(barTest.value - 0.1f);
            //
        }
        private bool isInMenu()
        {
            return displayMenu;
        }

        private void switchMenu()
        {
            displayMenu = !displayMenu;
            if (!displayMenu)
                UI_state = "basic";
        }

        private void setGold(int newValue)
        {
            string money = convertValueToString(newValue);
            Gold.UpdateText(money);
            shop.Gold.UpdateText(money);
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

        private void updateAccessory()
        {
            if (accessoryIsBomb == player.IsHoldingBomb)
                return;
            accessoryIsBomb = player.IsHoldingBomb;
            string newTexture = getAccessoryTexture();
            accessory.ChangeTexture(newTexture);
        }

        private string getAccessoryTexture()
        {
            if (player.IsHoldingBomb)
                return "bomb";
            return "ladder";
        }

        public void Render()
        {
            if (!isInMenu())
            {
                if (player.HasSelectedBlock)
                    blockSelection.Render();

                renderEquippedPickaxe();

                PickaxeFrame.Render(Camera.GetLeftUpperCorner());

                accessory.Render(Camera.GetLeftUpperCorner());

                buttonTest.Render(Camera.GetLeftLowerCorner());
                subtractButton.Render(Camera.GetLeftLowerCorner());
                barTest.Render(Camera.GetLeftLowerCorner());

                Coin.Render(Camera.GetRightUpperCorner());
                Gold.Render(Camera.GetRightUpperCorner());
                playTime.Render(Camera.GetLeftUpperCorner());
            }
            else
            {
                renderMenu();
            }

            cursor.Render();

            WIP.Render(Camera.GetRightLowerCorner());
        }

        private void renderEquippedPickaxe()
        {
            Sprite pickaxe = player.equippedPickaxe.Sprite;
            Transform pickaxeTransform = Transform.Default;
            pickaxeTransform.Position = Camera.GetLeftUpperCorner() + (-3.0f, -5.0f);
            pickaxeTransform.Scale = (2.0f, 2.0f);
            pickaxe.RenderWithTransform(pickaxeTransform);
        }

        private void renderMenu()
        {
            dimmBackground.Render(Camera.GetScreenCenter());
            CHAR_button.Render(Camera.GetScreenCenter());
            SHOP_button.Render(Camera.GetScreenCenter());
            OPT_button.Render(Camera.GetScreenCenter());

            renderUIBasedonState();
        }

        private void renderUIBasedonState()
        {
            switch (UI_state)
            {
                case "basic":
                    UIbackgound.Render(Camera.GetScreenCenter());
                    break;
                case "char":
                    charMenu.Render();
                    break;
                case "shop":
                    shop.Render();
                    break;
                case "options":
                    renderOptions();
                    break;
            }
        }

        private void renderOptions()
        {
            OPT_back.Render(Camera.GetScreenCenter());
        }
    }
}
