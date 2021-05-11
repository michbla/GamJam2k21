using System.Collections.Generic;
using OpenTK.Mathematics;
using System;

namespace GamJam2k21
{
    public class GameLevel
    {
        private readonly Player player;

        public int[,] mapData;
        public int[,] oreData;

        private readonly int width;
        private readonly int depth;

        public List<Block> currentBlocks = new List<Block>();
        public List<GameObject> backgrounds = new List<GameObject>();

        private Vector2i playerChunk;
        private Vector2i lastPlayerChunk;

        private Vector2 damagedBlockPosition = (0.0f, 0.0f);
        private Vector3 damagedBlockColor = (1.0f, 1.0f, 1.0f);
        private readonly ParticleEmmiter damageParticles;
        private readonly ParticleEmmiter destructionParticles;

        public GameLevel(Player player, int width, int depth)
        {
            this.player = player;
            this.width = width;
            this.depth = depth;
            mapData = new int[this.width, this.depth];
            oreData = new int[this.width, this.depth];
            for (var i = 0; i < this.depth; i++)
                for (var j = 0; j < this.width; j++)
                    mapData[j, i] = oreData[j, i] = 0;
            damageParticles = new ParticleEmmiter(
                ResourceManager.GetShader("particle"),
                ResourceManager.GetTexture("particle"),
                128);
            destructionParticles = new ParticleEmmiter(
                ResourceManager.GetShader("particle"),
                ResourceManager.GetTexture("particle"),
                128);
            Init();
        }
        public void Init()
        {
            float[,] blockData = Noise2D.GenerateNoiseMap(width * 2, depth, 5, 30.0f);

            fillOreData();
            generateGrassOnTop(blockData);
            fillMapData(blockData);

            spawnInitialChunks();
        }
        private void fillOreData()
        {
            float[,] coal = Noise2D.GenerateNoiseMap(width * 3, depth, 5, 200.0f);
            for (var i = 0; i < depth; i++)
                for (var j = 0; j < width; j++)
                    if (coal[j, i] > 0.7f)
                        oreData[j, i] = 1;

            float[,] copper = Noise2D.GenerateNoiseMap(width * 2, depth, 5, 200.0f);
            for (var i = 0; i < depth; i++)
                for (var j = 0; j < width; j++)
                    if (copper[j, i] > 0.8f)
                        oreData[j, i] = 2;

            float[,] iron = Noise2D.GenerateNoiseMap(width * 3, depth, 5, 200.0f);
            for (var i = 10; i < depth; i++)
                for (var j = 0; j < width; j++)
                    if (copper[j, i] > 0.83f)
                        oreData[j, i] = 3;

            float[,] gold = Noise2D.GenerateNoiseMap(width * 2, depth, 5, 200.0f);
            for (var i = 20; i < depth; i++)
                for (var j = 0; j < width; j++)
                    if (copper[j, i] > 0.84f)
                        oreData[j, i] = 4;

            float[,] diamond = Noise2D.GenerateNoiseMap(width * 2, depth, 5, 200.0f);
            for (var i = 35; i < depth; i++)
                for (var j = 0; j < width; j++)
                    if (copper[j, i] > 0.85f)
                        oreData[j, i] = 5;
        }

        private void generateGrassOnTop(float[,] blockData)
        {
            Random random = new Random();
            int dirtDepth;
            for (var i = 0; i < width; i++)
            {
                int firstBlockFromTop = 0;
                while (blockData[i, firstBlockFromTop] < 0.3f && firstBlockFromTop < depth)
                    firstBlockFromTop++;
                blockData[i, firstBlockFromTop] = 2.0f;

                dirtDepth = random.Next(4, 7);
                for (var k = 1; k < dirtDepth; k++)
                    if (blockData[i, firstBlockFromTop + k] > 0.3f)
                        blockData[i, firstBlockFromTop + k] = 1.0f;
            }
        }

        private void fillMapData(float[,] blockData)
        {
            for (var i = 0; i < depth; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if (blockData[j, i] > 1.5f)
                    {
                        mapData[j, i] = 1;
                    }
                    else if (blockData[j, i] < 0.3f)
                    {
                        mapData[j, i] = 0;
                    }
                    else if (blockData[j, i] < 0.7f)
                    {
                        mapData[j, i] = 3;
                    }
                    else
                    {
                        mapData[j, i] = 2;
                    }
                }
            }
        }

        private void spawnInitialChunks()
        {
            spawnChunk(4, 0);
            spawnChunk(3, 0);
            spawnChunk(4, 1);
            spawnChunk(3, 1);
        }

        public void Update()
        {
            Vector2 playerPos = player.Center;
            playerChunk = ((int)playerPos.X / 16, -(int)playerPos.Y / 16);
            if (playerChunk.X != lastPlayerChunk.X)
            {
                if (playerChunk.X < lastPlayerChunk.X)
                    for (int i = -1; i < 2; i++)
                    {
                        despawnChunk(playerChunk.X + 2, playerChunk.Y + i);
                        spawnChunk(playerChunk.X - 1, playerChunk.Y + i);
                    }
                else
                    for (int i = -1; i < 2; i++)
                    {
                        despawnChunk(playerChunk.X - 2, playerChunk.Y + i);
                        spawnChunk(playerChunk.X + 1, playerChunk.Y + i);
                    }
            }
            else if (playerChunk.Y != lastPlayerChunk.Y)
            {
                if (playerChunk.Y < lastPlayerChunk.Y)
                    for (int i = -1; i < 2; i++)
                    {
                        despawnChunk(playerChunk.X + i, playerChunk.Y + 2);
                        spawnChunk(playerChunk.X + i, playerChunk.Y - 1);
                    }
                else
                    for (int i = -1; i < 2; i++)
                    {
                        despawnChunk(playerChunk.X + i, playerChunk.Y - 2);
                        spawnChunk(playerChunk.X + i, playerChunk.Y + 1);
                    }
            }

            for (var i = 0; i < currentBlocks.Count; i++)
            {
                var block = currentBlocks[i];
                block.DistanceToPlayer = calculateDistanceToPlayer(block.Position);
                block.Update();
            }
            lastPlayerChunk = playerChunk;

            damageParticles.Update();
            destructionParticles.Update();
        }

