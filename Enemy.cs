using System;

namespace RPGGame
{
    public class Enemy
    {
        public string Name { get; }
        public string Description { get; }
        public int MaxHP { get; }
        public int HP { get; private set; }
        public int Attack { get; }
        public int Defense { get; }
        public int EXPReward { get; }
        public ConsoleColor NameColor { get; }

        // Status
        public bool IsStunned { get; private set; }
        public int StunTurns { get; private set; }
        public bool IsBurning { get; private set; }
        public int BurnDamage { get; private set; }
        public int BurnTurns { get; private set; }

        // Special attack
        public bool HasSpecial { get; private set; }
        public string SpecialName { get; private set; } = "";
        public int SpecialDamage { get; private set; }
        public int SpecialChance { get; private set; } // 0-100

        public bool IsAlive => HP > 0;

        public Enemy(string name, string desc, int hp, int atk, int def,
                     int exp, ConsoleColor color = ConsoleColor.Red)
        {
            Name = name; Description = desc;
            MaxHP = hp; HP = hp;
            Attack = atk; Defense = def;
            EXPReward = exp;
            NameColor = color;
        }

        public void SetSpecial(string name, int damage, int chance)
        {
            HasSpecial = true;
            SpecialName = name;
            SpecialDamage = damage;
            SpecialChance = chance;
        }

        // ── Combat ──────────────────────────────────────────────────────────

        /// <summary>Returns actual damage dealt after enemy defense.</summary>
        public int TakeDamage(int rawDamage)
        {
            int actual = Math.Max(1, rawDamage - Defense);
            HP = Math.Max(0, HP - actual);
            return actual;
        }

        public void ApplyBurn(int dmgPerTurn, int turns)
        {
            IsBurning = true;
            BurnDamage = dmgPerTurn;
            BurnTurns = turns;
        }

        /// <summary>Processes end-of-turn burn. Returns damage dealt (0 if not burning).</summary>
        public int TickBurn()
        {
            if (!IsBurning) return 0;
            HP = Math.Max(0, HP - BurnDamage);
            int dmg = BurnDamage;
            BurnTurns--;
            if (BurnTurns <= 0) IsBurning = false;
            return dmg;
        }

        public void ApplyStun(int turns)
        {
            IsStunned = true;
            StunTurns = turns;
        }

        /// <summary>Returns true if the enemy is (still) stunned this turn.</summary>
        public bool TickStun()
        {
            if (!IsStunned) return false;
            bool wasStunned = true;
            StunTurns--;
            if (StunTurns <= 0) IsStunned = false;
            return wasStunned;
        }

        // ── Display ─────────────────────────────────────────────────────────

        public void PrintStatus()
        {
            Console.Write("\n  【");
            Console.ForegroundColor = NameColor;
            Console.Write(Name);
            Console.ResetColor();
            Console.Write("】");

            if (IsStunned)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" [眩暈]");
            }
            if (IsBurning)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($" [燃燒x{BurnTurns}]");
            }
            Console.ResetColor();
            Console.WriteLine();

            Console.Write("  HP ");
            Utils.DrawProgressBar(HP, MaxHP, 20, ConsoleColor.DarkRed);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($" {HP}/{MaxHP}");
        }

        // ── Factory Methods ─────────────────────────────────────────────────

        public static Enemy CreateSlime() =>
            new Enemy("史萊姆", "一個搖搖晃晃的藍色史萊姆。看起來不太危險。",
                hp: 45, atk: 8, def: 1, exp: 30,
                color: ConsoleColor.Cyan);

        public static Enemy CreateGoblinKnight()
        {
            var e = new Enemy("哥布林騎士", "身著破爛盔甲、手持生鏽長矛的哥布林，眼神兇狠。",
                hp: 85, atk: 14, def: 5, exp: 65,
                color: ConsoleColor.Green);
            e.SetSpecial("毒矛突刺", 22, 28);
            return e;
        }

        public static Enemy CreateShadowWraith()
        {
            var e = new Enemy("暗影幽靈", "漂浮在黑暗中的靈體，眼中燃燒著仇恨的火焰。",
                hp: 70, atk: 18, def: 3, exp: 78,
                color: ConsoleColor.DarkGray);
            e.SetSpecial("靈魂侵蝕", 26, 32);
            return e;
        }

        public static Enemy CreateDarkPaladin()
        {
            var e = new Enemy("黑暗聖騎士", "曾經護衛王國的騎士，被魔王的力量徹底腐化。",
                hp: 140, atk: 23, def: 12, exp: 130,
                color: ConsoleColor.DarkRed);
            e.SetSpecial("黑暗審判", 36, 30);
            return e;
        }

        public static Enemy CreateDemonKing()
        {
            var e = new Enemy("魔王・夜陌魯斯", "統治黑暗三年的魔王，其力量超越人類想像的極限。",
                hp: 260, atk: 28, def: 14, exp: 500,
                color: ConsoleColor.DarkMagenta);
            e.SetSpecial("末日炎焰", 48, 35);
            return e;
        }
    }
}
