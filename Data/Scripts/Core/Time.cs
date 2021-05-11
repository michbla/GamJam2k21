using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{
    public class Time
    {
        private struct InGameTime
        {
            public int m;
            public int s;
            public int ms;
        };

        private static InGameTime igt;

        private static float deltaTime;
        private static float timeElapsed;


        #region Singleton
        private Time() 
        {
            timeElapsed = 0;
        }
        private static Time instance;
        public static Time GetInstance()
        {
            if (instance == null)
                instance = new Time();
            return instance;
        }
        #endregion

        public static float DeltaTime
        {
            get => deltaTime; set { deltaTime = value; }
        }
        
        public static void UpdateInGameTime()
        {
            timeElapsed += deltaTime;
            int totalseconds = (int)timeElapsed;
            int ms = (int)((timeElapsed - (float)totalseconds)* 1000);
            igt.m = (totalseconds % 3600) / 60;
            igt.s = totalseconds % 60;
            igt.ms = ms;
        }

        public static string GetTime()
        {
            string tf = "00";
            string tfms = "000";
            return igt.m.ToString(tf) + ":" + igt.s.ToString(tf) + ":" + igt.ms.ToString(tfms);
        }

        
    }


}
