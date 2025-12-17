using System;
using System.Collections.Generic;
using System.Linq;

namespace Galenvale
{
    static class Town
    {
        public static void VisitTown(
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
            Ui.Narrate($"You arrive at {townName}.");
            if (townName == "HARTLYN")
                Ui.Narrate("Lanternlight flickers, and the smell of stew fills the air.");
            else if (townName == "GALENWOOD")
                Ui.Narrate("Tall pines crowd the road, and a quiet strength hums through the town.");
            else
                Ui.Narrate("Stone towers rise above you. Guards watch, but do not interfere.");

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

                string choice = Input.GetNormalizedCommand();

                if (choice == "LEAVE")
                {
                    Ui.Narrate($"You leave {townName} and return to the road...");
                    return;
                }
                else if (choice == "INN")
                {
                    Ui.Narrate("You rest at the inn. The bed squeaks, but it heals.");
                    health = maxHealth;
                    Ui.Narrate($"Health restored to {health}/{maxHealth}.");
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
                        Ui.Narrate("There is no potion shop here.");
                        continue;
                    }

                    Potions.PotionShop(
                        townName,
                        ref gold,
                        ref health,
                        maxHealth,
                        hasHeal20Potion
                    );
                }
                else
                {
                    Ui.Narrate("That option is not available here.");
                }
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

                string raw = Input.GetRawInput(allowEmpty: true);
                string norm = Input.NormalizeKey(raw);

                if (norm == "BACK") return;

                if (!norm.StartsWith("BUY"))
                {
                    Ui.Narrate("Try: BUY <ITEM NAME> or BACK.");
                    continue;
                }

                string wantedKey = Input.NormalizeKey(norm.Substring(3).Trim());
                if (string.IsNullOrWhiteSpace(wantedKey))
                {
                    Ui.Narrate("Buy what?");
                    continue;
                }

                // Match by normalized key against item.Key or item.Name
                GearItem? itemToBuy = stock.FirstOrDefault(i =>
                    Input.NormalizeKey(i.Key) == wantedKey ||
                    Input.NormalizeKey(i.Name) == wantedKey);

                if (itemToBuy == null)
                {
                    Ui.Narrate("That item is not sold here.");
                    continue;
                }

                if (!itemToBuy.Allowed.Contains(playerClass))
                {
                    Ui.Narrate("The blacksmith shakes his head. That is not meant for your kind.");
                    continue;
                }

                if (ownedGearKeys.Contains(itemToBuy.Key))
                {
                    Ui.Narrate("You already own that piece. The blacksmith refuses to sell you another.");
                    continue;
                }

                if (gold < itemToBuy.Price)
                {
                    Ui.Narrate("You do not have enough gold.");
                    continue;
                }

                gold -= itemToBuy.Price;
                ownedGearKeys.Add(itemToBuy.Key);

                Ui.Narrate($"Purchased {itemToBuy.Name}.");

                // Always equip best possible owned items (highest tier per slot)
                RecomputeDerivedStats(
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

                Ui.Narrate("Your gear is updated to the best you own.");
            }
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
            foreach (Slot s in System.Enum.GetValues(typeof(Slot)))
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
    }

    static class Potions
    {
        public static void PotionShop(string townName, ref int gold, ref int health, int maxHealth, bool hasHeal20Potion)
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

                string input = Input.GetNormalizedCommand();
                if (input == "BACK") return;

                if (input.Contains("HEALING") || input == "HEAL")
                {
                    if (gold < 10) { Ui.Narrate("You do not have enough gold."); continue; }
                    gold -= 10;
                    int before = health;
                    health = Math.Min(maxHealth, health + 10);
                    Ui.Narrate($"You buy and drink a healing potion. {before} -> {health} HP.");
                    continue;
                }

                if (hasHeal20Potion && (input.Contains("GREATER") || input == "HEAL 20" || input == "HEAL20"))
                {
                    if (gold < 20) { Ui.Narrate("You do not have enough gold."); continue; }
                    gold -= 20;
                    int before = health;
                    health = Math.Min(maxHealth, health + 20);
                    Ui.Narrate($"You buy and drink a greater healing potion. {before} -> {health} HP.");
                    continue;
                }

                Ui.Narrate("Try buying a potion or type BACK.");
            }
        }

        public static bool UsePotionInCombat(
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
            Ui.Narrate("Potions:");
            Ui.Narrate("HEALING POTION (10 HP) - 10 GOLD");
            if (hasHeal20Potion) Ui.Narrate("GREATER HEALING POTION (20 HP) - 20 GOLD");
            Ui.Narrate("COOLDOWN RESET POTION - 70 GOLD (only works if your ability is on cooldown)");
            Console.Write("Type: HEAL 10 | HEAL 20 | RESET | BACK\n> ");

            string choice = Input.GetNormalizedCommand();
            if (choice == "BACK") return false;

            if (choice == "HEAL10" || choice == "HEAL 10")
            {
                if (gold < 10) { Ui.Narrate("You do not have enough gold."); return false; }
                gold -= 10;
                int before = health;
                health = Math.Min(maxHealth, health + 10);
                Ui.Narrate($"You drink a healing potion. {before} -> {health} HP.");
                return true;
            }

            if ((choice == "HEAL20" || choice == "HEAL 20") && hasHeal20Potion)
            {
                if (gold < 20) { Ui.Narrate("You do not have enough gold."); return false; }
                gold -= 20;
                int before = health;
                health = Math.Min(maxHealth, health + 20);
                Ui.Narrate($"You drink a greater healing potion. {before} -> {health} HP.");
                return true;
            }

            if (choice == "RESET")
            {
                int cd = Game.GetAbilityCooldown(playerClass, executeCooldown, shockCooldown, healCooldown);
                if (cd <= 0)
                {
                    Ui.Narrate("Your ability is not on cooldown. The potion would be wasted.");
                    return false;
                }
                if (gold < 70)
                {
                    Ui.Narrate("You do not have enough gold.");
                    return false;
                }

                gold -= 70;
                if (playerClass == PlayerClass.Warrior) executeCooldown = 0;
                else if (playerClass == PlayerClass.Mage) shockCooldown = 0;
                else healCooldown = 0;

                Ui.Narrate("You drink the cooldown reset potion. Your ability is ready!");
                return true;
            }

            Ui.Narrate("Invalid potion choice.");
            return false;
        }
    }
}