using System;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using GamJam2k21.Interface;


namespace GamJam2k21
{
    public class UI
    {
        private Player player;
        private Cursor cursor;

        private bool displayMenu = false;

        private GameObject blockSelection;

        private Sprite dimmBackground;

        private Text WIP = new Text((-6.4f, 0.0f), "WORK IN PROGRESS", TextType.white, 0.4f);
        private UI_Element PickaxeFrame;
        private Button buttonTest;
        private Button subtractButton;
        private ProgressBar barTest;

        private Icon UIbackgound;
        private Icon CHAR_back;
        private Icon SHOP_back;
        private Icon OPT_back;
        private Button CHAR_button;
        private Button SHOP_button;
        private Button OPT_button;

        private string UI_state = "basic";

        public UI(Player player)
        {
            this.player = player;
        }

        public void Initiate()
        {
            UIbackgound = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back"), (12.0f, 8.0f)));

            CHAR_back = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_char"), (12.0f, 8.0f)));
            SHOP_back = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_shop"), (12.0f, 8.0f)));
            OPT_back = new Icon((-6.0f, -4.5f), Sprite.Single(ResourceManager.GetTexture("UI_back_options"), (12.0f, 8.0f)));

            CHAR_button = new Button((-5.0f, 3.5f), (3, 1), "A", TextType.ui_icon);
            SHOP_button = new Button((-1.5f, 3.5f), (3, 1), "B", TextType.ui_icon);
            OPT_button = new Button((2.0f, 3.5f), (3, 1), "C", TextType.ui_icon);

            PickaxeFrame = new Icon((0.0f, -1.0f), Sprite.Single(ResourceManager.GetTexture("canvas"), Vector2.One));

            dimmBackground = Sprite.Single(ResourceManager.GetTexture("dimmBackground"), (24.0f, 13.5f));

            buttonTest = new Button((3.0f, 2.0f), (2, 1), "+");
            subtractButton = new Button((1.0f, 2.0f), (1, 1), "-");

            barTest = new ProgressBar((3.0f, 1.0f), 5);
            barTest.Colorize((1, 0, 0));

            Sprite selection = Sprite.Single(ResourceManager.GetTexture("blockSelection"),
                                             Vector2.One);
            blockSelection = new GameObject(selection, Transform.Default);
            cursor = new Cursor();
        }

        public void Update()
        {
            cursor.Update();
            blockSelection.Position = cursor.OnGridPos;
            player.Animator.GiveMouseLocation(cursor.InWorldPos);
            player.CursorOnGridPosition = cursor.OnGridPos;
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
            }

            if (Input.IsKeyPressed(Keys.E))
                switchMenu();

            UIbackgound.Update(cursor.InWorldPos);
            CHAR_back.Update(cursor.InWorldPos);
            SHOP_back.Update(cursor.InWorldPos);
            OPT_back.Update(cursor.InWorldPos);
            CHAR_button.Update(cursor.InWorldPos);
            SHOP_button.Update(cursor.InWorldPos);
            OPT_button.Update(cursor.InWorldPos);

            WIP.Update(cursor.InWorldPos);
            PickaxeFrame.Update(cursor.InWorldPos);
            buttonTest.Update(cursor.InWorldPos);
            barTest.Update(cursor.InWorldPos);
            subtractButton.Update(cursor.InWorldPos);

            if (buttonTest.CanPerformAction())
            {
                barTest.SetValue(barTest.value + 0.1f);
            }

            if (subtractButton.CanPerformAction())
            {
                barTest.SetValue(barTest.value - 0.1f);
            }

            if (CHAR_button.CanPerformAction())
            {
                UI_state = "char";
            }
            if (SHOP_button.CanPerformAction())
            {
                UI_state = "shop";
            }
            if (OPT_button.CanPerformAction())
            {
                UI_state = "options";
            }
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
            dimmBackground.RenderWithTransform(new Transform(Camera.GetLeftLowerCorner()));
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
                    renderShop();
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

        private void renderShop()
        {
            SHOP_back.Render(Camera.GetScreenCenter());
        }

        private void renderOptions()
        {
            OPT_back.Render(Camera.GetScreenCenter());
        }
    }
}
