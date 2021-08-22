using Core;
using Core.DB_Battle;
using Core.filters;
using Core.managers;
using Core.managers.events;
using Core.managers.server;
using Core.models.enums;
using Core.models.servers;
using Core.server;
using Core.xml;
using Game.data.chat;
using Game.data.managers;
using Game.data.model;
using Game.data.sync;
using Game.data.xml;
using Game.global.serverpacket;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Linq;
using Game.Progress;
using IntelliLock.Licensing;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace Game
{
    public class Programm
    {

        string KeyLoren = "keyLoren= g8ch?Jodr*d7";


        public static void Main(string[] args)
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

                    var result = System.Windows.Forms.MessageBox.Show("Você está tentando utilizar um arquivo sem licença, que tal comprar uma? :)", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
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

            try
            {
              

                Console.Title = "[PBEMU] 1.0 System...";
                    Console.WindowWidth = 100;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string srvtxt = "[1] FREEMODE \n" +
                     "[2] PREMIUMMODE \n" +
                     "[3] DEVMODE \n" +

                        "[Source] Diga seu código de serviço: ";
                    Console.WriteLine(srvtxt);
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.White;
                    int next = int.Parse(Console.ReadLine());
                    if (next != 0 && next < 8)
                        NextModel.Servicos(next);
                Console.Write("Você pretendo iniciar o servidor? [Sim] / [Nao] [Caso você seja premium digite sua chave] -> ");
                    Console.ResetColor();
                    string senha = Console.ReadLine();
                    if (senha.Equals(NextModel.Senha))
                    {
                        Console.Clear();
                    //Process.Start("http://lorenstudio.com");
                    LorenstudioSettings.Load();
                         LicenseSystem.Load();
                        ConfigLeitura.Loader();
                    string licenseFile = @"lorenstudio.license";

                    // To ensure SDK method calls doesn't block/delay the control flow the SDK method LoadLicense(...) should be run in asynchronous context (new Action()..) as well
                    new Action(() =>
                    {

                        EvaluationMonitor.LoadLicense(File.ReadAllBytes(licenseFile));
                        EvaluationMonitor.LicenseCheckFinished += () =>
                        {
                           // System.Windows.Forms.MessageBox.Show("Licença encontrada ^^");
                            OpenDepuracao();

                       

                        StringUtil @string = new StringUtil();

                        {
                            AnimationGame();
                               Logger.Info("==================LORENSTUDIO====================");
                               Logger.Info("========== PBEMU GAMESYSTEM[1.0] ==========");
                               Logger.Info("=================================================");
                                @string.AppendLine("");
                                Logger.Info(@string.GetString());
                        }

                        Logger.GameSystem($"╔════════════════ Game system ══════════════════╗");
                        Logger.GameSystem(" [System] Tipo: " + LorenstudioSettings.ProjectName + "");
                        Logger.GameSystem(" [System] Data: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                        Logger.GameSystem(" [System] System: " + (Environment.Is64BitProcess ? "64bits" : "32bits"));
                        Logger.GameSystem(" [System] Não altere as portas de sincronização..");
                        Settings.Load();
                        BasicInventoryJSON.Load();
                         MapsXML.Load();
                        ServerConfigSyncer.GenerateConfig(Settings.configId);
                        EventLoader.LoadAll();
                        ServersXML.Load();
                        ChannelsXML.Load(Settings.serverId);
                        TitlesXML.Load();
                        TitleAwardsXML.Load();
                        ClanManager.Load();
                        NickFilter.Load();
                        CupomEffectManagerJSON.LoadCupomFlags();
                        MissionCardXML.LoadBasicCards(1);
                        BattleServerJSON.Load();
                        RankJSON.Load();
                        RankJSON.RankAwards();
                        //RankJSON.GetGenerais();
                        ClanRankJSON.Load();
                        MissionAwardsJSON.Load();
                        MissionJSON.Load();
                        Translation.Load();
                        ShopLoader();
                        ClassicModeManager.LoadList();
                        RandomBoxXML.LoadBoxes();
                        Game_SyncNet.Start();
                        NextModel.IPPublic();
                        bool started = LoginManager.StartAuth() && GameManager.StartGame();
                        Logger.GameSystem(" [System] Game site: " + LoginManager.Config.ExitURL + "");
                        string texto = !started ? "  -> Servidor não iniciado. " : " -> Servidor conectado com sucesso.";
                        Logger.Sucess(texto, started);
                        Logger.GameSystem($"╚═══════════════════════════════════════════════════╝");
                        SendDebug.SendInfo("[Service]Os servidores estão se comunicando agora.");
                        SendDebug.SendInfo("[Service] Seu IP foi registrado e enviado! " + NextModel.IPAdress);
                        if (started)
                            new Thread(WaitConsole.UpdateRAM).Start();
                        Comands();
                        };

                    }).BeginInvoke(null, null);
                }

                    else
                    {
                        Console.Clear();
                        Logger.Error("[System] Senha inválida, esqueci sua senha? S / N");
                        string esqueceu = Console.ReadLine();
                        if (esqueceu != null && esqueceu.ToLower() == "s")
                            Process.Start("http://lorenstudio.com");
                        else
                        {
                        LicenseSystem.UpdateClosePB();
                    }
                    }

            }

            catch (Exception EX)
            {
                Console.WriteLine(EX.ToString());
            }
                Process.GetCurrentProcess().WaitForExit();

        }

        public void CheckExpirationDateLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.ExpirationDate_Enabled;
            System.DateTime expiration_date = EvaluationMonitor.CurrentLicense.ExpirationDate;
        }

        public void InvalidateLicense()
        {
            string confirmation_code = License_DeActivator.DeactivateLicense();
        }

        public byte[] GetLicense()
        {
            return EvaluationMonitor.GetCurrentLicenseAsByteArray();
        }

      
    public void ReadAdditonalLicenseInformation()
        {
          
            if (EvaluationMonitor.CurrentLicense.LicenseStatus == LicenseStatus.Licensed)
            {
             
                for (int i = 0; i < EvaluationMonitor.CurrentLicense.LicenseInformation.Count; i++)
                {
                    string key = EvaluationMonitor.CurrentLicense.LicenseInformation.GetKey(i).ToString();
                    string value = EvaluationMonitor.CurrentLicense.LicenseInformation.GetByIndex(i).ToString();
                }
            }
        }
        public bool IsValidLicenseAvailable()
        {
            return (EvaluationMonitor.CurrentLicense.LicenseStatus == LicenseStatus.Licensed);
        }

        public void CheckExecutionsLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.Executions_Enabled;
            int max_executions = EvaluationMonitor.CurrentLicense.Executions;
            int current_executions = EvaluationMonitor.CurrentLicense.Executions_Current;
        }



        public static void ShopLoader()
        {
            if (Settings.BlackFriday)
            {
                Logger.GameSystem(" [System] Evento, Loja Online da Metade do Preço.");
                ShopManager.Load(1, true);
            }
            else
                ShopManager.Load(1, false);
        }
        public static void AnimationGame()
        {
                    char[] anims = new char[12] { 'L', 'O', 'R', 'E', 'N', ' ', 'S', 'T', 'U', 'D', 'I', 'O'};
                    int animsIndex = 0;
                    for (int i = 0; i < anims.Length; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(anims[animsIndex++]);
                        Console.ResetColor();
                        Thread.Sleep(80);
                        Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                        if (animsIndex >= anims.Length)
                            animsIndex = 0;
                    }
                    Console.WriteLine(Environment.NewLine);
                }
        


        public static void OpenDepuracao()
        {

           
                {
                    if (Process.GetProcessesByName("Point Blank - Debug").Length == 0)
                        Process.Start("Point Blank - Debug.exe");
                }
        }

        public static void Comands()
        {
            while (true)
            {
                try
                {
                    Logger.Comandos("esperando pelo comando (.cmd):");
                    switch (Console.ReadLine().ToLower())
                    {
                        case ".msg":
                            {
                                Logger.Comandos("Digite o texto para enviar a todos:");
                                using SERVER_MESSAGE_ANNOUNCE_PAK pacote = new SERVER_MESSAGE_ANNOUNCE_PAK(Console.ReadLine());
                                SendDebug.SendInfo("Mensagem enviada para " + GameManager.SendPacketToAllClients(pacote) + " players.");
                                Logger.Sucess("Executado com sucesso.", true);
                                break;
                            }
                        case ".version":
                            {
                                Logger.Sucess("Você esta usando a versão 1.0 do PBEMU By LorenStudio.", true);
                                break;

                            }
                        //case ".portas":
                        //   {
                        //   Logger.Sucess("Battle: 5325,7468,57600", true);
                        //    Logger.Sucess("GameServer: 23372,w,7468", true);
                        //    break;

                        //  }
                        case ".dc":
                                {
                               SendDebug.SendInfo(KickAllPlayers.KickPlayersForServer());
                                    Logger.Sucess("Executado com sucesso.", true);
                              break;
                            }
                        //  case ".list":
                        //      {
                        //          LoginManager._lIstClient.Clear();
                        //          GameManager._lIstClient.Clear();
                        //          SendDebug.SendInfo("You have cleared the Captured list.");
                        //          Logger.Sucess("Successfully executed.", true);
                        //         break;
                        // }
                        // case ".evchat":
                        //     {
                        //         Settings.EventChat = !Settings.EventChat;
                        //         SendDebug.SendInfo(Settings.EventChat == false ? "chat event disabled." : "chat event enabled.");
                        //         Logger.Sucess("Successfully executed.", true);
                        //         break;
                        //     }
                        // case ".cap":
                        //     {
                        //         foreach (var count1 in from SocketsInProcess count1 in LoginManager._lIstClient.Values
                        //                                   where count1.Handler != null
                        //                                   select count1)
                        //            {
                        //                SendDebug.SendInfo("AUTH IP: " + count1.Handler);
                        //}

                        //foreach (var count2 in from SocketsInProcess count2 in GameManager._lIstClient.Values
                        //                                      where count2.Handler != null
                        //                                     select count2)
                        //              {
                        //                   SendDebug.SendInfo("GAME IP: " + count2.Handler);
                        //              }
                        //               Logger.Sucess("Successfully executed.", true);
                        //               break;
                        //           }
                              case ".events":
                                  {
                                      for (int i = -1; i <= 6; ++i)
                                          EventLoader.ReloadEvent(i);
                                      EventLoader.LoadAll();
                                      SendDebug.SendInfo("Events Restarted.");
                                      Logger.Sucess("Successfully executed.", true);
                                      break;
                                  }
                        //      case ".cons1":
                        //          {
                        //              if (Listcache._conn.Count > 0)
                        //              {
                        //                   Logger.Comandos("Enter the player's IP: ");
                        //     string ip = Console.ReadLine();
                        //           if (ip != null)
                        //          {
                        //                 bool rev = Listcache._conn.Remove(ip);
                        //                if (rev)
                        //                     SendDebug.SendInfo("IP Removed successfully.");
                        //              }
                        //            }
                        //            Logger.Sucess("Successfully executed.", true);
                        //             break;
                        //           }
                        //       case ".udp":
                        //           {
                        //               Logger.Comandos("Enter the desired UDP: ");
                        //               int udp = int.Parse(Console.ReadLine());
                        //               if (udp > 0 && udp < 5)
                        //                   Settings.udpType = (SERVER_UDP_STATE)udp;
                        //               SendDebug.SendInfo("the server UDP has been changed to " + Settings.udpType);
                        //               Logger.Sucess("Successfully executed.", true);
                        //               break;
                        //           }
                        //       case ".sk":
                        //          {
                        // GameManager.ClearGC();
                        //  Logger.Sucess("Successfully executed.", true);
                        // break;
                        // }
                        //     case ".camp":
                        //        {
                        //             if (!Settings.EnableClassicRules)
                        //                  Settings.EnableClassicRules = true;
                        //               if (ClassicModeManager.Clear())
                        //                   ClassicModeManager.LoadList();
                        //               SendDebug.SendInfo("@camp reloaded. ");
                        //                Logger.Sucess("Successfully executed.", true);
                        //                break;
                        //              }
                        //         case ".alls":
                        //             {
                        //                 string str = string.Empty;
                        //                 foreach (GameClient GC in GameManager._socketList.Values)
                        //                 {
                        //                     if (GC != null)
                        //                     {
                        //                         Account pr = GC._player;
                        //                         if (pr != null && pr._isOnline)
                        //                             str += $"nick: {pr.player_name} \n";
                        //                     }
                        //                 }
                        //                 Console.WriteLine(str);
                        //                 break;
                        //            }
                        //         case ".cons":
                        //             {
                        //          if (Listcache._conn.Count > 0)
                        //                  {
                        //                      Listcache._conn.Clear();
                        //                      Game_SyncNet.RemovePrestart(new IPEndPoint(IPAddress.Parse(Settings.IP_Jogo), 1910));
                        //                      SendDebug.SendInfo("those connected to battle were reset.");
                        //                  }
                        //                  Logger.Sucess("Successfully executed.", true);
                        //                  break;
                        //              }
                        case ".auto":
                                         {
                                             Settings.AUTO_ACCOUNTS = !Settings.AUTO_ACCOUNTS;
                           SendDebug.SendInfo(Settings.AUTO_ACCOUNTS == true ? "A criação automática de conta foi ativada." : "A criação automática de conta foi desativada.");
                                Logger.Sucess("Executado com sucesso.", true);
                                break;
                            }
                        case ".close":
                            {
                                LicenseSystem.UpdateClosePB();
                                break;
                            }
                        //    case ".clear":
                        //        {
                        //            Console.Clear();
                        //            SendDebug.Clear();
                        //            Logger.Sucess("Successfully executed.", true);
                        //            break;
                        //        }
                        //    case ".ons":
                        //        {
                        //            for (int serverId = 0; serverId < ServersXML._servers.Count; serverId++)
                        //            {
                        //                GameServerModel server = ServersXML.getServer(serverId);
                        //                if (server != null)
                        //                    SendDebug.SendInfo($"[S] ID: {server._id} Players: {server._LastCount} / {server._maxPlayers}");
                        //   }
                        //   Logger.Sucess("Successfully executed.", true);
                        //         break;
                        //}
                        //   case ".info":
                        //       {
                        //           int pOnlines = GameManager._socketList.Count;
                        //           int pCapturados1 = LoginManager._lIstClient.Count;
                        //           int pCapturados2 = GameManager._lIstClient.Count;
                        //           int total = pCapturados1 + pCapturados2;
                        //           string nome = LorenstudioSettings.ProjectName;
                        //           string BuildCompilation = NextModel.BuildPB;
                        //           string Creditos = "by: LorenStudio";
                        //           string Texto = $"Existem {pOnlines} onlines no jogo, e {total} conexões capturadas. \n" +
                        //               $"Nome do Projeto: {nome}. \n" +
                        //               $"Data de Compilação: {BuildCompilation} \n" +
                        //               $"{Creditos}";
                        //           SendDebug.SendInfo(Texto);
                        //     Logger.Sucess("Successfully executed.", true);
                        //       break;
                        //     }
                        case ".shop":
                                 {
                                     ShopManager.Reset();
                                     ShopManager.Load(1, false);
                                     Logger.Sucess("Executado com sucesso.", true);
                                     break;
                                 }
                            case ".vip":
                                {
                                    Logger.Comandos("Insira o ID do jogador:");
                                    long pID = long.Parse(Console.ReadLine());
                                    if(pID > 0)
                                    {
                                        Logger.Comandos("Digite o vip: 1, 2 ou 5 e 6");
                                 int vip = int.Parse(Console.ReadLine());
                                  if (vip > 0 && vip < 7 && vip != 3 && vip != 4)
                                  {
                                      Logger.Comandos("Insira o dia de término: Método exemplar(yyyyMMdd)");
                                      int finishdate = int.Parse(Console.ReadLine());
                                      if (finishdate > 0 && string.Concat(finishdate).Length == 8)
                                      {
                                          Account account = AccountManager.GetAccount(pID, 0);
                                          if(account != null && account._isOnline)
                                          {
                                              account.SendPacket(new AUTH_ACCOUNT_KICK_PAK(0));
                                              account.Close(1000);
                                          }
                                          int startdate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                          if(startdate < finishdate)
                               {
                               if (PlayerManager.UpdateAccountVip(pID, vip))
                                              {
                                          if(!PlayerManager.InsertPlayerVip(pID, startdate, finishdate))
                                              PlayerManager.UpdatePlayerVip(pID, startdate, finishdate);
                                          Logger.Sucess("Executado com sucesso.", true);
                                      }
                                  }
                              }
                          }
                      }
                      break;
                  }
                        case ".cmd":
                            {
                                Logger.Sucess(".msg      Envie mensagem para todos. \n" +
                                    // ".dc        desconecte todos os jogadores. \n" +
                                    // ".list      clear the list of caught. \n" +
                                    // ".info      Show all server information. \n" +
                                    ".version  Versão do emulador. \n" +
                                    //".portas Portas PBEMU. \n" +
                                    ".auto      Criação automática de conta ligada ou desligada?. \n" +
                                // ".udp       Change Udp from 1 to 4. \n" +
                                // ".cap       Show Captured ip.\n" +
                                // ".cons      Clear the firewall list (1910). \n" +
                                // ".cons1     Remove 1 firewall IP (1910). \n" +
                                //".camp      reload  @Camp. \n" +
                                 ".shop      reload  shop. \n" +
                                //".clear     Clear all from the Debug Console. \n" +
                                // ".alls      View Onlines. \n" +
                                // ".sk        Clean up lost sockets. \n" +
                                ".events    Restart Events. \n" +
                                 ".close    Fecha o pb de uma forma s. \n" +
                                // ".evchat    Enable or disable chat event. \n" +
                                ".vip       definir VIP.", true);
                                    // ".ons       How many Players are on each server.", true);
                                break;
                            }
                        default:
                            {
                                Logger.Sucess("Comandando inválido.", false);
                                break;
                            }
                    }
                }
                catch
                {
                    Console.WriteLine("Error");
                    break;
                }
            }
        }
    }
}