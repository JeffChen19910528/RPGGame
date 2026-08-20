using System;
using System.Collections.Generic;

namespace RPGGame.Domain
{
    /// <summary>Starting stat deltas and intro flavor for a playable class.</summary>
    public sealed class ClassDefinition
    {
        public PlayerClass Class { get; }
        public int HpDelta { get; }
        public int MpDelta { get; }
        public int AttackDelta { get; }
        public int DefenseDelta { get; }
        public string IntroKey { get; }
        public ConsoleColor IntroColor { get; }

        public ClassDefinition(PlayerClass cls, int hpDelta, int mpDelta, int attackDelta, int defenseDelta,
                                string introKey, ConsoleColor introColor)
        {
            Class = cls;
            HpDelta = hpDelta;
            MpDelta = mpDelta;
            AttackDelta = attackDelta;
            DefenseDelta = defenseDelta;
            IntroKey = introKey;
            IntroColor = introColor;
        }
    }

    public static class ClassDefinitions
    {
        private static readonly Dictionary<PlayerClass, ClassDefinition> All = new()
        {
            [PlayerClass.Warrior]  = new ClassDefinition(PlayerClass.Warrior,  hpDelta: 30, mpDelta: 0,  attackDelta: 0,  defenseDelta: 5,
                                          "INTRO_WARRIOR",  ConsoleColor.Yellow),
            [PlayerClass.Mage]     = new ClassDefinition(PlayerClass.Mage,     hpDelta: 0,  mpDelta: 30, attackDelta: 3,  defenseDelta: 0,
                                          "INTRO_MAGE",     ConsoleColor.Cyan),
            [PlayerClass.Assassin] = new ClassDefinition(PlayerClass.Assassin, hpDelta: -10, mpDelta: 0, attackDelta: 8,  defenseDelta: 0,
                                          "INTRO_ASSASSIN", ConsoleColor.DarkGray),
            [PlayerClass.Paladin]  = new ClassDefinition(PlayerClass.Paladin,  hpDelta: 40, mpDelta: 0,  attackDelta: -2, defenseDelta: 8,
                                          "INTRO_PALADIN",  ConsoleColor.Yellow),
            [PlayerClass.Ranger]   = new ClassDefinition(PlayerClass.Ranger,   hpDelta: 0,  mpDelta: 20, attackDelta: 5,  defenseDelta: 0,
                                          "INTRO_RANGER",   ConsoleColor.DarkGreen),
        };

        public static ClassDefinition Get(PlayerClass cls) => All[cls];

        public static ClassDefinition FromMenuChoice(int choice) => choice switch
        {
            1 => All[PlayerClass.Warrior],
            2 => All[PlayerClass.Mage],
            3 => All[PlayerClass.Assassin],
            4 => All[PlayerClass.Paladin],
            _ => All[PlayerClass.Ranger],
        };

        public static void ApplyStartingBonus(Player player, ClassDefinition def)
        {
            player.Class = def.Class;
            player.MaxHP += def.HpDelta; player.HP += def.HpDelta;
            player.MaxMP += def.MpDelta; player.MP += def.MpDelta;
            player.BaseAttack += def.AttackDelta;
            player.BaseDefense += def.DefenseDelta;
        }
    }
}
