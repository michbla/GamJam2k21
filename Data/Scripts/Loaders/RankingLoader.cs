using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace GamJam2k21.Loaders
{
    class RankingLoader
    {
        public List<ScoreJson> Scores;
        public class ScoreJson
        {
            public Score score { get; set; }
        }

        public class Score
        {
            public int RunId { get; set; }
            public string RunDate { get; set; }
            public string RunTime { get; set; }
            public float RunExp { get; set; }
        }

        public void AddToRanking(string date, string time, float exp)
        {
            int id = Scores.Count + 1;
            ScoreJson score = new ScoreJson();
            score.score.RunId = id;
            score.score.RunDate = date;
            score.score.RunTime = time;
            score.score.RunExp = exp;
            Scores.Add(score);
        }

        public void SaveRanking()
        {
            string json = JsonSerializer.Serialize(Scores);
            File.WriteAllText("Data/Instances/ranking.json", json);
        }

        public void LoadRanking()
        {
            string json = File.ReadAllText("Data/Instances/ranking.json");
            Scores = JsonSerializer.Deserialize<List<ScoreJson>>(json);
        }


    }
}
