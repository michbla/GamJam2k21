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

        private Icon accessory;
        private bool accessoryIsBomb = true;

        private Icon UIbackgound;
        private Icon OPT_back;
        private Icon SUM_back;

        private Button CHAR_button;
        private Button SHOP_button;
        private Button OPT_button;
        private Button EXIT_button;

        private Button RANK_button;
        private Button SUMMARY_button;

        private string UI_state = "basic";

        private Icon Coin;
        private Text Gold = new Text((-8.75f, -2.0f), "     0", TextType.golden, 1.0f);
        private Text playTime;

        

        private Shop shop;
        private Settings settings;
        private NativeWindow ns;
        private CharacterMenu charMenu;

        private ProgressBar staminaBar;
        private float staminaScale = 0.05f;

        public UI(Player player, NativeWindow _ns)
        {
            this.player = player;
            settings = new Settings(_ns);
            ns = _ns;
        }

        public void Initiate()
        {
            dimmBackground = new Icon((-12.0f, -6.75f),
                                      Sprite.Single(ResourceManager.GetTexture("dimmBackground"),
                                                    (24.0f, 13.5f)));

            UIbackgound = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back"), (12.0f, 8.0f)));

            OPT_back = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_options"), (12.0f, 8.0f)));

            SUM_back = new Icon((-7.5f, -5.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_options"), (15f, 11f)));

            CHAR_button = new Button((-5.0f, 3.5f), (3, 1), "A", TextType.ui_icon);
            SHOP_button = new Button((-1.5f, 3.5f), (3, 1), "B", TextType.ui_icon);
            OPT_button = new Button((2.0f, 3.5f), (3, 1), "C", TextType.ui_icon);
            EXIT_button = new Button((-1.5f, -3f), (3, 11), "Exit Game", TextType.ui);

            SUMMARY_button = new Button((-5F, -4F), (3, 11), "this run", TextType.ui);
            RANK_button = new Button((-5F, -4F), (3, 11), "ranking", TextType.ui);

            Coin = new Icon((-2.5f, -2.5f), Sprite.Single(ResourceManager.GetTexture("coin"), (2.0f, 2.0f)));

            PickaxeFrame = new Icon((0.0f, -1.0f), Sprite.Single(ResourceManager.GetTexture("canvas"), Vector2.One));

            accessory = new Icon((3.0f, -3.0f), Sprite.Single(ResourceManager.GetTexture("bomb"), Vector2.One));

            makeNewStaminaBar();

            ui_elements.Add(CHAR_button);
            ui_elements.Add(SHOP_button);
            ui_elements.Add(OPT_button);
            ui_elements.Add(EXIT_button);
            ui_elements.Add(SUMMARY_button);
            ui_elements.Add(RANK_button);

            shop = new Shop(player);
            charMenu = new CharacterMenu(player);

            Sprite selection = Sprite.Single(ResourceManager.GetTexture("blockSelection"),
                                             Vector2.One);
            blockSelection = new GameObject(selection, Transform.Default);
            cursor = new Cursor();

            Time.GetInstance();
            playTime = new Text((7.4f, -2.2f), Time.GetTime(), TextType.tall_white_o, 1f);
        }
        private float lastStamina = 0;
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

            if (player.StaminaMax != lastStamina)
                makeNewStaminaBar();
            lastStamina = player.StaminaMax;

            playTime.UpdateText(Time.GetTime());

            if (!isInMenu())
            {
                cursor.DisplayPickaxe = player.HasSelectedBlock;
                player.CanBeControlled = true;
                Camera.CanLookAround = true;
                updateAccessory();
                staminaBar.SetValue(player.Stamina * staminaScale);
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

                settings.MouseLocation = cursor.InWorldPos;
                settings.Update();


                if (CHAR_button.CanPerformAction())
                    UI_state = "char";
                if (SHOP_button.CanPerformAction())
                    UI_state = "shop";
                if (OPT_button.CanPerformAction())
                    UI_state = "options";
                if (EXIT_button.CanPerformAction())
                    ns.Close();
            }

            if (Input.IsKeyPressed(Keys.E))
                switchMenu();
            if (Input.IsKeyPressed(Keys.Escape))
                hideMenu();

            foreach (var e in ui_elements)
                e.Update(cursor.InWorldPos);
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

        private void hideMenu()
        {
            displayMenu = false;
            UI_state = "basic";
        }

        private void setGold(int newValue)
        {
            string money = convertValueToString(newValue);
            Gold.UpdateText(money);
            shop.Gold.UpdateText(money);
        }

        private void makeNewStaminaBar()
        {
            staminaBar = new ProgressBar((0.0f, -6.0f), player.StaminaMax * staminaScale);
            staminaBar.Colorize((1.0f, 1.0f, 0.6f));
            staminaBar.SetValue(player.Stamina * staminaScale);
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
            settings.RenderUpdate();

            if (!isInMenu())
            {
                if (player.HasSelectedBlock)
                    blockSelection.Render();

                renderEquippedPickaxe();

                PickaxeFrame.Render(Camera.GetLeftUpperCorner());

                accessory.Render(Camera.GetLeftUpperCorner());

                Coin.Render(Camera.GetRightUpperCorner());
                Gold.Render(Camera.GetRightUpperCorner());
                playTime.Render(Camera.GetLeftUpperCorner());

                staminaBar.Render(Camera.GetScreenCenter());
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
                    EXIT_button.Render(Camera.GetScreenCenter());
                    break;
                case "char":
                    charMenu.Render();
                    break;
                case "shop":
                    shop.Render();
                    break;
                case "options":
                    settings.Render();
                    break;
            }
        }

        public void renderRunSummary()
        {
            bool isSummary = true;
            Text GAMEOVER = new Text((-6.7f, 3.5f), "GAME OVER", TextType.ui, 1.5f);
            Text PLAYTIME = new Text((-7f, 2.5f), "Run Time: ", TextType.ui, 0.75f); 
            Text PlayTime =  new Text((-0.5f, 2.5f), Time.GetTime(), TextType.ui, 0.75f);
            dimmBackground.Render(Camera.GetScreenCenter());
            SUM_back.Render(Camera.GetScreenCenter());
            {
                GAMEOVER.Render(Camera.GetScreenCenter());
                PLAYTIME.Render(Camera.GetScreenCenter());
                PlayTime.Render(Camera.GetScreenCenter());
                RANK_button.Render(Camera.GetScreenCenter());
                EXIT_button.Render(Camera.GetScreenCenter() + (2f, -1f));
            }
            cursor.DisplayPickaxe = false;
            cursor.Render();
            //TODO 
            //move it to separate class
        }
    }
}
