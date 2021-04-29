using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21
{
    public class Time
    {
        private static float deltaTime;

        #region Singleton
        private Time() { }
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
    }
}
