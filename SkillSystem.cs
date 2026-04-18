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
                    name:       "火球術",
                    desc:       "召喚火焰轟炸，有機率使敵人燃燒",
                    mpCost:     15,
                    dmgMult:    2.2,
                    effect:     SkillEffect.Burn,
                    effectChance: 0.45f,
                    color:      ConsoleColor.Red
                ),
                new Skill(
                    name:       "治癒術",
                    desc:       "消耗 MP 恢復自身 HP",
                    mpCost:     20,
                    dmgMult:    0,
                    isHeal:     true,
                    healAmount: 35,
                    color:      ConsoleColor.Green
                ),
                new Skill(
                    name:       "盾擊",
                    desc:       "近身衝撞，有機率使敵人眩暈",
                    mpCost:     10,
                    dmgMult:    1.6,
                    effect:     SkillEffect.Stun,
                    effectChance: 0.40f,
                    color:      ConsoleColor.Cyan
                ),
                new Skill(
                    name:       "暴走衝擊",
                    desc:       "消耗 40 怒氣，釋放毀滅性打擊，高機率暴擊",
                    mpCost:     0,
                    rageCost:   40,
                    dmgMult:    3.0,
                    effect:     SkillEffect.Critical,
                    effectChance: 0.60f,
                    color:      ConsoleColor.Magenta
                )
            };
        }

        public static List<Skill> GetAdvancedSkills()
        {
            return new List<Skill>
            {
                new Skill(
                    name:       "冰霜新星",
                    desc:       "大範圍冰霜衝擊，必定眩暈敵人",
                    mpCost:     28,
                    dmgMult:    2.5,
                    effect:     SkillEffect.Stun,
                    effectChance: 1.0f,
                    color:      ConsoleColor.Blue
                ),
                new Skill(
                    name:       "大治癒術",
                    desc:       "強力治癒魔法，大量恢復 HP",
                    mpCost:     32,
                    dmgMult:    0,
                    isHeal:     true,
                    healAmount: 65,
                    color:      ConsoleColor.Green
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
