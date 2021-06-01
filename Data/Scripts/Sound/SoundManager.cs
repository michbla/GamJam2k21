using System.Media;
using GamJam2k21.Sound;

namespace GamJam2k21
{
    public static class SoundManager
    {
        private static Sounds s = new Sounds();

        private static SoundPlayer walkOnGrass = new SoundPlayer(s.walkOnGrass);
        private static SoundPlayer walkOnDirt = new SoundPlayer(s.walkOnDirt);
        private static SoundPlayer walkOnStone = new SoundPlayer(s.walkOnStone);
        private static SoundPlayer walkOnLadder = new SoundPlayer(s.walkOnLadder);

        private static void play(SoundPlayer s)
        {
            s.Stop();
            s.LoadAsync();
            s.Play();
        }

        public static void PlayWalk(string BlockName)
        {
            if (BlockName == "None")
                return;
            if (BlockName == "Grass")
            {
                play(walkOnGrass);
                return;
            }
            if (BlockName == "Dirt")
            {
                play(walkOnDirt);
                return;
            }
            if (BlockName == "Ladder")
            {
                play(walkOnLadder);
                return;
            }

            play(walkOnStone);
        }

        public static void PlayFloat(string BlockName)
        {
            if (BlockName != "Ladder")
                return;
            play(walkOnLadder);
            return;
        }
    }
}
