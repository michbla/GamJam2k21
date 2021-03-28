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
        public int[,] mapData;

        //Rozmiar siatki
        private int width;
        private int height;

        public List<Block> currentBlocks = new List<Block>();
        public List<GameObject> backgrounds = new List<GameObject>();

        private Vector2i playerChunk;
        private Vector2i lastPlayerChunk;

        //Kostruktor poziomu
        public GameLevel(int w, int h)
        {
            width = w;
            height = h;
            mapData = new int[width, height];
            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                {
                    mapData[j, i] = 0;
                }
            Init();
        }
        //TEMP:
        public void Update(Vector2 playerPos, float deltaTime)
        {
            playerChunk = ((int)playerPos.X / 16, -(int)playerPos.Y / 16);
            if (playerChunk.X != lastPlayerChunk.X)
            {
                if (playerChunk.X < lastPlayerChunk.X)
                {
                    DespawnChunk(playerChunk.X + 2, playerChunk.Y);
                    DespawnChunk(playerChunk.X + 2, playerChunk.Y - 1);
                    DespawnChunk(playerChunk.X + 2, playerChunk.Y + 1);

                    SpawnChunk(playerChunk.X - 1, playerChunk.Y);
                    SpawnChunk(playerChunk.X - 1, playerChunk.Y - 1);
                    SpawnChunk(playerChunk.X - 1, playerChunk.Y + 1);

                }
                else
                {
                    DespawnChunk(playerChunk.X - 2, playerChunk.Y);
                    DespawnChunk(playerChunk.X - 2, playerChunk.Y - 1);
                    DespawnChunk(playerChunk.X - 2, playerChunk.Y + 1);

                    SpawnChunk(playerChunk.X + 1, playerChunk.Y);
                    SpawnChunk(playerChunk.X + 1, playerChunk.Y - 1);
                    SpawnChunk(playerChunk.X + 1, playerChunk.Y + 1);

                }
            }
            else if (playerChunk.Y != lastPlayerChunk.Y)
            {
                if (playerChunk.Y < lastPlayerChunk.Y)
                {
                    DespawnChunk(playerChunk.X, playerChunk.Y + 2);
                    DespawnChunk(playerChunk.X - 1, playerChunk.Y + 2);
                    DespawnChunk(playerChunk.X + 1, playerChunk.Y + 2);

                    SpawnChunk(playerChunk.X, playerChunk.Y - 1);
                    SpawnChunk(playerChunk.X - 1, playerChunk.Y - 1);
                    SpawnChunk(playerChunk.X + 1, playerChunk.Y - 1);

                }
                else
                {
                    DespawnChunk(playerChunk.X, playerChunk.Y - 2);
                    DespawnChunk(playerChunk.X - 1, playerChunk.Y - 2);
                    DespawnChunk(playerChunk.X + 1, playerChunk.Y - 2);

                    SpawnChunk(playerChunk.X, playerChunk.Y + 1);
                    SpawnChunk(playerChunk.X - 1, playerChunk.Y + 1);
                    SpawnChunk(playerChunk.X + 1, playerChunk.Y + 1);

                }
            }

            for (var i = 0; i < currentBlocks.Count; i++)
            {
                var block = currentBlocks[i];
                if (!block.isDestroyed)
                {
                    float distance = (playerPos - (block.position + (block.size.X / 2f, block.size.Y / 2f))).Length;
                    if (distance < 5.0f)
                        block.distanceToPlayer = distance;
                    block.Update(deltaTime);
                }
            }
            lastPlayerChunk = playerChunk;
        }

        private void SpawnChunk(int x, int y)
        {
            int posX = x * 16;
            int posY = y * 16;
            if(posY >= 0)
                backgrounds.Add( new GameObject((posX,-posY - 15),(16.0f, 16.0f), ResourceManager.GetTexture("backgroundDirt")));
            for (var i = posY; i < posY + 16; i++)
            {
                for (var j = posX; j < posX + 16; j++)
                {
                    if (j < 0 || i < 0 || j >= width || i >= height)
                        return;
                    SpawnBlock(j, i);
                }
            }
        }
        private void DespawnChunk(int x, int y)
        {
            double posX = x * 16.0f;
            double posY = -y * 16.0f;
            for (var i = 0; i < currentBlocks.Count; i++)
            {
                var block = currentBlocks[i];
                if (block.position.X >= posX && block.position.X < posX + 16.0f && block.position.Y <= posY && block.position.Y > posY - 16.0f)
                {
                    currentBlocks.Remove(block);
                    i--;
                }
            }
            for(var i = 0; i < backgrounds.Count; i++){
                var bg = backgrounds[i];
                if(bg.position.X == posX && bg.position.Y == posY)
                {
                    backgrounds.Remove(bg);
                    i--;
                }
            }
        }

        public string getBlockName(int x, int y)
        {
            if (x < 0 || y < 0 || x > width || y > height)
                return null;
            for (var i = 0; i < currentBlocks.Count; i++)
            {
                var block = currentBlocks[i];
                if (block.distanceToPlayer <= 2.0f && block.position.X == x && block.position.Y == -y)
                {
                    //Console.WriteLine(block.name);
                    return block.name;
                }
            }
            return null;
        }

        public bool DamageBlock(int x, int y, Player player)
        {
            if (x < 0 || y < 0 || x > width || y > height)
                return false;
            for (var i = 0; i < currentBlocks.Count; i++)
            {
                var block = currentBlocks[i];
                if (block.distanceToPlayer <= 2.0f && block.position.X == x && block.position.Y == -y)
                {
                    if (player.isReadyToDamage && player.isDamaging)
                    {
                        if (block.Damage(player))
                        {
                            currentBlocks.Remove(block);
                            mapData[x, y] = 0;
                            return true;

                        }
                    }

                }
            }
            return false;
        }

        private void SpawnBlock(int x, int y)
        {
            if (mapData[x, y] != 0)
                currentBlocks.Add(new Block(ResourceManager.GetBlockByID(mapData[x,y]), (x, -y)));
        }

        public void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            foreach(var bg in backgrounds)
            {
                bg.Draw(rend,viewPos);
            }
            for (var i = 0; i < currentBlocks.Count; i++)
            {
                currentBlocks[i].Draw(rend, viewPos);
            }

        }
        public void Init()
        {
            float[,] data = GenerateNoiseMap(this.width * 2, this.height, 5, 30.0f);

            for (var j = 0; j < width; j++)
            {
                var i = 0;
                while (data[j, i] < 0.3f && i < height)
                {
                    i++;
                }
                data[j, i] = 2.0f;

                Random rand = new Random();
                int l = rand.Next(4, 7);
                for (var k = 1; k < l; k++)
                {
                    if (data[j, i + k] > 0.3f)
                        data[j, i + k] = 1.0f;
                }
            }

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if (data[j, i] > 1.5f)
                    {
                        mapData[j, i] = 1;
                    }
                    else if (data[j, i] < 0.3f)
                    {
                        mapData[j, i] = 0;
                    }
                    else if (data[j, i] < 0.7f)
                    {
                        mapData[j, i] = 3;
                    }
                    else
                    {
                        mapData[j, i] = 2;
                    }
                }
            }

            SpawnChunk(0, 0);
            SpawnChunk(1, 0);
            SpawnChunk(0, 1);
            SpawnChunk(1, 1);

        }

        private float[,] GenerateNoiseMap(int width, int height, int octaves, float freq)
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
            float[,] result = new float[width,height];
            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                    result[j, i] = MathHelper.Clamp((data[i, j] - min) / (max - min), 0.0f, 1.0f);

            return result;
        }
    }
}
