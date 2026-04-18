using System;
using System.Threading;

namespace RPGGame
{
    /// <summary>
    /// Manages all ASCII art rendering and combat animations.
    /// Uses Console.SetCursorPosition for in-place art updates.
    /// Falls back gracefully if cursor positioning is unavailable.
    /// </summary>
    public static class AnimationSystem
    {
        // Where the enemy art block starts on screen
        private static int _enemyArtRow = -1;
        private static int _enemyArtCol = 24;

        // Where the player art block starts on screen
        private static int _playerArtRow = -1;
        private static int _playerArtCol = 2;

        // Where the message area begins (below art + status)
        public static int MessageAreaRow { get; private set; } = -1;

        // ── BATTLE SCREEN ────────────────────────────────────────────────────

        /// <summary>
        /// Draws the complete battle screen and records art positions.
        /// Call at the start of each turn.
        /// </summary>
        public static void DrawBattleScreen(Player player, Enemy enemy, int turn)
        {
            Console.Clear();

            // ── Header ──────────────────────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + new string('═', 58));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚔  第 {turn} 回合          {player.Name}  vs  {enemy.Name}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + new string('─', 58));
            Console.ResetColor();

            // ── Enemy HP bar (above art) ────────────────────────────────────
            Console.Write("  ");
            Console.ForegroundColor = enemy.NameColor;
            Console.Write($"【{enemy.Name}】 ");
            Console.ResetColor();
            Utils.DrawProgressBar(enemy.HP, enemy.MaxHP, 22, ConsoleColor.DarkRed);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  {enemy.HP}/{enemy.MaxHP}");
            if (enemy.IsStunned)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("  [眩暈]");
            }
            if (enemy.IsBurning)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"  [燃燒x{enemy.BurnTurns}]");
            }
            Console.ResetColor();
            Console.WriteLine();

            // ── Enemy ASCII art (centered) ───────────────────────────────────
            _enemyArtRow = Console.CursorTop;
            _enemyArtCol = 24;

            string[] enemyArt = AsciiArt.GetEnemyArt(enemy.Name, "normal");
            PrintArtAt(_enemyArtRow, _enemyArtCol, enemyArt, enemy.NameColor);

            // Advance cursor past the art
            int afterArt = _enemyArtRow + enemyArt.Length;
            SafeSetCursor(0, afterArt);
            Console.WriteLine();

            // ── Player status (below art) ────────────────────────────────────
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + new string('─', 58));
            Console.ResetColor();

            // Player mini art on left, stats on right
            _playerArtRow = Console.CursorTop;
            _playerArtCol = 2;

            string[] playerArt = AsciiArt.GetPlayerArt(player, "normal");
            int statusCol = _playerArtCol + 18;

            // Print player art lines alongside stat text
            string[] statLines = BuildPlayerStatLines(player);
            int totalLines = Math.Max(playerArt.Length, statLines.Length);

            for (int i = 0; i < totalLines; i++)
            {
                string pLine = i < playerArt.Length ? playerArt[i] : new string(' ', 16);
                string sLine = i < statLines.Length ? statLines[i] : "";

                Console.ForegroundColor = player.IsBerserk ? ConsoleColor.Magenta : ConsoleColor.White;
                Console.Write("  " + pLine.PadRight(16));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("  " + sLine);
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + new string('═', 58));
            Console.ResetColor();

            MessageAreaRow = Console.CursorTop;
        }

        private static string[] BuildPlayerStatLines(Player player)
        {
            var lines = new System.Collections.Generic.List<string>();
            string name = player.IsBerserk ? $"★{player.Name} Lv.{player.Level}★" : $"【{player.Name} Lv.{player.Level}】";
            lines.Add(name);
            lines.Add($"HP {MiniBar(player.HP, player.MaxHP, 12, '█', '░')}  {player.HP}/{player.MaxHP}");
            lines.Add($"MP {MiniBar(player.MP, player.MaxMP, 12, '█', '░')}  {player.MP}/{player.MaxMP}");
            lines.Add($"怒 {MiniBar(player.RageEnergy, player.MaxRageEnergy, 12, '#', '·')}  {player.RageEnergy}/{player.MaxRageEnergy}");

            if (player.IsBerserk)
                lines.Add($"   ★ 暴走中 (剩餘 {player.BerserkTurnsLeft} 回合) ★");
            else if (player.CurrentStatus != StatusEffect.None && player.CurrentStatus != StatusEffect.Defending)
                lines.Add($"   [{player.CurrentStatus} x{player.StatusTurns}]");
            else if (player.CurrentStatus == StatusEffect.Defending)
                lines.Add("   [防禦姿態]");

            return lines.ToArray();
        }

        private static string MiniBar(int current, int max, int width, char fill, char empty)
        {
            if (max <= 0) max = 1;
            int filled = Math.Min(width, (int)((double)Math.Max(0, current) / max * width));
            return "[" + new string(fill, filled) + new string(empty, width - filled) + "]";
        }

        // ── ART UPDATES (in-place) ────────────────────────────────────────────

        public static void UpdateEnemyArt(Enemy enemy, string state, ConsoleColor color = ConsoleColor.White)
        {
            if (_enemyArtRow < 0) return;
            string[] art = AsciiArt.GetEnemyArt(enemy.Name, state);
            int saved = Console.CursorTop;
            PrintArtAt(_enemyArtRow, _enemyArtCol, art, color == ConsoleColor.White ? enemy.NameColor : color);
            SafeSetCursor(0, MessageAreaRow);
        }

        public static void UpdatePlayerArt(Player player, string state)
        {
            if (_playerArtRow < 0) return;
            string[] art = AsciiArt.GetPlayerArt(player, state);
            ConsoleColor col = state switch
            {
                "hurt"    => ConsoleColor.Red,
                "berserk" => ConsoleColor.Magenta,
                "attack"  => ConsoleColor.Yellow,
                "skill"   => ConsoleColor.Cyan,
                "defend"  => ConsoleColor.Cyan,
                _         => ConsoleColor.White,
            };
            PrintArtAt(_playerArtRow, _playerArtCol, art, col);
            SafeSetCursor(0, MessageAreaRow);
        }

        // ── ANIMATIONS ────────────────────────────────────────────────────────

        public static void AnimatePlayerAttack(Player player, Enemy enemy)
        {
            UpdatePlayerArt(player, "attack");
            Thread.Sleep(150);
            PlaySlashEffect(player.IsBerserk);
            Thread.Sleep(100);
            UpdateEnemyArt(enemy, "hurt", ConsoleColor.Red);
            Thread.Sleep(200);
            UpdateEnemyArt(enemy, "normal");
            UpdatePlayerArt(player, "normal");
        }

        public static void AnimateSkill(Player player, Enemy enemy, Skill skill)
        {
            UpdatePlayerArt(player, "skill");
            Thread.Sleep(150);
            PlaySkillEffect(skill);
            Thread.Sleep(100);
            UpdateEnemyArt(enemy, "hurt", ConsoleColor.Red);
            Thread.Sleep(250);
            UpdateEnemyArt(enemy, "normal");
            UpdatePlayerArt(player, "normal");
        }

        public static void AnimateHeal(Player player)
        {
            UpdatePlayerArt(player, "skill");
            PrintEffect(AsciiArt.EffectHeal, ConsoleColor.Green);
            Thread.Sleep(300);
            UpdatePlayerArt(player, "normal");
        }

        public static void AnimateDefend(Player player)
        {
            UpdatePlayerArt(player, "defend");
            PrintEffect(new[] { "  [ GUARD ] " }, ConsoleColor.Cyan);
        }

        public static void AnimateEnemyAttack(Enemy enemy, Player player, bool isSpecial)
        {
            UpdateEnemyArt(enemy, "attack");
            Thread.Sleep(200);
            if (isSpecial)
                PrintEffect(AsciiArt.EffectSpecialHit, ConsoleColor.DarkRed);
            UpdatePlayerArt(player, "hurt");
            Thread.Sleep(250);
            UpdatePlayerArt(player, "normal");
            UpdateEnemyArt(enemy, "normal");
        }

        public static void AnimateBerserkActivation(Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                UpdatePlayerArt(player, "berserk");
                PrintEffect(AsciiArt.EffectBerserkOn, ConsoleColor.Magenta);
                Thread.Sleep(150);
                UpdatePlayerArt(player, "normal");
                Thread.Sleep(80);
            }
            UpdatePlayerArt(player, "berserk");
        }

        public static void AnimateEnemyDeath(Enemy enemy)
        {
            for (int flash = 0; flash < 3; flash++)
            {
                UpdateEnemyArt(enemy, "hurt", ConsoleColor.Red);
                Thread.Sleep(120);
                UpdateEnemyArt(enemy, "normal");
                Thread.Sleep(80);
            }
            UpdateEnemyArt(enemy, "dead", ConsoleColor.DarkGray);
            Thread.Sleep(400);
        }

        // ── SPECIAL SCREENS ───────────────────────────────────────────────────

        public static void ShowEnemyIntro(Enemy enemy)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n  " + new string('═', 58));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ⚔  敵人出現！");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + new string('═', 58));
            Console.ResetColor();

            Console.WriteLine();
            string[] art = AsciiArt.GetEnemyArt(enemy.Name, "normal");
            foreach (string line in art)
            {
                Console.ForegroundColor = enemy.NameColor;
                Console.WriteLine("       " + line);
                Console.ResetColor();
                Thread.Sleep(40);
            }
            Console.WriteLine();
            Console.ForegroundColor = enemy.NameColor;
            Console.Write($"  【{enemy.Name}】 ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(enemy.Description);
            Console.ResetColor();

            Utils.Pause(400);
        }

        public static void ShowChapterScene(int chapter)
        {
            string[] art;
            string title;
            ConsoleColor col;

            switch (chapter)
            {
                case 1:
                    art = AsciiArt.SceneVillage;
                    title = "── 寧靜的村落 ──";
                    col = ConsoleColor.Green;
                    break;
                case 2:
                    art = AsciiArt.SceneForest;
                    title = "── 黑暗森林深處 ──";
                    col = ConsoleColor.DarkGray;
                    break;
                case 3:
                    art = AsciiArt.SceneCastle;
                    title = "── 魔王的城堡 ──";
                    col = ConsoleColor.DarkMagenta;
                    break;
                default:
                    return;
            }

            Console.WriteLine();
            Console.ForegroundColor = col;
            foreach (string line in art)
            {
                Console.WriteLine("  " + line);
                Thread.Sleep(35);
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n  {title}");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void ShowVictoryBanner(Enemy enemy)
        {
            SafeSetCursor(0, MessageAreaRow);
            string[] dead = AsciiArt.GetEnemyArt(enemy.Name, "dead");
            PrintArtAt(_enemyArtRow, _enemyArtCol, dead, ConsoleColor.DarkGray);
            SafeSetCursor(0, MessageAreaRow);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("  ╔═══════════════════════════════════╗");
            Console.WriteLine("  ║         VICTORY!  ★ 勝利！        ║");
            Console.WriteLine("  ╚═══════════════════════════════════╝");
            Console.ResetColor();
        }

        public static void ShowDefeatBanner()
        {
            if (_playerArtRow >= 0)
            {
                // Flash player dead art
                var dummyPlayer = new Player(""); // just for art lookup
                string[] dead = AsciiArt.PlayerDead;
                PrintArtAt(_playerArtRow, _playerArtCol, dead, ConsoleColor.DarkRed);
                SafeSetCursor(0, MessageAreaRow);
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("  ╔═══════════════════════════════════╗");
            Console.WriteLine("  ║          GAME  OVER...            ║");
            Console.WriteLine("  ╚═══════════════════════════════════╝");
            Console.ResetColor();
        }

        // ── PRIVATE HELPERS ───────────────────────────────────────────────────

        private static void PrintArtAt(int row, int col, string[] art, ConsoleColor color)
        {
            try
            {
                for (int i = 0; i < art.Length; i++)
                {
                    Console.SetCursorPosition(col, row + i);
                    Console.ForegroundColor = color;
                    Console.Write(art[i].PadRight(22));
                }
                Console.ResetColor();
            }
            catch
            {
                // Terminal doesn't support cursor positioning – fall back to sequential print
                Console.ForegroundColor = color;
                foreach (string line in art)
                    Console.WriteLine("  " + line);
                Console.ResetColor();
            }
        }

        private static void SafeSetCursor(int col, int row)
        {
            try
            {
                if (row >= 0 && row < Console.BufferHeight)
                    Console.SetCursorPosition(col, row);
            }
            catch { /* ignore */ }
        }

        private static void PlaySlashEffect(bool berserk)
        {
            SafeSetCursor(0, MessageAreaRow);
            string[] effect = berserk ? AsciiArt.EffectSlashBerserk : AsciiArt.EffectSlash;
            PrintEffect(effect, berserk ? ConsoleColor.Magenta : ConsoleColor.Yellow);
        }

        private static void PlaySkillEffect(Skill skill)
        {
            SafeSetCursor(0, MessageAreaRow);
            string[] effect = skill.Effect switch
            {
                SkillEffect.Burn     => AsciiArt.EffectFire,
                SkillEffect.Stun     => AsciiArt.EffectIce,
                SkillEffect.Critical => AsciiArt.EffectSlashBerserk,
                _                    => AsciiArt.EffectSlash,
            };
            PrintEffect(effect, skill.Color);
        }

        private static void PrintEffect(string[] lines, ConsoleColor color)
        {
            SafeSetCursor(0, MessageAreaRow);
            Console.ForegroundColor = color;
            foreach (string line in lines)
                Console.WriteLine("  " + line);
            Console.ResetColor();
            Thread.Sleep(120);
        }
    }
}
