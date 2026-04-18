using System;

namespace RPGGame
{
    public enum BattleResult { Victory, Defeat }

    public class BattleSystem
    {
        private readonly Player _player;
        private readonly Random _rng;

        public BattleSystem(Player player, Random rng)
        {
            _player = player;
            _rng = rng;
        }

        public BattleResult StartBattle(Enemy enemy, bool isFinalBoss = false)
        {
            AnimationSystem.ShowEnemyIntro(enemy);
            Utils.PressAnyKey("[ 按任意鍵開始戰鬥... ]");

            int turn = 0;

            while (_player.IsAlive && enemy.IsAlive)
            {
                turn++;

                // Redraw full battle screen at start of each turn
                AnimationSystem.DrawBattleScreen(_player, enemy, turn);

                // Player burn tick
                if (!ProcessPlayerBurn(enemy)) break;

                // Player action
                if (_player.CurrentStatus == StatusEffect.Stun)
                {
                    Log(ConsoleColor.Yellow, $"  {_player.Name} 眩暈中，無法行動！");
                    _player.CurrentStatus = StatusEffect.None;
                    _player.StatusTurns = 0;
                }
                else
                {
                    DoPlayerTurn(enemy);
                }

                if (!enemy.IsAlive) break;

                // Berserk countdown
                _player.UpdateBerserkTick();

                // Enemy turn
                DoEnemyTurn(enemy);

                // Enemy burn tick
                if (enemy.IsAlive && enemy.IsBurning)
                {
                    int burnDmg = enemy.TickBurn();
                    if (burnDmg > 0)
                    {
                        Log(ConsoleColor.DarkRed, $"  {enemy.Name} 受到灼燒傷害 {burnDmg}！(HP: {enemy.HP}/{enemy.MaxHP})");
                        if (!enemy.IsAlive)
                        {
                            Log(ConsoleColor.Yellow, "  燃燒使其倒下了！");
                        }
                    }
                }
            }

            return ResolveBattle(enemy, isFinalBoss);
        }

        // ── Player side ─────────────────────────────────────────────────────

        private bool ProcessPlayerBurn()
        {
            if (_player.CurrentStatus != StatusEffect.Burn) return true;
            int dmg = _player.BurnDamagePerTurn > 0 ? _player.BurnDamagePerTurn : 6;
            _player.HP = Math.Max(0, _player.HP - dmg);
            Log(ConsoleColor.DarkRed, $"  {_player.Name} 受到灼燒傷害 {dmg}！(HP: {_player.HP}/{_player.MaxHP})");
            _player.StatusTurns--;
            if (_player.StatusTurns <= 0)
            {
                _player.CurrentStatus = StatusEffect.None;
                _player.BurnDamagePerTurn = 0;
            }
            return _player.IsAlive;
        }

        private bool ProcessPlayerBurn(Enemy _) => ProcessPlayerBurn();

        private void DoPlayerTurn(Enemy enemy)
        {
            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  【行動選擇】");
                Console.ResetColor();
                Console.WriteLine("  [1] 普通攻擊");
                Console.WriteLine("  [2] 使用技能");
                Console.WriteLine("  [3] 防禦姿態（受到傷害減半）");
                Console.WriteLine("  [4] 查看狀態");

                int choice = Utils.GetChoice("選擇行動", 1, 4);
                switch (choice)
                {
                    case 1: DoNormalAttack(enemy); return;
                    case 2: if (DoSkillMenu(enemy)) return; break;
                    case 3: DoDefend(); return;
                    case 4:
                        // Redraw the screen to show updated stats
                        AnimationSystem.DrawBattleScreen(_player, enemy, 0);
                        break;
                }
            }
        }

        private void AddRageWithBerserkCheck(int amount)
        {
            bool wasBerserk = _player.IsBerserk;
            _player.AddRage(amount);
            if (!wasBerserk && _player.IsBerserk)
                AnimationSystem.AnimateBerserkActivation(_player);
        }

