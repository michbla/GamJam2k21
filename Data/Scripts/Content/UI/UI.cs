using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
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

        private Text VER = new Text((-3.6f, 0.0f), "Ver.1.0a", TextType.white, 0.4f);

        private AccessoryUI accessories;

        private Icon UIbackgound;
        private Icon SUM_back;

        private Button CHAR_button;
        private Button SHOP_button;
        private Button OPT_button;
        private Button EXIT_button;

        private Button RANK_button;
        private Button SUMMARY_button;

        private string UI_state = "basic";

        private Icon coin;
        private Text Gold = new Text((-6.8f, -1.4f), "     0", TextType.golden, 0.8f);
        private Text playTime;

        private Text lowLevel = new Text((1.3f, -5.4f), "0", TextType.white, 1.0f);
        private Text level = new Text((0.8f, -5.4f), "00", TextType.white, 1.0f);
        private Text lvl = new Text((1.0f, -6.0f), "LVL", TextType.white, 0.6f);

        private ProgressBar expBar;

        private Text levelReached = new Text((-5.0f, 0.2f), "   0M", TextType.white, 1.0f);

        private Shop shop;
        private Settings settings;
        private NativeWindow ns;
        private CharacterMenu charMenu;
        private RunSummary runSummary;
        private MainMenu menu;

        private ProgressBar staminaBar;
        private float staminaScale = 0.05f;

        public UI(Player player, NativeWindow _ns)
        {
            this.player = player;
            
            ns = _ns;
        }

        public void Initiate()
        {
            dimmBackground = new Icon((-12.0f, -6.75f),
                                      Sprite.Single(ResourceManager.GetTexture("dimmBackground"),
                                                    (24.0f, 13.5f)));

            UIbackgound = new Icon((-6.0f, -4.5f),
                Sprite.Single(ResourceManager.GetTexture("UI_back"), (12.0f, 8.0f)));

            SUM_back = new Icon((-7.5f, -5.5f),
                Sprite.Single(ResourceManager.GetTexture("UI_back_options"), (15f, 11f)));

            CHAR_button = new Button((-5.0f, 3.5f), (3, 1), "A", TextType.ui_icon);
            SHOP_button = new Button((-1.5f, 3.5f), (3, 1), "B", TextType.ui_icon);
            OPT_button = new Button((2.0f, 3.5f), (3, 1), "C", TextType.ui_icon);
            EXIT_button = new Button((-1.5f, -3f), (3, 11), "Exit Game", TextType.ui);

            SUMMARY_button = new Button((-5F, -4F), (3, 11), "this run", TextType.ui);
            RANK_button = new Button((-5F, -4F), (3, 11), "ranking", TextType.ui);

            coin = new Icon((-1.8f, -1.8f),
                Sprite.Single(ResourceManager.GetTexture("coin_mid"), (1.5f, 1.5f)));

            accessories = new AccessoryUI(player);

            makeNewExpBar();
            makeNewStaminaBar();
            ui_elements.Add(CHAR_button);
            ui_elements.Add(SHOP_button);
            ui_elements.Add(OPT_button);
            ui_elements.Add(EXIT_button);

            shop = new Shop(player);
            charMenu = new CharacterMenu(player);
            runSummary = new RunSummary(ns, player);
            menu = new MainMenu(ns, player);
            settings = new Settings(ns, (1,1));

            Sprite selection = Sprite.Single(ResourceManager.GetTexture("blockSelection"),
                                             Vector2.One);
            blockSelection = new GameObject(selection, Transform.Default);
            cursor = new Cursor();

            Time.GetInstance();
            playTime = new Text((8.4f, -2.2f), Time.GetTime(), TextType.tall_white_o, 1f);
        }
        private float lastStamina = 0;
        private int lastGold = 0;
        private int lastDepth = 0;
        private int lastLevel = 0;
        public void Update()
        {
            if (Input.IsClickingAButton && !Input.IsMouseButtonDown(MouseButton.Button1))
                Input.IsClickingAButton = false;

            cursor.Update();
            blockSelection.Position = cursor.OnGridPos;
            player.Animator.GiveMouseLocation(cursor.InWorldPos);
            player.CursorOnGridPosition = cursor.OnGridPos;

            if (player.Gold != lastGold)
                setGold(player.Gold);
            lastGold = player.Gold;

            int levelReached = player.stats.getLevelReached();
            if (levelReached != lastDepth)
                setLevelReached(levelReached);
            lastDepth = levelReached;

            if (player.Level != lastLevel)
                setLevel(player.Level);
            lastLevel = player.Level;

            if (player.StaminaMax != lastStamina)
                makeNewStaminaBar();
            lastStamina = player.StaminaMax;

            expBar.SetValue(player.ExpPercent * 2.0f);

            playTime.UpdateText(Time.GetTime());

            if (Game.state == GameState.menu)
            {
                menu.MouseLocation = cursor.InWorldPos;
                menu.Update();
            }

            if (Game.state == GameState.end)
                UI_state = "summary";
            if (!isInMenu())
            {
                if (UI_state == "summary")
                {
                    runSummary.MouseLocation = cursor.InWorldPos;
                    runSummary.Update();
                }
                
                cursor.DisplayPickaxe = player.HasSelectedBlock;
                player.CanBeControlled = true;
                Camera.CanLookAround = true;
                staminaBar.SetValue(player.Stamina * staminaScale);

                accessories.Update();
            }
            else
            {
                cursor.DisplayPickaxe = false;
                player.CanBeControlled = false;
                Camera.CanLookAround = false;

                if (UI_state == "basic")
                {
                    if (EXIT_button.CanPerformAction())
                        ns.Close();
                }

                if (UI_state == "shop")
                {
                    shop.MouseLocation = cursor.InWorldPos;
                    shop.Update();
                }

                if (UI_state == "char")
                {
                    charMenu.MouseLocation = cursor.InWorldPos;
                    charMenu.Update();
                }

                if (UI_state == "options")
                {
                    settings.MouseLocation = cursor.InWorldPos;
                    settings.Update();
                }

                if (CHAR_button.CanPerformAction())
                    UI_state = "char";
                if (SHOP_button.CanPerformAction())
                    UI_state = "shop";
                if (OPT_button.CanPerformAction())
                    UI_state = "options";
                if (Game.state == GameState.end)
                    UI_state = "summary";
            }
            if (UI_state != "summary")
            {
                if (Input.IsKeyPressed(Keys.E))
                    switchMenu();
                if (Input.IsKeyPressed(Keys.Escape))
                    switchMenu();
            }
            

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

        private void setGold(int newValue)
        {
            string money = convertValueToString(newValue);
            Gold.UpdateText(money);
            shop.Gold.UpdateText(money);
        }

        private void setLevelReached(int newValue)
        {
            string reached = convertDepthToString(newValue);
            levelReached.UpdateText(reached);
        }

        private void setLevel(int newValue)
        {
            String s = newValue.ToString();
            if (newValue < 10)
                lowLevel.UpdateText(s);
            else
                level.UpdateText(s);
            if (newValue == 40)
                lvl.UpdateText("MAX");
        }

        private void makeNewStaminaBar()
        {
            staminaBar = new ProgressBar((0.0f, -6.0f), player.StaminaMax * staminaScale);
            staminaBar.Colorize((1.0f, 1.0f, 0.6f));
            staminaBar.SetValue(player.Stamina * staminaScale);
        }

        private void makeNewExpBar()
        {
            expBar = new ProgressBar((1.9f, -7.0f), 2.5f);
            expBar.Colorize((0.76f, 0.94f, 0.16f));
            expBar.SetValue(player.ExpPercent * 2.5f );
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

        private string convertDepthToString(int depth)
        {
            string d = depth.ToString();
            string spaces = "";
            for (int i = 0; i < 4 - d.Length; i++)
                spaces += ' ';
            return spaces + d + 'M';
        }

        public void Render()
        {
            settings.RenderUpdate();
            

            if (UI_state == "summary")
            { 
                runSummary.Render(); 
            }
            if (Game.state == GameState.menu)
            {
                menu.Render();
            }

            else if (Game.state == GameState.active)
            {
                if (!isInMenu())
                {
                    if (player.HasSelectedBlock)
                        blockSelection.Render();

                    renderEquippedPickaxe();

                    accessories.Render();

                    coin.Render(Camera.GetRightUpperCorner());
                    Gold.Render(Camera.GetRightUpperCorner());
                    playTime.Render(Camera.GetLeftUpperCorner());
                    levelReached.Render(Camera.GetRightLowerCorner());

                    if (player.Level < 10)
                        lowLevel.Render(Camera.GetLeftUpperCorner());
                    else
                        level.Render(Camera.GetLeftUpperCorner());
                    lvl.Render(Camera.GetLeftUpperCorner());
                    expBar.Render(Camera.GetLeftUpperCorner());

                    staminaBar.Render(Camera.GetScreenCenter());
                }
                else
                {
                    renderMenu();
                }
            }

            cursor.Render();

            VER.Render(Camera.GetRightLowerCorner());
        }

        private void renderEquippedPickaxe()
        {
            Pickaxe pick = player.equippedPickaxe;
            int id = ResourceManager.GetPickaxeID(pick);
            String name = "pickaxe_shop_" + id;
            Texture tex = ResourceManager.GetTexture(name);
            Sprite pickaxe = Sprite.Single(tex, (3.0f, 5.0f));
            Transform pickaxeTransform = Transform.Default;
            pickaxeTransform.Position = Camera.GetLeftUpperCorner() + (0.0f, -4.5f);
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
    }
}
