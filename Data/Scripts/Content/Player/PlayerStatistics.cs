
namespace GamJam2k21.PlayerElements
{
    public class PlayerStatistics
    {
        private Player player;

        private int levelReached = 0;

        private readonly int maxLevel = 40;
        public int ExpLevel = 0;
        public float Exp = 0;
        public float ExpToNextLevel = 200;

        private static readonly float nextLevelExpMultiplier = 1.75f;

        public PlayerStatistics(Player player)
        {
            this.player = player;
        }

        public void AddExp(float value)
        {
            if (ExpLevel == maxLevel)
                return;
            Exp += value;
            checkIfNextLevel();
        }

        private void checkIfNextLevel()
        {
            while(Exp >= ExpToNextLevel)
            {
                Exp -= ExpToNextLevel;
                ExpLevel++;
                ExpToNextLevel = calculateExpToNextLevel();
                player.AddAttributePoint();
                SoundManager.PlayLevelUp();
            }
        }

        private float calculateExpToNextLevel()
        {
            return ExpToNextLevel * nextLevelExpMultiplier;
        }

        public void SetLevelReached(int newLevel)
        {
            levelReached = newLevel; 
        }

        public int getLevelReached()
        {
            return levelReached;
        }
    }
}
