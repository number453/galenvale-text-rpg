namespace Galenvale
{
    static class Flow
    {
        public static void ContinueGameAfterForest(
            PlayerClass playerClass,
            string playerNameUi,
            ref int gold,
            int baseMaxHealth,
            int baseDamage,
            int baseAccuracy,
            int baseCritChance,
            ref int maxHealth,
            ref int health,
            ref int damage,
            ref int accuracy,
            ref int critChance,
            ref int enemyCritReduction,
            ref int executeCooldown,
            ref int shockCooldown,
            ref int healCooldown,
            ref int bleedTurns,
            ref int burnTurns,
            ref int weaknessTurns,
            HashSet<string> ownedGearKeys,
            Dictionary<Slot, GearItem?> equippedBySlot,
            List<GearItem> hartlynShop,
            List<GearItem> galenwoodShop,
            List<GearItem> royalShop
        )
        {
            // Implementation goes here
        }
    }
}