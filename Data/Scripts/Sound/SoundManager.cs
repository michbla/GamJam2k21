using System.Media;
using GamJam2k21.Sound;
using System.Threading;
using System.Threading.Tasks;

namespace GamJam2k21
{
    public static class SoundManager
    {
        private static Sounds s = new Sounds();

        private static SoundPlayer walkOnGrass = new SoundPlayer(s.walkOnGrass);
        private static SoundPlayer walkOnDirt = new SoundPlayer(s.walkOnDirt);
        private static SoundPlayer walkOnStone = new SoundPlayer(s.walkOnStone);
        private static SoundPlayer walkOnLadder = new SoundPlayer(s.walkOnLadder);

        private static bool isPlayingWalkSound = false;

        public static void Init()
        {
            walkOnGrass.LoadAsync();
            walkOnDirt.LoadAsync();
            walkOnStone.LoadAsync();
            walkOnLadder.LoadAsync();
        }

        private static void playWalkOnThread(SoundPlayer s)
        {
            if (isPlayingWalkSound)
                return;
            isPlayingWalkSound = true;
            Thread soundThread = new Thread(new ParameterizedThreadStart(play));
            soundThread.Start(s);

            Task.Run(() =>
            {
                soundThread.Join();
                isPlayingWalkSound = false;
            });
        }

        private static void play(object s)
        {
            SoundPlayer p = (SoundPlayer)s;
            p.PlaySync();
        }

        public static void PlayWalk(string BlockName)
        {
            if (BlockName == "None")
                return;
            if (BlockName == "Grass")
            {
                playWalkOnThread(walkOnGrass);
                return;
            }
            if (BlockName == "Dirt")
            {
                playWalkOnThread(walkOnDirt);
                return;
            }
            if (BlockName == "Ladder")
            {
                playWalkOnThread(walkOnLadder);
                return;
            }

            playWalkOnThread(walkOnStone);
        }

        public static void PlayFloat(string BlockName)
        {
            if (BlockName != "Ladder")
                return;
            playWalkOnThread(walkOnLadder);
            return;
        }
    }
}
