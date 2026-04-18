using System;
using System.Threading;

namespace RPGGame
{
    public static class Utils
    {
        public static void TypeText(string text, int delay = 30, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                if (delay > 0) Thread.Sleep(delay);
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
            return string.IsNullOrWhiteSpace(result) ? L10n.Get("CREATE_DEFAULT_NAME") : result;
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
