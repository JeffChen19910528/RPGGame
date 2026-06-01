using System;

namespace RPGGame
{
    public static class AsciiArt
    {
        // All enemy art: 7 lines tall, 22 chars wide (padded)
        // All player art: 7 lines tall, 16 chars wide (padded)

        // ── PLAYER ──────────────────────────────────────────────────────────

        public static readonly string[] PlayerNormal =
        {
            "                ",
            "     .O.        ",
            "    /|=|\\       ",
            "   +-+.+-+      ",
            "     |          ",
            "    / \\         ",
            "                ",
        };

        public static readonly string[] PlayerAttack =
        {
            "                ",
            "     .O.        ",
            "    /|=|\\──>    ",
            "   +-+.+-+      ",
            "     |          ",
            "    / \\         ",
            "                ",
        };

        public static readonly string[] PlayerSkill =
        {
            "   * . * . *    ",
            "   .  .O.  .    ",
            "  *  /|=|\\  *   ",
            "  . +-+.+-+ .   ",
            "   *   |   *    ",
            "      / \\       ",
            "   * . * . *    ",
        };

        public static readonly string[] PlayerBerserk =
        {
            "  * *(.@.)* *   ",
            "  * /|=|\\  *   ",
            "  *+-+.+-+*     ",
            " ***  |  ***    ",
            "  *  / \\  *     ",
            "  * * * * *     ",
            "                ",
        };

        public static readonly string[] PlayerDefend =
        {
            "                ",
            "     .O.        ",
            "   [/|=|\\]      ",
            "  [+-+.+-+]     ",
            "    [ | ]       ",
            "    / \\         ",
            "                ",
        };

        public static readonly string[] PlayerHurt =
        {
            "    * *         ",
            "    x.O.x       ",
            "    /|=|\\       ",
            "   +-+.+-+      ",
            "     |          ",
            "    / \\         ",
            "                ",
        };

        public static readonly string[] PlayerDead =
        {
            "                ",
            "                ",
            "   _.-O-._      ",
            "  /=======\\     ",
            " ~~~~~~~~~~~    ",
            "                ",
            "                ",
        };

        // ── SLIME (22 wide) ──────────────────────────────────────────────────

        public static readonly string[] SlimeNormal =
        {
            "                      ",
            "      .~~~~~.         ",
            "     (  o o  )        ",
            "     (   v   )        ",
            "      '~~~~~'         ",
            "       '~~~'          ",
            "                      ",
        };

        public static readonly string[] SlimeHurt =
        {
            "     * * * *          ",
            "      .~~~~~.         ",
            "     (  x x  )        ",
            "     (   -   )        ",
            "      '~~~~~'         ",
            "      '~~~~~'         ",
            "                      ",
        };

        public static readonly string[] SlimeDead =
        {
            "                      ",
            "      .~~~~~.         ",
            "     (  - -  )        ",
            "     (   .   )        ",
            "      .~~~~~.         ",
            "         *            ",
            "                      ",
        };

        // ── GOBLIN KNIGHT (22 wide) ───────────────────────────────────────────

        public static readonly string[] GoblinNormal =
        {
            "      /^^^\\           ",
            "     | > < |          ",
            "     | --- |          ",
            "   /-|  +  |-\\        ",
            "     |  |  |          ",
            "     /  |  \\          ",
            "    /   |   \\         ",
        };

        public static readonly string[] GoblinAttack =
        {
            "      /^^^\\           ",
            "     | > < |          ",
            "     | --- |          ",
            "  <--| (+) |          ",
            "     |  |  |          ",
            "     /  |  \\          ",
            "    /   |   \\         ",
        };

        public static readonly string[] GoblinHurt =
        {
            "    * /^^^\\*          ",
            "     | x x |          ",
            "     | --- |          ",
            "   /-|  +  |-\\        ",
            "     |  |  |          ",
            "     /  |  \\          ",
            "    /   |   \\         ",
        };

        public static readonly string[] GoblinDead =
        {
            "                      ",
            "    ___/^^^\\__        ",
            "   / | x x | \\       ",
            "  /  | --- |  \\      ",
            " /~~~+~~+~~+~~~\\      ",
            " ~~~~~~~~~~~~~~~      ",
            "                      ",
        };

