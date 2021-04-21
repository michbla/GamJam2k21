using System;
using System.Diagnostics;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;


namespace GamJam2k21
{
    class UI
    {
        //renderery
        private SpriteRenderer spriteRenderer;
        private TextRenderer textRenderer;

        private PlayerStatistics playerStatistics;
        private Player player;

        private Texture frame;
        private Texture canvas;
        private Texture text;
        private Texture textWhite;

        private Vector2 viewPos;

        private List<Item> itemList = new List<Item>();

        private readonly int ROWS = 5;
        private readonly int COLUMNS = 5;

        public UI(SpriteRenderer spriteRenderer, TextRenderer textRenderer, PlayerStatistics playerStatistics, Player player, Vector2 viewPos)
        {
            this.spriteRenderer = spriteRenderer;
            this.textRenderer = textRenderer;
            this.playerStatistics = playerStatistics;
            this.player = player;
            this.viewPos = viewPos;
        }

        public void InitUI()
        {
            ResourceManager.LoadTexture("Data/Resources/Textures/frame.png", "itemFrame");
            ResourceManager.LoadTexture("Data/Resources/Textures/canvas.png", "canvas");
            canvas = ResourceManager.GetTexture("canvas");
            frame = ResourceManager.GetTexture("itemFrame");
            text = ResourceManager.GetTexture("textBitmap");
            textWhite = ResourceManager.GetTexture("textBitmapWhite");
        }

        public void DrawUI()
        {
            spriteRenderer.DrawSprite(frame, viewPos, viewPos + (-11.85f, 4.9f), (2.2f, 2f), 0);
            spriteRenderer.DrawSprite(player.equippedPickaxe.sprite, viewPos, viewPos + (-13.3f, 4.2f), (4, 4), 0);
        }

        public void DrawEQ()
        {
            int j = 0;
            int i = 0;
            spriteRenderer.DrawSprite(canvas, viewPos, viewPos + (-5.5f, -5.8f), (10, 13), 0);
            textRenderer.PrintText("EXPO:" + player.PlayerStatistics.getExp(), text, viewPos, viewPos + (-3f, 4.5f), (0.4f, 0.4f), (1, 1, 1));
            for (i = 0; i < COLUMNS; i++)
                for (j = 0; j < ROWS; j++)
                    spriteRenderer.DrawSprite(frame, viewPos, viewPos + (-3.6f + 1.2f * i, 3f - 1.2f * j), (1.5f, 1.5f), 0);

            itemList = player.eq.getInventory();

            i = 0;
            foreach (var item in itemList)
            {
                spriteRenderer.DrawSprite(item.icon, viewPos, viewPos + (-3.2f + 1.2f * (i % COLUMNS), 3.4f - 1.2f * (i / ROWS)), (0.7f, 0.7f), 0);
                int quantity = item.quantity;
                String qString = "";
                if (quantity < 10)
                    qString = " ";
                qString += quantity;
                textRenderer.PrintText(qString, textWhite, viewPos, viewPos + (-3.1f + 1.2f * (i % COLUMNS), 3.4f - 1.2f * (i / ROWS)), (0.35f, 0.35f), (1, 1, 1));
                i++;
            }
        }

        public void DrawInventoryFull()
        {
            //Red backpack icon?
        }
    }
}
