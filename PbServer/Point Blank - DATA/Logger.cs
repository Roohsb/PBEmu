using System;
using System.IO;

namespace Core
{
    public static class Logger
    {
        public static void Info(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(" [ INFO ] ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(text);
            Console.WriteLine();
            Save(text);
        }
        public static void Sucess(string text, bool sucess)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" [ SUCESS ] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(text);
            Console.WriteLine();
        }
        public static void Carregar(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" [ LOADING ] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(text);
            Console.WriteLine();
        }

        public static void GameSystem(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" [ SYSTEM ] ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.WriteLine();
        }

        public static void EventSystem(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(" [ EVENT ] ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(text);
            Console.WriteLine();
        }

        public static void Comandos(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" [ COMMANDS ] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[ >> ] " + text);
            Console.WriteLine();
        }
        public static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" [ ERROR ] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.WriteLine();
            Save(text);
        }

        public static void ChatLog(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" [ CHAT ] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.WriteLine();
            Save(text);
        }

        private static void Save(string text)
        {
            try
            {
                if (!Directory.Exists("logs/ServerSide"))
                    Directory.CreateDirectory("logs/ServerSide");
                using (StreamWriter stream = new StreamWriter("logs/ServerSide/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"))
                {
                    if (stream != null)
                        stream.WriteLine(text);
                    stream.Close();
                }
            }
            catch
            {

            }
        }
    }
}