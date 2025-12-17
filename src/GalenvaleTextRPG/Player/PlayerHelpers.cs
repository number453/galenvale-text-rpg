namespace Galenvale
{
    internal static class PlayerHelpers
    {
        public static PlayerClass SelectClass(string playerNameUi)
        {
            // Placeholder: implement class selection logic
            return PlayerClass.Warrior;
        }

        public static void InitBaseStats(PlayerClass playerClass, out int baseMaxHealth, out int baseDamage, out int baseAccuracy)
        {
            // Placeholder: implement stat initialization logic
            baseMaxHealth = 100;
            baseDamage = 10;
            baseAccuracy = 50;
        }

        public static void PriestTutorial(ref int bleedTurns, ref int burnTurns, ref int weaknessTurns)
        {
            // Placeholder: implement tutorial logic
        }
    }
}