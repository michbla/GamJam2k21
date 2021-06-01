using System.Media;
using GamJam2k21.Sound;
using System.Threading;
using System.Threading.Tasks;
using IrrKlang;

namespace GamJam2k21
{
    public static class SoundManager
    {
        private static ISoundEngine engine = new ISoundEngine();

        private static Sounds s = new Sounds();

        public static void PlayWalk(string BlockName)
        {
            if (BlockName == "None")
                return;
            if (BlockName == "Grass")
            {
                engine.Play2D(s.walkOnGrass);
                return;
            }
            if (BlockName == "Dirt")
            {
                engine.Play2D(s.walkOnDirt);
                return;
            }
            if (BlockName == "Ladder")
            {
                engine.Play2D(s.walkOnLadder);
                return;
            }

            engine.Play2D(s.walkOnStone);
        }

        public static void PlayFloat(string BlockName)
        {
            if (BlockName != "Ladder")
                return;
            engine.Play2D(s.walkOnLadder);
            return;
        }
    }
}
