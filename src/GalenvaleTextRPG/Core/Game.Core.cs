using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace Galenvale
{
    static partial class Game
    {
        internal static readonly Random Rng = new();

        // Start contains the original Program.Main game flow (only moved, not changed).
        public static void Start()
        {
            // ============================
            // RUNTIME STATE VARIABLES
            // ============================
            string playerNameUi = "";
            PlayerClass playerClass = PlayerClass.Warrior;

            // Base stats
            int baseMaxHealth = 0;
            int baseDamage = 0;
            int baseAccuracy = 0;
            int baseCritChance = 5;

            // Current stats
            int maxHealth = 0;
            int health = 0;
            int damage = 0;
            int accuracy = 0;
            int critChance = 0;

            // Currency & inventory
            int gold = 0;
            int potionHeal10Count = 0;
            int potionHeal20Count = 0;
            int potionResetCooldownCount = 0;

            // Cooldowns
            int executeCooldown = 0;
            int shockCooldown = 0;
            int healCooldown = 0;

            // Debuffs
            int bleedTurns = 0;
            int burnTurns = 0;
            int weaknessTurns = 0;

            // Gear
            int enemyCritReduction = 0;
            var ownedGearKeys = new HashSet<string>();
            var equippedBySlot = new Dictionary<Slot, GearItem?>()
            {
                [Slot.Weapon] = null,
                [Slot.Armor] = null,
                [Slot.Helm] = null
            };

            // Shops
            List<GearItem> hartlynShop = ShopBuilder.BuildHartlynShop();
            List<GearItem> galenwoodShop = ShopBuilder.BuildGalenwoodShop();
            List<GearItem> royalShop = ShopBuilder.BuildRoyalCastleShop();

            // ============================
            // LOAD SAVE / NEW GAME PROMPT
            // ============================
            SaveData? save = null;
            bool hasSave = SaveSystem.TryLoad(out save);

            if (hasSave && save != null)
            {
                Console.WriteLine("1) Continue");
                Console.WriteLine("2) New Game");
                Console.Write("> ");
                string choice = Input.GetRawInput(allowEmpty: false);

                if (choice == "2")
                {
                    SaveSystem.DeleteSave();
                    save = null;
                    hasSave = false;
                    Console.WriteLine("Starting a NEW game...");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"Continuing from {save.CurrentTownId} (Stage {save.EncounterStageIndex})...");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("1) New Game");
                Console.Write("> ");
                _ = Input.GetRawInput(allowEmpty: false);
                Console.WriteLine();
            }
            

            if (!hasSave)
            {
                Ui.Narrate("Greetings, traveler. Welcome to the land of Galenvale!");
                Console.Write("What be yer name? ");
                string rawName = Input.GetRawInput(allowEmpty: false);
                playerNameUi = rawName.Trim().ToUpperInvariant();

                playerClass = PlayerHelpers.SelectClass(playerNameUi);

                PlayerHelpers.InitBaseStats(playerClass, out baseMaxHealth, out baseDamage, out baseAccuracy);

                maxHealth = baseMaxHealth;
                health = baseMaxHealth;
                damage = baseDamage;
                accuracy = baseAccuracy;
                critChance = baseCritChance;

                gold = 150;
                
                Ui.Narrate($"CLASS LOCKED: {playerClass.ToString().ToUpperInvariant()}");
                Console.WriteLine();


                if (playerClass == PlayerClass.Priest)
                    PlayerHelpers.PriestTutorial(ref bleedTurns, ref burnTurns, ref weaknessTurns);
            }


            // ============================
            // LOAD SAVE STATE (GEAR + STATS)
            // ============================
            if (hasSave && save != null)
            {
                playerNameUi = save.PlayerNameUi;
                playerClass = save.PlayerClass;

                PlayerHelpers.InitBaseStats(playerClass, out baseMaxHealth, out baseDamage, out baseAccuracy);

                gold = save.Gold;

                potionHeal10Count = save.PotionHeal10Count;
                potionHeal20Count = save.PotionHeal20Count;
                potionResetCooldownCount = save.PotionResetCooldownCount;

                // Restore owned gear
                ownedGearKeys.Clear();
                if (save.OwnedGearKeys != null)
                {
                    foreach (var key in save.OwnedGearKeys)
                        if (!string.IsNullOrWhiteSpace(key))
                            ownedGearKeys.Add(key);
                }

                // IMPORTANT: start from BASE stats, then recompute gear bonuses ONCE
                maxHealth = baseMaxHealth;
                health = baseMaxHealth;
                damage = baseDamage;
                accuracy = baseAccuracy;
                critChance = baseCritChance;
                enemyCritReduction = 0;

                Town.RecomputeDerivedStats(
                    playerClass,
                    baseMaxHealth,
                    baseDamage,
                    baseAccuracy,
                    baseCritChance,
                    stockAllTowns: ShopBuilder.AllGearAcrossGame(),
                    ownedGearKeys,
                    equippedBySlot,
                    ref maxHealth,
                    ref health,
                    ref damage,
                    ref accuracy,
                    ref critChance,
                    ref enemyCritReduction
                );

                // Your design: full restore on load
                health = maxHealth;
                executeCooldown = 0;
                shockCooldown = 0;
                healCooldown = 0;
                bleedTurns = 0;
                burnTurns = 0;
                weaknessTurns = 0;
            }


            // ============================
            // FIRST HUB VISIT
            // - New game: start in HARTLYN
            // - Continue: spawn into the saved town hub UI
            // ============================
            string hubTownId = (hasSave && save != null) ? save.CurrentTownId : "HARTLYN";

            List<GearItem> hubShop =
                hubTownId == "ROYAL CASTLE" ? royalShop :
                hubTownId == "GALENWOOD" ? galenwoodShop :
                hartlynShop;

            bool hubHasPotionShop = hubTownId != "HARTLYN";
            bool hubHasHeal20Potion = hubTownId == "ROYAL CASTLE";

            // ============================
            // FIRST-TIME TOWN UI WALKTHROUGH (NEW GAME ONLY)
            // ============================
            if (!hasSave)
            {
                Console.WriteLine();
                Console.WriteLine("========================================");
                Console.WriteLine("            HARTLYN - TOWN BASICS        ");
                Console.WriteLine("========================================");
                Ui.Narrate("Before you head out, learn the town commands:");
                Console.WriteLine();
                Console.WriteLine("INN        : FULL HEAL to your MAX HP.");
                Console.WriteLine("BLACKSMITH : Buy gear upgrades (stats update immediately).");
                Console.WriteLine("LEAVE      : Progress the story (this starts the next encounter).");
                Console.WriteLine();
                Ui.Narrate("Tip: If you die, you'll respawn back in town with FULL HP and can shop before retrying.");
                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
            }

            Town.VisitTown(
                hubTownId,
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
                ref potionHeal10Count,
                ref potionHeal20Count,
                ref potionResetCooldownCount,
                ownedGearKeys,
                equippedBySlot,
                hubShop,
                hasPotionShop: hubHasPotionShop,
                hasHeal20Potion: hubHasHeal20Potion
            );



            if (!hasSave || (save != null && save.EncounterStageIndex <= 0))
            {

                // ============================
                // TRAINING DUMMY (single fight)
                // ============================
                Enemy trainingDummy = new Enemy("TRAINING DUMMY", hp: 80, dmg: 2, acc: 40, crit: 10);

                Ui.Narrate($"Well met, {playerNameUi} the {playerClass.ToString().ToUpperInvariant()}!");
                Ui.Narrate("A battered training dummy stands before you in the yard.");
                Ui.Narrate("Time to practice.");

                // ============================
                // TRAINING - COMBAT CONTROLS QUICK GUIDE
                // ============================
                if (!hasSave) // only show on brand new games
                {
                    Console.WriteLine();
                    Console.WriteLine("========================================");
                    Console.WriteLine("             COMBAT QUICK GUIDE          ");
                    Console.WriteLine("========================================");
                    Console.WriteLine("ATTACK  : Basic attack (uses your turn).");
                    Console.WriteLine("ABILITY : Use your class ability.");
                    Console.WriteLine("POTION  : Use a potion (combat only).");
                    Console.WriteLine("STATUS  : See current debuffs / cooldowns.");
                    Console.WriteLine();
                    Console.WriteLine("NOTES:");
                    Console.WriteLine("- Enemies are auto-targeted (first living enemy).");
                    Console.WriteLine("- COOLDOWNS tick down each turn.");
                    Console.WriteLine("- RESET potion refreshes your ability cooldown.");
                    Console.WriteLine();
                    Console.WriteLine("Press ENTER to start the training fight...");
                    Console.ReadLine();
                }


                Combat.RunEncounter(
                    "TRAINING YARD",
                    new List<Enemy> { trainingDummy },
                    0,
                    false,
                    false,
                    playerClass,
                    playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
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

                // ✅ After training win: reset cooldowns too
                executeCooldown = 0;
                shockCooldown = 0;
                healCooldown = 0;


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
                            ref potionHeal10Count,
                            ref potionHeal20Count,
                            ref potionResetCooldownCount,
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
            }
            // ============================
            // START CAMPAIGN AFTER HARTLYN
            // ============================

            // Determine starting stage
            int encounterStageIndex = (hasSave && save != null)
                ? save.EncounterStageIndex
                : 1;

            // Run campaign flow starting from Hartlyn
            Flow.ContinueGameAfterHartlyn(
                ref encounterStageIndex,
                playerClass,
                playerNameUi,
                ref gold,
                ref potionHeal10Count,
                ref potionHeal20Count,
                ref potionResetCooldownCount,
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
