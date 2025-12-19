namespace Galenvale
{
    public sealed class Enemy
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
}