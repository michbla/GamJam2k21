
namespace GamJam2k21.PlayerElements
{
    public class StaminaHandler
    {
        private Player player;

        public float Stamina = 100.0f;
        public float StaminaMax = 100.0f;

        private float staminaBase = 100.0f;
        private float staminaRegenSpeedBase = 1.0f;
        private float staminaRegenSpeed = 1.0f;
        private float staminaBurnRate = 10.0f;


        public StaminaHandler(Player player)
        {
            this.player = player;
        }

        public void Update()
        {
            if (player.IsDigging)
                staminaBurn();
            else
                staminaRegen();

            if (Stamina <= 5.0f)
                player.resetDiggingSpeed();

            updateSkills();
        }

        private void staminaBurn()
        {
            Stamina -= Time.DeltaTime * staminaBurnRate * staminaPercent(1.0f);
            if (Stamina <= 0.0f)
                Stamina = 0.0f;
        }

        private void staminaRegen()
        {
            Stamina += Time.DeltaTime * staminaRegenSpeed;
            if (Stamina >= StaminaMax)
                Stamina = StaminaMax;
        }

        private void updateSkills()
        {
            StaminaMax = staminaBase + player.Skills.EnergyPoints * 20.0f;
            staminaRegenSpeed = staminaRegenSpeedBase + player.Skills.RegenPoints + staminaPercent(5.0f);
        }

        private float staminaPercent(float percent) {
            return percent * 0.01f * StaminaMax;
        }
    }
}
