﻿using System.Collections.Generic;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public enum TextType
    {
        normal,
        bold,
        white,
        ui,
        ui_icon,
        golden,
        tall_white,
        tall_white_o
    }
    public class TextRenderer
    {
        private static Dictionary<TextType, Sprite> fonts;

        private static Vector2i SHEET_SIZE = (16, 8);

        public TextRenderer()
        {
            initFonts();
        }

        private static void initFonts()
        {
            if (fonts != null)
                return;
            fonts = new Dictionary<TextType, Sprite>();
            addFont(TextType.normal, "textBitmap");
            addFont(TextType.bold, "textBitmapBold");
            addFont(TextType.white, "textBitmapWhite");
            addFont(TextType.ui, "textBitmapUI");
            addFont(TextType.ui_icon, "textBitmapUI_icons");
            addFont(TextType.golden, "textBitmapGold");
            addFontWithCustomSize(TextType.tall_white, "textBitmapTallWhite", (1, 2));
            addFontWithCustomSize(TextType.tall_white_o, "textBitmapTallWhiteOutline", (1, 2));
        }

        private static void addFont(TextType type, string textureName)
        {
            fonts.Add(type,
                      Sprite.Sheet(ResourceManager.GetTexture(textureName),
                                   Vector2.One,
                                   SHEET_SIZE));
        }

        private static void addFontWithCustomSize(TextType type, string textureName, Vector2i size)
        {
            fonts.Add(type,
                      Sprite.Sheet(ResourceManager.GetTexture(textureName),
                                   size,
                                   SHEET_SIZE));
        }

        public void PrintText(string text, TextType type, Vector2 position, float scale = 1.0f)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Vector2i positionOnSheet;
                positionOnSheet.Y = text[i] / 16;
                positionOnSheet.X = (text[i]) % 16 - 1;
                if (positionOnSheet.X < 0)
                    positionOnSheet = (15, positionOnSheet.Y - 1);

                Transform charTransform = new Transform((position.X + scale * i, position.Y),
                                                        (scale, scale));
                fonts[type].RenderWithTransform(charTransform, positionOnSheet);
            }
        }
    }
}
