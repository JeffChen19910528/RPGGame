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
            Utils.PressAnyKey(L10n.Get("BATTLE_START"));

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
                    Log(ConsoleColor.Yellow, L10n.Get("BATTLE_STUN_PLAYER", _player.Name));
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
                        Log(ConsoleColor.DarkRed, L10n.Get("BATTLE_BURN_ENEMY", enemy.Name, burnDmg, enemy.HP, enemy.MaxHP));
                        if (!enemy.IsAlive)
                        {
                            Log(ConsoleColor.Yellow, L10n.Get("BATTLE_BURN_KILL"));
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
            Log(ConsoleColor.DarkRed, L10n.Get("BATTLE_BURN_PLAYER", _player.Name, dmg, _player.HP, _player.MaxHP));
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
                Console.WriteLine(L10n.Get("BATTLE_ACTION_TITLE"));
                Console.ResetColor();
                Console.WriteLine(L10n.Get("BATTLE_ACTION_1"));
                Console.WriteLine(L10n.Get("BATTLE_ACTION_2"));
                Console.WriteLine(L10n.Get("BATTLE_ACTION_3"));
                Console.WriteLine(L10n.Get("BATTLE_ACTION_4"));

                int choice = Utils.GetChoice(L10n.Get("BATTLE_ACTION_SELECT"), 1, 4);
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
                Log(ConsoleColor.DarkMagenta, L10n.Get("BATTLE_BACKFIRE", _player.Name, self, _player.HP, _player.MaxHP));
                Utils.Pause(200);
                if (!_player.IsAlive) return;
            }

            double variance = 0.88 + _rng.NextDouble() * 0.24;
            int damage = Math.Max(1, (int)(rawDamage * variance));
            int actual = enemy.TakeDamage(damage);

            // Play animation
            AnimationSystem.AnimatePlayerAttack(_player, enemy);

            string mark = _player.IsBerserk ? L10n.Get("BATTLE_BERSERK_TAG") : "";
            Log(ConsoleColor.Yellow, L10n.Get("BATTLE_ATTACK_HIT", _player.Name, mark, enemy.Name, actual));

            if (_player.CurrentStatus == StatusEffect.Defending)
                _player.CurrentStatus = StatusEffect.None;
        }

        private bool DoSkillMenu(Enemy enemy)
        {
            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(L10n.Get("BATTLE_SKILL_TITLE"));
                Console.ResetColor();

                var skills = _player.Skills;
                string rageLabel = L10n.Current == Language.English ? "RAGE" : "怒";
                for (int i = 0; i < skills.Count; i++)
                {
                    var s = skills[i];
                    bool canUse = s.MpCost <= _player.MP && s.RageCost <= _player.RageEnergy;
                    string cost = s.MpCost > 0 ? $"MP:{s.MpCost}" : $"{rageLabel}:{s.RageCost}";
                    Console.ForegroundColor = canUse ? s.Color : ConsoleColor.DarkGray;
                    Console.WriteLine($"  [{i + 1}] {s.Name,-12} ({cost,6}) - {s.Description}");
                    Console.ResetColor();
                }
                Console.WriteLine(L10n.Get("BATTLE_SKILL_BACK"));

                int choice = Utils.GetChoice(L10n.Get("BATTLE_SKILL_SELECT"), 0, skills.Count);
                if (choice == 0) return false;

                var skill = skills[choice - 1];

                if (skill.MpCost > _player.MP)
                {
                    Log(ConsoleColor.Red, L10n.Get("BATTLE_MP_LOW")); Utils.Pause(400); continue;
                }
                if (skill.RageCost > _player.RageEnergy)
                {
                    Log(ConsoleColor.Red, L10n.Get("BATTLE_RAGE_LOW")); Utils.Pause(400); continue;
                }

                UseSkill(skill, enemy);
                return true;
            }
        }

        private void UseSkill(Skill skill, Enemy enemy)
        {
            _player.MP -= skill.MpCost;
            _player.RageEnergy -= skill.RageCost;

            Log(skill.Color, L10n.Get("BATTLE_SKILL_USE", _player.Name, skill.Name));
            Utils.Pause(150);

            if (skill.IsHeal)
            {
                AnimationSystem.AnimateHeal(_player);
                _player.Heal(skill.HealAmount);
                Log(ConsoleColor.Green, L10n.Get("BATTLE_HEAL_MSG", skill.HealAmount, _player.HP, _player.MaxHP));
                return;
            }

            // Rage-skill backfire
            if (skill.RageCost > 0 && _rng.Next(100) < 22)
            {
                int selfDmg = Math.Max(1, (int)(_player.BaseAttack * 0.4));
                _player.HP = Math.Max(0, _player.HP - selfDmg);
                _player.CorruptionLevel++;
                AnimationSystem.UpdatePlayerArt(_player, "hurt");
                Log(ConsoleColor.DarkMagenta, L10n.Get("BATTLE_RAGE_SKILL_BACKFIRE", _player.Name, selfDmg));
                Utils.Pause(200);
                if (!_player.IsAlive) return;
            }

            AddRageWithBerserkCheck(20);

            int dmg = SkillSystem.CalculateSkillDamage(_player.Attack, skill, _player.IsBerserk, _rng);
            int actualDmg = enemy.TakeDamage(dmg);

            AnimationSystem.AnimateSkill(_player, enemy, skill);

            Log(ConsoleColor.Yellow, L10n.Get("BATTLE_SKILL_HIT", enemy.Name, actualDmg));

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
                    Log(ConsoleColor.Red, L10n.Get("BATTLE_BURN_APPLY", enemy.Name));
                    break;
                case SkillEffect.Stun:
                    enemy.ApplyStun(1);
                    Log(ConsoleColor.Yellow, L10n.Get("BATTLE_STUN_APPLY", enemy.Name));
                    break;
                case SkillEffect.Critical:
                    int extra = Math.Max(1, (int)(_player.Attack * 0.9));
                    enemy.TakeDamage(extra);
                    Log(ConsoleColor.Magenta, L10n.Get("BATTLE_CRIT_APPLY", extra));
                    break;
            }
        }

        private void DoDefend()
        {
            _player.CurrentStatus = StatusEffect.Defending;
            AddRageWithBerserkCheck(5);
            AnimationSystem.AnimateDefend(_player);
            Log(ConsoleColor.Cyan, L10n.Get("BATTLE_DEFEND", _player.Name));
        }

        // ── Enemy side ──────────────────────────────────────────────────────

        private void DoEnemyTurn(Enemy enemy)
        {
            Utils.Pause(300);

            if (enemy.TickStun())
            {
                Log(ConsoleColor.Yellow, L10n.Get("BATTLE_ENEMY_STUN", enemy.Name));
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
                Log(ConsoleColor.Cyan, L10n.Get("BATTLE_DEFEND_TAG"));
                _player.CurrentStatus = StatusEffect.None;
            }

            if (useSpecial)
                Log(ConsoleColor.DarkRed, L10n.Get("BATTLE_ENEMY_SPECIAL", enemy.Name, enemy.SpecialName, _player.Name, actualDamage, _player.HP, _player.MaxHP));
            else
                Log(ConsoleColor.Red, L10n.Get("BATTLE_ENEMY_ATTACK", enemy.Name, _player.Name, actualDamage, _player.HP, _player.MaxHP));
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
                Log(ConsoleColor.Cyan, L10n.Get("BATTLE_EXP_GAIN", enemy.EXPReward));

                if (leveled)
                {
                    Log(ConsoleColor.Yellow, L10n.Get("BATTLE_LEVEL_UP", _player.Name, _player.Level));
                    Log(ConsoleColor.Yellow, L10n.Get("BATTLE_LEVEL_STATS"));
                }

                _player.RestoreMP(15);
                Log(ConsoleColor.Blue, L10n.Get("BATTLE_MP_RESTORE"));

                Utils.PressAnyKey();
                return BattleResult.Victory;
            }
            else
            {
                AnimationSystem.ShowDefeatBanner();
                Log(ConsoleColor.Red, L10n.Get("BATTLE_DEFEAT_MSG", _player.Name));
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
