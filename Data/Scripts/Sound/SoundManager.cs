using GamJam2k21.Sound;
using IrrKlang;
using System;

namespace GamJam2k21
{
    public static class SoundManager
    {
        private static ISoundEngine effectsEngine = new ISoundEngine();
        private static ISoundEngine musicEngine = new ISoundEngine();

        private static Sounds s = new Sounds();

        public static void Update()
        {
            effectsEngine.Update();
        }

        private static void playVaried(string soundPath)
        {
            ISound sound = effectsEngine.Play2D(soundPath);
            Random r = new Random();
            float speed = 1.0f + ((float)r.NextDouble() - 0.5f) * 0.5f;
            sound.PlaybackSpeed = speed;
            effectsEngine.Update();
        }

        public static void PlayWalk(string BlockName)
        {
            if (BlockName == "None")
                return;
            if (BlockName == "Grass")
            {
                playVaried(s.walkOnGrass);
                return;
            }
            if (BlockName == "Dirt")
            {
                playVaried(s.walkOnDirt);
                return;
            }
            if (BlockName == "Ladder")
            {
                playVaried(s.walkOnLadder);
                return;
            }
            playVaried(s.walkOnStone);
        }

        public static void PlayFloat(string BlockName)
        {
            if (BlockName != "Ladder")
                return;
            playVaried(s.walkOnLadder);
            return;
        }
    }
}
