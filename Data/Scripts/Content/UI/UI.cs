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

        private Icon UIbackgound;
        private Icon CHAR_back;
        private Icon OPT_back;
        private Button CHAR_button;
        private Button SHOP_button;
        private Button OPT_button;

        private Text playTime;

        private string UI_state = "basic";

        private Icon Coin;
        private Text Gold = new Text((-8.75f, -2.0f), "     0", TextType.golden, 1.0f);

        private Shop shop;

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

            CHAR_back = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_char"), (12.0f, 8.0f)));
            OPT_back = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_options"), (12.0f, 8.0f)));

            CHAR_button = new Button((-5.0f, 3.5f), (3, 1), "A", TextType.ui_icon);
            SHOP_button = new Button((-1.5f, 3.5f), (3, 1), "B", TextType.ui_icon);
            OPT_button = new Button((2.0f, 3.5f), (3, 1), "C", TextType.ui_icon);

            Coin = new Icon((-2.5f, -2.5f), Sprite.Single(ResourceManager.GetTexture("coin"), (2.0f, 2.0f)));

            PickaxeFrame = new Icon((0.0f, -1.0f), Sprite.Single(ResourceManager.GetTexture("canvas"), Vector2.One));

            ui_elements.Add(CHAR_button);
            ui_elements.Add(SHOP_button);
            ui_elements.Add(OPT_button);

            shop = new Shop(player);

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
            playTime = new Text((3f, -1.2f), Time.GetTime(), TextType.white, 1f);
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

            if (!isInMenu())
            {
                cursor.DisplayPickaxe = player.HasSelectedBlock;
                player.CanBeControlled = true;
                Camera.CanLookAround = true;
            }
            else
            {
                cursor.DisplayPickaxe = false;
                player.CanBeControlled = false;
                Camera.CanLookAround = false;

                shop.MouseLocation = cursor.InWorldPos;
                shop.Update();

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

        public void Render()
        {
            if (!isInMenu())
            {
                if (player.HasSelectedBlock)
                    blockSelection.Render();

                renderEquippedPickaxe();

                PickaxeFrame.Render(Camera.GetLeftUpperCorner());

                buttonTest.Render(Camera.GetLeftLowerCorner());
                subtractButton.Render(Camera.GetLeftLowerCorner());
                barTest.Render(Camera.GetLeftLowerCorner());

                Coin.Render(Camera.GetRightUpperCorner());
                Gold.Render(Camera.GetRightUpperCorner());
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
            pickaxeTransform.Position = Camera.GetLeftUpperCorner() + (-2.5f, -5.5f);
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
                    renderAttributes();
                    break;
                case "shop":
                    shop.Render();
                    break;
                case "options":
                    renderOptions();
                    break;
            }
        }

        private void renderAttributes()
        {
            CHAR_back.Render(Camera.GetScreenCenter());
        }

        private void renderOptions()
        {
            OPT_back.Render(Camera.GetScreenCenter());
        }
    }
}
