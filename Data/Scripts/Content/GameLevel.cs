using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa odpowiadajaca za poziom gry (generowane bloki)
    /// </summary>
    public class GameLevel
    {
        //Dane przechowujace rodzaj bloku w danej pozycji
        //na dwuwymiarowej siatce
        public int[,] data;

        //Rozmiar siatki
        private int width;
        private int height;

        public List<Block> currentBlocks;

        //Kostruktor poziomu
        public GameLevel(int w, int h)
        {
            width = w;
            height = h;
            data = new int[width, height];
            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                    data[j, i] = 1;
            currentBlocks = new List<Block>();
            Init();

        }
        //TEMP:
        public void Update(Vector2 playerPos)
        {
            foreach (var block in currentBlocks)
                if (!block.isDestroyed)
                    block.distanceToPlayer = (playerPos - (block.position + (block.size.X/2f,block.size.Y/2f))).Length;
        }
        public void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            foreach (var block in currentBlocks)
                if (!block.isDestroyed)
                    block.Draw(rend,viewPos);
        }
        public void Init()
        {
            for (var i = 0; i > -height; i--)
                for (var j = 0; j < width; j++)
                {
                    if(i == 0)
                    {
                        if(j < 10 || j > 12)
                            currentBlocks.Add(new Block((j, i), ResourceManager.GetTexture("grass")));
                    }
                    else
                    {
                        currentBlocks.Add(new Block((j, i), ResourceManager.GetTexture("dirt")));
                    }
                }
        }
    }
}
