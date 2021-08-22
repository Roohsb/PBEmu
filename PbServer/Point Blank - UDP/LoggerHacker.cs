using System;
using System.IO;

namespace Battle
{
    public static class LoggerHacker
    {
        private static string name = "logs/HackerCheck/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
        private static object Sync = new object();
        private static void write(string text, ConsoleColor color)
        {
            try
            {
                lock (Sync)
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(text);
                    Console.ResetColor();
                    Save(text);
                }
            }
            catch
            {
            }
        }
        public static void Error1(string text)
        {
            write(text, ConsoleColor.Red);
        }
        public static void Warning1(string text)
        {
            write(text, ConsoleColor.Green);
        }
        public static void Carregar1(string text)
        {
            write(text, ConsoleColor.White);
        }
        public static void HackerCheck(string text)
        {
            lock (Sync)
            {
                //Console.ForegroundColor = ConsoleColor.DarkGray;
                //Console.Write($"[{DateTime.Now:yyyy-MM-dd}]");
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.Write(" [ DAMAGE ] ");
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.Write(text);
                //Console.WriteLine();
                Save(text);
            }
        }
        public static void InBattle1(string text)
        {
            write(text, ConsoleColor.DarkGray);
        }
        private static void Save(string text)
        {
            using (StreamWriter stream = new StreamWriter(name, true))
            {
                try
                {
                    if (stream != null)
                        stream.WriteLine(text);
                }
                catch { }
                finally
                {
                    stream.Dispose();
                    stream.Close();
                }
            }
        }
        public static void CheckDirectory()
        {
            if (!Directory.Exists("logs/HackerCheck"))
                Directory.CreateDirectory("logs/HackerCheck");
        }
    }
}