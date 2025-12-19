using System;

namespace Galenvale
{
    public static class PlayerHelpers
    {
        public static PlayerClass SelectClass(string playerNameUi)
        {
            while (true)
            {
                Console.Clear();

                Ui.Narrate("========================================");
                Ui.Narrate("              CHOOSE CLASS");
                Ui.Narrate("========================================");
                Console.WriteLine($"ADVENTURER: {playerNameUi}");
                Console.WriteLine();

                InitBaseStats(PlayerClass.Warrior, out int wHp, out int wDmg, out int wAcc);
                InitBaseStats(PlayerClass.Mage, out int mHp, out int mDmg, out int mAcc);
                InitBaseStats(PlayerClass.Priest, out int pHp, out int pDmg, out int pAcc);

                Console.WriteLine(" #  CLASS     HP   DMG  ACC   SIGNATURE");
                Console.WriteLine("------------------------------------------------------------");
                PrintClassRow("1", "WARRIOR", wHp, wDmg, wAcc, "EXECUTE - finisher burst / execute chance");
                PrintClassRow("2", "MAGE", mHp, mDmg, mAcc, "SHOCK   - guaranteed hit / chain potential");
                PrintClassRow("3", "PRIEST", pHp, pDmg, pAcc, "HEAL    - full heal + cleanse one debuff");
                Console.WriteLine("------------------------------------------------------------");

                Console.WriteLine("ABILITY DETAILS:");
                Console.WriteLine("• EXECUTE (4-TURN COOLDOWN, USES TURN: YES)");
                Console.WriteLine("  - Deals +50% damage with DOUBLE accuracy");
                Console.WriteLine("  - 10% chance to INSTANT-KILL");
                Console.WriteLine("  - Only usable if target is at or below 40% HP");
                Console.WriteLine();

                Console.WriteLine("• SHOCK (3-TURN COOLDOWN, USES TURN: NO)");
                Console.WriteLine("  - Guaranteed hit for FULL player damage");
                Console.WriteLine("  - In multi-enemy fights: 50% chance to also hit each additional enemy for FULL damage");
                Console.WriteLine();

                Console.WriteLine("• HEAL (5-TURN COOLDOWN, USES TURN: YES)");
                Console.WriteLine("  - Restores you to FULL health");
                Console.WriteLine("  - Cleanses ONE active debuff of your choice: BLEED / BURN / WEAKNESS");
                Console.WriteLine();

                Console.Write("TYPE 1/2/3 OR WARRIOR/MAGE/PRIEST > ");
                string choice = Input.GetNormalizedCommand();

                if (choice == "1" || choice == "WARRIOR") return PlayerClass.Warrior;
                if (choice == "2" || choice == "MAGE") return PlayerClass.Mage;
                if (choice == "3" || choice == "PRIEST") return PlayerClass.Priest;

                Ui.Narrate("Invalid choice. Press ENTER to try again.");
                _ = Input.GetRawInput(allowEmpty: true);
            }
        }

        private static void PrintClassRow(string num, string name, int hp, int dmg, int acc, string signature)
        {
            Console.WriteLine($" {num,-2} {name,-8} {hp,4} {dmg,5} {acc,5}%  {signature}");
        }



        // Base stats: ONLY edit these numbers when balancing classes
        public static void InitBaseStats(
    PlayerClass playerClass,
    out int baseMaxHealth,
    out int baseDamage,
    out int baseAccuracy
)
        {
            switch (playerClass)
            {
                case PlayerClass.Warrior:
                    baseMaxHealth = 45;
                    baseDamage = 9;
                    baseAccuracy = 75;
                    return;

                case PlayerClass.Mage:
                    baseMaxHealth = 30;
                    baseDamage = 12;
                    baseAccuracy = 65;
                    return;

                case PlayerClass.Priest:
                    baseMaxHealth = 40;
                    baseDamage = 8;
                    baseAccuracy = 70;
                    return;

                default:
                    throw new InvalidOperationException(
                        $"Invalid PlayerClass encountered: {playerClass}"
                    );
            }
        }
        public static void PriestTutorial(ref int bleedTurns, ref int burnTurns, ref int weaknessTurns)
        {
            Console.WriteLine();
            Ui.Narrate("A hooded healer steps forward, studying you with calm eyes...");
            Ui.Narrate("\"Before you face the wilds, you must learn to PURGE suffering.\"");
            Console.WriteLine();

            // Apply exactly ONE random debuff
            var rng = new Random();
            int roll = rng.Next(0, 3);

            bleedTurns = 0;
            burnTurns = 0;
            weaknessTurns = 0;

            string debuffName;
            string debuffEffect;

            if (roll == 0)
            {
                bleedTurns = 3;
                debuffName = "BLEED";
                debuffEffect = "You are WOUNDED. Left untreated, pain will follow you into battle.";
            }
            else if (roll == 1)
            {
                burnTurns = 3;
                debuffName = "BURN";
                debuffEffect = "You are SCORCHED. Flames cling to you like a curse.";
            }
            else
            {
                weaknessTurns = 3;
                debuffName = "WEAKNESS";
                debuffEffect = "Your strength FADES. Your strikes will feel dull and heavy.";
            }

            Ui.Narrate($"A dark aura grips you... {debuffName}!");
            Ui.Narrate(debuffEffect);
            Console.WriteLine();

            Ui.Narrate("As a PRIEST, your ABILITY can HEAL you to full and CLEANSE one debuff.");
            Ui.Narrate("In combat you will type: ABILITY");
            Ui.Narrate("Then you will choose: HEAL");
            Ui.Narrate($"Then you must select the debuff to cleanse: {debuffName}");
            Console.WriteLine();

            // Step 1: force ABILITY
            while (true)
            {
                Console.WriteLine("TYPE: ABILITY");
                Console.Write("> ");
                string input = Input.GetNormalizedCommand();

                if (input == "ABILITY")
                    break;

                Ui.Narrate("Focus. Type ABILITY.");
                Console.WriteLine();
            }

            // Step 2: force HEAL
            while (true)
            {
                Console.WriteLine("TYPE: HEAL");
                Console.Write("> ");
                string input = Input.GetNormalizedCommand();

                if (input == "HEAL")
                    break;

                Ui.Narrate("Good. Now choose HEAL.");
                Console.WriteLine();
            }

            // Step 3: force correct debuff selection (mirrors PriestHeal UI)
            while (true)
            {
                Ui.Narrate("Choose ONE active debuff to cleanse: BLEED | BURN | WEAKNESS");
                Console.Write("> ");
                string choice = Input.GetNormalizedCommand();

                bool active =
                    (choice == "BLEED" && bleedTurns > 0) ||
                    (choice == "BURN" && burnTurns > 0) ||
                    (choice == "WEAKNESS" && weaknessTurns > 0);

                if (!active)
                {
                    Ui.Narrate("That debuff is not active. Try again.");
                    Console.WriteLine();
                    continue;
                }

                // Cleanse chosen debuff
                if (choice == "BLEED") bleedTurns = 0;
                else if (choice == "BURN") burnTurns = 0;
                else if (choice == "WEAKNESS") weaknessTurns = 0;

                Ui.Narrate("A warm light surges through your veins...");
                Ui.Narrate("The sickness breaks. The curse is GONE.");
                Console.WriteLine();

                Ui.Narrate("\"Good.\" The healer nods.");
                Ui.Narrate("\"You look ready to go to the TRAINING GROUNDS now.\"");
                Console.WriteLine();
                break;
            }
        }


    }
}
