using System;
using System.Collections.Generic;
using System.Linq;

namespace Galenvale
{
    static class Combat
    {
        public static bool RunEncounter(
    string encounterTitle,
    List<Enemy> enemies,
    int rewardGold,
    bool allowPotions,
    bool hasHeal20Potion,
    PlayerClass playerClass,
    string playerNameUi,
    ref int gold,
    ref int potionHeal10Count,
    ref int potionHeal20Count,
    ref int potionResetCooldownCount,
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

                Ui.PrintPlayerSheet(playerNameUi, playerClass, health, maxHealth, damage, accuracy, critChance, gold);
                Console.WriteLine();
                Ui.PrintEnemySheet(enemies, enemyCritReduction);

                if (health <= 0)
                {
                    Console.WriteLine();
                    Ui.Narrate("You collapse. The world fades.");
                    Ui.Narrate("Game Over.");
                    return false;
                }

                if (enemies.All(e => !e.Alive))
                {
                    Console.WriteLine();
                    Ui.Narrate("All foes have fallen.");
                    if (rewardGold > 0)
                    {
                        gold += rewardGold;
                        Ui.Narrate($"You gain {rewardGold} gold.");
                    }
                    return true;
                }

                Ui.PrintDebuffs(bleedTurns, burnTurns, weaknessTurns);
                Ui.PrintCooldowns(playerClass, executeCooldown, shockCooldown, healCooldown);

                Console.WriteLine();
                Console.Write("Choose an action: ATTACK | ABILITY | WAIT");
                if (allowPotions) Console.Write(" | POTION");
                Console.Write(" | QUIT\n> ");

                string input = Input.GetNormalizedCommand();

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
                        MageShockGauntlet(Game.Rng, damage, ref shockCooldown, enemies, procChancePerEnemy: 50);
                        usedTurn = false; // Shock does NOT use a turn
                    }
                    else
                    {
                        usedTurn = PriestHeal(ref healCooldown, ref health, maxHealth, ref bleedTurns, ref burnTurns, ref weaknessTurns);
                    }
                }
                else if (input == "POTION" && allowPotions)
                {
                    Ui.Narrate("Choose a potion: HEAL10 | HEAL20 | RESET | BACK");
                    Console.Write("> ");
                    string potionChoice = Input.GetNormalizedCommand();

                    if (potionChoice == "BACK")
                        continue;

                    if (potionChoice == "HEAL10")
                    {
                        if (potionHeal10Count <= 0)
                        {
                            Ui.Narrate("You have no HEAL 10 potions.");
                            continue;
                        }

                        potionHeal10Count--;
                        health += 10;
                        if (health > maxHealth) health = maxHealth;
                        Ui.Narrate("You drink a HEAL 10 potion.");
                        usedTurn = true;
                    }
                    else if (potionChoice == "HEAL20")
                    {
                        if (!hasHeal20Potion)
                        {
                            Ui.Narrate("You do not know how to use this potion yet.");
                            continue;
                        }

                        if (potionHeal20Count <= 0)
                        {
                            Ui.Narrate("You have no HEAL 20 potions.");
                            continue;
                        }

                        potionHeal20Count--;
                        health += 20;
                        if (health > maxHealth) health = maxHealth;
                        Ui.Narrate("You drink a HEAL 20 potion.");
                        usedTurn = true;
                    }
                    else if (potionChoice == "RESET")
                    {
                        if (potionResetCooldownCount <= 0)
                        {
                            Ui.Narrate("You have no RESET potions.");
                            continue;
                        }

                        potionResetCooldownCount--;
                        executeCooldown = 0;
                        shockCooldown = 0;
                        healCooldown = 0;
                        Ui.Narrate("Your abilities feel refreshed!");
                        usedTurn = true;
                    }
                    else
                    {
                        Ui.Narrate("Invalid potion choice.");
                        continue;
                    }
                }

            
                else if (input == "WAIT")
                {
                    Ui.Narrate("You wait cautiously...");
                    usedTurn = true;
                }
                else
                {
                    Ui.Narrate("I don't understand that.");
                    continue;
                }

                // If Shock (or invalid) didn't use a turn, skip enemy phase + cooldown tick
                if (!usedTurn)
                    continue;

