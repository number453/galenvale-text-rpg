using System;

namespace Galenvale
{
    static class PotionShop
    {
        public static void Open(
            string townName,
            ref int gold,
            ref int potionHeal10Count,
            ref int potionHeal20Count,
            ref int potionResetCooldownCount,
            bool hasHeal20Potion
        )
        {
            while (true)
            {
                Ui.Narrate($"=== POTION SHOP - {townName} ===");
                Console.WriteLine($"GOLD: {gold}");
                Console.WriteLine($"HEAL 10: {potionHeal10Count}");
                Console.WriteLine($"HEAL 20: {potionHeal20Count}");
                Console.WriteLine($"RESET:   {potionResetCooldownCount}");
                Console.WriteLine();

                Console.WriteLine("BUY HEAL10 (10 GOLD)");
                if (hasHeal20Potion)
                    Console.WriteLine("BUY HEAL20 (20 GOLD)");
                Console.WriteLine("BUY RESET (70 GOLD)");
                Console.WriteLine("BACK");
                Console.Write("> ");

                string choice = Input.GetNormalizedCommand();

                if (choice == "BACK")
                    return;

                if (choice == "HEAL10" || choice == "BUY HEAL10")
                {
                    if (gold < 10) { Ui.Narrate("Not enough gold."); continue; }
                    gold -= 10;
                    potionHeal10Count++;
                    Ui.Narrate("Purchased HEAL 10 potion.");
                    continue;
                }

                if ((choice == "HEAL20" || choice == "BUY HEAL20"))
                {
                    if (!hasHeal20Potion) { Ui.Narrate("That potion is not sold here."); continue; }
                    if (gold < 20) { Ui.Narrate("Not enough gold."); continue; }
                    gold -= 20;
                    potionHeal20Count++;
                    Ui.Narrate("Purchased HEAL 20 potion.");
                    continue;
                }

                if (choice == "RESET" || choice == "BUY RESET")
                {
                    if (gold < 70) { Ui.Narrate("Not enough gold."); continue; }
                    gold -= 70;
                    potionResetCooldownCount++;
                    Ui.Narrate("Purchased RESET potion.");
                    continue;
                }

                Ui.Narrate("Invalid option.");
            }
        }
    }
}
