using System.Collections.Generic;

namespace Galenvale
{
    static class ShopBuilder
    {
        public static List<GearItem> BuildHartlynShop()
        {
            return new List<GearItem>
            {
                new GearItem("IRONBODY",   "IRON BODY",   Slot.Armor, tier: 1, price: 100, allowed: new[]{ PlayerClass.Warrior }, maxHpBonus: 10),
                new GearItem("IRONHELMET", "IRON HELMET", Slot.Helm,  tier: 1, price: 50,  allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, enemyCritReductionBonus: 15),
                new GearItem("IRONSWORD",  "IRON SWORD",  Slot.Weapon,tier: 1, price: 100, allowed: new[]{ PlayerClass.Warrior }, damageBonus: 4),
                new GearItem("MACE",       "MACE",        Slot.Weapon,tier: 1, price: 80,  allowed: new[]{ PlayerClass.Warrior, PlayerClass.Priest }, damageBonus: 3),
                new GearItem("OAKSTAFF",   "OAK STAFF",   Slot.Weapon,tier: 1, price: 80,  allowed: new[]{ PlayerClass.Mage, PlayerClass.Priest }, damageBonus: 2, critBonus: 5),
                new GearItem("LINENROBES", "LINEN ROBES", Slot.Armor, tier: 1, price: 100, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, maxHpBonus: 4, critBonus: 5),
                new GearItem("WIZARDHAT",  "WIZARD HAT",  Slot.Helm,  tier: 1, price: 50,  allowed: new[]{ PlayerClass.Mage }, critBonus: 5),
            };
        }

        public static List<GearItem> BuildGalenwoodShop()
        {
            return new List<GearItem>
            {
                new GearItem("STEELBODY",   "STEEL BODY",   Slot.Armor, tier: 2, price: 250, allowed: new[]{ PlayerClass.Warrior }, maxHpBonus: 18),
                new GearItem("STEELHELMET", "STEEL HELMET", Slot.Helm,  tier: 2, price: 140, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, enemyCritReductionBonus: 22),
                new GearItem("STEELSWORD",  "STEEL SWORD",  Slot.Weapon,tier: 2, price: 260, allowed: new[]{ PlayerClass.Warrior }, damageBonus: 7),
                new GearItem("STEELMACE",   "STEEL MACE",   Slot.Weapon,tier: 2, price: 220, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Priest }, damageBonus: 6),
                new GearItem("ELDERWOODSTAFF","ELDERWOOD STAFF", Slot.Weapon, tier: 2, price: 220, allowed: new[]{ PlayerClass.Mage, PlayerClass.Priest }, damageBonus: 5, critBonus: 8),
                new GearItem("FINEROBES",   "FINE ROBES",   Slot.Armor, tier: 2, price: 260, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, maxHpBonus: 8, critBonus: 8),
                new GearItem("MAGICWIZARDHAT","MAGICAL WIZARD HAT", Slot.Helm, tier: 2, price: 160, allowed: new[]{ PlayerClass.Mage }, critBonus: 10),
            };
        }

        public static List<GearItem> BuildRoyalCastleShop()
        {
            return new List<GearItem>
            {
                new GearItem("ROYALPLATE",   "ROYAL PLATE",   Slot.Armor, tier: 3, price: 500, allowed: new[]{ PlayerClass.Warrior }, maxHpBonus: 28),
                new GearItem("ROYALHELM",    "ROYAL HELM",    Slot.Helm,  tier: 3, price: 350, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, enemyCritReductionBonus: 30),
                new GearItem("ROYALGREATBLADE","ROYAL GREATBLADE", Slot.Weapon, tier: 3, price: 650, allowed: new[]{ PlayerClass.Warrior }, damageBonus: 11),
                new GearItem("BLESSEDMACE",  "BLESSED MACE",  Slot.Weapon,tier: 3, price: 600, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Priest }, damageBonus: 10),
                new GearItem("ARCHSTAFF",    "ARCH STAFF",    Slot.Weapon,tier: 3, price: 600, allowed: new[]{ PlayerClass.Mage, PlayerClass.Priest }, damageBonus: 9, critBonus: 12),
                new GearItem("IMPERIALROBES","IMPERIAL ROBES",Slot.Armor, tier: 3, price: 600, allowed: new[]{ PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Priest }, maxHpBonus: 12, critBonus: 12),
                new GearItem("CROWNEDHAT",   "CROWNED WIZARD HAT", Slot.Helm, tier: 3, price: 420, allowed: new[]{ PlayerClass.Mage }, critBonus: 15),
            };
        }

        // Utility for recompute to consider all gear available across the game
        public static List<GearItem> AllGearAcrossGame()
        {
            var all = new List<GearItem>();
            all.AddRange(BuildHartlynShop());
            all.AddRange(BuildGalenwoodShop());
            all.AddRange(BuildRoyalCastleShop());
            return all;
        }
    }
}