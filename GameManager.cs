using System;
using System.IO;
using System.Text.Json;

namespace RPGGame
{
    public class GameManager
    {
        private Player? _player;
        private BattleSystem? _battle;
        private StoryManager? _story;
        private readonly Random _rng = new Random();
        private const string SaveFile = "savegame.json";

        // ── Entry point ─────────────────────────────────────────────────────

        public void Run()
        {
            while (true)
            {
                ShowTitle();
                int choice = ShowMainMenu();

                switch (choice)
                {
                    case 1: StartNewGame(); break;
                    case 2: LoadGame(); break;
                    case 3: ShowHelp(); continue;
                    case 4: return;
                }

                // After game ends, offer restart
                if (_story?.RestartRequested == true) continue;
                break;
            }
        }

        // ── Title / Menu ────────────────────────────────────────────────────

        private static void ShowTitle()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(@"
  ╔═══════════════════════════════════════════════════════════╗
  ║                                                           ║
  ║    ██████╗  █████╗  ██████╗ ███████╗                    ║
  ║    ██╔══██╗██╔══██╗██╔════╝ ██╔════╝                    ║
  ║    ██████╔╝███████║██║  ███╗█████╗                       ║
  ║    ██╔══██╗██╔══██║██║   ██║██╔══╝                      ║
  ║    ██║  ██║██║  ██║╚██████╔╝███████╗                    ║
  ║    ╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝                   ║
  ║                                                           ║
  ║         Chronicles of Darkness  暴走：黑暗年代記          ║
  ║                                                           ║
  ╚═══════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        private int ShowMainMenu()
        {
            bool hasSave = File.Exists(SaveFile);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  【主 選 單】");
            Console.ResetColor();
            Console.WriteLine("  [1] 新遊戲");

            Console.ForegroundColor = hasSave ? ConsoleColor.White : ConsoleColor.DarkGray;
            Console.WriteLine($"  [2] 讀取存檔{(hasSave ? "" : "（無存檔）")}");
            Console.ResetColor();

            Console.WriteLine("  [3] 遊玩說明");
            Console.WriteLine("  [4] 退出遊戲");

            return Utils.GetChoice("選擇", 1, 4);
        }

        // ── New Game ─────────────────────────────────────────────────────────

        private void StartNewGame()
        {
            Console.Clear();
            Utils.TypeText("\n  歡迎來到《暴走：黑暗年代記》", 38, ConsoleColor.Yellow);
            Utils.TypeText("  這是一個關於憤怒、選擇與自我的故事。", 38);
            Utils.Pause(400);

            // ── Character creation ────────────────────────────────────────
            Utils.PrintTitle("創 建 角 色");
            string name = Utils.GetString("請輸入角色名字");

            Console.WriteLine("\n  選擇你的職業：");
            Console.WriteLine("  [1] 戰士  ── HP+30, DEF+5，怒氣累積較快");
            Console.WriteLine("  [2] 法師  ── MP+30, ATK+3，技能傷害提升");
            Console.WriteLine("  [3] 刺客  ── ATK+8，每次攻擊獲得更多怒氣");

            int cls = Utils.GetChoice("選擇職業", 1, 3);

            _player = new Player(name);
            switch (cls)
            {
                case 1:
                    _player.MaxHP += 30; _player.HP += 30;
                    _player.BaseDefense += 5;
                    Utils.TypeText($"\n  {name} 是一名無畏的戰士。", 38, ConsoleColor.Yellow);
                    break;
                case 2:
                    _player.MaxMP += 30; _player.MP += 30;
                    _player.BaseAttack += 3;
                    Utils.TypeText($"\n  {name} 是一名精通魔法的法師。", 38, ConsoleColor.Cyan);
                    break;
                case 3:
                    _player.BaseAttack += 8;
                    _player.MaxHP -= 10; _player.HP -= 10;
                    Utils.TypeText($"\n  {name} 是一名行動敏捷的刺客。", 38, ConsoleColor.DarkGray);
                    break;
            }

            Utils.PressAnyKey();

            _battle = new BattleSystem(_player, _rng);
            _story = new StoryManager(_player, _battle, _rng);

            // ── Play chapters ──────────────────────────────────────────────
            _story.PlayChapter1();
            if (_story.GameOverTriggered) return;

            SaveGame(chapter: 2);

            _story.PlayChapter2();
            if (_story.GameOverTriggered) return;

            SaveGame(chapter: 3);

            _story.PlayChapter3();
            if (_story.GameOverTriggered) return;

            ShowCredits();
        }

        // ── Load Game ─────────────────────────────────────────────────────────

        private void LoadGame()
        {
            if (!File.Exists(SaveFile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  ✗ 找不到存檔！");
                Console.ResetColor();
                Utils.Pause(1000);
                return;
            }

            try
            {
                string json = File.ReadAllText(SaveFile);
                var data = JsonSerializer.Deserialize<SaveData>(json)
                    ?? throw new InvalidDataException("存檔資料無效");

                _player = BuildPlayerFromSave(data);
                _battle = new BattleSystem(_player, _rng);
                _story = new StoryManager(_player, _battle, _rng);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  ✓ 讀取成功！繼續 {data.PlayerName} 的旅程。");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"  ▶ 從第 {data.CurrentChapter} 章繼續");
                Console.ResetColor();
                Utils.Pause(800);

                switch (data.CurrentChapter)
                {
                    case 2:
                        _story.PlayChapter2();
                        if (_story.GameOverTriggered) return;
                        SaveGame(chapter: 3);
                        _story.PlayChapter3();
                        break;
                    case 3:
                        _story.PlayChapter3();
                        break;
                    default:
                        StartNewGame();
                        return;
                }

                if (!_story.GameOverTriggered)
                    ShowCredits();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n  ✗ 存檔損壞（{ex.Message}），開始新遊戲。");
                Console.ResetColor();
                Utils.Pause(1200);
                StartNewGame();
            }
        }

        // ── Save / Load helpers ───────────────────────────────────────────────

        private void SaveGame(int chapter)
        {
            if (_player == null) return;

            var data = new SaveData
            {
                PlayerName      = _player.Name,
                Level           = _player.Level,
                HP              = _player.HP,
                MaxHP           = _player.MaxHP,
                MP              = _player.MP,
                MaxMP           = _player.MaxMP,
                BaseAttack      = _player.BaseAttack,
                BaseDefense     = _player.BaseDefense,
                EXP             = _player.EXP,
                EXPToNextLevel  = _player.EXPToNextLevel,
                CorruptionLevel = _player.CorruptionLevel,
                AcceptedDark    = _player.AcceptedDarkPower,
                HelpedVillager  = _player.HelpedVillager,
                BerserkUses     = _player.TotalBerserkUses,
                CurrentChapter  = chapter
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFile, json);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n  [遊戲已自動儲存]");
            Console.ResetColor();
            Utils.Pause(400);
        }

        private static Player BuildPlayerFromSave(SaveData d)
        {
            var p = new Player(d.PlayerName)
            {
                Level          = d.Level,
                HP             = d.HP,
                MaxHP          = d.MaxHP,
                MP             = d.MP,
                MaxMP          = d.MaxMP,
                BaseAttack     = d.BaseAttack,
                BaseDefense    = d.BaseDefense,
                EXP            = d.EXP,
                EXPToNextLevel = d.EXPToNextLevel,
                CorruptionLevel  = d.CorruptionLevel,
                AcceptedDarkPower = d.AcceptedDark,
                HelpedVillager    = d.HelpedVillager,
                TotalBerserkUses  = d.BerserkUses
            };

            // Re-unlock advanced skills if appropriate level
            if (p.Level >= 3)
                p.Skills.AddRange(SkillSystem.GetAdvancedSkills());

            return p;
        }

        // ── Help / Credits ───────────────────────────────────────────────────

        private static void ShowHelp()
        {
            Console.Clear();
            Utils.PrintTitle("遊 玩 說 明");
            Console.WriteLine();
            Console.WriteLine("  ◆ 基本玩法");
            Console.WriteLine("    每回合選擇行動：攻擊 / 技能 / 防禦");
            Console.WriteLine("    擊敗敵人獲得 EXP，升等後屬性提升");
            Console.WriteLine();
            Console.WriteLine("  ◆ 暴走系統");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("    攻擊和受傷都會累積怒氣（0-100）");
            Console.WriteLine("    怒氣滿了自動觸發【暴走狀態】（4回合）");
            Console.WriteLine("    暴走中：攻擊力 ×1.5，但每次攻擊有 20% 反噬");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("  ◆ 技能系統");
            Console.WriteLine("    火球術  ── 消耗 MP，高傷害 + 燃燒機率");
            Console.WriteLine("    治癒術  ── 消耗 MP，恢復 HP");
            Console.WriteLine("    盾擊    ── 消耗 MP，傷害 + 眩暈機率");
            Console.WriteLine("    暴走衝擊 ── 消耗 40 怒氣，超高傷害 + 暴擊");
            Console.WriteLine();
            Console.WriteLine("  ◆ 結局條件（共 3 種）");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("    好結局：腐化值低，以正直之心打倒魔王");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("    壞結局：接受黑暗力量或腐化值過高");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("    隱藏結局：(提示：在最終戰中保持暴走狀態...)");
            Console.ResetColor();
            Utils.PressAnyKey();
        }

        private void ShowCredits()
        {
            if (_player == null) return;

            Console.Clear();
            Utils.PrintTitle("旅 程 終 結");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  感謝遊玩《暴走：黑暗年代記》！");
            Console.WriteLine();
            Console.WriteLine("  ─── 本局統計 ───────────────────────────");
            Console.WriteLine($"  角色名稱    {_player.Name}");
            Console.WriteLine($"  最終等級    Lv.{_player.Level}");
            Console.WriteLine($"  最終 HP     {_player.HP}/{_player.MaxHP}");
            Console.WriteLine($"  腐化值      {_player.CorruptionLevel}");
            Console.WriteLine($"  暴走次數    {_player.TotalBerserkUses}");
            Console.WriteLine($"  幫助村民    {(_player.HelpedVillager ? "是" : "否")}");
            Console.WriteLine($"  接受黑暗    {(_player.AcceptedDarkPower ? "是" : "否")}");
            Console.WriteLine("  ─────────────────────────────────────────");
            Console.ResetColor();

            // Delete save on completion
            if (File.Exists(SaveFile))
                File.Delete(SaveFile);

            Utils.PressAnyKey("[ 按任意鍵返回主選單 ]");
        }
    }

    // ─── Save data ────────────────────────────────────────────────────────────

    public class SaveData
    {
        public string PlayerName { get; set; } = "";
        public int Level { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int EXP { get; set; }
        public int EXPToNextLevel { get; set; }
        public int CorruptionLevel { get; set; }
        public bool AcceptedDark { get; set; }
        public bool HelpedVillager { get; set; }
        public int BerserkUses { get; set; }
        public int CurrentChapter { get; set; }
    }
}