        private void DoNormalAttack(Enemy enemy)
        {
            AddRageWithBerserkCheck(15);

            int rawDamage = _player.Attack;
            if (_player.IsBerserk) rawDamage = (int)(rawDamage * 1.5);

            // Berserk backfire
            if (_player.IsBerserk && _rng.Next(100) < 20)
            {
                int self = Math.Max(1, (int)(_player.BaseAttack * 0.35));
                _player.HP = Math.Max(0, _player.HP - self);
                _player.CorruptionLevel++;
                AnimationSystem.UpdatePlayerArt(_player, "hurt");
                Log(ConsoleColor.DarkMagenta, $"  ★ 暴走失控！{_player.Name} 反噬自身 {self} 點！(HP: {_player.HP}/{_player.MaxHP})");
                Utils.Pause(200);
                if (!_player.IsAlive) return;
            }

            double variance = 0.88 + _rng.NextDouble() * 0.24;
            int damage = Math.Max(1, (int)(rawDamage * variance));
            int actual = enemy.TakeDamage(damage);

            // Play animation
            AnimationSystem.AnimatePlayerAttack(_player, enemy);

            string mark = _player.IsBerserk ? " ★暴走★" : "";
            Log(ConsoleColor.Yellow, $"  {_player.Name}{mark} 攻擊 {enemy.Name} → 造成 {actual} 點傷害！");

            if (_player.CurrentStatus == StatusEffect.Defending)
                _player.CurrentStatus = StatusEffect.None;
        }

