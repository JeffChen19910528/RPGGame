using System;

namespace RPGGame
{
    public class StoryManager
    {
        private readonly Player _player;
        private readonly BattleSystem _battle;
        private readonly Random _rng;

        public bool GameOverTriggered { get; private set; }
        public bool RestartRequested { get; private set; }

        public StoryManager(Player player, BattleSystem battle, Random rng)
        {
            _player = player;
            _battle = battle;
            _rng = rng;
        }

        // ════════════════════════════════════════════════════════════════════
        // CHAPTER 1 ── 覺醒
        // ════════════════════════════════════════════════════════════════════

        public void PlayChapter1()
        {
            Console.Clear();
            Utils.PrintTitle("第一章  覺醒");
            Utils.Pause(200);
            AnimationSystem.ShowChapterScene(1);

            Utils.TypeText("\n  黑暗中，一雙眼睛緩緩睜開。", 38);
            Utils.Pause(200);
            Utils.TypeText($"  你是 {_player.Name}，一名被世界遺忘的{_player.ClassTitle}。", 38);

            string classAwaken = _player.Class switch
            {
                PlayerClass.Warrior  => "  雙手本能地握緊——身體還記得劍的重量。",
                PlayerClass.Mage     => "  魔力在指尖微微流動，三年沉眠未能將它熄滅。",
                PlayerClass.Assassin => "  眼睛在黑暗中快速適應，習慣性地掃描每個角落。",
                PlayerClass.Paladin  => "  胸口一道溫熱的光緩緩流動——神聖的誓約從未真正沉睡。",
                PlayerClass.Ranger   => "  耳朵靈敏地捕捉著室外的每一個聲響，獵人的本能尚未消退。",
                _ => ""
            };
            Utils.TypeText(classAwaken, 38, ConsoleColor.DarkGray);
            Utils.Pause(200);
            Utils.TypeText("  三年前，魔王夜陌魯斯率領魔族大軍入侵，", 38);
            Utils.TypeText("  你在最後的戰役中受了致命重傷，此後一直昏迷。", 38);
            Utils.Pause(400);
            Console.WriteLine();
            Utils.TypeText("  「你... 終於醒了。」", 38, ConsoleColor.Cyan);
            Utils.TypeText("  一個蒼老的聲音從陰暗角落傳來——是村裡的老祭司。", 38);
            Utils.TypeText("  「魔王的軍隊三天後就會到達這個村子。」", 38, ConsoleColor.Cyan);
            Utils.TypeText("  「你... 是我們最後的希望。」", 38, ConsoleColor.Cyan);
            Utils.Pause(500);

            Utils.PressAnyKey();

            // ── Tutorial battle ──────────────────────────────────────────
            Console.Clear();
            Utils.TypeText("\n  踏出村子的那一刻，一隻史萊姆從草叢中彈了出來。", 38);
            Utils.TypeText("  「好吧，先熱身一下。」你低聲說，握緊武器。", 38);
            Utils.Pause(300);

            var result = _battle.StartBattle(Enemy.CreateSlime());
            if (HandleDefeat(result)) return;

            PlayFunnySidequest_SlimeLord();
            if (GameOverTriggered) return;

            // ── Random roaming encounter ─────────────────────────────────
            if (_rng.Next(2) == 0)
            {
                Console.Clear();
                Utils.TypeText("\n  繼續趕路時，路旁的陰影中忽然竄出了一個敵影。", 38);
                Utils.TypeText("  看來這片土地已被黑暗之力徹底感染，妖魔四處遊蕩。", 38);
                Utils.Pause(300);
                var roamEnemy = Enemy.GetRandomTier1Enemy(_rng);
                var roamResult = _battle.StartBattle(roamEnemy);
                if (HandleDefeat(roamResult)) return;
            }

            // ── Chapter 1 choice ─────────────────────────────────────────
            Console.Clear();
            Utils.TypeText("\n  繼續前進時，你聽到一陣哭喊聲。", 38);
            Utils.TypeText("  一名老婆婆被兩隻哥布林追趕，情況危急。", 38);
            Utils.TypeText("  哥布林手中還拿著一塊散發微光的石板。", 38);
            Utils.Pause(300);

            Utils.PrintTitle("抉 擇");
            Console.WriteLine("  [1] 立刻出手相救（消耗 15 MP）");
            Console.WriteLine("  [2] 趕路要緊，先別節外生枝");

            int choice = Utils.GetChoice("你的選擇", 1, 2);

            if (choice == 1)
            {
                if (_player.MP >= 15)
                    _player.MP -= 15;
                else
                    _player.HP = Math.Max(1, _player.HP - 10); // exhaust instead

                _player.HelpedVillager = true;
                Utils.TypeText("\n  你衝上前，一道魔法擊退了哥布林。", 38);
                Utils.TypeText("  老婆婆顫抖著站起身，淚流滿面地看著你：", 38);
                Utils.TypeText("  「謝謝你，孩子。我的孫子還在城裡...」", 38, ConsoleColor.Cyan);
                Utils.TypeText("  「請你一定要打倒魔王，救救他們。」", 38, ConsoleColor.Cyan);
                Utils.Pause(300);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  ✦ 獲得老婆婆的祝福（最大 HP +15）");
                Console.ResetColor();
                _player.MaxHP += 15;
                _player.HP = Math.Min(_player.HP + 15, _player.MaxHP);
            }
            else
            {
                Utils.TypeText("\n  你咬牙轉過身，加快腳步。", 38);
                Utils.TypeText("  老婆婆的呼救聲漸漸在身後消散。", 38);
                Utils.TypeText("  「對不起...」你心底一陣刺痛，但腳步沒有停下。", 38);
                _player.CorruptionLevel++;
            }

            Utils.PressAnyKey();
        }

        // ════════════════════════════════════════════════════════════════════
        // CHAPTER 2 ── 暗黑森林
        // ════════════════════════════════════════════════════════════════════

