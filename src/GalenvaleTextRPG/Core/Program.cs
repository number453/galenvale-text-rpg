using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static readonly Random Rng = new();

    enum PlayerClass { Warrior, Mage, Priest }
    enum Slot { Weapon, Armor, Helm }

    enum PotionType
    {
        Heal10,
        Heal20,
        ResetCooldown
    }

    sealed class GearItem
    {
        public string Key;          // internal id (no spaces)
        public string Name;         // UI name (ALL CAPS WITH SPACES)
        public Slot Slot;
        public int Tier;            // higher = better
        public int Price;
        public HashSet<PlayerClass> Allowed;
        public int MaxHpBonus;
        public int DamageBonus;
        public int CritBonus;
        public int EnemyCritReductionBonus;

        public GearItem(
            string key,
            string name,
            Slot slot,
            int tier,
            int price,
            IEnumerable<PlayerClass> allowed,
            int maxHpBonus = 0,
            int damageBonus = 0,
            int critBonus = 0,
            int enemyCritReductionBonus = 0)
        {
            Key = key;
            Name = name;
            Slot = slot;
            Tier = tier;
            Price = price;
            Allowed = new HashSet<PlayerClass>(allowed);
            MaxHpBonus = maxHpBonus;
            DamageBonus = damageBonus;
            CritBonus = critBonus;
            EnemyCritReductionBonus = enemyCritReductionBonus;
        }
    }

    sealed class Enemy
    {
        public string Name;
        public int MaxHp;
        public int Hp;
        public int Damage;
        public int Accuracy;
        public int CritChance;

        // status procs (on HIT)
        public int BleedChance;  // % chance to apply +1 bleed turns
        public int BurnChance;   // % chance to apply +1 burn turns
        public int WeaknessChance; // % chance to apply weakness turns (handled via flag rules)

        // special ability: GOD SMACK
        public bool HasGodSmack;
        public int GodSmackCooldown;     // fixed cooldown length
        public int GodSmackCooldownLeft; // countdown

        public Enemy(string name, int hp, int dmg, int acc, int crit = 0)
        {
            Name = name;
            MaxHp = hp;
            Hp = hp;
            Damage = dmg;
            Accuracy = acc;
            CritChance = crit;

            BleedChance = 0;
            BurnChance = 0;
            WeaknessChance = 0;

            HasGodSmack = false;
            GodSmackCooldown = 0;
            GodSmackCooldownLeft = 0;
        }

        public bool Alive => Hp > 0;
    }

    static void Main()
    {
        // ============================
        // INTRO / CHARACTER CREATION
        // ============================
        Narrate("Greetings, traveler. Welcome to the land of Galenvale!");
        Console.Write("What be yer name? ");
        string rawName = GetRawInput(allowEmpty: false);
        string playerNameUi = rawName.Trim().ToUpperInvariant();

        PlayerClass playerClass = SelectClass(playerNameUi);

        // Base stats (these are the ONLY numbers you should edit when balancing classes)
        int baseMaxHealth = 0;
        int baseDamage = 0;
        int baseAccuracy = 0;

        InitBaseStats(playerClass, out baseMaxHealth, out baseDamage, out baseAccuracy);

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
        List<GearItem> hartlynShop = BuildHartlynShop();
        List<GearItem> galenwoodShop = BuildGalenwoodShop();
        List<GearItem> royalShop = BuildRoyalCastleShop();

        // Priest tutorial
        if (playerClass == PlayerClass.Priest)
            PriestTutorial(ref bleedTurns, ref burnTurns, ref weaknessTurns);

        // First town: Hartlyn
        VisitTown(
            townName: "HARTLYN",
            playerClass: playerClass,
            ref gold,
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
            ownedGearKeys: ownedGearKeys,
            equippedBySlot: equippedBySlot,
            gearForThisTown: hartlynShop,
            hasPotionShop: false,
            hasHeal20Potion: false
        );

        // ============================
        // TRAINING DUMMY (single fight)
        // ============================
        Enemy trainingDummy = new Enemy("TRAINING DUMMY", hp: 80, dmg: 2, acc: 40, crit: 10);

        Narrate($"Well met, {playerNameUi} the {playerClass.ToString().ToUpperInvariant()}!");
        Narrate("A battered training dummy stands before you in the yard.");
        Narrate("Time to practice.");

        RunEncounter(
            encounterTitle: "TRAINING YARD",
            enemies: new List<Enemy> { trainingDummy },
            rewardGold: 0,
            allowPotions: false,
            hasHeal20Potion: false,
            playerClass: playerClass,
            playerNameUi: playerNameUi,
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
            Narrate("With the yard behind you, you can return to Hartlyn or take the Forest Path.");
            Console.Write("Type: HARTLYN | FOREST PATH: ");
            string choice = GetNormalizedCommand();

            if (choice == "HARTLYN")
            {
                VisitTown(
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

            Narrate("That path is not available. Try again.");
        }

        // ============================
        // FOREST PATH: BANDIT GAUNTLETS
        // ============================
        // Stage 1: 1 bandit (20 HP, 7 DMG)
        RunStageWithHubReturn(
            hubName: "HARTLYN",
            stageTitle: "FOREST PATH - BANDIT AMBUSH",
            enemies: MakeBandits(1),
            rewardGold: 50,
            allowPotions: false,
            hasHeal20Potion: false,
            playerClass: playerClass,
            playerNameUi: playerNameUi,
            ref gold,
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

        // Stage 2: 2 bandits
        RunStageWithHubReturn(
            "HARTLYN",
            "FOREST PATH - BANDIT PAIR",
            MakeBandits(2),
            rewardGold: 100,
            allowPotions: false,
            hasHeal20Potion: false,
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
            hubHasPotionShop: false,
            hubHasHeal20Potion: false
        );

        // Stage 3: 3 bandits
        RunStageWithHubReturn(
            "HARTLYN",
            "FOREST PATH - BANDIT TRIO",
            MakeBandits(3),
            rewardGold: 150,
            allowPotions: false,
            hasHeal20Potion: false,
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
            hubHasPotionShop: false,
            hubHasHeal20Potion: false
        );

        // Reach Galenwood (new town) with potion shop + better gear
        VisitTown(
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
            ownedGearKeys: ownedGearKeys,
            equippedBySlot: equippedBySlot,
            gearForThisTown: galenwoodShop,
            true,
            false
        );

        // ============================
        // LEAVING GALENWOOD: KNIGHT STAGES
        // ============================
        // Stage 1: Single knight 30 HP, 9 DMG, reward 200
        RunStageWithHubReturn(
            "GALENWOOD",
            "ROAD TO THE ROYAL LANDS - KING'S PATROL I",
            new List<Enemy> { MakeKnight("KNIGHT", 30, 9, 55, 10) },
            rewardGold: 200,
            allowPotions: true,
            hasHeal20Potion: false,
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
            galenwoodShop,
            hubHasPotionShop: true,
            hubHasHeal20Potion: false
        );

        // Stage 2: 1 knight + 1 squire (squire 25 HP, 6 DMG), reward 250
        RunStageWithHubReturn(
            "GALENWOOD",
            "ROAD TO THE ROYAL LANDS - KING'S PATROL II",
            new List<Enemy>
            {
                MakeKnight("KNIGHT", 30, 9, 55, 10),
                MakeSquire("SQUIRE", 25, 6, 55, 5)
            },
            rewardGold: 250,
            allowPotions: true,
            hasHeal20Potion: false,
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
            galenwoodShop,
            hubHasPotionShop: true,
            hubHasHeal20Potion: false
        );

        // Stage 3: 1 knight + 2 squires, reward 300
        RunStageWithHubReturn(
            "GALENWOOD",
            "ROAD TO THE ROYAL LANDS - KING'S PATROL III",
            new List<Enemy>
            {
                MakeKnight("KNIGHT", 30, 9, 55, 10),
                MakeSquire("SQUIRE", 25, 6, 55, 5),
                MakeSquire("SQUIRE", 25, 6, 55, 5)
            },
            rewardGold: 300,
            allowPotions: true,
            hasHeal20Potion: false,
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
            galenwoodShop,
            hubHasPotionShop: true,
            hubHasHeal20Potion: false
        );

        // Final stage out of Galenwood: Lord Edmund (50 HP, 12 DMG, bleed 20% on hit), reward 500
        {
            Enemy lordEdmund = new Enemy("LORD EDMUND", hp: 50, dmg: 12, acc: 55, crit: 10);
            lordEdmund.BleedChance = 20;

            RunStageWithHubReturn(
                "GALENWOOD",
                "ROAD TO THE ROYAL LANDS - LORD EDMUND",
                new List<Enemy> { lordEdmund },
                rewardGold: 500,
                allowPotions: true,
                hasHeal20Potion: false,
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
                galenwoodShop,
                hubHasPotionShop: true,
                hubHasHeal20Potion: false
            );
        }

        // ============================
        // ROYAL CASTLE HUB
        // ============================
        VisitTown(
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
            ownedGearKeys: ownedGearKeys,
            equippedBySlot: equippedBySlot,
            gearForThisTown: royalShop,
            true,
            true
        );

        // ============================
        // AFTER LEAVING ROYAL CASTLE: 4 FIGHTS
        // ============================
        // 1) KING'S FIRST BLADE
        {
            Enemy e = new Enemy("KING'S FIRST BLADE", hp: 60, dmg: 14, acc: 60, crit: 0);
            e.BleedChance = 50;

            RunStageWithHubReturn(
                "ROYAL CASTLE",
                "THE KING'S CHALLENGE I",
                new List<Enemy> { e },
                rewardGold: 650,
                allowPotions: true,
                hasHeal20Potion: true,
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
                royalShop,
                hubHasPotionShop: true,
                hubHasHeal20Potion: true
            );
        }

        // 2) KING'S TWIN FLAME
        {
            Enemy e = new Enemy("KING'S TWIN FLAME", hp: 50, dmg: 17, acc: 55, crit: 10);
            e.BurnChance = 75;

            RunStageWithHubReturn(
                "ROYAL CASTLE",
                "THE KING'S CHALLENGE II",
                new List<Enemy> { e },
                rewardGold: 700,
                allowPotions: true,
                hasHeal20Potion: true,
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
                royalShop,
                hubHasPotionShop: true,
                hubHasHeal20Potion: true
            );
        }

        // 3) KING'S STRENGTH
        {
            Enemy e = new Enemy("KING'S STRENGTH", hp: 85, dmg: 15, acc: 50, crit: 50);
            // 100% weakness on HIT unless player is priest
            e.WeaknessChance = 100;

            RunStageWithHubReturn(
                "ROYAL CASTLE",
                "THE KING'S CHALLENGE III",
                new List<Enemy> { e },
                rewardGold: 1000,
                allowPotions: true,
                hasHeal20Potion: true,
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
                royalShop,
                hubHasPotionShop: true,
                hubHasHeal20Potion: true
            );
        }

        // 4) GREAT KING (final)
        {
            Enemy e = new Enemy("GREAT KING", hp: 100, dmg: 20, acc: 44, crit: 44);
            e.HasGodSmack = true;
            e.GodSmackCooldown = 4;
            e.GodSmackCooldownLeft = 0;

            RunEncounter(
                "THE GREAT KING",
                new List<Enemy> { e },
                0, //  specify gold reward here; leaving as 0
                true,
                true,
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

            Console.WriteLine();
            Narrate("The Great King falls to one knee… then crashes to the stone.");
            Narrate("Silence spreads across the royal grounds, and the realm feels lighter.");
            Console.WriteLine();
            Narrate($"Victory, {playerNameUi}. You have beaten the game!");
            Console.WriteLine();
            Narrate("Press any key to exit...");
            Console.ReadKey();
            return;
        }
    }

    // =========================================================
    // CORE ENCOUNTER LOOP (GAUNTLET STYLE, ONE ENEMY ATTACKS)
    // =========================================================
    static void RunEncounter(
        string encounterTitle,
        List<Enemy> enemies,
        int rewardGold,
        bool allowPotions,
        bool hasHeal20Potion,
        PlayerClass playerClass,
        string playerNameUi,
        ref int gold,
        ref int health,
        ref int maxHealth,
        ref int damage,
        ref int accuracy,
        ref int critChance,
        ref int enemyCritReduction,
        ref int executeCooldown,
        ref int shockCooldown,
        ref int healCooldown,
        ref int bleedTurns,
        ref int burnTurns,
        ref int weaknessTurns
    )
    {
        int turn = 1;

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"=== {encounterTitle} ===");
            Console.WriteLine($"-- Turn {turn} --");

            PrintPlayerSheet(playerNameUi, playerClass, health, maxHealth, damage, accuracy, critChance, gold);
            Console.WriteLine();
            PrintEnemySheet(enemies, enemyCritReduction);

            if (health <= 0)
            {
                Console.WriteLine();
                Narrate("You collapse. The world fades.");
                Narrate("Game Over.");
                Environment.Exit(0);
            }

            if (enemies.All(e => !e.Alive))
            {
                Console.WriteLine();
                Narrate("All foes have fallen.");
                if (rewardGold > 0)
                {
                    gold += rewardGold;
                    Narrate($"You gain {rewardGold} gold.");
                }
                return;
            }

            PrintDebuffs(bleedTurns, burnTurns, weaknessTurns);
            PrintCooldowns(playerClass, executeCooldown, shockCooldown, healCooldown);

            Console.WriteLine();
            Console.Write("Choose an action: ATTACK | ABILITY | WAIT");
            if (allowPotions) Console.Write(" | POTION");
            Console.Write(" | QUIT\n> ");

            string input = GetNormalizedCommand();

            if (input == "QUIT") Environment.Exit(0);

            bool usedTurn = false;

            if (input == "ATTACK")
            {
                usedTurn = PlayerAttackGauntlet(enemies, damage, accuracy, critChance, weaknessTurns);
            }
            else if (input == "ABILITY")
            {
                if (playerClass == PlayerClass.Warrior)
                    usedTurn = WarriorExecuteGauntlet(enemies, damage, accuracy, ref executeCooldown);
                else if (playerClass == PlayerClass.Mage)
                {
                    MageShockGauntlet(Rng, damage, ref shockCooldown, enemies, procChancePerEnemy: 40);
                    usedTurn = false; // Shock does NOT use a turn
                }
                else
                {
                    usedTurn = PriestHeal(ref healCooldown, ref health, maxHealth, ref bleedTurns, ref burnTurns, ref weaknessTurns);
                }
            }
            else if (input == "POTION" && allowPotions)
            {
                usedTurn = UsePotionInCombat(
                    playerClass,
                    ref gold,
                    ref health,
                    maxHealth,
                    ref executeCooldown,
                    ref shockCooldown,
                    ref healCooldown,
                    hasHeal20Potion
                );
            }
            else if (input == "WAIT")
            {
                Narrate("You wait cautiously...");
                usedTurn = true;
            }
            else
            {
                Narrate("I don't understand that.");
                continue;
            }

            // If Shock (or invalid) didn't use a turn, skip enemy phase + cooldown tick
            if (!usedTurn)
                continue;

            // Enemy phase: ONLY ONE enemy attacks per turn (first alive)
            int attackerIndex = FirstAliveEnemyIndex(enemies);
            if (attackerIndex >= 0)
            {
                Enemy attacker = enemies[attackerIndex];
                EnemyAttack(
                    attacker,
                    playerClass,
                    ref health,
                    maxHealth,
                    enemyCritReduction,
                    ref bleedTurns,
                    ref burnTurns,
                    ref weaknessTurns
                );
            }

            // Debuff tick on player
            int dot = 0;
            if (bleedTurns > 0) dot += 1;
            if (burnTurns > 0) dot += 1;

            if (dot > 0)
            {
                health -= dot;
                if (health < 0) health = 0;
                Narrate($"You take {dot} damage from afflictions.");
            }

            // Advance timers
            AdvanceTurn(ref bleedTurns, ref burnTurns, ref weaknessTurns, ref executeCooldown, ref shockCooldown, ref healCooldown);

            // Advance enemy special cooldown countdowns
            foreach (var e in enemies)
            {
                if (e.HasGodSmack && e.GodSmackCooldownLeft > 0)
                    e.GodSmackCooldownLeft--;
            }

            turn++;
        }
    }

    // =========================================================
    // STAGE WRAPPER: AFTER STAGE YOU CAN RETURN TO HUB OR CONTINUE
    // =========================================================
    static void RunStageWithHubReturn(
        string hubName,
        string stageTitle,
        List<Enemy> enemies,
        int rewardGold,
        bool allowPotions,
        bool hasHeal20Potion,
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
        List<GearItem> hubShop,
        bool hubHasPotionShop,
        bool hubHasHeal20Potion
    )
    {
        RunEncounter(
            stageTitle,
            enemies,
            rewardGold,
            allowPotions,
            hasHeal20Potion,
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

        while (true)
        {
            Console.WriteLine();
            Narrate($"After the fight, you may return to {hubName} or continue.");
            Console.Write($"Type: {hubName} | CONTINUE\n> ");
            string choice = GetNormalizedCommand();

            if (choice == hubName || choice.Replace(" ", "") == hubName.Replace(" ", ""))
            {
                VisitTown(
                    hubName,
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
                    hubShop,
                    hubHasPotionShop,
                    hubHasHeal20Potion
                );
                // leaving hub returns automatically back to the road (continue)
                return;
            }
            if (choice == "CONTINUE")
                return;

            Narrate("Try again.");
        }
    }

    // =========================================================
    // PLAYER ACTIONS (GAUNTLET)
    // =========================================================
    static bool PlayerAttackGauntlet(List<Enemy> enemies, int damage, int accuracy, int critChance, int weaknessTurns)
    {
        int target = FirstAliveEnemyIndex(enemies);
        if (target < 0) return false;

        Enemy e = enemies[target];

        bool hit = RollHit(Rng, accuracy);
        if (!hit)
        {
            Narrate($"You swing at {e.Name} and miss!");
            return true;
        }

        int effectiveDamage = damage;
        if (weaknessTurns > 0)
            effectiveDamage = (int)Math.Ceiling(damage * 0.75);

        bool crit = RollHit(Rng, critChance);
        int dealt = crit ? effectiveDamage * 2 : effectiveDamage;

        e.Hp -= dealt;
        if (e.Hp < 0) e.Hp = 0;

        if (crit) Narrate($"Critical hit! You strike {e.Name} for {dealt} damage!");
        else Narrate($"You strike {e.Name} for {dealt} damage!");

        return true;
    }

    static bool WarriorExecuteGauntlet(List<Enemy> enemies, int playerDamage, int playerAccuracy, ref int executeCooldown)
    {
        int target = FirstAliveEnemyIndex(enemies);
        if (target < 0) return false;

        Enemy e = enemies[target];

        if (executeCooldown > 0)
        {
            Narrate($"Execute is on cooldown ({executeCooldown} turns remaining).");
            return false;
        }

        int threshold = (int)Math.Ceiling(e.MaxHp * 0.20);
        if (e.Hp > threshold)
        {
            Narrate("You cannot Execute yet! The foe must be below 20% health.");
            return false;
        }

        Narrate($"You lunge forward with EXECUTE at {e.Name}!");

        bool hit = RollHit(Rng, playerAccuracy) || RollHit(Rng, playerAccuracy); // double accuracy
        executeCooldown = 4;

        if (!hit)
        {
            Narrate("You miss!");
            return true;
        }

        int execDamage = (int)Math.Round(playerDamage * 1.5);

        // If damage would already kill, no instant kill roll.
        if (e.Hp - execDamage <= 0)
        {
            e.Hp = 0;
            Narrate($"It hits for {execDamage} damage and finishes the foe!");
            return true;
        }

        // 10% instant kill chance when it matters
        if (Rng.Next(1, 101) <= 10)
        {
            e.Hp = 0;
            executeCooldown += 1;
            Narrate("EXECUTE! The blow destroys the target instantly!");
            Narrate("The instant kill strains you—Execute cooldown increased by 1.");
            return true;
        }

        e.Hp -= execDamage;
        if (e.Hp < 0) e.Hp = 0;
        Narrate($"It hits for {execDamage} damage!");
        return true;
    }

    static void MageShockGauntlet(Random rng, int playerDamage, ref int shockCooldown, List<Enemy> enemies, int procChancePerEnemy)
    {
        if (shockCooldown > 0)
        {
            Narrate($"Shock is on cooldown ({shockCooldown} turns remaining).");
            return;
        }

        int primary = FirstAliveEnemyIndex(enemies);
        if (primary < 0) return;

        Narrate("You unleash Shock! Lightning strikes with guaranteed accuracy.");

        // Always hit primary
        enemies[primary].Hp -= playerDamage;
        if (enemies[primary].Hp < 0) enemies[primary].Hp = 0;
        Narrate($"Shock hits {enemies[primary].Name} for {playerDamage} damage!");

        // Chance to arc to each other enemy separately
        for (int i = 0; i < enemies.Count; i++)
        {
            if (i == primary) continue;
            if (!enemies[i].Alive) continue;

            if (RollHit(rng, procChancePerEnemy))
            {
                enemies[i].Hp -= playerDamage;
                if (enemies[i].Hp < 0) enemies[i].Hp = 0;
                Narrate($"The lightning arcs! {enemies[i].Name} takes {playerDamage} damage!");
            }
        }

        shockCooldown = 3;
    }

    static bool PriestHeal(ref int healCooldown, ref int health, int maxHealth, ref int bleedTurns, ref int burnTurns, ref int weaknessTurns)
    {
        if (healCooldown > 0)
        {
            Narrate($"Heal is on cooldown ({healCooldown} turns remaining).");
            return false;
        }

        health = maxHealth;
        Narrate("You call upon holy light. Your wounds knit shut.");
        Narrate($"Health restored to {health}/{maxHealth}.");

        bool any = bleedTurns > 0 || burnTurns > 0 || weaknessTurns > 0;
        if (any)
        {
            Narrate("Choose ONE active debuff to cleanse: BLEED | BURN | WEAKNESS");
            Console.Write("> ");
            string choice = GetNormalizedCommand();

            if (choice == "BLEED" && bleedTurns > 0) bleedTurns = 0;
            else if (choice == "BURN" && burnTurns > 0) burnTurns = 0;
            else if (choice == "WEAKNESS" && weaknessTurns > 0) weaknessTurns = 0;
            else
            {
                Narrate("That debuff is not active. The light steadies you, but nothing is cleansed.");
            }
        }
        else
        {
            Narrate("No debuffs cling to you, so the light simply warms your soul.");
        }

        healCooldown = 8;
        return true;
    }

    // =========================================================
    // POTIONS
    // =========================================================
    static bool UsePotionInCombat(
        PlayerClass playerClass,
        ref int gold,
        ref int health,
        int maxHealth,
        ref int executeCooldown,
        ref int shockCooldown,
        ref int healCooldown,
        bool hasHeal20Potion
    )
    {
        Console.WriteLine();
        Narrate("Potions:");
        Narrate("HEALING POTION (10 HP) - 10 GOLD");
        if (hasHeal20Potion) Narrate("GREATER HEALING POTION (20 HP) - 20 GOLD");
        Narrate("COOLDOWN RESET POTION - 70 GOLD (only works if your ability is on cooldown)");
        Console.Write("Type: HEAL 10 | HEAL 20 | RESET | BACK\n> ");

        string choice = GetNormalizedCommand();
        if (choice == "BACK") return false;

        if (choice == "HEAL10" || choice == "HEAL 10")
        {
            if (gold < 10) { Narrate("You do not have enough gold."); return false; }
            gold -= 10;
            int before = health;
            health = Math.Min(maxHealth, health + 10);
            Narrate($"You drink a healing potion. {before} -> {health} HP.");
            return true;
        }

        if ((choice == "HEAL20" || choice == "HEAL 20") && hasHeal20Potion)
        {
            if (gold < 20) { Narrate("You do not have enough gold."); return false; }
            gold -= 20;
            int before = health;
            health = Math.Min(maxHealth, health + 20);
            Narrate($"You drink a greater healing potion. {before} -> {health} HP.");
            return true;
        }

        if (choice == "RESET")
        {
            int cd = GetAbilityCooldown(playerClass, executeCooldown, shockCooldown, healCooldown);
            if (cd <= 0)
            {
                Narrate("Your ability is not on cooldown. The potion would be wasted.");
                return false;
            }
            if (gold < 70)
            {
                Narrate("You do not have enough gold.");
                return false;
            }

            gold -= 70;
            if (playerClass == PlayerClass.Warrior) executeCooldown = 0;
            else if (playerClass == PlayerClass.Mage) shockCooldown = 0;
            else healCooldown = 0;

            Narrate("You drink the cooldown reset potion. Your ability is ready!");
            return true;
        }

        Narrate("Invalid potion choice.");
        return false;
    }

    static int GetAbilityCooldown(PlayerClass pc, int executeCd, int shockCd, int healCd)
    {
        if (pc == PlayerClass.Warrior) return executeCd;
        if (pc == PlayerClass.Mage) return shockCd;
        return healCd;
    }

    // =========================================================
    // ENEMY PHASE
    // =========================================================
    static void EnemyAttack(
        Enemy attacker,
        PlayerClass playerClass,
        ref int health,
        int maxHealth,
        int enemyCritReduction,
        ref int bleedTurns,
        ref int burnTurns,
        ref int weaknessTurns
    )
    {
        // GOD SMACK logic
        if (attacker.HasGodSmack && attacker.GodSmackCooldownLeft == 0)
        {
            attacker.GodSmackCooldownLeft = attacker.GodSmackCooldown;

            // guaranteed hit, special damage (feels like a boss move)
            int effectiveCrit = Math.Max(0, attacker.CritChance - enemyCritReduction);
            bool crit = RollHit(Rng, effectiveCrit);
            int dealt = crit ? attacker.Damage * 4 : attacker.Damage * 2;

            health -= dealt;
            if (health < 0) health = 0;

            if (crit) Narrate($"Critical! {attacker.Name} uses GOD SMACK for {dealt} damage!");
            else Narrate($"{attacker.Name} uses GOD SMACK for {dealt} damage!");

            return;
        }

        bool hit = RollHit(Rng, attacker.Accuracy);
        if (!hit)
        {
            Narrate($"{attacker.Name} attacks… and misses!");
            return;
        }

        int effectiveEnemyCrit = Math.Max(0, attacker.CritChance - enemyCritReduction);
        bool enemyCrit = RollHit(Rng, effectiveEnemyCrit);

        int dealtNormal = enemyCrit ? attacker.Damage * 2 : attacker.Damage;

        health -= dealtNormal;
        if (health < 0) health = 0;

        if (enemyCrit) Narrate($"Critical! {attacker.Name} hits you for {dealtNormal} damage!");
        else Narrate($"{attacker.Name} hits you for {dealtNormal} damage!");

        // Apply status on-hit
        if (attacker.BleedChance > 0 && RollHit(Rng, attacker.BleedChance))
        {
            bleedTurns += 1; // stacks
            Narrate("Bleed stacks onto you!");
        }

        if (attacker.BurnChance > 0 && RollHit(Rng, attacker.BurnChance))
        {
            burnTurns += 1; // stacks
            Narrate("Burn stacks onto you!");
        }

        if (attacker.WeaknessChance > 0)
        {
            // "100% chance to apply weakness unless player is priest"
            if (playerClass != PlayerClass.Priest && RollHit(Rng, attacker.WeaknessChance))
            {
                weaknessTurns = Math.Max(weaknessTurns, 3);
                Narrate("Weakness clings to you!");
            }
        }
    }

    // =========================================================
    // TOWNS / SHOPS
    // =========================================================
    static void VisitTown(
        string townName,
        PlayerClass playerClass,
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
        HashSet<string> ownedGearKeys,
        Dictionary<Slot, GearItem?> equippedBySlot,
        List<GearItem> gearForThisTown,
        bool hasPotionShop,
        bool hasHeal20Potion
    )
    {
        Console.WriteLine();
        Narrate($"You arrive at {townName}.");
        if (townName == "HARTLYN")
            Narrate("Lanternlight flickers, and the smell of stew fills the air.");
        else if (townName == "GALENWOOD")
            Narrate("Tall pines crowd the road, and a quiet strength hums through the town.");
        else
            Narrate("Stone towers rise above you. Guards watch, but do not interfere.");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"=== {townName} ===");
            Console.WriteLine($"GOLD: {gold}");
            Console.WriteLine($"HP: {health}/{maxHealth} | DMG: {damage} | ACC: {accuracy}% | CRIT: {critChance}% | ENEMY CRIT REDUCTION: {enemyCritReduction}%");

            Console.Write("Options: INN | BLACKSMITH");
            if (hasPotionShop) Console.Write(" | POTION SHOP");
            Console.WriteLine(" | LEAVE");
            Console.Write("> ");

            string choice = GetNormalizedCommand();

            if (choice == "LEAVE")
            {
                Narrate($"You leave {townName} and return to the road...");
                return;
            }
            else if (choice == "INN")
            {
                Narrate("You rest at the inn. The bed squeaks, but it heals.");
                health = maxHealth;
                Narrate($"Health restored to {health}/{maxHealth}.");
            }
            else if (choice == "BLACKSMITH")
            {
                BlacksmithShop(
                    townName,
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
                    gearForThisTown
                );
            }
            else if (choice == "POTION SHOP" || choice == "POTIONSHOP")
            {
                if (!hasPotionShop)
                {
                    Narrate("There is no potion shop here.");
                    continue;
                }

                PotionShop(
                    townName,
                    ref gold,
                    ref health,
                    maxHealth,
                    hasHeal20Potion
                );
            }
            else
            {
                Narrate("That option is not available here.");
            }
        }
    }

    static void PotionShop(string townName, ref int gold, ref int health, int maxHealth, bool hasHeal20Potion)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"=== POTION SHOP - {townName} ===");
            Console.WriteLine($"GOLD: {gold} | HP: {health}/{maxHealth}");
            Console.WriteLine("BUY HEALING POTION (10 HP) - 10 GOLD");
            if (hasHeal20Potion) Console.WriteLine("BUY GREATER HEALING POTION (20 HP) - 20 GOLD");
            Console.WriteLine("BACK");
            Console.Write("> ");

            string input = GetNormalizedCommand();
            if (input == "BACK") return;

            if (input.Contains("HEALING") || input == "HEAL")
            {
                if (gold < 10) { Narrate("You do not have enough gold."); continue; }
                gold -= 10;
                int before = health;
                health = Math.Min(maxHealth, health + 10);
                Narrate($"You buy and drink a healing potion. {before} -> {health} HP.");
                continue;
            }

            if (hasHeal20Potion && (input.Contains("GREATER") || input == "HEAL 20" || input == "HEAL20"))
            {
                if (gold < 20) { Narrate("You do not have enough gold."); continue; }
                gold -= 20;
                int before = health;
                health = Math.Min(maxHealth, health + 20);
                Narrate($"You buy and drink a greater healing potion. {before} -> {health} HP.");
                continue;
            }

            Narrate("Try buying a potion or type BACK.");
        }
    }

    static void BlacksmithShop(
        string townName,
        PlayerClass playerClass,
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
        HashSet<string> ownedGearKeys,
        Dictionary<Slot, GearItem?> equippedBySlot,
        List<GearItem> stock
    )
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"=== BLACKSMITH - {townName} ===");
            Console.WriteLine($"GOLD: {gold}");
            Console.WriteLine();

            foreach (var item in stock)
            {
                bool allowed = item.Allowed.Contains(playerClass);
                string owned = ownedGearKeys.Contains(item.Key) ? " (OWNED)" : "";
                string lockTxt = allowed ? "" : " (NOT FOR YOUR CLASS)";

                Console.WriteLine($"{item.Name} - {item.Price} GOLD{owned}{lockTxt}");
            }

            Console.WriteLine();
            Console.WriteLine("Type: BUY <ITEM NAME>  |  BACK");
            Console.Write("> ");

            string raw = GetRawInput(allowEmpty: true);
            string norm = NormalizeKey(raw);

            if (norm == "BACK") return;

            if (!norm.StartsWith("BUY"))
            {
                Narrate("Try: BUY <ITEM NAME> or BACK.");
                continue;
            }

            string wantedKey = NormalizeKey(norm.Substring(3).Trim());
            if (string.IsNullOrWhiteSpace(wantedKey))
            {
                Narrate("Buy what?");
                continue;
            }

            // Match by normalized key against item.Key or item.Name
            GearItem? itemToBuy = stock.FirstOrDefault(i =>
                NormalizeKey(i.Key) == wantedKey ||
                NormalizeKey(i.Name) == wantedKey);

            if (itemToBuy == null)
            {
                Narrate("That item is not sold here.");
                continue;
            }

            if (!itemToBuy.Allowed.Contains(playerClass))
            {
                Narrate("The blacksmith shakes his head. That is not meant for your kind.");
                continue;
            }

            if (ownedGearKeys.Contains(itemToBuy.Key))
            {
                Narrate("You already own that piece. The blacksmith refuses to sell you another.");
                continue;
            }

            if (gold < itemToBuy.Price)
            {
                Narrate("You do not have enough gold.");
                continue;
            }

            gold -= itemToBuy.Price;
            ownedGearKeys.Add(itemToBuy.Key);

            Narrate($"Purchased {itemToBuy.Name}.");

            // Always equip best possible owned items (highest tier per slot)
            RecomputeDerivedStats(
                playerClass,
                baseMaxHealth,
                baseDamage,
                baseAccuracy,
                baseCritChance,
                stockAllTowns: AllGearAcrossGame(),
                ownedGearKeys,
                equippedBySlot,
                ref maxHealth,
                ref health,
                ref damage,
                ref accuracy,
                ref critChance,
                ref enemyCritReduction
            );

            Narrate("Your gear is updated to the best you own.");
        }
    }

    // =========================================================
    // DERIVED STATS / BEST EQUIP
    // =========================================================
    static List<GearItem> AllGearAcrossGame()
    {
        // union of all shop items (so best-equip can use owned items from older towns too)
        var all = new List<GearItem>();
        all.AddRange(BuildHartlynShop());
        all.AddRange(BuildGalenwoodShop());
        all.AddRange(BuildRoyalCastleShop());
        return all;
    }

    static void RecomputeDerivedStats(
        PlayerClass playerClass,
        int baseMaxHealth,
        int baseDamage,
        int baseAccuracy,
        int baseCritChance,
        List<GearItem> stockAllTowns,
        HashSet<string> ownedGearKeys,
        Dictionary<Slot, GearItem?> equippedBySlot,
        ref int maxHealth,
        ref int health,
        ref int damage,
        ref int accuracy,
        ref int critChance,
        ref int enemyCritReduction
    )
    {
        // find best owned per slot by tier
        foreach (Slot s in Enum.GetValues(typeof(Slot)))
        {
            GearItem? best = stockAllTowns
                .Where(i => i.Slot == s)
                .Where(i => ownedGearKeys.Contains(i.Key))
                .Where(i => i.Allowed.Contains(playerClass))
                .OrderByDescending(i => i.Tier)
                .FirstOrDefault();

            equippedBySlot[s] = best;
        }

        int hpBonus = 0;
        int dmgBonus = 0;
        int critBonus = 0;
        int enemyCritRedBonus = 0;

        foreach (var kv in equippedBySlot)
        {
            var item = kv.Value;
            if (item == null) continue;
            hpBonus += item.MaxHpBonus;
            dmgBonus += item.DamageBonus;
            critBonus += item.CritBonus;
            enemyCritRedBonus += item.EnemyCritReductionBonus;
        }

        int newMax = baseMaxHealth + hpBonus;
        maxHealth = newMax;

        // clamp current HP if max dropped (it can if you change base stats)
        if (health > maxHealth) health = maxHealth;

        damage = baseDamage + dmgBonus;
        accuracy = baseAccuracy;
        critChance = baseCritChance + critBonus;
        enemyCritReduction = enemyCritRedBonus;
    }

    // =========================================================
    // UI HELPERS (NO UNDERSCORES, ITEMS ALL CAPS WITH SPACES,
    // NAME + CLASS ALL CAPS, NARRATOR PROPER CAPITALIZATION)
    // =========================================================
    static void Narrate(string text) => Console.WriteLine(text);

    static void PrintPlayerSheet(string playerNameUi, PlayerClass pc, int hp, int maxHp, int dmg, int acc, int crit, int gold)
    {
        Console.WriteLine($"{playerNameUi} - {pc.ToString().ToUpperInvariant()}");
        Console.WriteLine($"HP: {hp}/{maxHp} | DMG: {dmg} | ACC: {acc}% | CRIT: {crit}% | GOLD: {gold}");
    }

    static void PrintEnemySheet(List<Enemy> enemies, int enemyCritReduction)
    {
        Console.WriteLine("Enemies:");
        for (int i = 0; i < enemies.Count; i++)
        {
            var e = enemies[i];
            if (!e.Alive) continue;

            int effCrit = Math.Max(0, e.CritChance - enemyCritReduction);
            Console.WriteLine($"- {e.Name}: {e.Hp}/{e.MaxHp} HP | DMG: {e.Damage} | ACC: {e.Accuracy}% | CRIT: {effCrit}%");
        }
    }

    static void PrintCooldowns(PlayerClass pc, int executeCooldown, int shockCooldown, int healCooldown)
    {
        Console.WriteLine();
        Console.WriteLine("Cooldowns:");
        if (pc == PlayerClass.Warrior)
            Console.WriteLine($"EXECUTE: {(executeCooldown == 0 ? "READY" : executeCooldown + " TURNS")}");
        else if (pc == PlayerClass.Mage)
            Console.WriteLine($"SHOCK: {(shockCooldown == 0 ? "READY" : shockCooldown + " TURNS")}");
        else
            Console.WriteLine($"HEAL: {(healCooldown == 0 ? "READY" : healCooldown + " TURNS")}");
    }

    static void PrintDebuffs(int bleedTurns, int burnTurns, int weaknessTurns)
    {
        Console.WriteLine();
        Console.WriteLine("Debuffs:");
        bool any = false;
        if (bleedTurns > 0) { Console.WriteLine($"BLEED ({bleedTurns} TURNS)"); any = true; }
        if (burnTurns > 0) { Console.WriteLine($"BURN ({burnTurns} TURNS)"); any = true; }
        if (weaknessTurns > 0) { Console.WriteLine($"WEAKNESS ({weaknessTurns} TURNS)"); any = true; }
        if (!any) Console.WriteLine("NONE");
    }

    // =========================================================
    // INPUT
    // =========================================================
    static string GetRawInput(bool allowEmpty = true)
    {
        string? raw = Console.ReadLine();
        string s = raw ?? "";
        if (!allowEmpty)
        {
            while (string.IsNullOrWhiteSpace(s))
            {
                Console.Write("> ");
                s = Console.ReadLine() ?? "";
            }
        }
        return s;
    }

    static string GetNormalizedCommand()
    {
        string raw = GetRawInput(allowEmpty: true);
        string norm = raw.Trim().ToUpperInvariant();
        // collapse multiple spaces
        norm = string.Join(" ", norm.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        return norm;
    }

    static string NormalizeKey(string s)
    {
        if (s == null) return "";
        s = s.ToUpperInvariant().Trim();
        // remove spaces and underscores for matching
        s = s.Replace("_", "").Replace(" ", "");
        return s;
    }

    // =========================================================
    // CLASS SELECT / BASE STATS
    // =========================================================
    static PlayerClass SelectClass(string playerNameUi)
    {
        // These are derived from InitBaseStats so UI never lies.
        int wHp, wDmg, wAcc;
        int mHp, mDmg, mAcc;
        int pHp, pDmg, pAcc;

        InitBaseStats(PlayerClass.Warrior, out wHp, out wDmg, out wAcc);
        InitBaseStats(PlayerClass.Mage, out mHp, out mDmg, out mAcc);
        InitBaseStats(PlayerClass.Priest, out pHp, out pDmg, out pAcc);

        while (true)
        {
            Console.WriteLine();
            Narrate($"Choose your class, {playerNameUi}: WARRIOR, MAGE, or PRIEST");
            Console.WriteLine();

            Console.WriteLine("Classes:");
            Console.WriteLine($"WARRIOR - {wHp} HP | {wDmg} DMG | {wAcc}% ACC");
            Console.WriteLine("  EXECUTE: Below 20% enemy HP, uses turn, double accuracy, +50% damage.");
            Console.WriteLine("  10% instant kill chance (only when normal Execute damage would NOT kill).");
            Console.WriteLine("  Cooldown: 4 turns (instant kill adds +1 cooldown).");
            Console.WriteLine();

            Console.WriteLine($"MAGE - {mHp} HP | {mDmg} DMG | {mAcc}% ACC");
            Console.WriteLine("  SHOCK: Guaranteed hit, does NOT use turn.");
            Console.WriteLine("  In gauntlets, Shock can arc to other enemies with a separate proc chance per enemy.");
            Console.WriteLine("  Cooldown: 3 turns.");
            Console.WriteLine();

            Console.WriteLine($"PRIEST - {pHp} HP | {pDmg} DMG | {pAcc}% ACC");
            Console.WriteLine("  HEAL: Full heal + cleanse one chosen debuff, uses turn.");
            Console.WriteLine("  Cooldown: 8 turns.");
            Console.WriteLine();

            Console.Write("Class: ");
            string choice = GetNormalizedCommand();

            if (choice == "WARRIOR") return PlayerClass.Warrior;
            if (choice == "MAGE") return PlayerClass.Mage;
            if (choice == "PRIEST") return PlayerClass.Priest;

            Narrate("That is not a class in these lands. Try again.");
        }
    }

    static void InitBaseStats(PlayerClass playerClass, out int maxHealth, out int damage, out int accuracy)
    {
        maxHealth = 0; damage = 0; accuracy = 0;

        switch (playerClass)
        {
            case PlayerClass.Warrior:
                maxHealth = 35;
                damage = 8;
                accuracy = 75;
                break;
            case PlayerClass.Mage:
                maxHealth = 24;
                damage = 12;
                accuracy = 55;
                break;
            case PlayerClass.Priest:
                maxHealth = 30;
                damage = 6;
                accuracy = 65;
                break;
        }
    }

    // =========================================================
    // PRIEST TUTORIAL
    // =========================================================
    static void PriestTutorial(ref int bleedTurns, ref int burnTurns, ref int weaknessTurns)
    {
        Console.WriteLine();
        Narrate("Woah there... You are a priest.");
        Narrate("Before you start swinging, learn to cleanse a debuff.");
        Console.WriteLine();

        bleedTurns = burnTurns = weaknessTurns = 0;

        int roll = Rng.Next(1, 4);
        if (roll == 1) bleedTurns = 3;
        else if (roll == 2) burnTurns = 3;
        else weaknessTurns = 3;

        Narrate("A strange hex clings to you!");
        PrintDebuffs(bleedTurns, burnTurns, weaknessTurns);

        Console.WriteLine();
        Narrate("Cleanse the ACTIVE debuff by typing its name: BLEED | BURN | WEAKNESS");
        Console.Write("> ");

        string choice = GetNormalizedCommand();

        if (choice == "BLEED" && bleedTurns > 0) bleedTurns = 0;
        else if (choice == "BURN" && burnTurns > 0) burnTurns = 0;
        else if (choice == "WEAKNESS" && weaknessTurns > 0) weaknessTurns = 0;

        Console.WriteLine();
        Narrate("Well done. The curse fades away.");
        PrintDebuffs(bleedTurns, burnTurns, weaknessTurns);

        Console.WriteLine();
        Narrate("Alright then. On to the yard...");
    }

    // =========================================================
    // TURN ADVANCE / ROLLS / TARGETING
    // =========================================================
    static void AdvanceTurn(
        ref int bleedTurns,
        ref int burnTurns,
        ref int weaknessTurns,
        ref int executeCooldown,
        ref int shockCooldown,
        ref int healCooldown)
    {
        if (bleedTurns > 0) bleedTurns--;
        if (burnTurns > 0) burnTurns--;
        if (weaknessTurns > 0) weaknessTurns--;

        if (executeCooldown > 0) executeCooldown--;
        if (shockCooldown > 0) shockCooldown--;
        if (healCooldown > 0) healCooldown--;
    }

    static bool RollHit(Random rng, int hitChancePercent)
    {
        int roll = rng.Next(1, 101);
        return roll <= hitChancePercent;
    }

    static int FirstAliveEnemyIndex(List<Enemy> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
            if (enemies[i].Alive) return i;
        return -1;
    }

    // =========================================================
    // ENEMY FACTORIES
    // =========================================================
    static List<Enemy> MakeBandits(int count)
    {
        var list = new List<Enemy>();
        for (int i = 1; i <= count; i++)
            list.Add(new Enemy($"BANDIT {i}", hp: 20, dmg: 7, acc: 55, crit: 10));
        return list;
    }

    static Enemy MakeKnight(string name, int hp, int dmg, int acc, int crit)
        => new Enemy(name, hp, dmg, acc, crit);

    static Enemy MakeSquire(string name, int hp, int dmg, int acc, int crit)
        => new Enemy(name, hp, dmg, acc, crit);

    // =========================================================
    // SHOPS (ALL ITEMS: ALL CAPS WITH SPACES)
    // =========================================================
    static List<GearItem> BuildHartlynShop()
    {
        return new List<GearItem>
        {
            // HARTLYN (IRON + LINEN + OAK + WIZARD HAT)
            new GearItem("IRONBODY",   "IRON BODY",   Slot.Armor, tier: 1, price: 100, allowed: new[]{ PlayerClass.Warrior }, maxHpBonus: 10),
            new GearItem("IRONHELMET", "IRON HELMET", Slot.Helm,  tier: 1, price: 50,  allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, enemyCritReductionBonus: 15),
            new GearItem("IRONSWORD",  "IRON SWORD",  Slot.Weapon,tier: 1, price: 100, allowed: new[]{ PlayerClass.Warrior }, damageBonus: 4),
            new GearItem("MACE",       "MACE",        Slot.Weapon,tier: 1, price: 80,  allowed: new[]{ PlayerClass.Warrior, PlayerClass.Priest }, damageBonus: 3),
            new GearItem("OAKSTAFF",   "OAK STAFF",   Slot.Weapon,tier: 1, price: 80,  allowed: new[]{ PlayerClass.Mage, PlayerClass.Priest }, damageBonus: 2, critBonus: 5),
            new GearItem("LINENROBES", "LINEN ROBES", Slot.Armor, tier: 1, price: 100, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, maxHpBonus: 4, critBonus: 5),
            new GearItem("WIZARDHAT",  "WIZARD HAT",  Slot.Helm,  tier: 1, price: 50,  allowed: new[]{ PlayerClass.Mage }, critBonus: 5),
        };
    }

    static List<GearItem> BuildGalenwoodShop()
    {
        return new List<GearItem>
        {
            // GALENWOOD (STEEL / FINE ROBES / MAGIC HAT / ELDERWOOD STAFF)
            new GearItem("STEELBODY",   "STEEL BODY",   Slot.Armor, tier: 2, price: 250, allowed: new[]{ PlayerClass.Warrior }, maxHpBonus: 18),
            new GearItem("STEELHELMET", "STEEL HELMET", Slot.Helm,  tier: 2, price: 140, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, enemyCritReductionBonus: 22),
            new GearItem("STEELSWORD",  "STEEL SWORD",  Slot.Weapon,tier: 2, price: 260, allowed: new[]{ PlayerClass.Warrior }, damageBonus: 7),
            new GearItem("STEELMACE",   "STEEL MACE",   Slot.Weapon,tier: 2, price: 220, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Priest }, damageBonus: 6),
            new GearItem("ELDERWOODSTAFF","ELDERWOOD STAFF", Slot.Weapon, tier: 2, price: 220, allowed: new[]{ PlayerClass.Mage, PlayerClass.Priest }, damageBonus: 5, critBonus: 8),
            new GearItem("FINEROBES",   "FINE ROBES",   Slot.Armor, tier: 2, price: 260, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, maxHpBonus: 8, critBonus: 8),
            new GearItem("MAGICWIZARDHAT","MAGICAL WIZARD HAT", Slot.Helm, tier: 2, price: 160, allowed: new[]{ PlayerClass.Mage }, critBonus: 10),
        };
    }

    static List<GearItem> BuildRoyalCastleShop()
    {
        return new List<GearItem>
        {
            // ROYAL CASTLE (better stuff)
            new GearItem("ROYALPLATE",   "ROYAL PLATE",   Slot.Armor, tier: 3, price: 500, allowed: new[]{ PlayerClass.Warrior }, maxHpBonus: 28),
            new GearItem("ROYALHELM",    "ROYAL HELM",    Slot.Helm,  tier: 3, price: 350, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, enemyCritReductionBonus: 30),
            new GearItem("ROYALGREATBLADE","ROYAL GREATBLADE", Slot.Weapon, tier: 3, price: 650, allowed: new[]{ PlayerClass.Warrior }, damageBonus: 11),
            new GearItem("BLESSEDMACE",  "BLESSED MACE",  Slot.Weapon,tier: 3, price: 600, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Priest }, damageBonus: 10),
            new GearItem("ARCHSTAFF",    "ARCH STAFF",    Slot.Weapon,tier: 3, price: 600, allowed: new[]{ PlayerClass.Mage, PlayerClass.Priest }, damageBonus: 9, critBonus: 12),
            new GearItem("IMPERIALROBES","IMPERIAL ROBES",Slot.Armor, tier: 3, price: 600, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, maxHpBonus: 12, critBonus: 12),
            new GearItem("CROWNEDHAT",   "CROWNED WIZARD HAT", Slot.Helm, tier: 3, price: 420, allowed: new[]{ PlayerClass.Mage }, critBonus: 15),
        };
    }
}
