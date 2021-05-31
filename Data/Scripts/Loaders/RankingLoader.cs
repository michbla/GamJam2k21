using System.Collections.Generic;
using System.IO;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    class RankingLoader
    {
        public class Score
        {
            public int RunId { get; set; }
            public string RunDate { get; set; }
            public string RunTime { get; set; }
            public float RunExp { get; set; }
        }
        public List<Score> Scores = new List<Score>();

        public void AddToRanking(string time, float exp)
        {
            int id = Scores.Count + 1;
            Score score = new Score();
            score.RunId = id;
            score.RunDate = DateTime.Now.ToString();
            score.RunTime = time;
            score.RunExp = exp;
            Scores.Add(score);
        }

        public void PrintRanking()
        {
            foreach (Score e in Scores)
            {
                Console.WriteLine("run " + e.RunId + ": " + e.RunDate + " " + e.RunTime + " " + e.RunExp);
            }
        }

        public void SaveRanking()
        {
            string json = JsonSerializer.Serialize(Scores);
            File.WriteAllText("Data/Instances/ranking.json", json);
        }

        public void LoadRanking()
        {
            string json = File.ReadAllText("Data/Instances/ranking.json");
            Scores = JsonSerializer.Deserialize<List<Score>>(json);
        }


    }
}
