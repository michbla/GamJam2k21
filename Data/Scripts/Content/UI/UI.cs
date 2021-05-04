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

        private List<Item> itemList = new List<Item>();

        private readonly int INVENTORY_ROWS = 5;
        private readonly int INVENTORY_COLUMNS = 5;

        private bool displayEq = false;
        private bool displaySummary = false;

        private GameObject blockSelection;

        private Sprite dimmBackground;

        private Text WIP = new Text((-6.4f, 0.0f), "WORK IN PROGRESS", TextType.white, 0.4f);
        private UI_Element PickaxeFrame;
        private Button buttonTest;
        private Button subtractButton;
        private ProgressBar barTest;

        public UI(Player player)
        {
            this.player = player;
        }

        public void Initiate()
        {
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
                switchInventory();

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
        }

        private bool isInMenu()
        {
            return displayEq || displaySummary;
        }

        private void switchInventory()
        {
            displayEq = !displayEq;
        }

        public void Render()
        {
            if (player.HasSelectedBlock && !isInMenu())
                blockSelection.Render();

            renderEquippedPickaxe();

            if (displayEq)
                RenderEQ();

            PickaxeFrame.Render(Camera.GetLeftUpperCorner());

            if(isInMenu())
                dimmBackground.RenderWithTransform(new Transform(Camera.GetLeftLowerCorner()));
            buttonTest.Render(Camera.GetLeftLowerCorner());
            subtractButton.Render(Camera.GetLeftLowerCorner());
            barTest.Render(Camera.GetLeftLowerCorner());

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

        public void RenderEQ()
        {
            int j = 0;
            int i = 0;
            Transform canvasTransform = Transform.Default;
            canvasTransform.Position = Camera.GetScreenCenter((-5.0f, -6.5f));
            //canvas.RenderWithTransform(canvasTransform);
            //textRenderer.PrintText("EXPO:" + player.PlayerStatistics.getExp(), text, Camera.Position + (-3f, 4.5f), (0.4f, 0.4f), (1, 1, 1));
            //for (i = 0; i < COLUMNS; i++)
            //    for (j = 0; j < ROWS; j++)
            //        spriteRenderer.DrawSprite(frame, new Transform(Camera.Position + (-3.2f + 1.2f * (i % COLUMNS), 3.4f - 1.2f * (i / ROWS)), (0.7f, 0.7f)), Vector3.One);

            /*
            int j = 0;
            int i = 0;
            spriteRenderer.DrawSprite(canvas, new Transform(Camera.Position + (-5.5f, -5.8f), (10.0f, 13.0f)), Vector3.One);
            textRenderer.PrintText("EXPO:" + player.PlayerStatistics.getExp(), text, Camera.Position + (-3f, 4.5f), (0.4f, 0.4f), (1, 1, 1));
            for (i = 0; i < COLUMNS; i++)
                for (j = 0; j < ROWS; j++)
                    spriteRenderer.DrawSprite(frame, new Transform(Camera.Position + (-3.2f + 1.2f * (i % COLUMNS), 3.4f - 1.2f * (i / ROWS)), (0.7f, 0.7f)), Vector3.One);

            itemList = player.eq.GetInventoryItems();

            i = 0;
            foreach (var item in itemList)
            {
                spriteRenderer.DrawSprite(item.icon, new Transform(Camera.Position + (-3.6f + 1.2f * i, 3f - 1.2f * j), (1.5f, 1.5f)), Vector3.One);
                int quantity = item.quantity;
                String qString = "";
                if (quantity < 10)
                    qString = " ";
                qString += quantity;
                textRenderer.PrintText(qString, textWhite, Camera.Position + (-3.1f + 1.2f * (i % COLUMNS), 3.4f - 1.2f * (i / ROWS)), (0.35f, 0.35f), (1, 1, 1));
                i++;
            }*/
        }

        public void DrawInventoryFull()
        {
            //Red backpack icon?
        }

        public void DrawDaySummary(int day)
        {
            /*
            itemList = player.eq.GetInventoryItems();
            int i = 0;
            spriteRenderer.DrawSprite(canvas, new Transform(Camera.Position + (-6.2f, -7.5f), (12.0f, 15.0f)), Vector3.One);
            textRenderer.PrintText("SUMMARY", textBold, Camera.Position + (-3.80f, 4.2f), (1f, 1f), (1f, 1f, 1f));
            textRenderer.PrintText("DAY " + day, textBold, Camera.Position + (-1.6f, 3.6f), (0.5f, 0.5f), (1f, 1f, 1f));
            foreach (var item in itemList)
            {
                Console.WriteLine(item.name);
                spriteRenderer.DrawSprite(item.icon, new Transform(Camera.Position + (-4.3f, 2.0f - i), Vector2.One), Vector3.One);
                textRenderer.PrintText("X" + item.quantity, text, Camera.Position + (-3.2f, 2.2f - i), (0.5f, 0.5f), (1f, 1f, 1f));
                i++;
            }
            textRenderer.PrintText("ORES VALUE:" + playerStatistics.getExp(), text, Camera.Position + (-4.2f, 2.2f - i), (0.4f, 0.4f), (1f, 1f, 1f));
            textRenderer.PrintText("CURRENT DEPTH:" + playerStatistics.getLevelReached(), text, Camera.Position + (-4.2f, 1.2f - i), (0.4f, 0.4f), (1f, 1f, 1f));
            textRenderer.PrintText("PRESS ENTER TO PROCEED", text, Camera.Position + (-5f, -5.5f), (0.4f, 0.4f), (1f, 1f, 1f));
            */
        }
    }
}
