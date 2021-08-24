using Battle.config;
using Battle.data.sync;
using Battle.data.sync.client_side;
using Battle.data.xml;
using Battle.network;
using Core.server;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace Battle
{
    public class Program
    {

        protected static void Main(string[] args)
        {
            LicenseVerif();
            Console.WindowWidth = 100;
            CheckIntegry();
            Config.Load();
            Config.LoadDB();
            Logger.CheckDirectory();
            LoggerHacker.CheckDirectory();
            Console.Title = "[PBEMU] 1.0 UDP";

            StringUtil @string = new StringUtil();
            {
                AnimationBattle();
                Logger.Info("==================LORENSTUDIO====================");
                Logger.Info("========== PBEMU UDPSYSTEM[1.0] ==========");
                Logger.Info("=================================================");
                @string.AppendLine("");
                Logger.Info(@string.GetString());
            }


            Process.GetCurrentProcess().WaitForExit();
        }
        public static void AnimationBattle()
        {
            char[] anims = new char[4] { 'R', 'O', 'O', 'H' };
            int animsIndex = 0;
            for (int i = 0; i < anims.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(anims[animsIndex++]);
                Console.ResetColor();
                Thread.Sleep(80);
                Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                if (animsIndex >= anims.Length)
                    animsIndex = 0;
            }
            Console.WriteLine("\n");
        }
        public static void CheckIntegry()
        {
            if (Process.GetProcessesByName("Point Blank - UDP").Length == 0)
                Environment.Exit(0);
        }

        public static void LicenseVerif()
        {
            new Action(() =>
            {
                {
                    Logger.Info($"╔════════════════ L.S System ══════════════════╗");
                    Logger.Warning(" [UDP System] IP: " + Config.hosIp + ":" + Config.hosPort + "");
                    Logger.Warning(" [UDP System] Versões: " + Config.udpVersion + " -> " + Config.ServerVersion + "");
                    MappingXML.Load();
                    CharaJSON.Load();
                    MeleeExceptionsJSON.Load();
                    ServersXML.Load();
                    Battle_SyncNet.Start();
                    BattleManager.Init();
                    GetPrestart.Remove();
                    Logger.Info("╚═══════════════════════════════════════════════╝");
                }
            }).BeginInvoke(null, null);

        }
    }
}