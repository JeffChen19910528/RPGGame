# RAGE: Chronicles of Darkness
## 暴走：黑暗年代記

A pure text-based turn-based RPG built with C# / .NET 8.  
Three-chapter story, humorous sidequests, multiple endings, ASCII animated combat, moral choice system, and tri-lingual support.

> 🌐 **Language / 語言切換:** [繁體中文](README.md) | **English**

---

## Table of Contents

- [Story Background](#story-background)
- [Quick Start](#quick-start)
- [How to Play](#how-to-play)
  - [Main Menu](#main-menu)
  - [Character Creation](#character-creation)
  - [Combat System](#combat-system)
  - [Skill System](#skill-system)
  - [Rage / Berserk System](#rage--berserk-system)
  - [Character Progression](#character-progression)
  - [Save & Load](#save--load)
- [Language Support](#language-support)
- [Enemy Compendium](#enemy-compendium)
- [Humorous Sidequests](#humorous-sidequests)
- [Multiple Endings](#multiple-endings)
- [Project Structure](#project-structure)
- [Build Instructions](#build-instructions)
- [Automated Tests](#automated-tests)

---

## Story Background

Three years ago, the Demon King **Yamatolusu** led his demonic army in an invasion. The hero fell in the final battle with fatal wounds and has been unconscious ever since.

Three years later, you open your eyes.

The village elder priest tells you: **The Demon King's army will reach this village in three days.**

You are their last hope — but in the three years you lay unconscious, the world has changed, and your heart will be forced to face choice after choice on the journey ahead.

**This is not just a war to defeat the Demon King. It is a journey about rage, choice, and the self.**

---

## Quick Start

### One-Click Launch (Recommended)

**Windows:**
```
Double-click start.bat
```

**macOS / Linux:**
```bash
chmod +x start.sh
./start.sh
```

### Manual Launch

```bash
dotnet run
```

> Requires [.NET 8 SDK](https://dotnet.microsoft.com/download).

---

## How to Play

### Main Menu

After launch, select your **language**, then the main menu appears:

```
  [1] New Game
  [2] Load Game
  [3] How to Play
  [4] Quit
  [5] Run Tests
```

---

### Character Creation

#### 1. Enter Your Name

Type your character's name (press Enter to use the default).

#### 2. Choose a Class (5 options)

| Class | HP | ATK | DEF | MP | Playstyle |
|-------|----|-----|-----|----|----|
| **Warrior** | +30 | — | +5 | — | High HP and defense, rage builds fast |
| **Mage** | — | +3 | — | +30 | High MP, strong skill damage |
| **Assassin** | −10 | +8 | — | — | Very high attack, high-risk playstyle |
| **Paladin** | +40 | −2 | +8 | — | Maximum defense, strong heals and crowd control |
| **Ranger** | — | +5 | — | +20 | Poison attacks and precision shots, balanced output |

> Your class affects not only stats, but also your exclusive skill set, story lines, and hidden ending conditions.

---

### Combat System

Combat is turn-based; the player always acts first. Each turn, choose an action:

```
  [1] Attack
  [2] Use Skill
  [3] Defend (halve damage taken this turn)
  [4] View Status
```

#### Damage Formula

```
Final Damage = Base ATK × Skill Multiplier × Random Factor (±12%) − Enemy DEF
```

In Berserk state, all damage is multiplied by ×1.5, but there is a **20% chance of backfire** (deals damage to yourself).

#### Status Effects

| Status | Effect |
|--------|--------|
| **Burn** | Deals ~9 HP damage per turn for 3 turns |
| **Stun** | Skips the target's next action |
| **Defending** | Damage taken halved until next action |

---

### Skill System

Each class has a completely unique skill set. Unlocking happens as you level up:

- **Level 3** → Two advanced skills unlocked
- **Level 5** → One ultimate skill awakened

#### Warrior

| Skill | Cost | Effect |
|-------|------|--------|
| Power Slash | MP 8 | High-multiplier physical damage |
| Herbal Remedy | MP 18 | Restore 30 HP |
| Shield Bash | MP 10 | Damage + stun chance |
| Berserk Smash | Rage 40 | Very high damage + critical chance |
| **Hurricane Slash** ★ | MP 24 | Multi-slash + burn chance |
| **Holy Shield Blessing** ★ | MP 20 | Restore 50 HP |
| **Dragon Aura Blade** ★★ | MP 35 | Ultimate slash + high burn chance |

#### Mage

| Skill | Cost | Effect |
|-------|------|--------|
| Fireball | MP 15 | High damage + burn chance |
| Heal | MP 20 | Restore 35 HP |
| Frost Shot | MP 12 | Damage + stun chance |
| Elemental Burst | Rage 40 | Very high damage + high burn chance |
| **Ice Nova** ★ | MP 28 | High damage + guaranteed stun |
| **Greater Heal** ★ | MP 32 | Restore 65 HP |
| **Meteor Impact** ★★ | MP 40 | Meteor strike + stun chance |

#### Assassin

| Skill | Cost | Effect |
|-------|------|--------|
| Shadow Strike | MP 12 | High damage + critical chance |
| Poison Blade | MP 14 | Damage + high poison chance |
| Smoke Bomb | MP 10 | Damage + high stun chance |
| Annihilate | Rage 40 | Extreme damage + very high critical |
| **Death Mark** ★ | MP 22 | Strong damage + high stun chance |
| **Life Drain** ★ | MP 26 | Restore 50 HP |
| **God Slayer Combo** ★★ | MP 30 | Lightning combo + extreme critical rate |

#### Paladin

| Skill | Cost | Effect |
|-------|------|--------|
| Holy Strike | MP 14 | Damage + stun chance |
| Holy Mend | MP 18 | Restore 40 HP |
| Divine Shield | MP 10 | Damage + high stun chance |
| Divine Wrath | Rage 40 | High damage + burn chance |
| **Heavenly Judgment** ★ | MP 26 | Very high damage + burn |
| **Miracle Heal** ★ | MP 35 | Restore 80 HP |
| **Holy Rebirth** ★★ | MP 45 | Restore 100 HP (strongest heal) |

#### Ranger

| Skill | Cost | Effect |
|-------|------|--------|
| Piercing Arrow | MP 10 | High piercing damage |
| Poison Arrow | MP 14 | Damage + very high poison chance |
| Suppressing Shot | MP 10 | Damage + stun chance |
| Rapid Shot | Rage 40 | High damage + high critical rate |
| **Explosive Arrow** ★ | MP 25 | High damage + burn chance |
| **Nature's Remedy** ★ | MP 28 | Restore 60 HP |
| **Thousand Arrow Barrage** ★★ | MP 38 | Massive volley + high burn chance |

> ★ = Level 3 Advanced Skill　★★ = Level 5 Ultimate Skill (unique animation)

---

### Rage / Berserk System

Rage is the core mechanic of this game:

```
RAGE ████████░░  80/100
```

- **How rage builds:** Normal attack +15, taking damage +12, defending +5
- **Trigger:** When rage reaches 100, automatically enters **Berserk State** (4 turns)
- **Berserk effects:** All attack damage ×1.5, but 20% chance of backfire per action

> Berserk is powerful, but not without cost. How you wield your rage is the central question of this journey.

---

### Character Progression

Defeat enemies to earn EXP and level up:

| On Level Up | Value |
|-------------|-------|
| Max HP | +20 |
| Max MP | +10 |
| Attack | +3 |
| Defense | +2 |

- Reach **Level 3** → Unlock two advanced class skills
- Reach **Level 5** → Awaken your class's ultimate skill

---

### Save & Load

- **Auto-saves** at the end of each chapter; resume from where you left off
- Save file path: `bin/Debug/net8.0/savegame.json`
- **Save file is deleted automatically after completing the game**

---

## Language Support

The game supports the following languages, selectable at startup:

| Language | Option |
|----------|--------|
| Traditional Chinese | `[1]` |
| English | `[2]` |
| Japanese | `[3]` |

---

## Enemy Compendium

Encounters are randomized; every playthrough has a different combination. All enemies feature ASCII animations, and stronger foes have signature special attacks.

| Enemy | Tier | Special Attack | Appears In |
|-------|------|---------------|------------|
| **Slime** | ★ | — | Chapter 1 (tutorial, fixed) |
| **Crystal Spider** | ★★ | Crystal Shatter | Chapter 1 (random) |
| **Vampire Bat Swarm** | ★★ | Mass Blood Drain | Chapter 1 (random) |
| **Skeleton Archer** | ★★ | Bone Arrow Volley | Chapters 1–2 (random) |
| **Goblin Knight** | ★★ | Poison Lance Thrust | Chapter 2 (random) |
| **Poison Lizard** | ★★ | Venom Spit | Chapter 2 (random) |
| **Frost Witch** | ★★★ | Freeze Curse | Chapter 2 (random) |
| **Stone Serpent King** | ★★★ | Petrifying Venom | Chapter 2 (random) |
| **Shadow Wraith** | ★★★ | Soul Erosion | Chapter 2 (random) |
| **Corrupted Treant** | ★★★ | Poison Vine Bind | Chapter 2 (random) |
| **Angry Golem** | ★★★ | Stone Fist Slam | Chapter 2 (random) |
| **Abyss Demon** | ★★★ | Soul Devour | Chapter 2 (random) |
| **Phantom Knight** | ★★★★ | Void Slash | Chapter 3 (random) |
| **Corrupted Bishop** | ★★★★ | Dark Curse | Chapter 3 (random) |
| **Demon Soldier** | ★★★ | Panicked Strike | Chapter 3 (sidequest option) |
| **Dark Paladin** | ★★★★ | Dark Judgment | Chapter 3 (fixed gatekeeper) |
| **Demon King Yamatolusu** | ★★★★★ | Doomsday Flame | Chapter 3 (fixed final boss) |

> Random encounter pools scale with each chapter, providing a fresh challenge every run.

---

## Humorous Sidequests

Each chapter hides one or more humorous sidequest events. They provide breathing room between intense battles while offering real rewards.

How you respond determines what you receive — and occasionally affects your corruption level.

What you encounter and how you handle it is for you to discover.

---

## Multiple Endings

There are **9 endings** in total, determined by your corruption level, moral choices, class, and number of berserk activations — including several hidden endings that require specific conditions to trigger.

Every choice you make accumulates. The ending will tell you the truth.

---

## Project Structure

```
RPGGame/
├── start.bat           # Windows one-click launcher
├── start.sh            # macOS/Linux one-click launcher
├── test.bat            # Windows test runner
├── test.sh             # macOS/Linux test runner
├── Program.cs          # Entry point (supports --test flag)
├── GameManager.cs      # Main game loop, chapter flow, save/load
├── GameConstants.cs    # Global constants
├── Player.cs           # Player stats, progression, skills, berserk logic
├── Enemy.cs            # Enemy data (17 types), status effects, encounter pools
├── BattleSystem.cs     # Combat engine: turn flow, action resolution
├── SkillSystem.cs      # Class-exclusive skill definitions and damage calculation
├── StoryManager.cs     # Story text, sidequests, branch choices, 9 ending logic
├── GameTests.cs        # Automated test suite (275 tests)
├── Localization.cs     # Multi-language strings (ZH / EN / JA)
├── AnimationSystem.cs  # ASCII animation rendering
├── AsciiArt.cs         # ASCII art assets
└── Utils.cs            # UI utilities: typewriter, menus, progress bars
```

---

## Build Instructions

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Commands

```bash
dotnet build   # Build
dotnet run     # Run
```

### Terminal Recommendations

- **Windows:** Windows Terminal (UTF-8 and ANSI color support)
- **macOS:** iTerm2 or the native Terminal app
- **Linux:** Any UTF-8 capable terminal emulator

---

## Automated Tests

### How to Run

**Windows:**
```
Double-click test.bat
```

**macOS / Linux:**
```bash
chmod +x test.sh
./test.sh
```

**Manual:**
```bash
dotnet run -- --test
```

### Test Coverage

| Test Group | Content |
|------------|---------|
| Enemy Factory Tests | HP, ATK, alive status, and special attack validation for all 17 enemies |
| Random Pool Tests | Each tier pool covers all enemies across 200 samples |
| Player Creation Tests | Initial stats and corruption level for all 5 classes |
| Combat Basics Tests | Damage calculation, burn, stun, and death logic |
| Status Effect Edge Cases | Burn expiry, minimum damage floor, HP lower bound |
| New Enemy Tests | Stats, special attacks, and defense values for 4 new enemies |
| Tier 3 Pool Tests | Chapter 3 pool correctly includes all new enemies |
| Ultra Skill Tests | Properties and effects of each class's ultimate skill |
| IsUltra Flag Tests | Correct skill tier tagging (starter / advanced / ultra) |
| GameConstants Tests | Correctness of all global constant values |

> **275 tests total — all passing.**

---

> *The direction of the story is determined by your choices.*