        public void PlayChapter2()
        {
            Console.Clear();
            Utils.PrintTitle("第二章  暗黑森林");
            Utils.Pause(200);
            AnimationSystem.ShowChapterScene(2);

            Utils.TypeText("\n  踏入暗黑森林，濃稠的黑氣如同活物般纏繞四周。", 38);
            Utils.TypeText("  樹木扭曲成奇異的形狀，彷彿在無聲地哀嚎。", 38);
            Utils.TypeText("  「這裡的一切都被魔王的力量腐化了...」你心想。", 38);
            Utils.Pause(400);

            // ── Battle: Random mid enemy ──────────────────────────────────
            var midEnemy1 = Enemy.GetRandomTier2EnemyA(_rng);
            PrintRandomEncounterIntro(midEnemy1);
            Utils.Pause(300);

            var r1 = _battle.StartBattle(midEnemy1);
            if (HandleDefeat(r1)) return;

            // ── Dark power choice ─────────────────────────────────────────
            Console.Clear();
            Utils.TypeText("\n  從敵人的遺骸旁，你發現了一塊漆黑的符石。", 38);
            Utils.TypeText("  它在你手中顫動，散發著既危險又誘人的黑暗力量。", 38);
            Utils.Pause(300);

            Utils.PrintTitle("抉 擇");
            Console.WriteLine("  [1] 吸收符石的力量（ATK +6，腐化值大幅上升）");
            Console.WriteLine("  [2] 砸碎符石，拒絕黑暗誘惑");

            int choice2 = Utils.GetChoice("你的選擇", 1, 2);

            if (choice2 == 1)
            {
                _player.AcceptedDarkPower = true;
                _player.CorruptionLevel += 2;
                _player.BaseAttack += 6;
                _player.EquipWeapon(new Equipment("黑暗符石強化", atk: 6));

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Utils.TypeText("\n  黑暗能量衝入你的全身，你感到無比強大。", 38, ConsoleColor.DarkMagenta);
                Utils.TypeText("  但心中某個純粹的東西，似乎在這一刻悄然消逝了。", 38);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  ✦ ATK +6，但腐化值大幅上升");
                Console.ResetColor();
            }
            else
            {
                Utils.TypeText("\n  你用力將符石砸在地上，黑暗能量四散消逝。", 38);
                Utils.TypeText("  「我不需要這種東西。」你低聲說，聲音十分堅定。", 38, ConsoleColor.White);
                _player.EquipArmor(new Equipment("林中鐵甲", def: 4, hp: 10));
                _player.MaxHP += 10;
                _player.HP = Math.Min(_player.HP + 10, _player.MaxHP);
            }

            Utils.PressAnyKey();

            PlayFunnySidequest_LostMinion();
            if (GameOverTriggered) return;

            // ── Battle: Random deep forest enemy ─────────────────────────
            Console.Clear();
            var midEnemy2 = Enemy.GetRandomTier2EnemyB(_rng);
            PrintRandomEncounterIntro(midEnemy2);
            Utils.Pause(300);

            var r2 = _battle.StartBattle(midEnemy2);
            if (HandleDefeat(r2)) return;

            // ── Berserk hint ──────────────────────────────────────────────
            Console.Clear();
            Utils.TypeText("\n  靈體消散後，你感到胸腔中有什麼東西在沸騰。", 38);
            Utils.TypeText("  每一次攻擊、每一次受傷，都讓那股熱流更加洶湧。", 38);

            if (_player.TotalBerserkUses > 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Utils.TypeText("\n  你已經嘗過那種失控的感覺——那股暴走之力。", 38, ConsoleColor.DarkMagenta);
                Utils.TypeText("  「它... 真的能被我掌控嗎？」你不確定地問自己。", 38);
                Console.ResetColor();
            }
            else
            {
                Utils.TypeText("\n  「這股力量...是憤怒？還是別的什麼？」你自問。", 38);
            }

            Utils.PressAnyKey();
        }

        // ════════════════════════════════════════════════════════════════════
        // CHAPTER 3 ── 魔王城
        // ════════════════════════════════════════════════════════════════════

