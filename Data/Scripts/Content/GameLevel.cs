using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa odpowiadajaca za poziom gry (generowane bloki)
    /// </summary>
    public class GameLevel
    {
        //Dane przechowujace rodzaj bloku w danej pozycji
        //na dwuwymiarowej siatce
        public float[,] data;

        //Rozmiar siatki
        private int width;
        private int height;

        public List<Block> currentBlocks;

        //Kostruktor poziomu
        public GameLevel(int w, int h)
        {
            width = w;
            height = h;
            data = new float[width, height];
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
                    block.distanceToPlayer = (playerPos - (block.position + (block.size.X / 2f, block.size.Y / 2f))).Length;
        }
        public void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            foreach (var block in currentBlocks)
                if (!block.isDestroyed)
                    block.Draw(rend, viewPos);
        }
        public void Init()
        {
            GenerateNoiseMap(this.width, this.height, 1, 10.0f);

                for (var j = 0; j < width; j++)
                {
                    var i = 0;
                    while(data[j,i] < 0.3f && i < height)
                    {
                        i++;
                    }
                data[j, i] = 2.0f;

                Random rand = new Random();
                int l = rand.Next(4,7);
                for (var k = 1; k < l; k++)
                {
                    if(data[j, i + k] > 0.3f)
                        data[j, i + k] = 1.0f;
                }
            }

            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                {
                    if (data[j, i] > 1.5f)
                    {
                        currentBlocks.Add(new Block((j, -i), ResourceManager.GetTexture("grass")));
                    }
                    else if (data[j, i] < 0.3f)
                    {
                        //Powietrze
                    }
                    else if (data[j, i] < 0.7f)
                    {
                        currentBlocks.Add(new Block((j, -i), ResourceManager.GetTexture("stone")));
                    }
                    else
                    {
                        currentBlocks.Add(new Block((j, -i), ResourceManager.GetTexture("dirt")));
                    }
                }
        }

        private void GenerateNoiseMap(int width, int height, int octaves, float freq)
        {
            var data = new float[height, width];

            var min = float.MaxValue;
            var max = float.MinValue;

            Noise2d.Reseed();

            var frequency = freq;
            var amplitude = 1f;

            for (var octave = 0; octave < octaves; octave++)
            {
                Parallel.For(0, width * height, (offset) =>
                    {
                        var j = offset % width;
                        var i = offset / width;
                        var noise = Noise2d.Noise(i * frequency * 1f / width, j * frequency * 1f / height);
                        noise = data[i, j] += noise * amplitude;

                        min = MathHelper.Min(min, noise);
                        max = MathHelper.Max(max, noise);

                    }
                );
                frequency *= 4;
                amplitude /= 10;
            }

            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                    this.data[j, i] = MathHelper.Clamp((data[i, j] - min) / (max - min),0.0f,1.0f);
        }
    }
}
