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
        public static List<Skill> GetStarterSkills()
        {
            return new List<Skill>
            {
                new Skill(
                    name:         L10n.Get("SKILL_FIREBALL_NAME"),
                    desc:         L10n.Get("SKILL_FIREBALL_DESC"),
                    mpCost:       15,
                    dmgMult:      2.2,
                    effect:       SkillEffect.Burn,
                    effectChance: 0.45f,
                    color:        ConsoleColor.Red
                ),
                new Skill(
                    name:         L10n.Get("SKILL_HEAL_NAME"),
                    desc:         L10n.Get("SKILL_HEAL_DESC"),
                    mpCost:       20,
                    dmgMult:      0,
                    isHeal:       true,
                    healAmount:   35,
                    color:        ConsoleColor.Green
                ),
                new Skill(
                    name:         L10n.Get("SKILL_SHIELD_NAME"),
                    desc:         L10n.Get("SKILL_SHIELD_DESC"),
                    mpCost:       10,
                    dmgMult:      1.6,
                    effect:       SkillEffect.Stun,
                    effectChance: 0.40f,
                    color:        ConsoleColor.Cyan
                ),
                new Skill(
                    name:         L10n.Get("SKILL_BERSERK_NAME"),
                    desc:         L10n.Get("SKILL_BERSERK_DESC"),
                    mpCost:       0,
                    rageCost:     40,
                    dmgMult:      3.0,
                    effect:       SkillEffect.Critical,
                    effectChance: 0.60f,
                    color:        ConsoleColor.Magenta
                )
            };
        }

        public static List<Skill> GetAdvancedSkills()
        {
            return new List<Skill>
            {
                new Skill(
                    name:         L10n.Get("SKILL_ICE_NAME"),
                    desc:         L10n.Get("SKILL_ICE_DESC"),
                    mpCost:       28,
                    dmgMult:      2.5,
                    effect:       SkillEffect.Stun,
                    effectChance: 1.0f,
                    color:        ConsoleColor.Blue
                ),
                new Skill(
                    name:         L10n.Get("SKILL_GREATHEAL_NAME"),
                    desc:         L10n.Get("SKILL_GREATHEAL_DESC"),
                    mpCost:       32,
                    dmgMult:      0,
                    isHeal:       true,
                    healAmount:   65,
                    color:        ConsoleColor.Green
                )
            };
        }

        public static int CalculateSkillDamage(int attackStat, Skill skill, bool isBerserk, Random rng)
        {
            double base_ = attackStat * skill.DamageMultiplier;
            if (isBerserk) base_ *= 1.5;
            double variance = 0.88 + rng.NextDouble() * 0.24; // ±12%
            return Math.Max(1, (int)(base_ * variance));
        }
    }
}
