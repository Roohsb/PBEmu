using System;
using System.IO;

namespace Battle
{
    public static class Logger
    {
        private static string name = "logs/battle/" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".log";
        private static object Sync = new object();
       
        public static void Error(string text)
        { 
         lock (Sync)
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
}
       public static void Warning(string text)
        {
            {
                lock (Sync)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(" [ WARNING ] ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(text);
                    Console.WriteLine();
                    Save(text);
                }
            }
        }
        public static void Carregar(string text)
        {
            {
                lock (Sync)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" [ LOADING ] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(text);
                    Console.WriteLine();
                    Save(text);
                }
            }
        }
        public static void Info(string text)
        {
            {
                {
                    lock (Sync)
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
                }
            }
        }
        public static void InBattle(string text)
        {
            {
                {
                    lock (Sync)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(" [ BATTLE ] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(text);
                        Console.WriteLine();
                        Save(text);
                    }
                }
            }
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
                catch{}
                finally
                {
                    stream.Dispose();
                    //stream.Flush();
                    stream.Close();
                }
            }
        }
        public static void CheckDirectory()
        {
            if (!Directory.Exists("logs/battle"))
                Directory.CreateDirectory("logs/battle");
        }
    }
}