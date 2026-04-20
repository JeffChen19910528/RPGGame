using System;

namespace RPGGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console configuration
            Console.Title = "暴走：黑暗年代記  |  RAGE: Chronicles of Darkness";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                if (Console.WindowWidth < 70)
                    Console.WindowWidth = 70;
            }
            catch { /* Ignore if terminal doesn't support resizing */ }

            if (args.Length > 0 && args[0] == "--test")
            {
                GameTests.RunAll();
                return;
            }

            var game = new GameManager();
            game.Run();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n  感謝遊玩。按任意鍵退出...");
            Console.ResetColor();
            Console.ReadKey(true);
        }
    }
}