        public void PlayChapter3()
        {
            Console.Clear();
            Utils.PrintTitle("第三章  魔王城");
            Utils.Pause(200);
            AnimationSystem.ShowChapterScene(3);

            Utils.TypeText("\n  魔王城就在眼前，黑色的尖塔刺穿陰雲密布的天空。", 38);
            Utils.TypeText("  城門大開，彷彿在靜靜等待著你的到來。", 38);
            Utils.Pause(300);
            Utils.TypeText("\n  城門前，一名身著精緻黑甲的騎士緩緩轉身。", 38);
            Utils.TypeText("  「我曾是這個王國最偉大的聖騎士。」", 38, ConsoleColor.DarkRed);
            Utils.TypeText("  「如今...我是魔王手中最鋒利的劍。」", 38, ConsoleColor.DarkRed);
            Utils.Pause(400);

            // ── Battle: Dark Paladin ──────────────────────────────────────
            var rp = _battle.StartBattle(Enemy.CreateDarkPaladin());
            if (HandleDefeat(rp)) return;

            // ── Dark paladin's end ────────────────────────────────────────
            Console.Clear();
            Utils.TypeText("\n  黑甲從騎士身上碎裂，黑暗能量緩緩消散。", 38);
            Utils.TypeText("  倒在地上的騎士喃喃道：", 38);
            Utils.TypeText("  「謝... 謝謝你... 終於... 解脫了...」", 38, ConsoleColor.DarkGray);
            Utils.TypeText("  他閉上眼睛，臉上浮現出多年未有的平靜神情。", 38);
            Utils.Pause(500);

            // ── Equipment drop ────────────────────────────────────────────
            if (_player.AcceptedDarkPower)
            {
                var darkBlade = new Equipment("黑暗聖劍", atk: 12);
                _player.EquipWeapon(darkBlade);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("\n  ✦ 黑暗能量注入聖劍，你感覺它在呼應你體內的腐化。");
                Console.ResetColor();
            }
            else
            {
                var holyBlade = new Equipment("淨化聖劍", atk: 10, def: 2);
                _player.EquipWeapon(holyBlade);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  ✦ 聖劍的力量在你手中閃耀，彷彿認可了你的意志。");
                Console.ResetColor();
            }

            Utils.PressAnyKey();

            PlayFunnySidequest_DemonCafeteria();
            if (GameOverTriggered) return;

            // ── Final choice ──────────────────────────────────────────────
            Console.Clear();
            Utils.TypeText("\n  進入魔王城深處，黑暗的聲音在牆壁間迴盪：", 38);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Utils.TypeText("  「勇者，你終於來了。我等你等了很久。」", 38, ConsoleColor.DarkMagenta);
            Utils.TypeText("  「你知道嗎？你體內那股暴走之力，」", 38, ConsoleColor.DarkMagenta);
            Utils.TypeText("  「與我的魔力，其實來自同一個源頭——純粹的憤怒。」", 38, ConsoleColor.DarkMagenta);
            Console.ResetColor();
            Utils.Pause(400);

            Utils.PrintTitle("最終抉擇");
            Console.WriteLine("  [1] 「無論如何，我必須終結這一切！」（勇者之路）");
            Console.WriteLine("  [2] 「...我想聽你說完。」（接受魔王的交易）");

            int finalChoice = Utils.GetChoice("你的選擇", 1, 2);

            if (finalChoice == 2)
            {
                _player.CorruptionLevel += 3;
                _player.AcceptedDarkPower = true;
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Utils.TypeText("\n  魔王低笑：「明智。那麼，讓我給你真正的力量。」", 38, ConsoleColor.DarkMagenta);
                Utils.TypeText("  一股滔天的黑暗能量湧入你的身體。", 38);
                Utils.TypeText("  你感到前所未有的強大——以及前所未有的空洞。", 38);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n  ✦ 所有屬性大幅提升，但腐化值已達警戒線");
                Console.ResetColor();
                _player.BaseAttack += 10;
                _player.BaseDefense += 8;
                _player.HP = Math.Min(_player.MaxHP, _player.HP + 40);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Utils.TypeText("\n  「我不需要了解你！」", 38, ConsoleColor.White);
                Utils.TypeText("  你怒吼著，衝向黑暗的深處！", 38);
                Console.ResetColor();
            }

            Utils.PressAnyKey();

            // ── FINAL BOSS ────────────────────────────────────────────────
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(@"
  ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★
       最 終 決 戰
  ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★ ★");
            Console.ResetColor();
            Utils.Pause(600);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Utils.TypeText("\n  「來吧，勇者。讓我親眼見識，」", 38, ConsoleColor.DarkMagenta);
            Utils.TypeText("  「人類的憤怒，究竟能燃燒多久。」", 38, ConsoleColor.DarkMagenta);
            Console.ResetColor();
            Utils.Pause(400);

            var finalResult = _battle.StartBattle(Enemy.CreateDemonKing(), isFinalBoss: true);
            if (HandleDefeat(finalResult)) return;

            // ── Ending ────────────────────────────────────────────────────
            DetermineEnding();
        }

        // ════════════════════════════════════════════════════════════════════
        // ENDINGS  ── 7 endings based on choices, class, corruption
        // ════════════════════════════════════════════════════════════════════

        private void DetermineEnding()
        {
            Console.Clear();
            Utils.Pause(700);

            // Hidden ✦: Never berserk, never dark, always helped, no corruption
            if (_player.TotalBerserkUses == 0 && !_player.AcceptedDarkPower
                && _player.CorruptionLevel == 0 && _player.HelpedVillager)
            {
                EndingHiddenPure(); return;
            }

            // Hidden ★: Final boss defeated while berserk, no dark power
            if (_player.FinalBossDefeatedInBerserk && !_player.AcceptedDarkPower
                && _player.CorruptionLevel <= 1)
            {
                EndingHiddenBerserk(); return;
            }

            // Worst: Completely consumed by darkness
            if (_player.CorruptionLevel >= 6)
            {
                EndingWorst(); return;
            }

            // Bad: Significantly corrupted
            if (_player.CorruptionLevel >= 4 || (_player.AcceptedDarkPower && _player.CorruptionLevel >= 3))
            {
                EndingBad(); return;
            }

            // Grey: Touched darkness but not lost
            if (_player.AcceptedDarkPower || _player.CorruptionLevel >= 2)
            {
                EndingGrey(); return;
            }

            // Good (Lonely): Clean but ignored someone who needed help
            if (!_player.HelpedVillager)
            {
                EndingGoodLonely(); return;
            }

            // Class-exclusive open endings
            if (_player.Class == PlayerClass.Paladin && _player.HelpedVillager && _player.CorruptionLevel <= 1)
            {
                EndingPaladinLight(); return;
            }
            if (_player.Class == PlayerClass.Ranger && _player.TotalBerserkUses >= 1 && !_player.AcceptedDarkPower)
            {
                EndingRangerWild(); return;
            }

            // Good (Light): True hero path
            EndingGood();
        }

        private void EndingHiddenPure()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║      ✦ 隱藏結局：純淨之道 ✦               ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.TypeText("\n  純白的光從天空傾瀉而下——", 38, ConsoleColor.White);
            Utils.TypeText("  不是魔法，不是憤怒，而是你自己的光。", 38, ConsoleColor.White);
            Utils.Pause(300);
            Utils.TypeText($"\n  {_player.Name} 站在魔王消散的地方，雙手沒有顫抖。", 38);
            Utils.TypeText("  整段旅程，你從未讓憤怒佔據你，", 38);
            Utils.TypeText("  從未以黑暗換取捷徑，", 38);
            Utils.TypeText("  你甚至在最疲憊的時候，回頭救了那位老婆婆。", 38);
            Utils.Pause(300);
            Utils.TypeText("\n  老祭司看著你回來，沉默良久後，緩緩跪下。", 38);
            Utils.TypeText("  「我見過無數勇者，」他說，聲音顫抖，", 38, ConsoleColor.Cyan);
            Utils.TypeText("  「但只有你——是真正意義上的英雄。」", 38, ConsoleColor.Cyan);
            Utils.Pause(400);

            string classLine = _player.Class switch
            {
                PlayerClass.Warrior  => "  人們傳說，那名戰士的劍從未沾染過不義之血。",
                PlayerClass.Mage     => "  人們傳說，那名法師的魔法之中，帶著光一般的溫度。",
                PlayerClass.Assassin => "  人們傳說，那名刺客從陰影中走出，卻把陽光帶回了世界。",
                PlayerClass.Paladin  => "  人們傳說，那名聖騎士的祈禱，讓每一道傷口都癒合了。",
                PlayerClass.Ranger   => "  人們傳說，那名遊俠的箭矢指向黑暗，從未有一箭射偏過。",
                _ => ""
            };
            Utils.TypeText($"\n  {classLine}", 38, ConsoleColor.DarkYellow);
            Utils.Pause(300);

            Utils.TypeText("\n  天空的顏色，比昨天更藍了一些。", 38);
            Utils.TypeText("  關於明天，你心裡還沒有答案。", 38);
            Utils.TypeText("  但這一刻，只有平靜。", 38, ConsoleColor.White);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\n  ══════════════════════════════════════════════");
            Console.WriteLine("  HIDDEN ENDING ✦  ─  純淨之道");
            Console.WriteLine("  ✦ 恭喜！你以最純粹的方式完成了整段旅程！");
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.ResetColor();
        }

        private void EndingHiddenBerserk()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║      ★ 隱藏結局：暴走聖者 ★              ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.TypeText("\n  在暴走的狂風暴雨之中，你打倒了魔王。", 38, ConsoleColor.Magenta);
            Utils.TypeText("  那股失控的力量——它一直都在你的掌控之中。", 38);
            Utils.Pause(300);
            Utils.TypeText($"\n  {_player.Name} 緩緩從暴走狀態中甦醒，", 38);
            Utils.TypeText("  卻比任何時候都更加清醒、更加平靜。", 38);
            Utils.Pause(300);
            Utils.TypeText("\n  「我明白了。不是壓制憤怒，」", 38, ConsoleColor.White);
            Utils.TypeText("  「而是——與它共存。」", 38, ConsoleColor.White);
            Utils.Pause(400);

            switch (_player.Class)
            {
                case PlayerClass.Warrior:
                    Utils.TypeText("\n  從今以後，那把劍所到之處，", 38, ConsoleColor.Yellow);
                    Utils.TypeText("  怒火不再是破壞，而是一道守護的屏障。", 38, ConsoleColor.Yellow);
                    break;
                case PlayerClass.Mage:
                    Utils.TypeText("\n  從今以後，你的魔法中融入了怒與靜，", 38, ConsoleColor.Yellow);
                    Utils.TypeText("  那是這個世界前所未見的力量形態。", 38, ConsoleColor.Yellow);
                    break;
                case PlayerClass.Assassin:
                    Utils.TypeText("\n  從今以後，你在黑暗中行動，", 38, ConsoleColor.Yellow);
                    Utils.TypeText("  但那團怒火，成了你永不熄滅的方向感。", 38, ConsoleColor.Yellow);
                    break;
                case PlayerClass.Paladin:
                    Utils.TypeText("\n  從今以後，你的聖光中融入了怒火的熾熱，", 38, ConsoleColor.Yellow);
                    Utils.TypeText("  那份溫度，是神明賜予的，也是你自己的。", 38, ConsoleColor.Yellow);
                    break;
                case PlayerClass.Ranger:
                    Utils.TypeText("\n  從今以後，你的每一支箭都帶著暴走的速度，", 38, ConsoleColor.Yellow);
                    Utils.TypeText("  卻始終精準指向那個你選擇守護的方向。", 38, ConsoleColor.Yellow);
                    break;
            }
            Utils.Pause(300);

