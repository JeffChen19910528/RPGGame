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

        public static Enemy CreateSkeletonArcher()
        {
            var e = new Enemy("骷髏弓手", "死亡魔法所驅使的骨骸弓手，空洞的眼眶中透出冰冷的殺意。",
                hp: 60, atk: 13, def: 1, exp: 58,
                color: ConsoleColor.Gray);
            e.SetSpecial("骨箭齊射", 20, 35);
            return e;
        }

        public static Enemy CreatePoisonLizard()
        {
            var e = new Enemy("毒沼蜥蜴", "棲息在腐敗沼澤中的巨型蜥蜴，皮膚分泌劇毒黏液。",
                hp: 75, atk: 15, def: 4, exp: 70,
                color: ConsoleColor.DarkGreen);
            e.SetSpecial("毒液噴吐", 24, 30);
            return e;
        }

        public static Enemy CreateCrystalSpider()
        {
            var e = new Enemy("水晶蜘蛛", "身體由半透明水晶構成的詭異蜘蛛，每踏一步都發出清脆的碎裂聲。",
                hp: 55, atk: 11, def: 2, exp: 48,
                color: ConsoleColor.Cyan);
            e.SetSpecial("水晶碎射", 20, 30);
            return e;
        }

        public static Enemy CreateVampireBatSwarm()
        {
            var e = new Enemy("吸血蝙蝠群", "數十隻被魔力感染的蝙蝠集結成群，翅膀拍動聲如同死亡的鼓點。",
                hp: 50, atk: 10, def: 1, exp: 40,
                color: ConsoleColor.DarkGray);
            e.SetSpecial("群體吸血", 18, 35);
            return e;
        }

        public static Enemy CreateFrostWitch()
        {
            var e = new Enemy("冰霜女巫", "在暗黑森林深處修行的老巫婆，嘴角永遠掛著令人不寒而慄的笑。",
                hp: 80, atk: 17, def: 3, exp: 82,
                color: ConsoleColor.Blue);
            e.SetSpecial("冰凍咒術", 28, 30);
            return e;
        }

        public static Enemy CreateAngryGolem()
        {
            var e = new Enemy("憤怒石像", "被魔王怨念喚醒的廢棄神殿守衛，每一步都讓大地顫抖。",
                hp: 120, atk: 20, def: 8, exp: 95,
                color: ConsoleColor.DarkYellow);
            e.SetSpecial("石拳暴砸", 32, 25);
            return e;
        }

        public static Enemy CreateCorruptedTreant()
        {
            var e = new Enemy("腐化樹妖", "曾是守護森林的古老樹靈，被黑暗之力腐化，成了憤怒的怪物。",
                hp: 95, atk: 16, def: 6, exp: 88,
                color: ConsoleColor.DarkGreen);
            e.SetSpecial("毒藤纏繞", 25, 28);
            return e;
        }

        public static Enemy CreateDemonSoldier()
        {
            var e = new Enemy("魔族哨兵", "魔王城的普通守衛，剛才還在喝湯，現在被迫作戰，表情十分委屈。",
                hp: 65, atk: 12, def: 3, exp: 55,
                color: ConsoleColor.DarkRed);
            e.SetSpecial("慌亂攻擊", 18, 25);
            return e;
        }

        // ── Random Encounter Pools ───────────────────────────────────────────

        public static Enemy GetRandomTier1Enemy(Random rng)
        {
            return rng.Next(4) switch
            {
                0 => CreateSlime(),
                1 => CreateSkeletonArcher(),
                2 => CreateCrystalSpider(),
                _ => CreateVampireBatSwarm()
            };
        }

        public static Enemy GetRandomTier2EnemyA(Random rng)
        {
            return rng.Next(3) switch
            {
                0 => CreateGoblinKnight(),
                1 => CreateFrostWitch(),
                _ => CreatePoisonLizard()
            };
        }

        public static Enemy GetRandomTier2EnemyB(Random rng)
        {
            return rng.Next(3) switch
            {
                0 => CreateShadowWraith(),
                1 => CreateCorruptedTreant(),
                _ => CreateAngryGolem()
            };
        }
    }
}
