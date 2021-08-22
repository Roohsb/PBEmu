using Core.server;
using Point_Blank_Debug.Conection;
using Point_Blank_Debug.core;
using System;
using System.Diagnostics;

namespace Point_Blank_Debug
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.Title = "[PBEMU] 1.0 Debug";
                Console.WindowWidth = 100;
                if (CheckIntegry())
                {
                    StringUtil @string = new StringUtil();
                    {
                        Loggers.LorenStudio("==================LORENSTUDIO====================");
                        Loggers.LorenStudio("========== PBEMU DEBUGSYSTEM[1.0] ==========");
                        Loggers.LorenStudio("=================================================");
                        @string.AppendLine("");
                        Loggers.LorenStudio(@string.GetString());
                    }
                    if (SessionUDP.Start())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Sistema Iniciado -> ");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Loggers.Red(ex.ToString());
            }
            Process.GetCurrentProcess().WaitForExit();
        }
        public static bool CheckIntegry()
        {
            try
            {
                if (Process.GetProcessesByName("Point Blank - Debug").Length == 0)
                    Environment.Exit(0);
                else if (Process.GetProcessesByName("Point Blank - Emulador").Length == 0)
                {
                    Loggers.Red("[Service] Abra este aplicativo após abrir o Point Blank System.");
                    return false;
                }
                else if (Process.GetProcessesByName("Point Blank - Debug").Length > 1)
                {
                    Loggers.Red("[Service] Este aplicativo já está aberto.");
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
