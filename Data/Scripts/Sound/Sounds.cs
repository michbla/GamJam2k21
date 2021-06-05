
namespace GamJam2k21.Sound
{
    public class Sounds
    {
        private static readonly string dir = "Data/Resources/Sound/";
        private static readonly string ext = ".wav";

        public string background = dir + "background" + ext;

        public string hitGrass = dir + "hitGrass" + ext;
        public string destGrass = dir + "destGrass" + ext;
        public string walkOnGrass = dir + "walkOnGrass" + ext;

        public string hitDirt = dir + "hitDirt" + ext;
        public string destDirt = dir + "destDirt" + ext;
        public string walkOnDirt = dir + "walkOnDirt" + ext;

        public string hitStone = dir + "hitStone" + ext;
        public string destStone = dir + "destStone" + ext;
        public string walkOnStone = dir + "walkOnStone" + ext;

        public string walkOnLadder = dir + "walkOnLadder" + ext;
        public string placeLadder = dir + "placeLadder" + ext;

        public string explosion = dir + "explosion" + ext;

        public string click = dir + "clickButton" + ext;
        public string unclick = dir + "unclickButton" + ext;

        public string levelUp = dir + "levelUp" + ext;

        public string coins = dir + "coins" + ext;
    }
}
