using System.Collections.Generic;

namespace Galenvale
{
    internal sealed class GearItem
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
}