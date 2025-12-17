using System;
using System.Linq;

namespace Galenvale
{
    static class Input
    {
        public static string GetRawInput(bool allowEmpty = true)
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

        public static string GetNormalizedCommand()
        {
            string raw = GetRawInput(allowEmpty: true);
            string norm = raw.Trim().ToUpperInvariant();
            // collapse multiple spaces
            norm = string.Join(" ", norm.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return norm;
        }

        public static string NormalizeKey(string s)
        {
            if (s == null) return "";
            s = s.ToUpperInvariant().Trim();
            // remove spaces and underscores for matching
            s = s.Replace("_", "").Replace(" ", "");
            return s;
        }
    }

    static class Ui
    {
        public static void Narrate(string text) => Console.WriteLine(text);

        public static void PrintPlayerSheet(string playerNameUi, PlayerClass pc, int hp, int maxHp, int dmg, int acc, int crit, int gold)
        {
            Console.WriteLine($"{playerNameUi} - {pc.ToString().ToUpperInvariant()}");
            Console.WriteLine($"HP: {hp}/{maxHp} | DMG: {dmg} | ACC: {acc}% | CRIT: {crit}% | GOLD: {gold}");
        }

        public static void PrintEnemySheet(System.Collections.Generic.List<Enemy> enemies, int enemyCritReduction)
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

        public static void PrintCooldowns(PlayerClass pc, int executeCooldown, int shockCooldown, int healCooldown)
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

        public static void PrintDebuffs(int bleedTurns, int burnTurns, int weaknessTurns)
        {
            Console.WriteLine();
            Console.WriteLine("Debuffs:");
            bool any = false;
            if (bleedTurns > 0) { Console.WriteLine($"BLEED ({bleedTurns} TURNS)"); any = true; }
            if (burnTurns > 0) { Console.WriteLine($"BURN ({burnTurns} TURNS)"); any = true; }
            if (weaknessTurns > 0) { Console.WriteLine($"WEAKNESS ({weaknessTurns} TURNS)"); any = true; }
            if (!any) Console.WriteLine("NONE");
        }
    }
}