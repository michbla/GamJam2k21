using GamJam2k21.Sound;
using IrrKlang;
using System;

namespace GamJam2k21
{
    public static class SoundManager
    {
        private static ISoundEngine engine = new ISoundEngine();
        private static ISoundEngine backgroundEngine = new ISoundEngine();

        private static Sounds s = new Sounds();

        public static void Update()
        {
            engine.Update();
        }
        
        public static void StartBackgroundMusic()
        {
            backgroundEngine.SoundVolume = 0.3f;
            backgroundEngine.Play2D(s.background, true);
        }
        public static void StopBackgroundMusic()
        {
            backgroundEngine.StopAllSounds();
        }


        private static void playVaried(string soundPath)
        {
            ISound sound = engine.Play2D(soundPath);
            Random r = new Random();
            float speed = 1.0f + ((float)r.NextDouble() - 0.5f) * 0.5f;
            sound.PlaybackSpeed = speed;
            engine.Update();
        }

        private static void play(string soundPath)
        {
            engine.Play2D(soundPath);
            engine.Update();
        }

        public static void PlayWalk(string BlockName)
        {
            switch (BlockName)
            {
                case "Name":
                    break;
                case "Grass":
                    playVaried(s.walkOnGrass);
                    break;
                case "Dirt":
                    playVaried(s.walkOnDirt);
                    break;
                case "Ladder":
                    playVaried(s.walkOnLadder);
                    break;
                default:
                    playVaried(s.walkOnStone);
                    break;
            }
        }

        public static void PlayFloat(string BlockName)
        {
            if (BlockName != "Ladder")
                return;
            playVaried(s.walkOnLadder);
            return;
        }

        public static void PlayHit(string BlockName)
        {
            switch (BlockName)
            {
                case "Name":
                    break;
                case "Grass":
                    playVaried(s.hitGrass);
                    break;
                case "Dirt":
                    playVaried(s.hitDirt);
                    break;
                default:
                    playVaried(s.hitStone);
                    break;
            }
        }

        public static void PlayDest(string BlockName)
        {
            switch (BlockName)
            {
                case "Name":
                    break;
                case "Grass":
                    playVaried(s.destGrass);
                    break;
                case "Dirt":
                    playVaried(s.destDirt);
                    break;
                default:
                    playVaried(s.destStone);
                    break;
            }
        }

        public static void PlayPlaceLadder()
        {
            playVaried(s.placeLadder);
        }

        public static void PlayExplosion()
        {
            playVaried(s.explosion);
        }

        public static void PlayLevelUp()
        {
            play(s.levelUp);
        }

        public static void PlayCoins()
        {
            play(s.coins);
        }

        public static void PlayButtonClick()
        {
            play(s.click);
        }

        public static void PlayButtonUnClick()
        {
            play(s.unclick);
        }
    }
}