        // ── SHADOW WRAITH (22 wide) ───────────────────────────────────────────

        public static readonly string[] WraithNormal =
        {
            "    ~~~~*~~~~         ",
            "   / O     O \\        ",
            "  /  \\  v  /  \\       ",
            "  \\   ~~~~~   /       ",
            "   ~~~~~~~~~~~        ",
            "    ~~ ~~~ ~~         ",
            "     ~     ~          ",
        };

        public static readonly string[] WraithAttack =
        {
            "    ~~~~*~~~~         ",
            "  </ O     O \\        ",
            "  /  \\  ^  /  \\       ",
            "  \\   ~~~~~   /       ",
            "   ~~~~~~~~~~~        ",
            "    ~~ ~~~ ~~         ",
            "     ~     ~          ",
        };

        public static readonly string[] WraithHurt =
        {
            "   * ~~~~*~~~~ *      ",
            "   / x     x \\        ",
            "  /  \\  -  /  \\       ",
            "  \\   ~~~~~   /       ",
            "   ~~~~~~~~~~~        ",
            "    ~~ ~~~ ~~         ",
            "     ~     ~          ",
        };

        public static readonly string[] WraithDead =
        {
            "                      ",
            "    ....~....         ",
            "   .  -     -  .      ",
            "  .    .....    .     ",
            "   .............      ",
            "    .. ... ..         ",
            "                      ",
        };

        // ── DARK PALADIN (22 wide) ────────────────────────────────────────────

        public static readonly string[] PaladinNormal =
        {
            "     [=^=]            ",
            "     |> <|            ",
            "     [===]            ",
            "    /| + |\\           ",
            "   / [===] \\          ",
            "     / | \\            ",
            "    /  |  \\           ",
        };

        public static readonly string[] PaladinAttack =
        {
            "     [=^=]            ",
            "     |> <|            ",
            "     [===]            ",
            " <--/| (+)|\\          ",
            "   / [===] \\          ",
            "     / | \\            ",
            "    /  |  \\           ",
        };

        public static readonly string[] PaladinHurt =
        {
            "   * [=^=] *          ",
            "     |x  x|           ",
            "     [===]            ",
            "    /| - |\\           ",
            "   / [===] \\          ",
            "     / | \\            ",
            "    /  |  \\           ",
        };

        public static readonly string[] PaladinDead =
        {
            "                      ",
            "   ____[=^=]____      ",
            "  / -- |x  x| --\\     ",
            " /  -- [===] --  \\    ",
            "/~~~~/|  -  |\\~~~~\\   ",
            " ~~~~~~~~~~~~~~~~~~~~  ",
            "                      ",
        };

        // ── DEMON KING (22 wide) ──────────────────────────────────────────────

        public static readonly string[] DemonKingNormal =
        {
            "  Y  [=^=]  Y         ",
            "     |@ @|            ",
            "     | V |            ",
            "    [=====]           ",
            "   /|     |\\          ",
            "  / [=====] \\         ",
            "    /     \\           ",
        };

        public static readonly string[] DemonKingAttack =
        {
            "  Y  [=^=]  Y         ",
            "     |@ @|            ",
            "     | V |            ",
            " <--[=====]           ",
            "   /|     |\\          ",
            "  / [=====] \\         ",
            "    /     \\           ",
        };

        public static readonly string[] DemonKingHurt =
        {
            "* Y  [=^=]  Y *       ",
            "     |x  x|           ",
            "     | - |            ",
            "    [=====]           ",
            "   /|     |\\          ",
            "  / [=====] \\         ",
            "    /     \\           ",
        };

        public static readonly string[] DemonKingDead =
        {
            "                      ",
            "  Y__..[=^=]..__ Y    ",
            "      |x  x|          ",
            "      | - |           ",
            "   __[=====]__        ",
            " ~~~~~~~~~~~~~~~~~    ",
            "  ~~~~~~~~~~~~~~~     ",
        };

        // ── SERPENT KING (22 wide) ───────────────────────────────────────────

