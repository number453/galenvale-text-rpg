using System;
using System.Collections.Generic;
using System.Linq;

namespace Galenvale
{
    internal static class Flow
    {
        // Make non-critical args optional so hub-only saves compile cleanly.
        private static void SaveCheckpoint(
            string hubName,
            int encounterStageIndex,
            PlayerClass playerClass,
            string playerNameUi,
            int gold,
            int potionHeal10Count,
            int potionHeal20Count,
            int potionResetCooldownCount,
            HashSet<string> ownedGearKeys,
            int enemyCritReduction = 0,
            int maxHealth = 0,
            int health = 0,
            int damage = 0,
            int accuracy = 0,
            int critChance = 0
        )
        {
            var data = new SaveData
            {
                PlayerNameUi = playerNameUi,
                PlayerClass = playerClass,

                CurrentTownId = hubName,
                EncounterStageIndex = encounterStageIndex,

                Gold = gold,

                PotionHeal10Count = potionHeal10Count,
                PotionHeal20Count = potionHeal20Count,
                PotionResetCooldownCount = potionResetCooldownCount,

                OwnedGearKeys = ownedGearKeys.ToList(),

                EnemyCritReduction = enemyCritReduction,

                MaxHealth = maxHealth,
                Health = health,
                Damage = damage,
                Accuracy = accuracy,
                CritChance = critChance
            };

            SaveSystem.Write(data);
        }

        internal static void ContinueGameAfterHartlyn(
            ref int encounterStageIndex,
            PlayerClass playerClass,
            string playerNameUi,
            ref int gold,
            ref int potionHeal10Count,
            ref int potionHeal20Count,
            ref int potionResetCooldownCount,
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
            //================================//
            // FOREST PATH: BANDIT GAUNTLETS  //
            //================================//

            if (encounterStageIndex == 1)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "HARTLYN",
                    stageTitle: "FOREST PATH - BANDIT AMBUSH",
                    stageIndex: 1,
                    enemyFactory: () => EnemyFactory.MakeBandits(1),
                    rewardGold: 50,
                    allowPotions: false,
                    hasHeal20Potion: false,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: hartlynShop,
                    hubHasPotionShop: false,
                    hubHasHeal20Potion: false
                );

                if (!won) return;
                encounterStageIndex = 2;
            }

