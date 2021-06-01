
namespace GamJam2k21.Sound
{
    public class Sounds
    {
        private static readonly string dir = "Data/Resources/Sound/";
        private static readonly string ext = ".wav";

        public string hitGrass = "";
        public string walkOnGrass = dir + "walkOnGrass" + ext;

        public string hitDirt = "";
        public string walkOnDirt = dir + "walkOnDirt" + ext;

        public string hitStone = "";
        public string walkOnStone = dir + "walkOnStone" + ext;

        public string walkOnLadder = dir + "walkOnLadder" + ext;
    }
}
