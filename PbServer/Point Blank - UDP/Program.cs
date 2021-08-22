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
using IntelliLock.Licensing;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace Battle
{
    public class Program
    {

        string KeyLoren = "keyLoren= g8ch?Jodr*d7";

        protected static void Main(string[] args)
        {
            Verific();
            LicenseVerif();
            // ConfigLeitura.Loader();
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
            char[] anims = new char[12] { 'L', 'O', 'R', 'E', 'N', ' ', 'S', 'T', 'U', 'D', 'I', 'O' };
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
            string licenseFile = @"lorenstudio.license";

              new Action(() =>
            {

                EvaluationMonitor.LoadLicense(File.ReadAllBytes(licenseFile));
                EvaluationMonitor.LicenseCheckFinished += () =>
                {
                    MessageBox.Show("Licença encontrada ^^");
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
                };

            }).BeginInvoke(null, null);

        }

        public static void Verific()
        {
            WebClient request = new WebClient();
            String KeyLoren = "keyLoren= g8ch?Jodr*d7";
            String NewVersion = request.DownloadString("https://lorenstudio.com/versionfree.txt");
            String Update = request.DownloadString("https://lorenstudio.com/versionfree.txt");
            NewVersion = new WebClient().DownloadString("https://lorenstudio.com/versionfree.txt");
            NewVersion = new WebClient().DownloadString("https://lorenstudio.com/versionfree.txt");
            if (Update.Contains(KeyLoren))
            {


            }
            else
            {

                var result = MessageBox.Show("Você está tentando utilizar um arquivo sem licença, que tal comprar uma? :)", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {

                    Process.Start("http://community.lorenstudio.com/");

                }
                if (result == DialogResult.No)
                {

                    Console.Clear();
                    Environment.Exit(0);

                }

            }

        }
    }
}