        private bool DoSkillMenu(Enemy enemy)
        {
            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  【技能列表】");
                Console.ResetColor();

                var skills = _player.Skills;
                for (int i = 0; i < skills.Count; i++)
                {
                    var s = skills[i];
                    bool canUse = s.MpCost <= _player.MP && s.RageCost <= _player.RageEnergy;
                    string cost = s.MpCost > 0 ? $"MP:{s.MpCost}" : $"怒:{s.RageCost}";
                    Console.ForegroundColor = canUse ? s.Color : ConsoleColor.DarkGray;
                    Console.WriteLine($"  [{i + 1}] {s.Name,-8} ({cost,6}) - {s.Description}");
                    Console.ResetColor();
                }
                Console.WriteLine("  [0] 返回");

                int choice = Utils.GetChoice("選擇技能", 0, skills.Count);
                if (choice == 0) return false;

                var skill = skills[choice - 1];

                if (skill.MpCost > _player.MP)
                {
                    Log(ConsoleColor.Red, "  ✗ MP 不足！"); Utils.Pause(400); continue;
                }
                if (skill.RageCost > _player.RageEnergy)
                {
                    Log(ConsoleColor.Red, "  ✗ 怒氣不足！"); Utils.Pause(400); continue;
                }

                UseSkill(skill, enemy);
                return true;
            }
        }

        private void UseSkill(Skill skill, Enemy enemy)
        {
            _player.MP -= skill.MpCost;
            _player.RageEnergy -= skill.RageCost;

            Log(skill.Color, $"\n  ✦ {_player.Name} 使用了【{skill.Name}】！");
            Utils.Pause(150);

            if (skill.IsHeal)
            {
                AnimationSystem.AnimateHeal(_player);
                _player.Heal(skill.HealAmount);
                Log(ConsoleColor.Green, $"  ♥ 恢復了 {skill.HealAmount} 點 HP！(HP: {_player.HP}/{_player.MaxHP})");
                return;
            }

            // Rage-skill backfire
            if (skill.RageCost > 0 && _rng.Next(100) < 22)
            {
                int selfDmg = Math.Max(1, (int)(_player.BaseAttack * 0.4));
                _player.HP = Math.Max(0, _player.HP - selfDmg);
                _player.CorruptionLevel++;
                AnimationSystem.UpdatePlayerArt(_player, "hurt");
                Log(ConsoleColor.DarkMagenta, $"  ★ 力量失控！{_player.Name} 反噬 {selfDmg} 點！");
                Utils.Pause(200);
                if (!_player.IsAlive) return;
            }

            AddRageWithBerserkCheck(20);

            int dmg = SkillSystem.CalculateSkillDamage(_player.Attack, skill, _player.IsBerserk, _rng);
            int actualDmg = enemy.TakeDamage(dmg);

            AnimationSystem.AnimateSkill(_player, enemy, skill);

            Log(ConsoleColor.Yellow, $"  → 對 {enemy.Name} 造成 {actualDmg} 點傷害！");

            // Apply effect
            if (skill.Effect != SkillEffect.None && _rng.NextDouble() < skill.EffectChance)
                ApplyEffect(skill.Effect, enemy);

            if (_player.CurrentStatus == StatusEffect.Defending)
                _player.CurrentStatus = StatusEffect.None;
        }

        private void ApplyEffect(SkillEffect effect, Enemy enemy)
        {
            switch (effect)
            {
                case SkillEffect.Burn:
                    enemy.ApplyBurn(9, 3);
                    Log(ConsoleColor.Red, $"  🔥 {enemy.Name} 陷入燃燒狀態！(每回合 9 點)");
                    break;
                case SkillEffect.Stun:
                    enemy.ApplyStun(1);
                    Log(ConsoleColor.Yellow, $"  ⚡ {enemy.Name} 被眩暈了！下回合無法行動！");
                    break;
                case SkillEffect.Critical:
                    int extra = Math.Max(1, (int)(_player.Attack * 0.9));
                    enemy.TakeDamage(extra);
                    Log(ConsoleColor.Magenta, $"  ★★ 追加暴擊！額外 {extra} 點傷害！");
                    break;
            }
        }

        private void DoDefend()
        {
            _player.CurrentStatus = StatusEffect.Defending;
            AddRageWithBerserkCheck(5);
            AnimationSystem.AnimateDefend(_player);
            Log(ConsoleColor.Cyan, $"  🛡 {_player.Name} 進入防禦姿態！本回合受到傷害減半。");
        }

        // ── Enemy side ──────────────────────────────────────────────────────

        private void DoEnemyTurn(Enemy enemy)
        {
            Utils.Pause(300);

            if (enemy.TickStun())
            {
                Log(ConsoleColor.Yellow, $"\n  {enemy.Name} 眩暈中，無法行動！");
                return;
            }

            bool useSpecial = enemy.HasSpecial && _rng.Next(100) < enemy.SpecialChance;
            int rawDamage = useSpecial ? enemy.SpecialDamage : enemy.Attack;

            bool wasDefending = _player.CurrentStatus == StatusEffect.Defending;
            int actualDamage = _player.TakeDamage(rawDamage);
            AddRageWithBerserkCheck(useSpecial ? 25 : 12);

            AnimationSystem.AnimateEnemyAttack(enemy, _player, useSpecial);

            if (wasDefending)
            {
                Log(ConsoleColor.Cyan, $"  🛡 防禦！");
                _player.CurrentStatus = StatusEffect.None;
            }

            if (useSpecial)
                Log(ConsoleColor.DarkRed, $"  ☠ {enemy.Name} 使用【{enemy.SpecialName}】！→ {_player.Name} 受到 {actualDamage} 點傷害！(HP: {_player.HP}/{_player.MaxHP})");
            else
                Log(ConsoleColor.Red, $"  {enemy.Name} 攻擊 {_player.Name} → 受到 {actualDamage} 點傷害！(HP: {_player.HP}/{_player.MaxHP})");
        }

        // ── Resolution ──────────────────────────────────────────────────────

        private BattleResult ResolveBattle(Enemy enemy, bool isFinalBoss)
        {
            if (_player.IsAlive)
            {
                AnimationSystem.AnimateEnemyDeath(enemy);
                AnimationSystem.ShowVictoryBanner(enemy);

                if (isFinalBoss && _player.IsBerserk)
                    _player.FinalBossDefeatedInBerserk = true;

                bool leveled = _player.GainEXP(enemy.EXPReward);
                Log(ConsoleColor.Cyan, $"  ▶ 獲得 {enemy.EXPReward} EXP！");

                if (leveled)
                {
                    Log(ConsoleColor.Yellow, $"\n  ★★ 升級！{_player.Name} 達到 Lv.{_player.Level}！");
                    Log(ConsoleColor.Yellow, "     HP+20 / MP+10 / ATK+3 / DEF+2");
                }

                _player.RestoreMP(15);
                Log(ConsoleColor.Blue, "  ▶ 戰鬥後恢復 15 MP");

                Utils.PressAnyKey();
                return BattleResult.Victory;
            }
            else
            {
                AnimationSystem.ShowDefeatBanner();
                Log(ConsoleColor.Red, $"\n  {_player.Name} 倒下了...");
                Utils.PressAnyKey();
                return BattleResult.Defeat;
            }
        }

        // ── Helpers ─────────────────────────────────────────────────────────

        private static void Log(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