                // Enemy phase: ONLY ONE enemy attacks per turn (first alive)
                int attackerIndex = Game.FirstAliveEnemyIndex(enemies);
                if (attackerIndex >= 0)
                {
                    Enemy attacker = enemies[attackerIndex];
                    EnemyPhase.EnemyAttack(
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
                    Ui.Narrate($"You take {dot} damage from afflictions.");
                }

                // Advance timers
                Game.AdvanceTurn(ref bleedTurns, ref burnTurns, ref weaknessTurns, ref executeCooldown, ref shockCooldown, ref healCooldown);

                // Advance enemy special cooldown countdowns
                foreach (var e in enemies)
                {
                    if (e.HasGodSmack && e.GodSmackCooldownLeft > 0)
                        e.GodSmackCooldownLeft--;
                }

                turn++;
            }
        }

        // Player actions extracted
        public static bool PlayerAttackGauntlet(List<Enemy> enemies, int damage, int accuracy, int critChance, int weaknessTurns)
        {
            int target = Game.FirstAliveEnemyIndex(enemies);
            if (target < 0) return false;

            Enemy e = enemies[target];

            bool hit = Game.RollHit(Game.Rng, accuracy);
            if (!hit)
            {
                Ui.Narrate($"You swing at {e.Name} and miss!");
                return true;
            }

            int effectiveDamage = damage;
            if (weaknessTurns > 0)
                effectiveDamage = (int)Math.Ceiling(damage * 0.75);

            bool crit = Game.RollHit(Game.Rng, critChance);
            int dealt = crit ? effectiveDamage * 2 : effectiveDamage;

            e.Hp -= dealt;
            if (e.Hp < 0) e.Hp = 0;

            if (crit) Ui.Narrate($"Critical hit! You strike {e.Name} for {dealt} damage!");
            else Ui.Narrate($"You strike {e.Name} for {dealt} damage!");

            return true;
        }

        public static bool WarriorExecuteGauntlet(List<Enemy> enemies, int playerDamage, int playerAccuracy, ref int executeCooldown)
        {
            int target = Game.FirstAliveEnemyIndex(enemies);
            if (target < 0) return false;

            Enemy e = enemies[target];

            if (executeCooldown > 0)
            {
                Ui.Narrate($"Execute is on cooldown ({executeCooldown} turns remaining).");
                return false;
            }

            int threshold = (int)Math.Ceiling(e.MaxHp * 0.40);
            if (e.Hp > threshold)
            {
                Ui.Narrate("You cannot Execute yet! The foe must be below 40% health.");
                return false;
            }

            Ui.Narrate($"You lunge forward with EXECUTE at {e.Name}!");

            bool hit = Game.RollHit(Game.Rng, playerAccuracy) || Game.RollHit(Game.Rng, playerAccuracy); // double accuracy
            executeCooldown = 4;

            if (!hit)
            {
                Ui.Narrate("You miss!");
                return true;
            }

            int execDamage = (int)Math.Round(playerDamage * 1.5);

            // If damage would already kill, no instant kill roll.
            if (e.Hp - execDamage <= 0)
            {
                e.Hp = 0;
                Ui.Narrate($"It hits for {execDamage} damage and finishes the foe!");
                return true;
            }

            // 10% instant kill chance when it matters
            if (Game.Rng.Next(1, 101) <= 10)
            {
                e.Hp = 0;
                executeCooldown += 1;
                Ui.Narrate("EXECUTE! The blow destroys the target instantly!");
                Ui.Narrate("The instant kill strains you—Execute cooldown increased by 1.");
                return true;
            }

            e.Hp -= execDamage;
            if (e.Hp < 0) e.Hp = 0;
            Ui.Narrate($"It hits for {execDamage} damage!");
            return true;
        }

        public static void MageShockGauntlet(Random rng, int playerDamage, ref int shockCooldown, List<Enemy> enemies, int procChancePerEnemy)
        {
            if (shockCooldown > 0)
            {
                Ui.Narrate($"Shock is on cooldown ({shockCooldown} turns remaining).");
                return;
            }

            int primary = Game.FirstAliveEnemyIndex(enemies);
            if (primary < 0) return;

            Ui.Narrate("You unleash Shock! Lightning strikes with guaranteed accuracy.");

            // Always hit primary
            enemies[primary].Hp -= playerDamage;
            if (enemies[primary].Hp < 0) enemies[primary].Hp = 0;
            Ui.Narrate($"Shock hits {enemies[primary].Name} for {playerDamage} damage!");

            // Chance to arc to each other enemy separately
            for (int i = 0; i < enemies.Count; i++)
            {
                if (i == primary) continue;
                if (!enemies[i].Alive) continue;

                if (Game.RollHit(rng, procChancePerEnemy))
                {
                    enemies[i].Hp -= playerDamage;
                    if (enemies[i].Hp < 0) enemies[i].Hp = 0;
                    Ui.Narrate($"The lightning arcs! {enemies[i].Name} takes {playerDamage} damage!");
                }
            }

            shockCooldown = 3;
        }

        public static bool PriestHeal(ref int healCooldown, ref int health, int maxHealth, ref int bleedTurns, ref int burnTurns, ref int weaknessTurns)
        {
            if (healCooldown > 0)
            {
                Ui.Narrate($"Heal is on cooldown ({healCooldown} turns remaining).");
                return false;
            }

            health = maxHealth;
            Ui.Narrate("You call upon holy light. Your wounds knit shut.");
            Ui.Narrate($"Health restored to {health}/{maxHealth}.");

            bool any = bleedTurns > 0 || burnTurns > 0 || weaknessTurns > 0;
            if (any)
            {
                Ui.Narrate("Choose ONE active debuff to cleanse: BLEED | BURN | WEAKNESS");
                Console.Write("> ");
                string choice = Input.GetNormalizedCommand();

                if (choice == "BLEED" && bleedTurns > 0) bleedTurns = 0;
                else if (choice == "BURN" && burnTurns > 0) burnTurns = 0;
                else if (choice == "WEAKNESS" && weaknessTurns > 0) weaknessTurns = 0;
                else
                {
                    Ui.Narrate("That debuff is not active. The light steadies you, but nothing is cleansed.");
                }
            }
            else
            {
                Ui.Narrate("No debuffs cling to you, so the light simply warms your soul.");
            }

            healCooldown = 5;
            return true;
        }
    }

    static class EnemyPhase
    {
        public static void EnemyAttack(
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
                bool crit = Game.RollHit(Game.Rng, effectiveCrit);
                int dealt = crit ? attacker.Damage * 2 : attacker.Damage * 1;

                health -= dealt;
                if (health < 0) health = 0;

                if (crit) Ui.Narrate($"Critical! {attacker.Name} uses GOD SMACK for {dealt} damage!");
                else Ui.Narrate($"{attacker.Name} uses GOD SMACK for {dealt} damage!");

                return;
            }

            bool hit = Game.RollHit(Game.Rng, attacker.Accuracy);
            if (!hit)
            {
                Ui.Narrate($"{attacker.Name} attacks… and misses!");
                return;
            }

            int effectiveEnemyCrit = Math.Max(0, attacker.CritChance - enemyCritReduction);
            bool enemyCrit = Game.RollHit(Game.Rng, effectiveEnemyCrit);

            int dealtNormal = enemyCrit ? attacker.Damage * 2 : attacker.Damage;

            health -= dealtNormal;
            if (health < 0) health = 0;

            if (enemyCrit) Ui.Narrate($"Critical! {attacker.Name} hits you for {dealtNormal} damage!");
            else Ui.Narrate($"{attacker.Name} hits you for {dealtNormal} damage!");

            // Apply status on-hit
            if (attacker.BleedChance > 0 && Game.RollHit(Game.Rng, attacker.BleedChance))
            {
                bleedTurns += 1; // stacks
                Ui.Narrate("Bleed stacks onto you!");
            }

            if (attacker.BurnChance > 0 && Game.RollHit(Game.Rng, attacker.BurnChance))
            {
                burnTurns += 1; // stacks
                Ui.Narrate("Burn stacks onto you!");
            }

            if (attacker.WeaknessChance > 0)
            {
                // "100% chance to apply weakness unless player is priest"
                if (playerClass != PlayerClass.Priest && Game.RollHit(Game.Rng, attacker.WeaknessChance))
                {
                    weaknessTurns = Math.Max(weaknessTurns, 3);
                    Ui.Narrate("Weakness clings to you!");
                }
            }
        }
    }
}