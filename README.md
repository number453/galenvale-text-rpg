# Galenvale Text RPG

A modular C# console-based role-playing game built to demonstrate clean system design, combat logic, state management, and multi-file organization.

This project focuses on deterministic gameplay systems (not graphics), showing structured programming, extensible architecture, and maintainable code layout.

---

## Overview

Galenvale Text RPG is a turn-based console RPG where you choose a class, manage resources, fight through gauntlet encounters, visit towns, buy gear, use potions, and progress through increasingly difficult enemies.

The game is intentionally split across multiple files with separation of concerns to reflect how larger codebases are structured.

---

## Key Features

### Combat System
- Turn-based combat with Accuracy, Crit Chance, cooldowns, and debuffs
- Multi-enemy gauntlet encounters
- Class-specific abilities:
  - Warrior: Execute (conditional finisher logic with cooldown rules)
  - Mage: Shock (can proc across multiple enemies, does not consume a turn)
  - Priest: Heal (full heal + cleanse one chosen active debuff)

### Status Effects
- Bleed, Burn, and Weakness debuffs
- Turn-based decay and predictable timing
- Certain enemies can apply debuffs on hit

### Town Progression
- Multiple towns with escalating difficulty and stronger shop inventories
- Inn fully restores your HP
- Blacksmith offers town-tier gear and auto-equips upgrades
- Duplicate gear purchases are prevented
- Potion shop sells healing potions and cooldown-reset potions usable in combat

### Progression Scaling
- Sequential encounter stages
- Increasing rewards (Gold)
- Enemy groups and bosses scale in complexity

---

## How to Run

Requirements:
- .NET SDK installed (recommended .NET 8 or later)

Run from the repository root:

dotnet run --project src/GalenvaleTextRPG/GalenvaleTextRPG.csproj

---

## How to Play

### Character Creation
Pick a class: WARRIOR, MAGE, or PRIEST.  
Each class has unique base stats and a unique ability.

### Combat Commands
- attack  : Standard attack using your Accuracy and Crit Chance
- ability : Uses your class ability (may be on cooldown)
- potion  : Use a potion in combat (if you own any)
- wait    : Skip your turn
- quit    : Exit the game

### Town Commands
- inn        : Rest and fully restore HP
- blacksmith : Buy gear (varies by town tier)
- potions    : Buy consumables (varies by town tier)
- leave      : Continue to the next encounters

### Gear Rules
- You cannot buy the exact same gear piece twice.
- When you buy a better item for a slot, the game keeps the best owned item equipped.

### Potions
- Basic healing potions restore HP (shop may offer stronger versions later).
- Cooldown-reset potion can only be used if your ability is currently on cooldown (prevents wasting it).

---

## Design Notes

This project was built to practice:
- Separation of concerns across files/modules
- Game state management across towns, combat, inventory, and cooldowns
- Extensible combat systems (adding new enemies, stages, and items cleanly)
- Clean input/output helpers for readable console UI

---

## License

MIT License
