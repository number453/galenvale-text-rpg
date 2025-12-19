using System;
using System.Collections.Generic;

namespace Galenvale
{
    public sealed class SaveData
    {
        public int Version { get; set; } = 1;

        // Core identity
        public string PlayerNameUi { get; set; } = "UNKNOWN";
        public PlayerClass PlayerClass { get; set; }

        // Player stats (current)
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        public int Accuracy { get; set; }
        public int CritChance { get; set; }
        public int Gold { get; set; }

        // Consumables (stackable)
        public int PotionHeal10Count { get; set; }
        public int PotionHeal20Count { get; set; }
        public int PotionResetCooldownCount { get; set; }

        // Progress
        public string CurrentTownId { get; set; } = "HARTLYN";
        public int EncounterStageIndex { get; set; } = 0;

        // Spawn behavior (you’ve been using this idea already)
        public bool SpawnIntoEncounter { get; set; } = false;

        // Owned gear keys (THIS is what lets us rebuild stats on load)
        public List<string> OwnedGearKeys { get; set; } = new();

        public int EnemyCritReduction { get; set; }

        // Optional: last time saved
        public DateTime SavedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