        public static readonly string[] SerpentNormal =
        {
            "   ~*[=III=]*~        ",
            "  / (o     o) \\      ",
            "  \\ ( ~v~~~ ) /      ",
            "   )~~~~~~~~~~(       ",
            "  (~~~~~~~~~~~~)      ",
            "  ~~~~~~~~~~~~~~~~~   ",
            "     ~~~   ~~~        ",
        };

        public static readonly string[] SerpentAttack =
        {
            "   ~*[=III=]*~        ",
            "<-/ (o     o) \\      ",
            "  \\ ( ~v~~~ ) /      ",
            "   )~~~~~~~~~~(       ",
            "  (~~~~~~~~~~~~)      ",
            "  ~~~~~~~~~~~~~~~~~   ",
            "     ~~~   ~~~        ",
        };

        public static readonly string[] SerpentHurt =
        {
            "  *~*[=III=]*~*       ",
            "  / (x     x) \\      ",
            "  \\ ( -~-~- ) /      ",
            "   )~~~~~~~~~~(       ",
            "  (~~~~~~~~~~~~)      ",
            "  ~~~~~~~~~~~~~~~~~   ",
            "     ~~~   ~~~        ",
        };

        public static readonly string[] SerpentDead =
        {
            "                      ",
            "  __~*[=III=]*~__     ",
            "  / (-.    .-) \\     ",
            "  ~~~~~~~~~~~~~~~~~   ",
            "  ~~~~~~~~~~~~~~~~~   ",
            "  ~~~  ~~~~~  ~~~     ",
            "                      ",
        };

        // ── ABYSS DEMON (22 wide) ────────────────────────────────────────────

        public static readonly string[] AbyssDemonNormal =
        {
            " \\     ~~~     /      ",
            "  \\Y (o   o) Y/      ",
            "  (  | >-< |  )       ",
            "  \\  |=====|  /      ",
            "  /  |=====|  \\      ",
            " /   / ||| \\   \\    ",
            "     ~~   ~~          ",
        };

        public static readonly string[] AbyssDemonAttack =
        {
            " \\     ~~~     /      ",
            "  \\Y (o   o) Y/      ",
            "<-(  | >-< |  )       ",
            "  \\  |=====|  /      ",
            "  /  |=====|  \\      ",
            " /   / ||| \\   \\    ",
            "     ~~   ~~          ",
        };

        public static readonly string[] AbyssDemonHurt =
        {
            "*\\     ~~~     /*     ",
            "  \\Y (x   x) Y/      ",
            "  (  | --- |  )       ",
            "  \\  |=====|  /      ",
            "  /  |=====|  \\      ",
            " /   / ||| \\   \\    ",
            "     ~~   ~~          ",
        };

        public static readonly string[] AbyssDemonDead =
        {
            "                      ",
            "   \\Y       Y/        ",
            "    ( -   - )         ",
            "  ~~~|=====|~~~       ",
            "  ~~~~~~~~~~~~~       ",
            "  ~~~~~~~~~~~~~       ",
            "                      ",
        };

        // ── PHANTOM KNIGHT (22 wide) ─────────────────────────────────────────

        public static readonly string[] PhantomNormal =
        {
            "   ~~[=^=]~~          ",
            "   ~ |> <| ~          ",
            "   ~ [===] ~          ",
            "  ~~/|   |\\~~        ",
            "  ~/       \\~        ",
            "  ~/   |   \\~        ",
            "  ~~   |   ~~         ",
        };

        public static readonly string[] PhantomAttack =
        {
            "   ~~[=^=]~~          ",
            "   ~ |> <| ~          ",
            "   ~ [===] ~          ",
            "<-~~/|   |\\~~        ",
            "  ~/       \\~        ",
            "  ~/   |   \\~        ",
            "  ~~   |   ~~         ",
        };

        public static readonly string[] PhantomHurt =
        {
            "  *~~[=^=]~~*         ",
            "   ~ |x  x| ~         ",
            "   ~ [===] ~          ",
            "  ~~/|   |\\~~        ",
            "  ~/       \\~        ",
            "  ~/   |   \\~        ",
            "  ~~   |   ~~         ",
        };

        public static readonly string[] PhantomDead =
        {
            "                      ",
            "  ....~~[=^=]..       ",
            "  . ~ |-..-| ~.       ",
            "  . ~ [===] ~.        ",
            "  ..............      ",
            "   ............       ",
            "                      ",
        };

