using System;
using System.Threading;

namespace RPGGame
{
    public enum TextSpeed { Instant, Fast, Normal, Slow }
    public enum DifficultyLevel { Easy, Normal, Hard }

    public static class GameSettings
    {
        public static TextSpeed Speed { get; set; } = TextSpeed.Normal;
        public static DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Normal;

        public static double SpeedFactor => Speed switch
        {
            TextSpeed.Instant => 0.0,
            TextSpeed.Fast    => 0.33,
            TextSpeed.Slow    => 2.0,
            _                 => 1.0
        };

        public static double EnemyDamageMultiplier => Difficulty switch
        {
            DifficultyLevel.Easy => 0.75,
            DifficultyLevel.Hard => 1.4,
            _                    => 1.0
        };

        public static double EXPMultiplier => Difficulty switch
        {
            DifficultyLevel.Easy => 1.2,
            DifficultyLevel.Hard => 0.85,
            _                    => 1.0
        };
    }

    public static class Utils
    {
        public static void TypeText(string text, int delay = 30, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            int actualDelay = (int)(delay * GameSettings.SpeedFactor);
            foreach (char c in text)
            {
                Console.Write(c);
                if (actualDelay > 0) Thread.Sleep(actualDelay);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void PrintTitle(string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("  ╔═══════════════════════════════════════════╗");
            string padded = $"  {title}";
            Console.WriteLine($"  ║  {title.PadRight(41)} ║");
            Console.WriteLine("  ╚═══════════════════════════════════════════╝");
            Console.ResetColor();
            _ = padded; // suppress warning
        }

        public static void PressAnyKey(string? prompt = null)
        {
            string label = prompt ?? L10n.Get("PRESS_ANY_KEY");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"\n  {label}");
            Console.ResetColor();
            Console.ReadKey(true);
            Console.WriteLine();
        }

        public static int GetChoice(string prompt, int min, int max)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n  {prompt} [{min}-{max}]: ");
                Console.ResetColor();

                string? input = Console.ReadLine();
                if (int.TryParse(input, out int choice) && choice >= min && choice <= max)
                    return choice;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(L10n.Get("INPUT_ERROR", min, max));
                Console.ResetColor();
            }
        }

        public static string GetString(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"\n  {prompt}: ");
            Console.ResetColor();
            string? result = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(result))
                return L10n.Get("CREATE_DEFAULT_NAME");
            return result.Length > GameConstants.MaxPlayerNameLength
                ? result[..GameConstants.MaxPlayerNameLength]
                : result;
        }

        public static void DrawProgressBar(int current, int max, int width = 20, ConsoleColor fillColor = ConsoleColor.Green)
        {
            if (max <= 0) max = 1;
            int filled = (int)((double)Math.Max(0, current) / max * width);
            filled = Math.Min(filled, width);
            Console.Write("[");
            Console.ForegroundColor = fillColor;
            Console.Write(new string('█', filled));
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', width - filled));
            Console.ResetColor();
            Console.Write("]");
        }

        public static void Pause(int ms = 800) => Thread.Sleep(ms);

        public static void Separator(char c = '─', int width = 47)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + new string(c, width));
            Console.ResetColor();
        }

        public static void PrintColored(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}
