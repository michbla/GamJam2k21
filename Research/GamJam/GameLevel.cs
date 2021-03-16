using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Mathematics;

namespace GamJam
{
    /// <summary>
    /// Klasa odpowidajaca za poziom w grze
    /// </summary>
    public class GameLevel
    {
        public List<GameObject> Bricks;

        public GameLevel() {
            Bricks = new List<GameObject>();
        }
        //Ladowanie poziomu z pliku
        public void Load(string path, uint width,uint height)
        {
            Bricks.Clear();
            //uint tileCode; //Nie wiem po co to jest, bo nic nie robi
            //GameLevel level;
            List<List<uint>> tileData = new List<List<uint>>();
            string[] lines = File.ReadAllLines(path);
            foreach(string line in lines)
            {
                List<uint> row = new List<uint>();
                for (var i = 0; i < line.Length; i++)
                    row.Add((uint)line[i] - '0');
                tileData.Add(row);
            }
            if (tileData.Count > 0)
                Init(tileData, width, height);
        }
        //Rysowanie poziomu
        public void Draw(SpriteRenderer rend)
        {
            foreach (var tile in Bricks)
                if (!tile.Destroyed)
                    tile.Draw(rend);
        }
        //Walidacja wygranej
        public bool IsCompleted()
        {
            foreach (var tile in Bricks)
                if (!tile.Destroyed && !tile.IsSolid)
                    return false;
            return true;
        }
        //Inicjalizacja poziomu
        private void Init(List<List<uint>> tileData, uint width,uint height)
        {
            int _height = tileData.Count; //8
            int _width = tileData[0].Count; //15
            float unitWidth = width / (float)_width; //800/15 = 53.3
            float unitHeight = height / (float)_height; //300/8

            for(var y = 0; y < _height; y++)
            {
                for(var x = 0; x < _width; x++)
                {
                    if(tileData[y][x] == 1)
                    {
                        Vector3 pos = new Vector3(unitWidth * x - width/2, height - unitHeight * (y + 1), 0.0f);
                        Vector2 size = new Vector2(unitWidth, unitHeight);
                        GameObject obj = new GameObject(pos, size, ResourceManager.GetTexture("block_solid"),(.8f,.8f,.7f));
                        obj.IsSolid = true;
                        Bricks.Add(obj);
                    }
                    else if(tileData[y][x] > 1)
                    {
                        Vector3 color = new Vector3(1.0f);
                        if (tileData[y][x] == 2)
                            color = new Vector3(.2f, .6f, 1f);
                        else if(tileData[y][x] == 3)
                            color = new Vector3(0f, .7f, 0f);
                        else if (tileData[y][x] == 4)
                            color = new Vector3(.8f, .8f, .4f);
                        else if (tileData[y][x] == 5)
                            color = new Vector3(1f, .5f, 0f);

                        Vector3 pos = new Vector3(unitWidth * x - width/2, height - unitHeight * y - unitHeight, 0.0f);
                        Vector2 size = new Vector2(unitWidth, unitHeight);
                        Bricks.Add(new GameObject(pos, size, ResourceManager.GetTexture("block"), color));
                    }
                }
            }
        }
    }
}