        // ── CORRUPTED BISHOP (22 wide) ───────────────────────────────────────

        public static readonly string[] BishopNormal =
        {
            "      /|\\             ",
            "     /[+]\\            ",
            "    / |+| \\           ",
            "   [++++++]           ",
            "   |  )  ( |          ",
            "   | /    \\|          ",
            "   |/______\\|         ",
        };

        public static readonly string[] BishopAttack =
        {
            "      /|\\             ",
            "     /[+]\\            ",
            "    / |+| \\           ",
            "<--[++++++]           ",
            "   |  )  ( |          ",
            "   | /    \\|          ",
            "   |/______\\|         ",
        };

        public static readonly string[] BishopHurt =
        {
            "    * /|\\ *           ",
            "     /[x]\\            ",
            "    / |+| \\           ",
            "   [++++++]           ",
            "   |  )  ( |          ",
            "   | /    \\|          ",
            "   |/______\\|         ",
        };

        public static readonly string[] BishopDead =
        {
            "                      ",
            "  ____/|\\____         ",
            " / -- /[x]\\ --\\      ",
            "/  --[++++++]-- \\    ",
            " ~~~~~~~~~~~~~~~~     ",
            " ~~~~~~~~~~~~~~~~     ",
            "                      ",
        };

        // ── SCENES ────────────────────────────────────────────────────────────

        public static readonly string[] SceneVillage =
        {
            "  /\\     /\\          /\\    /\\   ",
            " /  \\   /  \\   /\\   /  \\  /  \\  ",
            "/    \\_/    \\ /  \\ /    \\/    \\ ",
            "|  _ | _  _ ||  ||  _    _  _  |",
            "| |_|| |_||_||  || |_|  |_||_| |",
            "|    |         |  |              |",
            " \\______________\\|______________/",
        };

        public static readonly string[] SceneForest =
        {
            "  *   *  *    *   *   *   *  *  ",
            " ***  *** *  *** *** *** ***  ** ",
            "*****  |  **  |  *|  |  *  |  **",
            "  |    |   |  |  ||  |   |  |   ",
            "  |    |   |  |  ||  |   |  |   ",
            "~~|~~~~|~~~|~~|~~||~~|~~~|~~|~~~",
            "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~",
        };

        public static readonly string[] SceneCastle =
        {
            "    ||  ||    ||  ||    ||  ||  ",
            "   [||][||]  [||][||]  [||][||] ",
            "╔══╩══╩══╩════╩══╩══╩════╩══╩══╗",
            "║                              ║",
            "║   ╔══════╗      ╔══════╗    ║",
            "║   ║ >  < ║      ║ >  < ║    ║",
            "╚═══╩══════╩══════╩══════╩════╝",
        };

        public static readonly string[] SceneBattleflash =
        {
            " * * * * * * * * * * * * * * * ",
            "* BATTLE START! BATTLE START! * ",
            " * * * * * * * * * * * * * * * ",
        };

        // ── EFFECTS ────────────────────────────────────────────────────────

        public static readonly string[] EffectSlash =
        {
            "   ~~~~~~~~~~~~~~>  ",
        };

        public static readonly string[] EffectSlashBerserk =
        {
            " *~~~~~~~~~~~~~~>*  ",
            "  *~~~~~~~~~~~>* *  ",
        };

        public static readonly string[] EffectFire =
        {
            "  )  )  )  ) )      ",
            " ( ) ( ) ( )(       ",
            "  )  )  )  ) )      ",
        };

        public static readonly string[] EffectIce =
        {
            "  * . * . * . *     ",
            " . * . * . * . *    ",
            "  * . * . * . *     ",
        };

        public static readonly string[] EffectHeal =
        {
            "  + . + . + . +     ",
            "   + . + . + .      ",
            "  + . + . + . +     ",
        };

        public static readonly string[] EffectBerserkOn =
        {
            " *#*#*#*#*#*#*#*#*  ",
            "#*    BERSERK    *#  ",
            " *#*#*#*#*#*#*#*#*  ",
        };

        public static readonly string[] EffectHit =
        {
            "    * WHAM! *       ",
        };

