using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{
    public class Time
    {

        private static float deltaTime;
        private static float timeElapsed;

        private static InGameTime igt = new InGameTime();
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
            igt.SetTime(timeElapsed);

        }

        public static void GetTime()
        {
            Console.WriteLine(timeElapsed);//igt.GetHour() + " " + igt.GetMinute() + " " + igt.GetSecond()
        }

        
    }

    public class InGameTime
    {
        private int hour;
        private int minute;
        private int second;

        public InGameTime()
        {
            hour = 0;
            minute = 0;
            second = 0;
        }

        public void SetTime(float deltasum)
        {
            float delta = deltasum;
            int secondstotal = (int)delta;
            second = (int)secondstotal / (minute + 1);
            if (second%5 == 0)
            {
                minute = second % 5;
                second = 0;
            }
                
            hour = minute / 60;
        }
        public int GetHour()
        {
            return hour;
        }
        public int GetMinute()
        {
            return minute;
        }
        public int GetSecond()
        {
            return second;
        }
    }
}
