using System;
using RPGGame.Content;
using RPGGame.Domain;
using RPGGame.Persistence;
using RPGGame.Presentation;
using RPGGame.Systems;

namespace RPGGame.App
{
    public class GameManager
    {
        private Player? _player;
        private BattleSystem? _battle;
        private StoryManager? _story;
        private readonly Random _rng = new Random();
        private readonly SaveGameService _saveService = new SaveGameService();

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
                    case 4: ShowSettings(); continue;
                    case 5: return;
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
            bool hasSave = _saveService.HasSave();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(L10n.Get("MENU_TITLE"));
            Console.ResetColor();
            Console.WriteLine(L10n.Get("MENU_NEW_GAME"));

            Console.ForegroundColor = hasSave ? ConsoleColor.White : ConsoleColor.DarkGray;
            Console.WriteLine(hasSave ? L10n.Get("MENU_LOAD") : L10n.Get("MENU_LOAD_NONE"));
            Console.ResetColor();

            Console.WriteLine(L10n.Get("MENU_HELP"));
            Console.WriteLine(L10n.Get("MENU_SETTINGS"));
            Console.WriteLine(L10n.Get("MENU_QUIT"));

            return Utils.GetChoice(L10n.Get("MENU_SELECT"), 1, 5);
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
            Console.WriteLine(L10n.Get("CREATE_CLASS_4"));
            Console.WriteLine(L10n.Get("CREATE_CLASS_5"));

            int cls = Utils.GetChoice(L10n.Get("CREATE_CLASS_SELECT"), 1, 5);
            var classDef = ClassDefinitions.FromMenuChoice(cls);

            _player = new Player(name);
            ClassDefinitions.ApplyStartingBonus(_player, classDef);
            Utils.TypeText($"\n  {name} {L10n.Get(classDef.IntroKey)}。", 38, classDef.IntroColor);
            _player.InitClassSkills();

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
            if (!_saveService.HasSave())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(L10n.Get("LOAD_FAIL"));
                Console.ResetColor();
                Utils.Pause(1000);
                return;
            }

            try
            {
                var (player, chapter) = _saveService.Load();
                _player = player;
                _battle = new BattleSystem(_player, _rng);
                _story = new StoryManager(_player, _battle, _rng);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(L10n.Get("LOAD_SUCCESS", _player.Name));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(L10n.Get("LOAD_CHAPTER", chapter));
                Console.ResetColor();
                Utils.Pause(800);

                switch (chapter)
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
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"  ({ex.Message})");
                Console.ResetColor();
                Utils.Pause(1500);
                StartNewGame();
            }
        }

        // ── Save helper ─────────────────────────────────────────────────────

        private void SaveGame(int chapter)
        {
            if (_player == null) return;

            _saveService.Save(_player, chapter);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(L10n.Get("SAVE_AUTO"));
            Console.ResetColor();
            Utils.Pause(400);
        }

        // ── Settings ─────────────────────────────────────────────────────────

        private static void ShowSettings()
        {
            Console.Clear();
            Utils.PrintTitle(L10n.Get("SETTINGS_TITLE"));

            // Text Speed
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(L10n.Get("SETTINGS_SPEED"));
            Console.ResetColor();
            Console.WriteLine(L10n.Get("SETTINGS_SPEED_1"));
            Console.WriteLine(L10n.Get("SETTINGS_SPEED_2"));
            Console.WriteLine(L10n.Get("SETTINGS_SPEED_3"));
            Console.WriteLine(L10n.Get("SETTINGS_SPEED_4"));
            string currentSpeed = GameSettings.Speed switch
            {
                TextSpeed.Instant => L10n.Get("SETTINGS_SPEED_INSTANT"),
                TextSpeed.Fast    => L10n.Get("SETTINGS_SPEED_FAST"),
                TextSpeed.Slow    => L10n.Get("SETTINGS_SPEED_SLOW"),
                _                 => L10n.Get("SETTINGS_SPEED_NORMAL")
            };
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(L10n.Get("SETTINGS_CURRENT", currentSpeed));
            Console.ResetColor();

            int speedChoice = Utils.GetChoice(L10n.Get("MENU_SELECT"), 1, 4);
            GameSettings.Speed = speedChoice switch
            {
                1 => TextSpeed.Instant,
                2 => TextSpeed.Fast,
                3 => TextSpeed.Normal,
                _ => TextSpeed.Slow
            };

            // Difficulty
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(L10n.Get("SETTINGS_DIFFICULTY"));
            Console.ResetColor();
            Console.WriteLine(L10n.Get("SETTINGS_DIFF_1"));
            Console.WriteLine(L10n.Get("SETTINGS_DIFF_2"));
            Console.WriteLine(L10n.Get("SETTINGS_DIFF_3"));
            string currentDiff = GameSettings.Difficulty switch
            {
                DifficultyLevel.Easy => L10n.Get("SETTINGS_DIFF_EASY"),
                DifficultyLevel.Hard => L10n.Get("SETTINGS_DIFF_HARD"),
                _                    => L10n.Get("SETTINGS_DIFF_NORMAL")
            };
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(L10n.Get("SETTINGS_CURRENT", currentDiff));
            Console.ResetColor();

            int diffChoice = Utils.GetChoice(L10n.Get("MENU_SELECT"), 1, 3);
            GameSettings.Difficulty = diffChoice switch
            {
                1 => DifficultyLevel.Easy,
                2 => DifficultyLevel.Normal,
                _ => DifficultyLevel.Hard
            };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(L10n.Get("SETTINGS_SAVED"));
            Console.ResetColor();
            Utils.Pause(800);
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
            Utils.PrintTitle(L10n.Get("CREDITS_TITLE"));
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(L10n.Get("CREDITS_THANKS"));
            Console.WriteLine();
            Console.WriteLine(L10n.Get("CREDITS_STATS"));
            Console.WriteLine($"  {L10n.Get("CREDITS_NAME"),-10}  {_player.Name}");
            Console.WriteLine($"  {L10n.Get("CREDITS_LEVEL"),-10}  Lv.{_player.Level}");
            Console.WriteLine($"  {L10n.Get("CREDITS_HP"),-10}  {_player.HP}/{_player.MaxHP}");
            Console.WriteLine($"  {L10n.Get("CREDITS_CORRUPT"),-10}  {_player.CorruptionLevel}");
            Console.WriteLine($"  {L10n.Get("CREDITS_BERSERK"),-10}  {_player.TotalBerserkUses}");
            Console.WriteLine($"  {L10n.Get("CREDITS_HELPED"),-10}  {(_player.HelpedVillager ? L10n.Get("CREDITS_YES") : L10n.Get("CREDITS_NO"))}");
            Console.WriteLine($"  {L10n.Get("CREDITS_DARK"),-10}  {(_player.AcceptedDarkPower ? L10n.Get("CREDITS_YES") : L10n.Get("CREDITS_NO"))}");
            Console.WriteLine("  ─────────────────────────────────────────");
            Console.ResetColor();

            _saveService.Delete();

            Utils.PressAnyKey(L10n.Get("PRESS_ANY_KEY_MENU"));
        }
    }
}
