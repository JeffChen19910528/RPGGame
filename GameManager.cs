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
            SelectLanguage();
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

        // ── Language Selection ───────────────────────────────────────────────

        private static void SelectLanguage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║   Select Language  /  語言選擇  /  言語選択  ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine(L10n.Get("LANG_ZH"));
            Console.WriteLine(L10n.Get("LANG_EN"));
            Console.WriteLine(L10n.Get("LANG_JA"));
            int lang = Utils.GetChoice(L10n.Get("LANG_PROMPT"), 1, 3);
            L10n.Current = lang switch { 1 => Language.Chinese, 2 => Language.English, _ => Language.Japanese };
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
            Console.WriteLine(L10n.Get("MENU_TITLE"));
            Console.ResetColor();
            Console.WriteLine(L10n.Get("MENU_NEW_GAME"));

            Console.ForegroundColor = hasSave ? ConsoleColor.White : ConsoleColor.DarkGray;
            Console.WriteLine(hasSave ? L10n.Get("MENU_LOAD") : L10n.Get("MENU_LOAD_NONE"));
            Console.ResetColor();

            Console.WriteLine(L10n.Get("MENU_HELP"));
            Console.WriteLine(L10n.Get("MENU_QUIT"));

            return Utils.GetChoice(L10n.Get("MENU_SELECT"), 1, 4);
        }

        // ── New Game ─────────────────────────────────────────────────────────

        private void StartNewGame()
        {
            Console.Clear();
            Utils.TypeText(L10n.Get("CREATE_WELCOME_1"), 38, ConsoleColor.Yellow);
            Utils.TypeText(L10n.Get("CREATE_WELCOME_2"), 38);
            Utils.Pause(400);

            // ── Character creation ────────────────────────────────────────
            Utils.PrintTitle(L10n.Get("CREATE_TITLE"));
            string name = Utils.GetString(L10n.Get("CREATE_NAME_PROMPT"));

            Console.WriteLine(L10n.Get("CREATE_CLASS_INTRO"));
            Console.WriteLine(L10n.Get("CREATE_CLASS_1"));
            Console.WriteLine(L10n.Get("CREATE_CLASS_2"));
            Console.WriteLine(L10n.Get("CREATE_CLASS_3"));

            int cls = Utils.GetChoice(L10n.Get("CREATE_CLASS_SELECT"), 1, 3);

            _player = new Player(name);
            _player.Class = cls switch { 1 => PlayerClass.Warrior, 2 => PlayerClass.Mage, _ => PlayerClass.Assassin };
            switch (cls)
            {
                case 1:
                    _player.MaxHP += 30; _player.HP += 30;
                    _player.BaseDefense += 5;
                    Utils.TypeText($"\n  {name} {L10n.Get("INTRO_WARRIOR")}。", 38, ConsoleColor.Yellow);
                    break;
                case 2:
                    _player.MaxMP += 30; _player.MP += 30;
                    _player.BaseAttack += 3;
                    Utils.TypeText($"\n  {name} {L10n.Get("INTRO_MAGE")}。", 38, ConsoleColor.Cyan);
                    break;
                case 3:
                    _player.BaseAttack += 8;
                    _player.MaxHP -= 10; _player.HP -= 10;
                    Utils.TypeText($"\n  {name} {L10n.Get("INTRO_ASSASSIN")}。", 38, ConsoleColor.DarkGray);
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
                Console.WriteLine(L10n.Get("LOAD_FAIL"));
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
                Console.WriteLine(L10n.Get("LOAD_SUCCESS", data.PlayerName));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(L10n.Get("LOAD_CHAPTER", data.CurrentChapter));
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
                Console.WriteLine(L10n.Get("LOAD_CORRUPT"));
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
                ClassId         = (int)_player.Class,
                CurrentChapter  = chapter
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFile, json);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(L10n.Get("SAVE_AUTO"));
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
                CorruptionLevel   = d.CorruptionLevel,
                AcceptedDarkPower = d.AcceptedDark,
                HelpedVillager    = d.HelpedVillager,
                TotalBerserkUses  = d.BerserkUses,
                Class             = (PlayerClass)d.ClassId
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
            Utils.PrintTitle(L10n.Get("HELP_TITLE"));
            Utils.PrintTitle("遊 玩 說 明");
            Console.WriteLine();
            Console.WriteLine(L10n.Get("HELP_BASICS"));
            Console.WriteLine(L10n.Get("HELP_BASICS_1"));
            Console.WriteLine(L10n.Get("HELP_BASICS_2"));
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(L10n.Get("HELP_BERSERK"));
            Console.WriteLine(L10n.Get("HELP_BERSERK_1"));
            Console.WriteLine(L10n.Get("HELP_BERSERK_2"));
            Console.WriteLine(L10n.Get("HELP_BERSERK_3"));
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine(L10n.Get("HELP_SKILLS"));
            Console.WriteLine(L10n.Get("HELP_SKILLS_1"));
            Console.WriteLine(L10n.Get("HELP_SKILLS_2"));
            Console.WriteLine(L10n.Get("HELP_SKILLS_3"));
            Console.WriteLine(L10n.Get("HELP_SKILLS_4"));
            Console.WriteLine();
            Console.WriteLine(L10n.Get("HELP_ENDINGS"));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(L10n.Get("HELP_ENDINGS_HINT"));
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
        public int ClassId { get; set; }
        public int CurrentChapter { get; set; }
    }
}