            if (encounterStageIndex == 2)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "HARTLYN",
                    stageTitle: "FOREST PATH - BANDIT PAIR",
                    stageIndex: 2,
                    enemyFactory: () => EnemyFactory.MakeBandits(2),
                    rewardGold: 100,
                    allowPotions: false,
                    hasHeal20Potion: false,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: hartlynShop,
                    hubHasPotionShop: false,
                    hubHasHeal20Potion: false
                );

                if (!won) return;
                encounterStageIndex = 3;
            }

            if (encounterStageIndex == 3)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "HARTLYN",
                    stageTitle: "FOREST PATH - BANDIT TRIO",
                    stageIndex: 3,
                    enemyFactory: () => EnemyFactory.MakeBandits(3),
                    rewardGold: 150,
                    allowPotions: false,
                    hasHeal20Potion: false,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: hartlynShop,
                    hubHasPotionShop: false,
                    hubHasHeal20Potion: false
                );

                if (!won) return;
                encounterStageIndex = 4;
            }

            if (encounterStageIndex == 4)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "HARTLYN",
                    stageTitle: "FOREST PATH - BANDIT LEADER",
                    stageIndex: 4,
                    enemyFactory: () => new List<Enemy> { EnemyFactory.MakeBanditLeader() },
                    rewardGold: 200,
                    allowPotions: false,
                    hasHeal20Potion: false,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: hartlynShop,
                    hubHasPotionShop: false,
                    hubHasHeal20Potion: false
                );

                if (!won) return;
                encounterStageIndex = 5;
            }

            //=================================//================//
            //  GALENWOOD HUB (only when stage 5 is next)         //
            //=================================//================//
            if (encounterStageIndex == 5)
            {
                Town.VisitTown(
                    "GALENWOOD",
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
                    galenwoodShop,
                    hasPotionShop: true,
                    hasHeal20Potion: false
                );

                SaveCheckpoint(
                    hubName: "GALENWOOD",
                    encounterStageIndex: encounterStageIndex,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    gold: gold,
                    potionHeal10Count: potionHeal10Count,
                    potionHeal20Count: potionHeal20Count,
                    potionResetCooldownCount: potionResetCooldownCount,
                    ownedGearKeys: ownedGearKeys,
                    enemyCritReduction: enemyCritReduction,
                    maxHealth: maxHealth,
                    health: health,
                    damage: damage,
                    accuracy: accuracy,
                    critChance: critChance
                );
            }

            //==================================//
            // LEAVING GALENWOOD: KNIGHT STAGES //
            //==================================//
            if (encounterStageIndex == 5)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "GALENWOOD",
                    stageTitle: "ROAD TO THE ROYAL LANDS - KING'S PATROL I",
                    stageIndex: 5,
                    enemyFactory: () => new List<Enemy> { EnemyFactory.MakeKnight() },
                    rewardGold: 250,
                    allowPotions: true,
                    hasHeal20Potion: false,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: galenwoodShop,
                    hubHasPotionShop: true,
                    hubHasHeal20Potion: false
                );

                if (!won) return;
                encounterStageIndex = 6;
            }

            if (encounterStageIndex == 6)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "GALENWOOD",
                    stageTitle: "ROAD TO THE ROYAL LANDS - KING'S PATROL II",
                    stageIndex: 6,
                    enemyFactory: () => new List<Enemy>
                    {
                        EnemyFactory.MakeKnight(),
                        EnemyFactory.MakeSquire()
                    },
                    rewardGold: 300,
                    allowPotions: true,
                    hasHeal20Potion: false,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: galenwoodShop,
                    hubHasPotionShop: true,
                    hubHasHeal20Potion: false
                );

                if (!won) return;
                encounterStageIndex = 7;
            }

            if (encounterStageIndex == 7)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "GALENWOOD",
                    stageTitle: "ROAD TO THE ROYAL LANDS - KING'S PATROL III",
                    stageIndex: 7,
                    enemyFactory: () => new List<Enemy>
                    {
                        EnemyFactory.MakeKnight(),
                        EnemyFactory.MakeSquire(),
                        EnemyFactory.MakeSquire()
                    },
                    rewardGold: 350,
                    allowPotions: true,
                    hasHeal20Potion: false,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: galenwoodShop,
                    hubHasPotionShop: true,
                    hubHasHeal20Potion: false
                );

                if (!won) return;
                encounterStageIndex = 8;
            }

            if (encounterStageIndex == 8)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "GALENWOOD",
                    stageTitle: "ROAD TO THE ROYAL LANDS - LORD EDMUND",
                    stageIndex: 8,
                    enemyFactory: () =>
                    {
                        Enemy lordEdmund = new Enemy("LORD EDMUND", hp: 50, dmg: 12, acc: 60, crit: 15);
                        lordEdmund.BleedChance = 20;
                        return new List<Enemy> { lordEdmund };
                    },
                    rewardGold: 500,
                    allowPotions: true,
                    hasHeal20Potion: false,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: galenwoodShop,
                    hubHasPotionShop: true,
                    hubHasHeal20Potion: false
                );

                if (!won) return;
                encounterStageIndex = 9;
            }

            //==================================//
            // ROYAL CASTLE HUB (only when stage 9 is next)
            //==================================//
            if (encounterStageIndex == 9)
            {
                Town.VisitTown(
                    "ROYAL CASTLE",
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
                    royalShop,
                    hasPotionShop: true,
                    hasHeal20Potion: true
                );

                SaveCheckpoint(
                    hubName: "ROYAL CASTLE",
                    encounterStageIndex: encounterStageIndex,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    gold: gold,
                    potionHeal10Count: potionHeal10Count,
                    potionHeal20Count: potionHeal20Count,
                    potionResetCooldownCount: potionResetCooldownCount,
                    ownedGearKeys: ownedGearKeys,
                    enemyCritReduction: enemyCritReduction,
                    maxHealth: maxHealth,
                    health: health,
                    damage: damage,
                    accuracy: accuracy,
                    critChance: critChance
                );
            }

            //=======================================//
            // AFTER ROYAL CASTLE: KING'S CHALLENGES //
            //=======================================//
            if (encounterStageIndex == 9)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "ROYAL CASTLE",
                    stageTitle: "THE KING'S CHALLENGE I",
                    stageIndex: 9,
                    enemyFactory: () => new List<Enemy> { EnemyFactory.MakeKingsBlade() },
                    rewardGold: 650,
                    allowPotions: true,
                    hasHeal20Potion: true,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: royalShop,
                    hubHasPotionShop: true,
                    hubHasHeal20Potion: true
                );

                if (!won) return;
                encounterStageIndex = 10;
            }

            if (encounterStageIndex == 10)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "ROYAL CASTLE",
                    stageTitle: "THE KING'S CHALLENGE II",
                    stageIndex: 10,
                    enemyFactory: () => new List<Enemy> { EnemyFactory.MakeKingsFlame() },
                    rewardGold: 700,
                    allowPotions: true,
                    hasHeal20Potion: true,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: royalShop,
                    hubHasPotionShop: true,
                    hubHasHeal20Potion: true
                );

                if (!won) return;
                encounterStageIndex = 11;
            }

            if (encounterStageIndex == 11)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "ROYAL CASTLE",
                    stageTitle: "THE KING'S CHALLENGE III",
                    stageIndex: 11,
                    enemyFactory: () => new List<Enemy> { EnemyFactory.MakeKingsStrength(playerClass) },
                    rewardGold: 1000,
                    allowPotions: true,
                    hasHeal20Potion: true,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: royalShop,
                    hubHasPotionShop: true,
                    hubHasHeal20Potion: true
                );

                if (!won) return;
                encounterStageIndex = 12;
            }

            if (encounterStageIndex == 12)
            {
                bool won = RunStageWithHubReturn(
                    hubName: "ROYAL CASTLE",
                    stageTitle: "THE GREAT KING",
                    stageIndex: 12,
                    enemyFactory: () => new List<Enemy> { EnemyFactory.MakeGreatKing() },
                    rewardGold: 0,
                    allowPotions: true,
                    hasHeal20Potion: true,
                    playerClass: playerClass,
                    playerNameUi: playerNameUi,
                    ref gold,
                    ref potionHeal10Count,
                    ref potionHeal20Count,
                    ref potionResetCooldownCount,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
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
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    hubShop: royalShop,
                    hubHasPotionShop: true,
                    hubHasHeal20Potion: true
                );

                if (!won) return;
            }

            Console.WriteLine();
            Ui.Narrate("The Great King falls to one knee… then crashes to the stone.");
            Ui.Narrate("Silence spreads across the royal grounds, and the realm feels lighter.");
            Console.WriteLine();
            Ui.Narrate($"Victory, {playerNameUi}. You have beaten the game!");
            Console.WriteLine();
            Ui.Narrate("Press any key to exit...");
            Console.ReadKey();
        }

        // Returns true if won, false if died (and got sent back to hub).
        private static bool RunStageWithHubReturn(
            string hubName,
            string stageTitle,
            int stageIndex,
            Func<List<Enemy>> enemyFactory,
            int rewardGold,
            bool allowPotions,
            bool hasHeal20Potion,
            PlayerClass playerClass,
            string playerNameUi,
            ref int gold,
            ref int potionHeal10Count,
            ref int potionHeal20Count,
            ref int potionResetCooldownCount,
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
            List<GearItem> hubShop,
            bool hubHasPotionShop,
            bool hubHasHeal20Potion
        )
        {
            // Save BEFORE attempt so death reloads same stage.
            SaveCheckpoint(
                hubName: hubName,
                encounterStageIndex: stageIndex,
                playerClass: playerClass,
                playerNameUi: playerNameUi,
                gold: gold,
                potionHeal10Count: potionHeal10Count,
                potionHeal20Count: potionHeal20Count,
                potionResetCooldownCount: potionResetCooldownCount,
                ownedGearKeys: ownedGearKeys,
                enemyCritReduction: enemyCritReduction,
                maxHealth: maxHealth,
                health: health,
                damage: damage,
                accuracy: accuracy,
                critChance: critChance
            );

            List<Enemy> enemies = enemyFactory();

            bool won = Combat.RunEncounter(
                encounterTitle: stageTitle,
                enemies: enemies,
                rewardGold: rewardGold,
                allowPotions: allowPotions,
                hasHeal20Potion: hasHeal20Potion,
                playerClass: playerClass,
                playerNameUi: playerNameUi,
                gold: ref gold,
                potionHeal10Count: ref potionHeal10Count,
                potionHeal20Count: ref potionHeal20Count,
                potionResetCooldownCount: ref potionResetCooldownCount,
                health: ref health,
                maxHealth: ref maxHealth,
                damage: ref damage,
                accuracy: ref accuracy,
                critChance: ref critChance,
                enemyCritReduction: ref enemyCritReduction,
                executeCooldown: ref executeCooldown,
                shockCooldown: ref shockCooldown,
                healCooldown: ref healCooldown,
                bleedTurns: ref bleedTurns,
                burnTurns: ref burnTurns,
                weaknessTurns: ref weaknessTurns
            );

            if (!won)
            {
                Console.WriteLine();
                Ui.Narrate($"You awaken back in {hubName}.");

                health = maxHealth;
                bleedTurns = burnTurns = weaknessTurns = 0;
                executeCooldown = shockCooldown = healCooldown = 0;

                Town.VisitTown(
                    townName: hubName,
                    playerClass: playerClass,
                    gold: ref gold,
                    baseMaxHealth: baseMaxHealth,
                    baseDamage: baseDamage,
                    baseAccuracy: baseAccuracy,
                    baseCritChance: baseCritChance,
                    maxHealth: ref maxHealth,
                    health: ref health,
                    damage: ref damage,
                    accuracy: ref accuracy,
                    critChance: ref critChance,
                    enemyCritReduction: ref enemyCritReduction,
                    potionHeal10Count: ref potionHeal10Count,
                    potionHeal20Count: ref potionHeal20Count,
                    potionResetCooldownCount: ref potionResetCooldownCount,
                    ownedGearKeys: ownedGearKeys,
                    equippedBySlot: equippedBySlot,
                    gearForThisTown: hubShop,
                    hasPotionShop: hubHasPotionShop,
                    hasHeal20Potion: hubHasHeal20Potion
                );

                return false;
            }

            // WIN: reset cooldowns after winning an encounter
            executeCooldown = 0;
            shockCooldown = 0;
            healCooldown = 0;

            // Save that the NEXT stage is now unlocked (stageIndex + 1)
            SaveCheckpoint(
                hubName: hubName,
                encounterStageIndex: stageIndex + 1,
                playerClass: playerClass,
                playerNameUi: playerNameUi,
                gold: gold,
                potionHeal10Count: potionHeal10Count,
                potionHeal20Count: potionHeal20Count,
                potionResetCooldownCount: potionResetCooldownCount,
                ownedGearKeys: ownedGearKeys,
                enemyCritReduction: enemyCritReduction,
                maxHealth: maxHealth,
                health: health,
                damage: damage,
                accuracy: accuracy,
                critChance: critChance
            );

            Console.WriteLine();
            Ui.Narrate($"Victory. You may prepare again in {hubName}.");

            Town.VisitTown(
                townName: hubName,
                playerClass: playerClass,
                gold: ref gold,
                baseMaxHealth: baseMaxHealth,
                baseDamage: baseDamage,
                baseAccuracy: baseAccuracy,
                baseCritChance: baseCritChance,
                maxHealth: ref maxHealth,
                health: ref health,
                damage: ref damage,
                accuracy: ref accuracy,
                critChance: ref critChance,
                enemyCritReduction: ref enemyCritReduction,
                potionHeal10Count: ref potionHeal10Count,
                potionHeal20Count: ref potionHeal20Count,
                potionResetCooldownCount: ref potionResetCooldownCount,
                ownedGearKeys: ownedGearKeys,
                equippedBySlot: equippedBySlot,
                gearForThisTown: hubShop,
                hasPotionShop: hubHasPotionShop,
                hasHeal20Potion: hubHasHeal20Potion
            );

            return true;
        }
    }
}
