using System;
using System.IO;

namespace Point_Blank_Debug.core
{
    public class Loggers
    {
        public static void Green(string txt)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(txt);
            Console.ResetColor();
            Save(txt);
        }
        public static void ColorFundo(string txt)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(txt);
            Console.ResetColor();
        }
        public static void Red(string txt)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" [ ERROR ] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(txt);
            Console.WriteLine();
            Save(txt);
        }
        public static void Yellow(string txt)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" [ SYSTEM ] ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(txt);
            Console.WriteLine();
            Save(txt);
        }
        public static void Blue(string txt)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(txt);
            Console.ResetColor();
        }

        public static void LorenStudio(string txt)
        {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(" [ INFO ] ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(txt);
                Console.WriteLine();
        }
        public static void FeedBack(string txt)
        {
            if (!Directory.Exists("logs/FeedBack"))
                Directory.CreateDirectory("logs/FeedBack");
            using (StreamWriter stream = new StreamWriter("logs/FeedBack/" + DateTime.Now.ToString("ddMMyy") + ".log", true))
            {
                try
                {
                    if (stream != null && txt != null)
                    {
                        stream.WriteLine(txt);
                        stream.Flush();
                        stream.Close();
                    }
                }
                catch { }
            }
        }
        public static void MegaFone(string txt)
        {
            if (!Directory.Exists("logs/MegaFone"))
                Directory.CreateDirectory("logs/MegaFone");
            using (StreamWriter stream = new StreamWriter("logs/MegaFone/" + DateTime.Now.ToString("ddMMyy") + ".log", true))
            {
                try
                {
                    if (stream != null && txt != null)
                    {
                        stream.WriteLine(txt);
                        stream.Flush();
                        stream.Close();
                    }
                }
                catch { }
            }
        }
        public static void Hack(string txt)
        {
            if (!Directory.Exists("logs/Hack"))
                Directory.CreateDirectory("logs/Hack");
            using (StreamWriter stream = new StreamWriter("logs/Hack/" + DateTime.Now.ToString("ddMMyy") + ".log", true))
            {
                try
                {
                    if (stream != null && txt != null)
                    {
                        stream.WriteLine(txt);
                        stream.Flush();
                        stream.Close();
                    }
                }
                catch { }
            }
        }

        public static void Comandos(string v)
        {
            if (!Directory.Exists("logs/Commandos"))
                Directory.CreateDirectory("logs/Commandos");
            using (StreamWriter stream = new StreamWriter("logs/Commandos/" + DateTime.Now.ToString("ddMMyy") + ".log", true))
            {
                try
                {
                    if (stream != null && v != null)
                    {
                        stream.WriteLine(v);
                        stream.Flush();
                        stream.Close();
                    }
                }
                catch { }
            }
        }

        public static void ChatLog(string v)
        {
            if (!Directory.Exists("logs/ChatLog"))
                Directory.CreateDirectory("logs/ChatLog");
            using (StreamWriter stream = new StreamWriter("logs/ChatLog/" + DateTime.Now.ToString("ddMMyy") + ".log", true))
            {
                try
                {
                    if (stream != null && v != null)
                    {
                        stream.WriteLine(v);
                        stream.Flush();
                        stream.Close();
                    }
                }
                catch { }
            }
        }

        public static void Save(string str)
        {
            if (!Directory.Exists("logs/srvOn"))
                Directory.CreateDirectory("logs/srvOn");
            using (StreamWriter stream = new StreamWriter("logs/srvOn/" + DateTime.Now.ToString("ddMMyy") + ".log", true))
            {
                try
                {
                    if (stream != null && str != null)
                    {
                        stream.WriteLine(str);
                        stream.Flush();
                        stream.Close();
                    }
                }
                catch { }
            }
        }
    }
}
