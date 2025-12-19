using System.Collections.Generic;

namespace Galenvale
{
    public static class EnemyFactory
    {
        public static Enemy MakeTrainingDummy()
        {
            // Matches what you had earlier
            Enemy trainingDummy = new Enemy("TRAINING DUMMY", hp: 60, dmg: 0, acc: 0, crit: 0);
            return trainingDummy;
        }

        public static List<Enemy> MakeBandits(int count)
        {
            var enemies = new List<Enemy>();

            for (int i = 0; i < count; i++)
            {
                Enemy bandit = new Enemy($"BANDIT {i + 1}", hp: 20, dmg: 5, acc: 55, crit: 5);
                bandit.BleedChance = 10;
                enemies.Add(bandit);
            }

            return enemies;
        }

        public static Enemy MakeBanditLeader()
        {
            Enemy leader = new Enemy("BANDIT LEADER", hp: 40, dmg: 10, acc: 55, crit: 10);
            leader.BleedChance = 25;
            return leader;
        }

        public static Enemy MakeKnight()
        {
            Enemy knight = new Enemy("KNIGHT", hp: 30, dmg: 8, acc: 60, crit: 10);
            knight.BleedChance = 15;
            return knight;
        }

        public static Enemy MakeSquire()
        {
            Enemy squire = new Enemy("SQUIRE", hp: 25, dmg: 6, acc: 60, crit: 5);
            squire.BleedChance = 5;
            return squire;
        }

        public static Enemy MakeKingsBlade()
        {
            Enemy e = new Enemy("KING'S BLADE", hp: 65, dmg: 13, acc: 65, crit: 15);
            e.BleedChance = 50;
            return e;
        }

        public static Enemy MakeKingsFlame()
        {
            Enemy e = new Enemy("KING'S FLAME", hp: 55, dmg: 17, acc: 60, crit: 25);
            e.BurnChance = 50;
            return e;
        }

        public static Enemy MakeKingsStrength(PlayerClass playerClass)
        {
            Enemy e = new Enemy("KING'S STRENGTH", hp: 75, dmg: 15, acc: 50, crit: 50);
            e.WeaknessChance = (playerClass == PlayerClass.Priest) ? 0 : 100;
            return e;
        }


        public static Enemy MakeGreatKing()
        {
            Enemy e = new Enemy("GREAT KING", hp: 100, dmg: 18, acc: 65, crit: 35);
            e.HasGodSmack = true;
            e.GodSmackCooldown = 3;
            e.GodSmackCooldownLeft = 2;
            e.BleedChance = 15;
            e.BurnChance = 15;
            e.WeaknessChance = 15;
            return e;
        }

    }
}
