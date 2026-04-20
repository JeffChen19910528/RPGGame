using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGGame
{
    public static class GameTests
    {
        private static int _passed;
        private static int _failed;
        private static readonly List<string> _failures = new();

        public static void RunAll()
        {
            try { Console.Clear(); } catch { }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╔════════════════════════════════════════════╗");
            Console.WriteLine("  ║          RPGGame 自動測試套件              ║");
            Console.WriteLine("  ╚════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            TestAllEnemies();
            TestRandomPools();
            TestPlayerCreation();
            TestBattleBasics();
            TestEnemyStatusEffects();

            PrintSummary();
        }

        // ── Enemy Factory Tests ───────────────────────────────────────────

        private static void TestAllEnemies()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  [敵人工廠測試]");
            Console.ResetColor();

            var enemies = new (string label, Func<Enemy> factory, int expectedHp, int expectedAtk)[]
            {
                ("史萊姆",       Enemy.CreateSlime,            45,  8),
                ("哥布林騎士",   Enemy.CreateGoblinKnight,     85, 14),
                ("骷髏弓手",     Enemy.CreateSkeletonArcher,   60, 13),
                ("毒沼蜥蜴",     Enemy.CreatePoisonLizard,     75, 15),
                ("暗影幽靈",     Enemy.CreateShadowWraith,     70, 18),
                ("黑暗聖騎士",   Enemy.CreateDarkPaladin,     140, 23),
                ("魔王・夜陌魯斯", Enemy.CreateDemonKing,     260, 28),
                // new enemies
                ("水晶蜘蛛",     Enemy.CreateCrystalSpider,   55, 11),
                ("吸血蝙蝠群",   Enemy.CreateVampireBatSwarm, 50, 10),
                ("冰霜女巫",     Enemy.CreateFrostWitch,      80, 17),
                ("憤怒石像",     Enemy.CreateAngryGolem,     120, 20),
                ("腐化樹妖",     Enemy.CreateCorruptedTreant, 95, 16),
                ("魔族哨兵",     Enemy.CreateDemonSoldier,    65, 12),
            };

            foreach (var (label, factory, hp, atk) in enemies)
            {
                var e = factory();
                Assert($"{label} HP={hp}",  e.MaxHP == hp);
                Assert($"{label} ATK={atk}", e.Attack == atk);
                Assert($"{label} IsAlive",   e.IsAlive);
                Assert($"{label} Name非空",  !string.IsNullOrEmpty(e.Name));
            }

            // Verify special attacks on new enemies
            var specials = new (string name, Func<Enemy> f)[]
            {
                ("水晶蜘蛛",   Enemy.CreateCrystalSpider),
                ("吸血蝙蝠群", Enemy.CreateVampireBatSwarm),
                ("冰霜女巫",   Enemy.CreateFrostWitch),
                ("憤怒石像",   Enemy.CreateAngryGolem),
                ("腐化樹妖",   Enemy.CreateCorruptedTreant),
                ("魔族哨兵",   Enemy.CreateDemonSoldier),
            };
            foreach (var (name, f) in specials)
            {
                var e = f();
                Assert($"{name} HasSpecial", e.HasSpecial);
                Assert($"{name} SpecialName非空", !string.IsNullOrEmpty(e.SpecialName));
                Assert($"{name} SpecialDmg>0",    e.SpecialDamage > 0);
                Assert($"{name} SpecialChance>0",  e.SpecialChance > 0);
            }

            Console.WriteLine();
        }

        // ── Random Pool Tests ─────────────────────────────────────────────

        private static void TestRandomPools()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  [隨機遭遇池測試]");
            Console.ResetColor();

            var rng = new Random(42);
            const int iterations = 200;

            var tier1Names = new HashSet<string>();
            var tier2ANames = new HashSet<string>();
            var tier2BNames = new HashSet<string>();

            for (int i = 0; i < iterations; i++)
            {
                tier1Names.Add(Enemy.GetRandomTier1Enemy(rng).Name);
                tier2ANames.Add(Enemy.GetRandomTier2EnemyA(rng).Name);
                tier2BNames.Add(Enemy.GetRandomTier2EnemyB(rng).Name);
            }

            // Tier 1 should include all 4 enemies
            Assert("Tier1 含史萊姆",     tier1Names.Contains("史萊姆"));
            Assert("Tier1 含骷髏弓手",   tier1Names.Contains("骷髏弓手"));
            Assert("Tier1 含水晶蜘蛛",   tier1Names.Contains("水晶蜘蛛"));
            Assert("Tier1 含吸血蝙蝠群", tier1Names.Contains("吸血蝙蝠群"));

            // Tier 2A should include 3 enemies
            Assert("Tier2A 含哥布林騎士", tier2ANames.Contains("哥布林騎士"));
            Assert("Tier2A 含毒沼蜥蜴",   tier2ANames.Contains("毒沼蜥蜴"));
            Assert("Tier2A 含冰霜女巫",   tier2ANames.Contains("冰霜女巫"));

            // Tier 2B should include 3 enemies
            Assert("Tier2B 含暗影幽靈",   tier2BNames.Contains("暗影幽靈"));
            Assert("Tier2B 含腐化樹妖",   tier2BNames.Contains("腐化樹妖"));
            Assert("Tier2B 含憤怒石像",   tier2BNames.Contains("憤怒石像"));

            Console.WriteLine();
        }

        // ── Player Creation Tests ─────────────────────────────────────────

        private static void TestPlayerCreation()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  [玩家創建測試]");
            Console.ResetColor();

            var classes = new[] {
                PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Assassin,
                PlayerClass.Paladin, PlayerClass.Ranger
            };

            foreach (var cls in classes)
            {
                var p = new Player("測試者") { Class = cls };
                p.InitClassSkills();
                Assert($"{cls} HP>0",        p.MaxHP > 0);
                Assert($"{cls} MP>0",        p.MaxMP > 0);
                Assert($"{cls} ATK>0",       p.BaseAttack > 0);
                Assert($"{cls} 腐化值初始=0", p.CorruptionLevel == 0);
                Assert($"{cls} ClassTitle非空", !string.IsNullOrEmpty(p.ClassTitle));
            }

            Console.WriteLine();
        }

        // ── Battle Basic Tests ────────────────────────────────────────────

        private static void TestBattleBasics()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  [戰鬥基礎測試]");
            Console.ResetColor();

            var e = Enemy.CreateSlime();
            int before = e.HP;
            int actual = e.TakeDamage(20);
            Assert("史萊姆受傷後HP降低", e.HP < before);
            Assert("實際傷害>=1",        actual >= 1);
            Assert("實際傷害計算正確",   e.HP == before - actual);

            // Burn test
            e.ApplyBurn(5, 3);
            Assert("燃燒狀態啟動",  e.IsBurning);
            Assert("燃燒回合=3",    e.BurnTurns == 3);
            int burnDmg = e.TickBurn();
            Assert("燃燒傷害=5",    burnDmg == 5);
            Assert("燃燒回合剩2",   e.BurnTurns == 2);

            // Stun test
            var e2 = Enemy.CreateGoblinKnight();
            e2.ApplyStun(2);
            Assert("眩暈狀態啟動",    e2.IsStunned);
            Assert("眩暈第一回合=true", e2.TickStun());
            Assert("眩暈剩1回合",      e2.StunTurns == 1);
            Assert("眩暈第二回合=true", e2.TickStun());
            Assert("眩暈已解除",       !e2.IsStunned);

            // Death test
            var e3 = Enemy.CreateVampireBatSwarm();
            e3.TakeDamage(9999);
            Assert("吸血蝙蝠群可被擊殺", !e3.IsAlive);
            Assert("死亡後HP=0",         e3.HP == 0);

            Console.WriteLine();
        }

        // ── Enemy Status Effect Edge Cases ────────────────────────────────

        private static void TestEnemyStatusEffects()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  [狀態效果邊界測試]");
            Console.ResetColor();

            // Burn expires correctly
            var e = Enemy.CreateCrystalSpider();
            e.ApplyBurn(3, 1);
            e.TickBurn();
            Assert("燃燒一回合後自然消除", !e.IsBurning);

            // TakeDamage minimum = 1 (high defense vs low attack)
            var golem = Enemy.CreateAngryGolem(); // def=8
            int dmg = golem.TakeDamage(5); // 5-8 = -3 → should be 1
            Assert("最低傷害=1", dmg == 1);

            // HP never goes below 0
            var witch = Enemy.CreateFrostWitch();
            witch.TakeDamage(99999);
            Assert("HP不會低於0", witch.HP == 0);

            // New enemies have valid NameColor
            var newEnemies = new Func<Enemy>[]
            {
                Enemy.CreateCrystalSpider, Enemy.CreateVampireBatSwarm,
                Enemy.CreateFrostWitch,    Enemy.CreateAngryGolem,
                Enemy.CreateCorruptedTreant, Enemy.CreateDemonSoldier
            };
            foreach (var f in newEnemies)
            {
                var en = f();
                Assert($"{en.Name} NameColor有效", Enum.IsDefined(typeof(ConsoleColor), en.NameColor));
            }

            Console.WriteLine();
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static void Assert(string testName, bool condition)
        {
            if (condition)
            {
                _passed++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"    ✔ {testName}");
            }
            else
            {
                _failed++;
                _failures.Add(testName);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"    ✘ {testName}");
            }
            Console.ResetColor();
        }

        private static void PrintSummary()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.WriteLine($"  測試結果：{_passed + _failed} 項   通過 {_passed}   失敗 {_failed}");
            Console.WriteLine("  ══════════════════════════════════════════════");

            if (_failures.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  失敗項目：");
                foreach (var f in _failures)
                    Console.WriteLine($"    • {f}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  所有測試通過！遊戲內容正確。");
            }

            Console.ResetColor();
            if (Console.IsInputRedirected)
            {
                Console.WriteLine();
                return;
            }
            Console.WriteLine("\n  按任意鍵返回主選單...");
            Console.ReadKey(true);
        }
    }
}