            Utils.TypeText("\n  世界還有很多裂縫未被縫合，", 38);
            Utils.TypeText("  但那都是明天的事了。", 38);
            Utils.TypeText("  此刻，你只需要站在這裡，呼吸。", 38, ConsoleColor.Magenta);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\n  ══════════════════════════════════════════════");
            Console.WriteLine("  SECRET ENDING ★  ─  暴走聖者");
            Console.WriteLine("  🎊 恭喜你發現了隱藏結局！");
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.ResetColor();
        }

        private void EndingWorst()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║          最壞結局：魔王轉世                  ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.TypeText("\n  夜陌魯斯死了。", 38, ConsoleColor.DarkRed);
            Utils.TypeText("  但世界並不因此感到安全。", 38, ConsoleColor.DarkRed);
            Utils.Pause(300);
            Utils.TypeText($"\n  {_player.Name} 緩緩轉身，黑暗的紋路已爬滿全身，", 38);
            Utils.TypeText("  那雙眼睛裡，燃燒著和魔王一模一樣的光。", 38, ConsoleColor.DarkMagenta);
            Utils.Pause(400);
            Utils.TypeText("\n  遠處，逃難的士兵對同伴低語：", 38, ConsoleColor.DarkGray);
            Utils.TypeText("  「魔王死了...但那個人...」", 38, ConsoleColor.DarkGray);
            Utils.TypeText("  「那個人比魔王更可怕。」", 38, ConsoleColor.DarkGray);
            Utils.Pause(400);

            switch (_player.Class)
            {
                case PlayerClass.Warrior:
                    Utils.TypeText("\n  曾被稱為無畏戰士的名字，", 38, ConsoleColor.DarkMagenta);
                    Utils.TypeText("  如今成了孩子們夜間最怕聽到的詞語。", 38, ConsoleColor.DarkMagenta);
                    break;
                case PlayerClass.Mage:
                    Utils.TypeText("\n  曾被稱為智慧法師的那雙手，", 38, ConsoleColor.DarkMagenta);
                    Utils.TypeText("  如今只知道如何施展毀滅的魔法。", 38, ConsoleColor.DarkMagenta);
                    break;
                case PlayerClass.Assassin:
                    Utils.TypeText("\n  曾在陰影中守護的那個身影，", 38, ConsoleColor.DarkMagenta);
                    Utils.TypeText("  如今成了陰影本身。", 38, ConsoleColor.DarkMagenta);
                    break;
                case PlayerClass.Paladin:
                    Utils.TypeText("\n  曾以聖光守護眾生的誓約，", 38, ConsoleColor.DarkMagenta);
                    Utils.TypeText("  如今被黑暗徹底燒盡，化為詛咒。", 38, ConsoleColor.DarkMagenta);
                    break;
                case PlayerClass.Ranger:
                    Utils.TypeText("\n  那雙曾指向希望的眼睛，", 38, ConsoleColor.DarkMagenta);
                    Utils.TypeText("  現在只剩下黑暗與虛無的倒影。", 38, ConsoleColor.DarkMagenta);
                    break;
            }
            Utils.Pause(400);

