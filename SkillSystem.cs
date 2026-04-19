using System;
using System.Collections.Generic;

namespace RPGGame
{
    public enum SkillEffect
    {
        None,
        Burn,
        Stun,
        Critical
    }

    public class Skill
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MpCost { get; set; }
        public int RageCost { get; set; }
        public double DamageMultiplier { get; set; }
        public SkillEffect Effect { get; set; }
        public float EffectChance { get; set; }
        public bool IsHeal { get; set; }
        public int HealAmount { get; set; }
        public ConsoleColor Color { get; set; }

        public Skill(string name, string desc, int mpCost, double dmgMult,
                     SkillEffect effect = SkillEffect.None, float effectChance = 0f,
                     bool isHeal = false, int healAmount = 0, int rageCost = 0,
                     ConsoleColor color = ConsoleColor.Cyan)
        {
            Name = name;
            Description = desc;
            MpCost = mpCost;
            RageCost = rageCost;
            DamageMultiplier = dmgMult;
            Effect = effect;
            EffectChance = effectChance;
            IsHeal = isHeal;
            HealAmount = healAmount;
            Color = color;
        }
    }

    public static class SkillSystem
    {
        public static List<Skill> GetStarterSkills(PlayerClass cls) => cls switch
        {
            PlayerClass.Warrior  => GetWarriorSkills(),
            PlayerClass.Mage     => GetMageSkills(),
            PlayerClass.Assassin => GetAssassinSkills(),
            PlayerClass.Paladin  => GetPaladinSkills(),
            PlayerClass.Ranger   => GetRangerSkills(),
            _                    => GetWarriorSkills()
        };

        public static List<Skill> GetAdvancedSkills(PlayerClass cls) => cls switch
        {
            PlayerClass.Warrior  => GetWarriorAdvanced(),
            PlayerClass.Mage     => GetMageAdvanced(),
            PlayerClass.Assassin => GetAssassinAdvanced(),
            PlayerClass.Paladin  => GetPaladinAdvanced(),
            PlayerClass.Ranger   => GetRangerAdvanced(),
            _                    => GetWarriorAdvanced()
        };

        // ── Warrior ──────────────────────────────────────────────────────────

        private static List<Skill> GetWarriorSkills() => new()
        {
            new Skill(L10n.Get("SKILL_POWSLASH_NAME"), L10n.Get("SKILL_POWSLASH_DESC"),
                mpCost: 8, dmgMult: 1.9, color: ConsoleColor.Yellow),
            new Skill(L10n.Get("SKILL_HERBAL_NAME"), L10n.Get("SKILL_HERBAL_DESC"),
                mpCost: 18, dmgMult: 0, isHeal: true, healAmount: 30, color: ConsoleColor.Green),
            new Skill(L10n.Get("SKILL_SHIELD_NAME"), L10n.Get("SKILL_SHIELD_DESC"),
                mpCost: 10, dmgMult: 1.6, effect: SkillEffect.Stun, effectChance: 0.40f, color: ConsoleColor.Cyan),
            new Skill(L10n.Get("SKILL_BERSERK_NAME"), L10n.Get("SKILL_BERSERK_DESC"),
                mpCost: 0, rageCost: 40, dmgMult: 3.0, effect: SkillEffect.Critical, effectChance: 0.60f, color: ConsoleColor.Magenta)
        };

        private static List<Skill> GetWarriorAdvanced() => new()
        {
            new Skill(L10n.Get("SKILL_HURRICANE_NAME"), L10n.Get("SKILL_HURRICANE_DESC"),
                mpCost: 24, dmgMult: 2.6, effect: SkillEffect.Burn, effectChance: 0.35f, color: ConsoleColor.DarkYellow),
            new Skill(L10n.Get("SKILL_HOLYSHIELD_NAME"), L10n.Get("SKILL_HOLYSHIELD_DESC"),
                mpCost: 20, dmgMult: 0, isHeal: true, healAmount: 50, color: ConsoleColor.White)
        };

        // ── Mage ─────────────────────────────────────────────────────────────

        private static List<Skill> GetMageSkills() => new()
        {
            new Skill(L10n.Get("SKILL_FIREBALL_NAME"), L10n.Get("SKILL_FIREBALL_DESC"),
                mpCost: 15, dmgMult: 2.2, effect: SkillEffect.Burn, effectChance: 0.45f, color: ConsoleColor.Red),
            new Skill(L10n.Get("SKILL_HEAL_NAME"), L10n.Get("SKILL_HEAL_DESC"),
                mpCost: 20, dmgMult: 0, isHeal: true, healAmount: 35, color: ConsoleColor.Green),
            new Skill(L10n.Get("SKILL_FROSTSHOT_NAME"), L10n.Get("SKILL_FROSTSHOT_DESC"),
                mpCost: 12, dmgMult: 1.8, effect: SkillEffect.Stun, effectChance: 0.35f, color: ConsoleColor.Cyan),
            new Skill(L10n.Get("SKILL_ELEMBURST_NAME"), L10n.Get("SKILL_ELEMBURST_DESC"),
                mpCost: 0, rageCost: 40, dmgMult: 3.2, effect: SkillEffect.Burn, effectChance: 0.55f, color: ConsoleColor.Magenta)
        };

        private static List<Skill> GetMageAdvanced() => new()
        {
            new Skill(L10n.Get("SKILL_ICE_NAME"), L10n.Get("SKILL_ICE_DESC"),
                mpCost: 28, dmgMult: 2.5, effect: SkillEffect.Stun, effectChance: 1.0f, color: ConsoleColor.Blue),
            new Skill(L10n.Get("SKILL_GREATHEAL_NAME"), L10n.Get("SKILL_GREATHEAL_DESC"),
                mpCost: 32, dmgMult: 0, isHeal: true, healAmount: 65, color: ConsoleColor.Green)
        };

        // ── Assassin ─────────────────────────────────────────────────────────

        private static List<Skill> GetAssassinSkills() => new()
        {
            new Skill(L10n.Get("SKILL_SHADOWSTRIKE_NAME"), L10n.Get("SKILL_SHADOWSTRIKE_DESC"),
                mpCost: 12, dmgMult: 2.1, effect: SkillEffect.Critical, effectChance: 0.45f, color: ConsoleColor.DarkGray),
            new Skill(L10n.Get("SKILL_POISONBLADE_NAME"), L10n.Get("SKILL_POISONBLADE_DESC"),
                mpCost: 14, dmgMult: 1.5, effect: SkillEffect.Burn, effectChance: 0.60f, color: ConsoleColor.DarkGreen),
            new Skill(L10n.Get("SKILL_SMOKEBOMB_NAME"), L10n.Get("SKILL_SMOKEBOMB_DESC"),
                mpCost: 10, dmgMult: 1.2, effect: SkillEffect.Stun, effectChance: 0.50f, color: ConsoleColor.Gray),
            new Skill(L10n.Get("SKILL_ANNIHILATE_NAME"), L10n.Get("SKILL_ANNIHILATE_DESC"),
                mpCost: 0, rageCost: 40, dmgMult: 3.5, effect: SkillEffect.Critical, effectChance: 0.70f, color: ConsoleColor.Magenta)
        };

        private static List<Skill> GetAssassinAdvanced() => new()
        {
            new Skill(L10n.Get("SKILL_DEATHMARK_NAME"), L10n.Get("SKILL_DEATHMARK_DESC"),
                mpCost: 22, dmgMult: 2.4, effect: SkillEffect.Stun, effectChance: 0.60f, color: ConsoleColor.DarkMagenta),
            new Skill(L10n.Get("SKILL_LIFEDRAIN_NAME"), L10n.Get("SKILL_LIFEDRAIN_DESC"),
                mpCost: 26, dmgMult: 0, isHeal: true, healAmount: 50, color: ConsoleColor.DarkRed)
        };

        // ── Paladin ──────────────────────────────────────────────────────────

        private static List<Skill> GetPaladinSkills() => new()
        {
            new Skill(L10n.Get("SKILL_HOLYSTRIKE_NAME"), L10n.Get("SKILL_HOLYSTRIKE_DESC"),
                mpCost: 14, dmgMult: 1.8, effect: SkillEffect.Stun, effectChance: 0.30f, color: ConsoleColor.Yellow),
            new Skill(L10n.Get("SKILL_HOLYMEND_NAME"), L10n.Get("SKILL_HOLYMEND_DESC"),
                mpCost: 18, dmgMult: 0, isHeal: true, healAmount: 40, color: ConsoleColor.White),
            new Skill(L10n.Get("SKILL_DIVINESHIELD_NAME"), L10n.Get("SKILL_DIVINESHIELD_DESC"),
                mpCost: 10, dmgMult: 1.4, effect: SkillEffect.Stun, effectChance: 0.50f, color: ConsoleColor.Cyan),
            new Skill(L10n.Get("SKILL_DIVINEWRATH_NAME"), L10n.Get("SKILL_DIVINEWRATH_DESC"),
                mpCost: 0, rageCost: 40, dmgMult: 2.8, effect: SkillEffect.Burn, effectChance: 0.50f, color: ConsoleColor.DarkYellow)
        };

        private static List<Skill> GetPaladinAdvanced() => new()
        {
            new Skill(L10n.Get("SKILL_HEAVENJUDGE_NAME"), L10n.Get("SKILL_HEAVENJUDGE_DESC"),
                mpCost: 26, dmgMult: 3.0, effect: SkillEffect.Burn, effectChance: 0.45f, color: ConsoleColor.Yellow),
            new Skill(L10n.Get("SKILL_MIRACLEHEAL_NAME"), L10n.Get("SKILL_MIRACLEHEAL_DESC"),
                mpCost: 35, dmgMult: 0, isHeal: true, healAmount: 80, color: ConsoleColor.White)
        };

        // ── Ranger ───────────────────────────────────────────────────────────

        private static List<Skill> GetRangerSkills() => new()
        {
            new Skill(L10n.Get("SKILL_PIERCEARROW_NAME"), L10n.Get("SKILL_PIERCEARROW_DESC"),
                mpCost: 10, dmgMult: 2.0, color: ConsoleColor.DarkYellow),
            new Skill(L10n.Get("SKILL_POISONARROW_NAME"), L10n.Get("SKILL_POISONARROW_DESC"),
                mpCost: 14, dmgMult: 1.5, effect: SkillEffect.Burn, effectChance: 0.65f, color: ConsoleColor.DarkGreen),
            new Skill(L10n.Get("SKILL_SUPPSHOT_NAME"), L10n.Get("SKILL_SUPPSHOT_DESC"),
                mpCost: 10, dmgMult: 1.3, effect: SkillEffect.Stun, effectChance: 0.45f, color: ConsoleColor.Gray),
            new Skill(L10n.Get("SKILL_RAPIDSHOT_NAME"), L10n.Get("SKILL_RAPIDSHOT_DESC"),
                mpCost: 0, rageCost: 40, dmgMult: 2.8, effect: SkillEffect.Critical, effectChance: 0.65f, color: ConsoleColor.Magenta)
        };

        private static List<Skill> GetRangerAdvanced() => new()
        {
            new Skill(L10n.Get("SKILL_EXPLOARROW_NAME"), L10n.Get("SKILL_EXPLOARROW_DESC"),
                mpCost: 25, dmgMult: 2.8, effect: SkillEffect.Burn, effectChance: 0.55f, color: ConsoleColor.Red),
            new Skill(L10n.Get("SKILL_NATUREHEAL_NAME"), L10n.Get("SKILL_NATUREHEAL_DESC"),
                mpCost: 28, dmgMult: 0, isHeal: true, healAmount: 60, color: ConsoleColor.Green)
        };

        // ── Damage Calculation ────────────────────────────────────────────────

        public static int CalculateSkillDamage(int attackStat, Skill skill, bool isBerserk, Random rng)
        {
            double base_ = attackStat * skill.DamageMultiplier;
            if (isBerserk) base_ *= 1.5;
            double variance = 0.88 + rng.NextDouble() * 0.24; // ±12%
            return Math.Max(1, (int)(base_ * variance));
        }
    }
}
