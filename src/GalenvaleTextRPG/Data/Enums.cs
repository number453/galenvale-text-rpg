namespace Galenvale
{
    // Small enums moved into a dedicated file to improve discoverability.
    internal enum PlayerClass { Warrior, Mage, Priest }
    internal enum Slot { Weapon, Armor, Helm }

    internal enum PotionType
    {
        Heal10,
        Heal20,
        ResetCooldown
    }
}