            Utils.TypeText("\n  黑暗的輪迴並沒有終結——", 38);
            Utils.TypeText("  只是換了一個人來背負它。", 38, ConsoleColor.DarkRed);
            Utils.TypeText("\n  你覺得，這一次，還有人能阻止它嗎？", 38, ConsoleColor.DarkGray);

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n\n  ══════════════════════════════════════════════");
            Console.WriteLine("  WORST ENDING  ─  魔王轉世");
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.ResetColor();
        }

        private void EndingBad()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║          壞結局：黑暗的繼承者                ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.TypeText("\n  魔王倒下了，但你感到某種陌生的空洞。", 38);
            Utils.TypeText("  那些吸收的黑暗、那些交易換來的力量——", 38);
            Utils.TypeText("  它們不知何時已深深紮根在你的靈魂之中。", 38);
            Utils.Pause(300);
            Utils.TypeText($"\n  {_player.Name} 緩緩舉起雙手，", 38);
            Utils.TypeText("  手掌上蔓延著黑暗的紋路，和魔王死前的模樣如出一轍。", 38);
            Utils.Pause(400);
            Utils.TypeText("\n  身後，村民們看見了你的變化，紛紛後退。", 38, ConsoleColor.DarkGray);
            Utils.TypeText("  「怪物...那是怪物！」", 38, ConsoleColor.DarkGray);
            Utils.Pause(300);

            switch (_player.Class)
            {
                case PlayerClass.Warrior:
                    Utils.TypeText("\n  盔甲之下，那份曾作為戰士的驕傲，", 38);
                    Utils.TypeText("  正在慢慢碎裂。", 38, ConsoleColor.DarkMagenta);
                    break;
                case PlayerClass.Mage:
                    Utils.TypeText("\n  魔力的流動已不再純淨，", 38);
                    Utils.TypeText("  你甚至分不清那是你的意志，還是黑暗的呼喚。", 38, ConsoleColor.DarkMagenta);
                    break;
                case PlayerClass.Assassin:
                    Utils.TypeText("\n  陰影一直是你的盟友，", 38);
                    Utils.TypeText("  只是現在，連陰影都開始感到陌生。", 38, ConsoleColor.DarkMagenta);
                    break;
                case PlayerClass.Paladin:
                    Utils.TypeText("\n  聖光的誓言在胸口碎裂如同玻璃，", 38);
                    Utils.TypeText("  取而代之的，是冰冷的黑暗在那位置燃燒。", 38, ConsoleColor.DarkMagenta);
                    break;
                case PlayerClass.Ranger:
                    Utils.TypeText("\n  那些曾射向黑暗的箭矢，", 38);
                    Utils.TypeText("  如今轉過了方向，對準了曾守護的人們。", 38, ConsoleColor.DarkMagenta);
                    break;
            }
            Utils.Pause(300);

            Utils.TypeText("\n  打倒魔王的你，已經成為了新的黑暗。", 38, ConsoleColor.DarkMagenta);
            Utils.TypeText("  也許某天，又會有一個勇者被老祭司叫醒，", 38);
            Utils.TypeText("  被派來尋找你。", 38, ConsoleColor.DarkGray);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\n  ══════════════════════════════════════════════");
            Console.WriteLine("  BAD ENDING  ─  黑暗的繼承者");
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.ResetColor();
        }

        private void EndingGrey()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║         灰色結局：業火之中的行者             ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.TypeText("\n  魔王的黑暗消散了，但你的心底——", 38, ConsoleColor.DarkYellow);
            Utils.TypeText("  還留著一片灰燼。", 38, ConsoleColor.DarkYellow);
            Utils.Pause(300);
            Utils.TypeText($"\n  {_player.Name} 沒有像英雄一樣歡呼，", 38);
            Utils.TypeText("  也沒有因黑暗沉淪——只是靜靜地站著，", 38);
            Utils.TypeText("  望著遠方，某種說不清的東西壓在胸口。", 38);
            Utils.Pause(400);

            if (_player.HelpedVillager)
            {
                Utils.TypeText("\n  老婆婆從人群中走來，握住你的手。", 38);
                Utils.TypeText("  「孩子，你還在這裡。這就夠了。」", 38, ConsoleColor.Cyan);
            }
            else
            {
                Utils.TypeText("\n  老祭司靜靜走來，沒有稱讚，也沒有責備。", 38);
                Utils.TypeText("  「你回來了。」他說，僅此而已。", 38, ConsoleColor.Cyan);
            }
            Utils.Pause(400);

            switch (_player.Class)
            {
                case PlayerClass.Warrior:
                    Utils.TypeText("\n  戰士的路，原以為只是勝與敗，", 38);
                    Utils.TypeText("  卻不料，中間還有寬闊的灰色地帶。", 38, ConsoleColor.DarkYellow);
                    break;
                case PlayerClass.Mage:
                    Utils.TypeText("\n  魔法的真理，從不是非黑即白，", 38);
                    Utils.TypeText("  你終於明白了，只是代價有點大。", 38, ConsoleColor.DarkYellow);
                    break;
                case PlayerClass.Assassin:
                    Utils.TypeText("\n  生活在陰影中的人，最了解灰色的味道，", 38);
                    Utils.TypeText("  但這次嚐到的，比任何時候都深。", 38, ConsoleColor.DarkYellow);
                    break;
                case PlayerClass.Paladin:
                    Utils.TypeText("\n  聖光不一定能照亮所有地方，", 38);
                    Utils.TypeText("  你在這段旅程中學到了，有些東西只能自己背負。", 38, ConsoleColor.DarkYellow);
                    break;
                case PlayerClass.Ranger:
                    Utils.TypeText("\n  遊俠的眼睛向來能看清遠方，", 38);
                    Utils.TypeText("  但這一次，連自己的內心都看不清楚了。", 38, ConsoleColor.DarkYellow);
                    break;
            }
            Utils.Pause(400);

            Utils.TypeText("\n  你不確定自己算是英雄還是罪人，", 38);
            Utils.TypeText("  也許，答案會在往後的歲月中慢慢浮現。", 38);
            Utils.TypeText("  或者，永遠不會。", 38, ConsoleColor.DarkGray);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n\n  ══════════════════════════════════════════════");
            Console.WriteLine("  GREY ENDING  ─  業火之中的行者");
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.ResetColor();
        }

        private void EndingGoodLonely()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║          好結局：孤獨的勝者                  ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.TypeText("\n  魔王夜陌魯斯轟然倒地，黑暗之力從世界各角落消散。", 38);
            Utils.TypeText("  天空緩緩破曉，久違的陽光穿透雲層，照亮大地。", 38);
            Utils.Pause(300);
            Utils.TypeText($"\n  {_player.Name} 佇立在廢墟上，等待著什麼。", 38);
            Utils.TypeText("  村民們向你奔來，歡呼聲響徹雲霄。", 38);
            Utils.Pause(300);
            Utils.TypeText("\n  但你記得那條路上的哭聲，", 38, ConsoleColor.DarkGray);
            Utils.TypeText("  記得你沒有停下腳步。", 38, ConsoleColor.DarkGray);
            Utils.Pause(400);

            switch (_player.Class)
            {
                case PlayerClass.Warrior:
                    Utils.TypeText("\n  戰士的信條是前進、前進、再前進，", 38);
                    Utils.TypeText("  但此刻，你第一次懷疑那條信條是否完整。", 38, ConsoleColor.Cyan);
                    break;
                case PlayerClass.Mage:
                    Utils.TypeText("\n  知識讓你強大，書讓你聰明，", 38);
                    Utils.TypeText("  但有些課題，書上根本找不到。", 38, ConsoleColor.Cyan);
                    break;
                case PlayerClass.Assassin:
                    Utils.TypeText("\n  刺客的本能是不回頭，不留戀，", 38);
                    Utils.TypeText("  但今天，那種本能讓你感到一種難以名狀的刺痛。", 38, ConsoleColor.Cyan);
                    break;
                case PlayerClass.Paladin:
                    Utils.TypeText("\n  聖騎士的誓言是守護，守護每一個需要幫助的靈魂，", 38);
                    Utils.TypeText("  但那條路上，你選擇了沉默。這份重量，難以卸下。", 38, ConsoleColor.Cyan);
                    break;
                case PlayerClass.Ranger:
                    Utils.TypeText("\n  遊俠習慣了獨行，習慣了不留痕跡，", 38);
                    Utils.TypeText("  但這一次的獨行，感覺與往日截然不同。", 38, ConsoleColor.Cyan);
                    break;
            }
            Utils.Pause(400);

            Utils.TypeText("\n  世界得救了，這是事實。", 38);
            Utils.TypeText("  你一個人做到了——這也是事實。", 38);
            Utils.TypeText("  在慶祝的人群中，你感到一種奇異的疏離。", 38, ConsoleColor.DarkGray);
            Utils.TypeText("  「勝利是什麼味道？」", 38, ConsoleColor.DarkGray);
            Utils.TypeText("  也許，等你找到答案，才算真的完成了這段旅程。", 38);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n  ══════════════════════════════════════════════");
            Console.WriteLine("  GOOD ENDING  ─  孤獨的勝者");
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.ResetColor();
        }

        private void EndingGood()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║          好結局：光明的彼岸                  ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.TypeText("\n  魔王夜陌魯斯轟然倒地，黑暗之力從世界各角落消散。", 38);
            Utils.TypeText("  天空緩緩破曉，久違的陽光穿透雲層，照亮大地。", 38);
            Utils.Pause(300);
            Utils.TypeText($"\n  {_player.Name} 佇立在魔王城的廢墟上，", 38);
            Utils.TypeText("  三年的昏迷，三天的旅程，此刻都化為胸中的平靜。", 38);
            Utils.Pause(300);
            Utils.TypeText("\n  遠處，村民們向你奔來，歡呼聲響徹雲霄。", 38);
            Utils.TypeText("  那位老婆婆也在人群中，對你露出感激的微笑。", 38);
            Utils.Pause(400);
            Utils.TypeText("\n  「我做到了。」", 38, ConsoleColor.White);
            Utils.TypeText("  這三個字，你等了三年。", 38);
            Utils.Pause(300);

            switch (_player.Class)
            {
                case PlayerClass.Warrior:
                    Utils.TypeText("\n  有人問你，接下來要去哪裡。", 38);
                    Utils.TypeText("  「繼續走吧，」你說，「世界還有更多地方需要守護。」", 38, ConsoleColor.Yellow);
                    break;
                case PlayerClass.Mage:
                    Utils.TypeText("\n  有人問你，那股強大的魔力是怎麼來的。", 38);
                    Utils.TypeText("  「也許...是來自你們的期待吧。」你微微一笑。", 38, ConsoleColor.Cyan);
                    break;
                case PlayerClass.Assassin:
                    Utils.TypeText("\n  人群中，你沒有站在最前面，", 38);
                    Utils.TypeText("  但那個角落裡的微笑，是真實的。", 38, ConsoleColor.DarkGray);
                    break;
                case PlayerClass.Paladin:
                    Utils.TypeText("\n  你單膝跪地，向天空默默祈禱。", 38);
                    Utils.TypeText("  「感謝這份守護的力量——願它長存。」", 38, ConsoleColor.Yellow);
                    break;
                case PlayerClass.Ranger:
                    Utils.TypeText("\n  你摘下弓，靠在肩上，朝著地平線走去。", 38);
                    Utils.TypeText("  「下一片森林，等著我呢。」你輕聲說。", 38, ConsoleColor.DarkGreen);
                    break;
            }
            Utils.Pause(300);

            Utils.TypeText("\n  至於未來——", 38);
            Utils.TypeText("  那是另一個故事了。", 38, ConsoleColor.Yellow);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n  ══════════════════════════════════════════════");
            Console.WriteLine("  GOOD ENDING  ─  光明的彼岸");
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.ResetColor();
        }

        private void EndingPaladinLight()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║      ✦ 聖騎士結局：聖光的歸途 ✦           ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.TypeText("\n  魔王的黑暗在聖光中灰飛煙滅。", 38, ConsoleColor.Yellow);
            Utils.TypeText("  你的手掌微微顫抖，但那道光，從未熄滅過。", 38);
            Utils.Pause(300);
            Utils.TypeText($"\n  {_player.Name} 緩緩轉身，", 38);
            Utils.TypeText("  那些被魔王腐化的靈魂，在聖光的洗禮中一一得到安息。", 38);
            Utils.Pause(300);
            Utils.TypeText("\n  老婆婆拉著你的手，淚眼婆娑：", 38);
            Utils.TypeText("  「孩子，你不只是英雄——你是神明派來的使者。」", 38, ConsoleColor.Cyan);
            Utils.Pause(400);
            Utils.TypeText("\n  你搖搖頭，輕聲說：", 38);
            Utils.TypeText("  「不，我只是一個不願放棄誓言的人。」", 38, ConsoleColor.White);
            Utils.Pause(300);
            Utils.TypeText("\n  聖光在你身後緩緩消散，", 38);
            Utils.TypeText("  留下的，是治癒的土地，和重新呼吸的人們。", 38);
            Utils.TypeText("  這，就是聖騎士的歸途。", 38, ConsoleColor.Yellow);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\n  ══════════════════════════════════════════════");
            Console.WriteLine("  PALADIN ENDING ✦  ─  聖光的歸途");
            Console.WriteLine("  ✦ 以信念與聖光完成了屬於聖騎士的旅程！");
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.ResetColor();
        }

        private void EndingRangerWild()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════╗");
            Console.WriteLine("  ║      ✦ 遊俠結局：荒野的呼喚 ✦             ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.TypeText("\n  最後一箭穿透了黑暗，魔王終於倒下。", 38, ConsoleColor.DarkGreen);
            Utils.TypeText("  你從暴走的混沌中甦醒，弓弦還在顫抖。", 38);
            Utils.Pause(300);
            Utils.TypeText($"\n  {_player.Name} 站在原地，望著四面八方恢復生機的大地。", 38);
            Utils.TypeText("  風從山林間吹來，帶著泥土和野花的氣息。", 38);
            Utils.Pause(300);
            Utils.TypeText("\n  勝利的歡呼聲在遠處響起，但你沒有走過去。", 38);
            Utils.TypeText("  不是因為冷漠——而是因為，你知道自己屬於哪裡。", 38);
            Utils.Pause(400);
            Utils.TypeText("\n  荒野在呼喚你。", 38, ConsoleColor.DarkGreen);
            Utils.TypeText("  那裡有另一片未知的森林，另一種需要守護的生命。", 38, ConsoleColor.DarkGreen);
            Utils.Pause(300);
            Utils.TypeText("\n  你彎弓，朝著那片綠色的遠方，踏出了第一步。", 38);
            Utils.TypeText("  英雄的故事有很多種結尾。", 38);
            Utils.TypeText("  這一個，叫做——繼續前行。", 38, ConsoleColor.DarkGreen);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\n\n  ══════════════════════════════════════════════");
            Console.WriteLine("  RANGER ENDING ✦  ─  荒野的呼喚");
            Console.WriteLine("  ✦ 以暴走之力與遊俠之心完成了屬於你的旅程！");
            Console.WriteLine("  ══════════════════════════════════════════════");
            Console.ResetColor();
        }

        // ── Game Over ────────────────────────────────────────────────────────

        public void ShowGameOver()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"
  ╔═══════════════════════════════════════════╗
  ║                                           ║
  ║             G A M E  O V E R             ║
  ║                                           ║
  ╚═══════════════════════════════════════════╝");
            Console.ResetColor();

            Utils.TypeText($"\n  {_player.Name} 倒在了黑暗之中...", 45, ConsoleColor.DarkRed);
            Utils.TypeText("\n  黑暗漫過大地，吞噬了一切光明。", 45, ConsoleColor.DarkGray);
            Utils.TypeText("\n  世界，失去了最後的希望。", 45, ConsoleColor.DarkGray);
            Utils.Pause(400);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n\n  按 [R] 重新開始，按其他鍵退出...");
            Console.ResetColor();

            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.R)
                RestartRequested = true;
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void PrintRandomEncounterIntro(Enemy enemy)
        {
            Console.Clear();
            string intro = enemy.Name switch
            {
                "哥布林騎士"  => "  一個身著黑鐵盔甲的哥布林騎士擋住了去路，「死在這裡吧，人類！」",
                "骷髏弓手"   => "  一具骷髏從枯木後方竄出，空洞的眼眶射出骨箭！",
                "毒沼蜥蜴"   => "  一隻巨大的蜥蜴從地底破土而出，口中噴吐著紫色毒液！",
                "暗影幽靈"   => "  深入森林，一個漆黑的靈體從陰影中浮現，「你以為能打倒魔王？」",
                "水晶蜘蛛"   => "  枯木縫隙中鑽出一隻水晶蜘蛛，透明的身軀折射出詭異的光！",
                "吸血蝙蝠群" => "  樹冠上騰起一片黑影——大批吸血蝙蝠如浪潮般俯衝而下！",
                "冰霜女巫"   => "  霧氣中緩緩走出一名蒼老的女巫，手中冰晶旋轉發光，「你來送死了？」",
                "憤怒石像"   => "  地面突然震動，苔蘚碎裂間，一尊巨大石像緩緩站起，眼中迸出怒火！",
                "腐化樹妖"   => "  身旁最高的那棵樹忽然扭動，齜牙裂嘴向你撲來，根鬚如鞭橫掃！",
                _            => $"  一個危險的敵人【{enemy.Name}】突然擋住了你的去路！"
            };
            Utils.TypeText($"\n  {intro}", 38, ConsoleColor.DarkGray);
        }

        // ════════════════════════════════════════════════════════════════════
        // FUNNY SIDEQUESTS ── 幽默支線劇情
        // ════════════════════════════════════════════════════════════════════

        private void PlayFunnySidequest_SlimeLord()
        {
            Console.Clear();
            Utils.TypeText("\n  你剛打完史萊姆，繼續往前走了幾步。", 38);
            Utils.TypeText("  路中間，又站著一隻史萊姆。", 38);
            Utils.Pause(300);
            Utils.TypeText("  但這隻不太一樣。", 38);
            Utils.Pause(400);
            Utils.TypeText("  牠頭上頂著一個用枯葉折成的小王冠，", 38);
            Utils.TypeText("  面前插著一塊木牌，用爪痕歪歪扭扭地刻著：", 38);
            Utils.Pause(200);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ┌───────────────────────────────┐");
            Console.WriteLine("  │  此路通行費：1 金幣           │");
            Console.WriteLine("  │  本大爺乃上古龍王轉世         │");
            Console.WriteLine("  │  違者格殺勿論                 │");
            Console.WriteLine("  └───────────────────────────────┘");
            Console.ResetColor();
            Utils.TypeText("\n  牠沒有手。", 38, ConsoleColor.DarkGray);
            Utils.Pause(400);

            Utils.PrintTitle("你 要 怎 麼 做");
            Console.WriteLine("  [1] 「你連手都沒有，怎麼『格殺勿論』？」（迎戰）");
            Console.WriteLine("  [2] 「好吧...就當捐款了。」（付出 5 HP）");
            Console.WriteLine("  [3] 「龍王轉世？說來聽聽。」");

            int c = Utils.GetChoice("你的選擇", 1, 3);

            if (c == 1)
            {
                Utils.TypeText("\n  史萊姆沉默了一秒，然後開始劇烈顫抖。", 38);
                Utils.TypeText("  「...你找死！本大爺全力以赴！」", 38, ConsoleColor.Yellow);
                Utils.Pause(300);
                var r = _battle.StartBattle(Enemy.CreateSlime());
                if (HandleDefeat(r)) return;
                Utils.TypeText("\n  王冠在地上滾了幾圈，停在你腳邊。", 38, ConsoleColor.DarkGray);
                Utils.TypeText("  「原來...龍王的第九條命...也不過如此...」", 38, ConsoleColor.DarkGray);
            }
            else if (c == 2)
            {
                _player.HP = Math.Max(1, _player.HP - 5);
                Utils.TypeText("\n  你象徵性地往牠面前放了一塊小石頭。", 38);
                Utils.TypeText("  史萊姆盯著石頭看了三秒，開心地晃動了一下。", 38, ConsoleColor.Yellow);
                Utils.TypeText("  「哼，識時務！本大爺賜你通行。」", 38, ConsoleColor.Yellow);
                Utils.TypeText("  然後牠吐出一顆藍色的圓珠，彈到你腳邊。", 38);
                Utils.TypeText("  「龍族的禮物，拿去吧，凡人。」", 38, ConsoleColor.Yellow);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  ✦ 獲得「龍珠（偽）」── 恢復 20 MP");
                Console.ResetColor();
                _player.MP = Math.Min(_player.MaxMP, _player.MP + 20);
            }
            else
            {
                Utils.TypeText("\n  史萊姆挺了挺王冠，清了清喉嚨（不確定牠有沒有喉嚨）。", 38);
                Utils.TypeText("  「本大爺三千年前是九頭火龍，因得罪天道被詛咒轉世...」", 38, ConsoleColor.Yellow);
                Utils.Pause(200);
                Utils.TypeText("  「我本來有九條命，」牠說，語氣突然低沉，「現在...」", 38, ConsoleColor.Yellow);
                Utils.Pause(400);
                Utils.TypeText("  「...只剩一條了。不然我早揍你了！」", 38, ConsoleColor.Yellow);
                Utils.Pause(200);
                Utils.TypeText("  「所以你選擇在路邊收費？」你問。", 38);
                Utils.TypeText("  「生活所迫！你見過史萊姆成功求職的嗎！」", 38, ConsoleColor.Yellow);
                Utils.Pause(400);
                Utils.TypeText("\n  你同情地點了點頭，掏出一塊乾糧放在木牌前。", 38);
                Utils.TypeText("  「...謝謝。」史萊姆沉默片刻，小聲說。", 38, ConsoleColor.Yellow);
                Utils.Pause(200);
                Utils.TypeText("  「對了，魔王城在東邊。別說是我告訴你的。」", 38, ConsoleColor.DarkGray);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  ✦ 善良之心：恢復 10 HP");
                Console.ResetColor();
                _player.HP = Math.Min(_player.MaxHP, _player.HP + 10);
            }

            Utils.PressAnyKey();
        }

        private void PlayFunnySidequest_LostMinion()
        {
            Console.Clear();
            Utils.TypeText("\n  深入森林，你聽到灌木叢後有動靜。", 38);
            Utils.TypeText("  不是獸的腳步，而是......抱怨聲？", 38);
            Utils.Pause(300);
            Utils.TypeText("  「向右...不對...向左...這破地圖根本看不懂啊啊啊！」", 38, ConsoleColor.DarkRed);
            Utils.Pause(200);
            Utils.TypeText("  你撥開草叢，發現一個頭戴彎角頭盔的小惡魔，", 38);
            Utils.TypeText("  正對著一張皺巴巴的地圖抓耳撓腮，完全沒發現你。", 38);
            Utils.TypeText("  牠手上還提著一個冒熱氣的飯盒，上面貼著：", 38);
            Utils.Pause(200);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n  「魔王大人的午餐 ── 請勿轉交給勇者」");
            Console.ResetColor();
            Utils.Pause(400);

            Utils.PrintTitle("你 要 怎 麼 做");
            Console.WriteLine("  [1] 「你在找什麼地方？」（搭話）");
            Console.WriteLine("  [2] 悄悄繞過去（別惹麻煩）");
            Console.WriteLine("  [3] 直接打倒牠");

            int c = Utils.GetChoice("你的選擇", 1, 3);

            if (c == 1)
            {
                Utils.TypeText("\n  小惡魔一臉感激地轉向你：「你知道魔王城怎麼走嗎！」", 38, ConsoleColor.DarkRed);
                Utils.TypeText("  「我在這個森林轉了三個小時了！老大的午飯都涼了！」", 38, ConsoleColor.DarkRed);
                Utils.Pause(200);
                Utils.TypeText("  你朝東邊指了指。（這也是你要去的方向。）", 38);
                Utils.TypeText("  「謝謝！你真是個好人！這個給你！」", 38, ConsoleColor.DarkRed);
                Utils.TypeText("  牠從腰間掏出一個備用飯盒塞到你手裡，轉身狂奔。", 38);
                Utils.Pause(400);
                Utils.TypeText("  跑了五步，牠突然停下來，緩緩回頭。", 38);
                Utils.Pause(600);
                Utils.TypeText("  「......等等。」", 38, ConsoleColor.DarkRed);
                Utils.TypeText("  「你......是人類？」", 38, ConsoleColor.DarkRed);
                Utils.TypeText("  「你......拿著武器？」", 38, ConsoleColor.DarkRed);
                Utils.Pause(300);
                Utils.TypeText("  「你......是那個傳說中的勇者？！」", 38, ConsoleColor.DarkRed);
                Utils.Pause(300);
                Utils.TypeText("  小惡魔發出一聲撕心裂肺的慘叫，以你從未見過的速度消失在森林深處。", 38);
                Utils.TypeText("  只留下一陣「老大我不去送午餐了啊啊啊」的迴響。", 38, ConsoleColor.DarkGray);
                Utils.Pause(400);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  ✦ 獲得「魔族備用飯盒」── 恢復 35 HP");
                Console.ResetColor();
                _player.HP = Math.Min(_player.MaxHP, _player.HP + 35);
            }
            else if (c == 2)
            {
                Utils.TypeText("\n  你小心翼翼地側身繞過草叢，幾乎成功了。", 38);
                Utils.Pause(500);
                Utils.TypeText("  然後你踩到一根樹枝。", 38, ConsoleColor.DarkGray);
                Utils.Pause(400);
                Utils.TypeText("  「咦？那是......人類？！救命啊啊啊！」", 38, ConsoleColor.DarkRed);
                Utils.TypeText("  小惡魔扔下飯盒，以光速逃離。", 38);
                Utils.TypeText("  飯盒滾到你腳邊，蓋子彈開，熱氣升騰。", 38);
                Utils.Pause(300);
                Utils.TypeText("  你看了看四周，撿起來吃了。", 38);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  ✦ 撿到「魔王的午餐（涼了）」── 恢復 20 HP");
                Console.ResetColor();
                _player.HP = Math.Min(_player.MaxHP, _player.HP + 20);
            }
            else
            {
                Utils.TypeText("\n  小惡魔還沒看到你，就被打倒在地。", 38);
                Utils.TypeText("  倒地之際，牠嘟囔了一句：", 38);
                Utils.TypeText("  「...午飯...沒送到...老大一定會扣我薪水的...」", 38, ConsoleColor.DarkGray);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\n  ✦ 獲得「戰利品午餐」── 恢復 15 HP");
                Console.WriteLine("  ※ 腐化值 +1（打倒一個毫無威脅的外送員，有點說不過去）");
                Console.ResetColor();
                _player.HP = Math.Min(_player.MaxHP, _player.HP + 15);
                _player.CorruptionLevel++;
            }

            Utils.PressAnyKey();
        }

        private void PlayFunnySidequest_DemonCafeteria()
        {
            Console.Clear();
            Utils.TypeText("\n  走入魔王城內部，走廊深處飄來了......香噴噴的食物味？", 38);
            Utils.Pause(400);
            Utils.TypeText("  你推開一扇偏門，發現三名魔族士兵圍著小桌喝湯，", 38);
            Utils.TypeText("  正在大聲抱怨。", 38);
            Utils.Pause(200);
            Utils.TypeText("  「魔王大人今天又發火了！說我巡邏時間到了還在摸魚！」", 38, ConsoleColor.DarkRed);
            Utils.TypeText("  「誰叫你巡邏時偷偷看那個人類頻道...」", 38, ConsoleColor.DarkRed);
            Utils.TypeText("  「那個勇者三年後才來，這班根本無聊到要命！」", 38, ConsoleColor.DarkRed);
            Utils.Pause(400);
            Utils.TypeText("  三個魔族一回頭，和你四目相交。", 38);
            Utils.Pause(600);
            Utils.TypeText("  沉默。", 38, ConsoleColor.DarkGray);
            Utils.Pause(500);

            Utils.PrintTitle("你 要 怎 麼 做");
            Console.WriteLine("  [1] 「繼續，當我沒來。」（不打擾）");
            Console.WriteLine("  [2] 「你們魔王在哪裡？」（套話）");
            Console.WriteLine("  [3] 「巡邏的就該盡職！」（迎戰）");

            int c = Utils.GetChoice("你的選擇", 1, 3);

            if (c == 1)
            {
                Utils.TypeText("\n  你轉身準備離開。", 38);
                Utils.TypeText("  身後低聲傳來：「...勇者不打我們？」「別動。」「裝死。」", 38, ConsoleColor.DarkGray);
                Utils.Pause(300);
                Utils.TypeText("  你走到門口，一碗熱湯悄悄推了過來。", 38);
                Utils.TypeText("  你回頭，三個魔族立刻望向天花板，吹口哨。", 38, ConsoleColor.DarkRed);
                Utils.Pause(300);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  ✦ 獲得「魔族大鍋湯」── 恢復 40 HP 和 20 MP");
                Console.ResetColor();
                _player.HP = Math.Min(_player.MaxHP, _player.HP + 40);
                _player.MP = Math.Min(_player.MaxMP, _player.MP + 20);
            }
            else if (c == 2)
            {
                Utils.TypeText("  三個魔族同時開口：「在最頂層！」「在地下室！」「在廁所！」", 38, ConsoleColor.DarkRed);
                Utils.TypeText("  三人互看一眼：「哪個對？」「我不確定！」「你說什麼？！」", 38, ConsoleColor.DarkRed);
                Utils.Pause(300);
                Utils.TypeText("  混亂之中，一個魔族悄悄對你耳語：", 38);
                Utils.TypeText("  「最頂層，一直走就到了。今天在氣頭上，小心。」", 38, ConsoleColor.DarkRed);
                Utils.TypeText("  然後大喊：「撤退！勇者來了！」三人落荒而逃。", 38);
                Utils.Pause(200);
                Utils.TypeText("  桌上的湯還熱著。你端起來喝了。", 38);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  ✦ 獲得「情報 + 廢棄的湯」── 恢復 25 HP");
                Console.ResetColor();
                _player.HP = Math.Min(_player.MaxHP, _player.HP + 25);
            }
            else
            {
                Utils.TypeText("\n  「好吧好吧！打就打！反正薪水也不夠！」", 38, ConsoleColor.DarkRed);
                Utils.TypeText("  三個魔族磨磨蹭蹭站起來，拿起武器。", 38);
                Utils.TypeText("  「等等，我湯還沒喝完——」「先打架！」「........」", 38, ConsoleColor.DarkGray);
                Utils.Pause(300);
                var r = _battle.StartBattle(Enemy.CreateDemonSoldier());
                if (HandleDefeat(r)) return;
                Utils.TypeText("\n  三個魔族狼狽逃竄，其中一個還抱走了湯碗。", 38, ConsoleColor.DarkGray);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  ※ 腐化值 +1（魔族打工仔在氣頭上也是人……算了，是魔族）");
                Console.ResetColor();
                _player.CorruptionLevel++;
            }

            Utils.PressAnyKey();
        }

        private bool HandleDefeat(BattleResult result)
        {
            if (result != BattleResult.Defeat) return false;
            ShowGameOver();
            GameOverTriggered = true;
            return true;
        }
    }
}
