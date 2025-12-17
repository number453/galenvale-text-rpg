using System;
using System.Collections.Generic;
using System.Linq;

namespace Galenvale
{
    static partial class Game
    {
        // Shared small helpers used by combat/town logic
        public static void AdvanceTurn(
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

        public static bool RollHit(Random rng, int hitChancePercent)
        {
            int roll = rng.Next(1, 101);
            return roll <= hitChancePercent;
        }

        public static int FirstAliveEnemyIndex(List<Enemy> enemies)
        {
            for (int i = 0; i < enemies.Count; i++)
                if (enemies[i].Alive) return i;
            return -1;
        }

        public static int GetAbilityCooldown(PlayerClass pc, int executeCd, int shockCd, int healCd)
        {
            if (pc == PlayerClass.Warrior) return executeCd;
            if (pc == PlayerClass.Mage) return shockCd;
            return healCd;
        }
    }
}