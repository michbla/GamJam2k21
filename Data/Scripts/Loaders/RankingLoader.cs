using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GamJam2k21
{
    class RankingLoader
    {
        public class Score
        {
            public int RunId { get; set; }
            public string RunTime { get; set; }
            public float RunExp { get; set; }
        }

        public List<Score> Scores = new List<Score>();

        public int AddToRanking(string time, float exp)
        {
            int index = Scores.FindIndex(x => x.RunTime == time);
            if (index == -1)
            {
                int id = Scores.Count + 1;
                Score score = new Score();
                score.RunId = id;
                score.RunTime = time;
                score.RunExp = exp;
                Scores.Add(score);
                Scores.Sort(delegate (Score x, Score y)
                {
                    if (x.RunTime == null && y.RunTime == null) return 0;
                    else if (x.RunTime == null) return -1;
                    else if (y.RunTime == null) return 1;
                    else return x.RunTime.CompareTo(y.RunTime);
                });
                index = Scores.FindIndex(x => x.RunTime == time);
            }
            return index;
        }

        public string GetRank(int place)
        {
            string result = Scores[place].RunTime + " " + Scores[place].RunExp;
            return result;
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
