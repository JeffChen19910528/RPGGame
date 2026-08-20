using System;
using System.Collections.Generic;
using RPGGame.Content;

namespace RPGGame.Domain
{
    public enum StatusEffect { None, Burn, Stun, Defending }
    public enum PlayerClass { Warrior, Mage, Assassin, Paladin, Ranger }

    public class Equipment
    {
        public string Name { get; }
        public int AtkBonus { get; }
        public int DefBonus { get; }
        public int HpBonus { get; }

        public Equipment(string name, int atk = 0, int def = 0, int hp = 0)
        {
            Name = name; AtkBonus = atk; DefBonus = def; HpBonus = hp;
        }
    }

    public class Player
    {
        // Core stats
        public string Name { get; set; }
        public int MaxHP { get; set; }
        public int HP { get; set; }
        public int MaxMP { get; set; }
        public int MP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }

        // Progression
        public int Level { get; set; }
        public int EXP { get; set; }
        public int EXPToNextLevel { get; set; }

        // Berserk System
        public int RageEnergy { get; set; }
        public int MaxRageEnergy { get; } = 100;
        public bool IsBerserk { get; private set; }
        public int BerserkTurnsLeft { get; private set; }
        public int TotalBerserkUses { get; set; }

        // Class
        public PlayerClass Class { get; set; } = PlayerClass.Warrior;
        public string ClassTitle => Class switch
        {
            PlayerClass.Warrior  => L10n.Get("CLASS_WARRIOR"),
            PlayerClass.Mage     => L10n.Get("CLASS_MAGE"),
            PlayerClass.Assassin => L10n.Get("CLASS_ASSASSIN"),
            PlayerClass.Paladin  => L10n.Get("CLASS_PALADIN"),
            PlayerClass.Ranger   => L10n.Get("CLASS_RANGER"),
            _ => L10n.Get("CLASS_HERO")
        };

        // Story flags
        public int CorruptionLevel { get; set; }
        public bool AcceptedDarkPower { get; set; }
        public bool HelpedVillager { get; set; }
        public bool FinalBossDefeatedInBerserk { get; set; }

        // Status effect
        public StatusEffect CurrentStatus { get; set; }
        public int StatusTurns { get; set; }
        public int BurnDamagePerTurn { get; set; }

        // Equipment
        public Equipment? Weapon { get; set; }
        public Equipment? Armor { get; set; }

        // Skills
        public List<Skill> Skills { get; set; }

        /// <summary>Skills unlocked by the most recent LevelUp; empty otherwise. Caller drains it for display.</summary>
        public List<Skill> LastUnlockedSkills { get; } = new();

        // Computed stats
        public int Attack => BaseAttack + (Weapon?.AtkBonus ?? 0);
        public int Defense => BaseDefense + (Armor?.DefBonus ?? 0);
        public bool IsAlive => HP > 0;

        public Player(string name)
        {
            Name = name;
            Level = 1;
            MaxHP = 100; HP = 100;
            MaxMP = 60;  MP = 60;
            BaseAttack = 15;
            BaseDefense = 8;
            EXP = 0;
            EXPToNextLevel = 100;
            CurrentStatus = StatusEffect.None;
            Skills = SkillSystem.GetStarterSkills(PlayerClass.Warrior);
        }

        public void InitClassSkills()
        {
            Skills = SkillSystem.GetStarterSkills(Class);
        }

        // ── Berserk ─────────────────────────────────────────────────────────

        public void AddRage(int amount)
        {
            if (IsBerserk) return;
            RageEnergy = Math.Min(MaxRageEnergy, RageEnergy + amount);
            if (RageEnergy >= MaxRageEnergy)
                TriggerBerserk();
        }

        private void TriggerBerserk()
        {
            IsBerserk = true;
            BerserkTurnsLeft = GameConstants.BerserkDuration;
            TotalBerserkUses++;
            RageEnergy = 0;

            if (TotalBerserkUses >= 2)
                CorruptionLevel++;
        }

        /// <summary>Ticks down an active berserk state. Returns true the turn it ends.</summary>
        public bool UpdateBerserkTick()
        {
            if (!IsBerserk) return false;
            BerserkTurnsLeft--;
            if (BerserkTurnsLeft <= 0)
            {
                IsBerserk = false;
                return true;
            }
            return false;
        }

        // ── Combat ──────────────────────────────────────────────────────────

        /// <summary>Apply raw incoming damage; returns actual damage taken.</summary>
        public int TakeDamage(int rawDamage)
        {
            int actual = Math.Max(1, rawDamage - Defense);
            if (CurrentStatus == StatusEffect.Defending)
                actual = Math.Max(1, (int)(actual * 0.5));
            HP = Math.Max(0, HP - actual);
            return actual;
        }

        public void Heal(int amount)
        {
            HP = Math.Min(MaxHP, HP + amount);
        }

        public void RestoreMP(int amount)
        {
            MP = Math.Min(MaxMP, MP + amount);
        }

        // ── Progression ─────────────────────────────────────────────────────

        /// <summary>Returns true if the player levelled up.</summary>
        public bool GainEXP(int amount)
        {
            EXP += amount;
            if (EXP < EXPToNextLevel) return false;
            LevelUp();
            return true;
        }

        private void LevelUp()
        {
            LastUnlockedSkills.Clear();

            Level++;
            EXP -= EXPToNextLevel;
            EXPToNextLevel = (int)(EXPToNextLevel * 1.6);

            MaxHP += 20; HP = Math.Min(HP + 20, MaxHP);
            MaxMP += 10; MP = Math.Min(MP + 10, MaxMP);
            BaseAttack += 3;
            BaseDefense += 2;

            if (Level == 3)
            {
                var advanced = SkillSystem.GetAdvancedSkills(Class);
                Skills.AddRange(advanced);
                LastUnlockedSkills.AddRange(advanced);
            }

            if (Level == GameConstants.UltraSkillLevel)
            {
                var ultra = SkillSystem.GetUltraSkills(Class);
                Skills.AddRange(ultra);
                LastUnlockedSkills.AddRange(ultra);
            }
        }

        // ── Equip ───────────────────────────────────────────────────────────

        public void EquipWeapon(Equipment weapon)
        {
            Weapon = weapon;
        }

        public void EquipArmor(Equipment armor)
        {
            Armor = armor;
        }
    }
}