        public static readonly string[] EffectCritical =
        {
            "  ** CRITICAL!! **  ",
        };

        public static readonly string[] EffectSpecialHit =
        {
            "  !! SPECIAL !!     ",
        };

        public static readonly string[] EffectLightning =
        {
            "  /\\/\\/\\/\\/\\  ",
            " /\\/\\/\\/\\/\\/ ",
            "  /\\/\\/\\/\\/\\  ",
        };

        public static readonly string[] EffectUltra =
        {
            " ★★★★★★★★★★★★★★★ ",
            " ★   U L T R A !  ★ ",
            " ★★★★★★★★★★★★★★★ ",
        };

        public static readonly string[] SceneGraveyard =
        {
            "  +    +   +    +   +  ",
            " /|\\  /|\\  /|\\  /|\\ /|\\ ",
            "  |    |    |    |   |  ",
            "__|____|____|____|___|__",
            "  .~.   .~.  .~.  .~.  ",
            " (   ) (   )(   )(   ) ",
            "  `~`   `~`  `~`  `~`  ",
        };

        // ── HELPERS ───────────────────────────────────────────────────────────

        public static string[] GetEnemyArt(string name, string state = "normal")
        {
            return (name, state) switch
            {
                ("史萊姆",      "normal") => SlimeNormal,
                ("史萊姆",      "hurt")   => SlimeHurt,
                ("史萊姆",      "dead")   => SlimeDead,
                ("史萊姆",      "attack") => SlimeHurt,

                ("哥布林騎士",  "normal") => GoblinNormal,
                ("哥布林騎士",  "hurt")   => GoblinHurt,
                ("哥布林騎士",  "dead")   => GoblinDead,
                ("哥布林騎士",  "attack") => GoblinAttack,

                ("暗影幽靈",    "normal") => WraithNormal,
                ("暗影幽靈",    "hurt")   => WraithHurt,
                ("暗影幽靈",    "dead")   => WraithDead,
                ("暗影幽靈",    "attack") => WraithAttack,

                ("黑暗聖騎士",  "normal") => PaladinNormal,
                ("黑暗聖騎士",  "hurt")   => PaladinHurt,
                ("黑暗聖騎士",  "dead")   => PaladinDead,
                ("黑暗聖騎士",  "attack") => PaladinAttack,

                ("魔王・夜陌魯斯", "normal") => DemonKingNormal,
                ("魔王・夜陌魯斯", "hurt")   => DemonKingHurt,
                ("魔王・夜陌魯斯", "dead")   => DemonKingDead,
                ("魔王・夜陌魯斯", "attack") => DemonKingAttack,

                ("石化蛇王",   "normal") => SerpentNormal,
                ("石化蛇王",   "hurt")   => SerpentHurt,
                ("石化蛇王",   "dead")   => SerpentDead,
                ("石化蛇王",   "attack") => SerpentAttack,

                ("深淵惡魔",   "normal") => AbyssDemonNormal,
                ("深淵惡魔",   "hurt")   => AbyssDemonHurt,
                ("深淵惡魔",   "dead")   => AbyssDemonDead,
                ("深淵惡魔",   "attack") => AbyssDemonAttack,

                ("幽靈騎士",   "normal") => PhantomNormal,
                ("幽靈騎士",   "hurt")   => PhantomHurt,
                ("幽靈騎士",   "dead")   => PhantomDead,
                ("幽靈騎士",   "attack") => PhantomAttack,

                ("腐化主教",   "normal") => BishopNormal,
                ("腐化主教",   "hurt")   => BishopHurt,
                ("腐化主教",   "dead")   => BishopDead,
                ("腐化主教",   "attack") => BishopAttack,

                _ => new[] { $"  [ {name} ]  " }
            };
        }

        public static string[] GetPlayerArt(Player player, string state = "normal")
        {
            if (player.IsBerserk && state is "normal" or "attack")
                return PlayerBerserk;

            return state switch
            {
                "attack"  => PlayerAttack,
                "skill"   => PlayerSkill,
                "defend"  => PlayerDefend,
                "hurt"    => PlayerHurt,
                "dead"    => PlayerDead,
                "berserk" => PlayerBerserk,
                _         => PlayerNormal,
            };
        }
    }
}
