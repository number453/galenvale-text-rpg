using System;
using System.Collections.Generic;
using Galenvale;

namespace Galenvale
{
    static partial class Game
    {
        internal static readonly Random Rng = new();

        // Start contains the original Program.Main game flow (only moved, not changed).
        public static void Start()
        {
            // ============================
            // INTRO / CHARACTER CREATION
            // ============================
            Ui.Narrate("Greetings, traveler. Welcome to the land of Galenvale!");
            Console.Write("What be yer name? ");
            string rawName = Input.GetRawInput(allowEmpty: false);
            string playerNameUi = rawName.Trim().ToUpperInvariant();

            PlayerClass playerClass = PlayerHelpers.SelectClass(playerNameUi);

            // Base stats (these are the ONLY numbers you should edit when balancing classes)
            int baseMaxHealth = 0;
            int baseDamage = 0;
            int baseAccuracy = 0;

            PlayerHelpers.InitBaseStats(playerClass, out baseMaxHealth, out baseDamage, out baseAccuracy);

            // Dynamic stats (derived from base + best owned gear)
            int maxHealth = baseMaxHealth;
            int health = baseMaxHealth;
            int damage = baseDamage;
            int accuracy = baseAccuracy;

            int baseCritChance = 5; // global base crit
            int critChance = baseCritChance;

            int gold = 150;

            // Cooldowns (0 = ready)
            int executeCooldown = 0; // warrior
            int shockCooldown = 0;   // mage
            int healCooldown = 0;    // priest

            // Player debuffs (turn counters; 0 = not active)
            int bleedTurns = 0;
            int burnTurns = 0;
            int weaknessTurns = 0;

            // Gear ownership / best-equip logic
            var ownedGearKeys = new HashSet<string>();
            var equippedBySlot = new Dictionary<Slot, GearItem?>()
            {
                [Slot.Weapon] = null,
                [Slot.Armor] = null,
                [Slot.Helm] = null
            };

            // Enemy crit reduction from helm gear (applies against ALL enemies)
            int enemyCritReduction = 0;

            // Shops (gear pools)
            List<GearItem> hartlynShop = ShopBuilder.BuildHartlynShop();
            List<GearItem> galenwoodShop = ShopBuilder.BuildGalenwoodShop();
            List<GearItem> royalShop = ShopBuilder.BuildRoyalCastleShop();

            // Priest tutorial
            if (playerClass == PlayerClass.Priest)
                PlayerHelpers.PriestTutorial(ref bleedTurns, ref burnTurns, ref weaknessTurns);

            // First town: Hartlyn
            Town.VisitTown(
                "HARTLYN",
                playerClass,
                ref gold,
                baseMaxHealth,
                baseDamage,
                baseAccuracy,
                baseCritChance,
                ref maxHealth,
                ref health,
                ref damage,
                ref accuracy,
                ref critChance,
                ref enemyCritReduction,
                ownedGearKeys,
                equippedBySlot,
                hartlynShop,
                false,
                false
            );

            // ============================
            // TRAINING DUMMY (single fight)
            // ============================
            Enemy trainingDummy = new Enemy("TRAINING DUMMY", hp: 80, dmg: 2, acc: 40, crit: 10);

            Ui.Narrate($"Well met, {playerNameUi} the {playerClass.ToString().ToUpperInvariant()}!");
            Ui.Narrate("A battered training dummy stands before you in the yard.");
            Ui.Narrate("Time to practice.");

            Combat.RunEncounter(
                "TRAINING YARD",
                new List<Enemy> { trainingDummy },
                0,
                false,
                false,
                playerClass,
                playerNameUi,
                ref gold,
                ref health,
                ref maxHealth,
                ref damage,
                ref accuracy,
                ref critChance,
                ref enemyCritReduction,
                ref executeCooldown,
                ref shockCooldown,
                ref healCooldown,
                ref bleedTurns,
                ref burnTurns,
                ref weaknessTurns
            );

            // After dummy: go back Hartlyn or take Forest Path
            while (true)
            {
                Console.WriteLine();
                Ui.Narrate("With the yard behind you, you can return to Hartlyn or take the Forest Path.");
                Console.Write("Type: HARTLYN | FOREST PATH: ");
                string choice = Input.GetNormalizedCommand();

                if (choice == "HARTLYN")
                {
                    Town.VisitTown(
                        "HARTLYN",
                        playerClass,
                        ref gold,
                        baseMaxHealth,
                        baseDamage,
                        baseAccuracy,
                        baseCritChance,
                        ref maxHealth,
                        ref health,
                        ref damage,
                        ref accuracy,
                        ref critChance,
                        ref enemyCritReduction,
                        ownedGearKeys,
                        equippedBySlot,
                        hartlynShop,
                        hasPotionShop: false,
                        hasHeal20Potion: false
                    );
                    // once you leave Hartlyn from now on, you automatically take the Forest Path
                    break;
                }
                if (choice == "FOREST PATH" || choice == "FORESTPATH")
                {
                    break;
                }

                Ui.Narrate("That path is not available. Try again.");
            }

            // The rest of the flow remains unchanged - delegate to partials that implement stages, towns and final fights.
            // For brevity, invoke a helper that continues the scripted progression (keeps logic identical).
            Flow.ContinueGameAfterForest(
                playerClass,
                playerNameUi,
                ref gold,
                baseMaxHealth,
                baseDamage,
                baseAccuracy,
                baseCritChance,
                ref maxHealth,
                ref health,
                ref damage,
                ref accuracy,
                ref critChance,
                ref enemyCritReduction,
                ref executeCooldown,
                ref shockCooldown,
                ref healCooldown,
                ref bleedTurns,
                ref burnTurns,
                ref weaknessTurns,
                ownedGearKeys,
                equippedBySlot,
                hartlynShop,
                galenwoodShop,
                royalShop
            );
        }
    }
}