        private void spawnChunk(int x, int y)
        {
            int posX = x * 16;
            int posY = y * 16;
            string background = getBackgroundByDepth(posY);
            if (posY >= 0)
            {
                Sprite backgroundSprite = Sprite.Single(
                    ResourceManager.GetTexture(background),
                    (16.0f, 16.0f));
                Transform backgroundTransform = Transform.Default;
                backgroundTransform.Position = (posX, -posY - 15.0f);
                backgrounds.Add(new GameObject(backgroundSprite, backgroundTransform));
            }
            for (var i = posY; i < posY + 16; i++)
            {
                for (var j = posX; j < posX + 16; j++)
                {
                    if (j < 0 || i < 0 || j >= width || i >= depth)
                        return;
                    spawnBlock(j, i);
                }
            }
        }

        private string getBackgroundByDepth(int depth)
        {
            if (depth > 1)
                return "backgroundStone";
            return "backgroundDirt";
        }

        private void spawnBlock(int x, int y)
        {
            Ore ore = ResourceManager.GetOreByID(oreData[x, y]);
            if (mapData[x, y] == 1)
                ore = null;
            if (mapData[x, y] != 0)
                currentBlocks.Add(new Block(
                    ResourceManager.GetBlockByID(mapData[x, y]),
                    new Transform((x, -y), Vector2.One),
                    ore));
        }

        private void despawnChunk(int x, int y)
        {
            double posX = x * 16.0f;
            double posY = -y * 16.0f;
            for (var i = 0; i < currentBlocks.Count; i++)
            {
                var block = currentBlocks[i];
                if (block.Position.X >= posX
                    && block.Position.X < posX + 16.0f
                    && block.Position.Y <= posY
                    && block.Position.Y > posY - 16.0f)
                {
                    currentBlocks.Remove(block);
                    i--;
                }
            }
            for (var i = 0; i < backgrounds.Count; i++)
            {
                var bg = backgrounds[i];
                if (bg.Position.X == posX && bg.Position.Y == posY)
                {
                    backgrounds.Remove(bg);
                    i--;
                }
            }
        }

        public void Render()
        {
            drawBackgrounds();
            for (var i = 0; i < currentBlocks.Count; i++)
                currentBlocks[i].Render();
            damageParticles.Draw();
            destructionParticles.Draw();
        }

        private void drawBackgrounds()
        {
            foreach (var bg in backgrounds)
                bg.Render();
        }

        public Block GetBlockInPlayersRange(Vector2 position)
        {
            var block = getBlockAtPosition(position);
            if (block != null && isInPlayersRange(block, 2.0f))
                return block;
            return null;
        }

        private Block getBlockAtPosition(Vector2 position)
        {
            if (isOutOfBounds(position))
                return null;
            for (var i = 0; i < currentBlocks.Count; i++)
            {
                var block = currentBlocks[i];
                if (isAtLocation(block, position))
                    return block;
            }
            return null;
        }

        private bool isOutOfBounds(Vector2 position)
        {
            return position.X < 0
                || position.Y > 0
                || position.X > width
                || position.Y < -depth;
        }

        private bool isInPlayersRange(Block block, float range)
        {
            return block.DistanceToPlayer <= range;
        }

        private bool isAtLocation(Block block, Vector2 location)
        {
            return block.Position.X == location.X
                && block.Position.Y == location.Y;
        }

        public void DestoyBlockAtPosition(Block block, Vector2i position)
        {
            destructionParticles.SpawnParticles(
                                damagedBlockPosition,
                                32,
                                (0.5f, 0.5f),
                                damagedBlockColor,
                                (0.0f, 0.0f),
                                (2.0f, 2.0f),
                                true,
                                true,
                                true);
            currentBlocks.Remove(block);
            mapData[position.X, -position.Y] = 0;
        }

        public void PlaceBlockAtPosition(Block block, Vector2i position)
        {
            if (calculateDistanceToPlayer(position) > 3.0f
                || isOutOfBounds(position)
                || getBlockAtPosition(position) != null)
                return;
            mapData[position.X, -position.Y] = ResourceManager.GetBlockID(block);
            oreData[position.X, -position.Y] = 0;
            spawnBlock(position.X, -position.Y);
        }

        private float calculateDistanceToPlayer(Vector2 position)
        {
            float distance = (player.Center - (position + (0.5f, 0.5f))).Length;
            return distance;
        }

        public void playDamageParticles(Block block)
        {
            damagedBlockPosition = block.Position;
            damagedBlockColor = block.EffectsColor;
            damageParticles.SpawnParticles(
                damagedBlockPosition,
                8,
                (0.5f, 0.5f),
                damagedBlockColor,
                (0.0f, -1.0f),
                (1.0f, 1.0f),
                false,
                true,
                true);
        }
    }
}
