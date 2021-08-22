using Core;
using Core.models.enums;
using Core.models.enums.global;
using System;
using System.Text;

namespace Game
{
    public static class Settings
    {
        public static string ChannelPass, IP_Jogo, MeioDeComunicacao;
        public static bool isTestMode, debugMode, winCashPerBattle, showCashReceiveWarn, EnableClassicRules, BlackFriday, EnableTimeReal, ShowInitialRoom, AUTO_ACCOUNTS, Outpost;
        public static SERVER_UDP_STATE udpType;
        public static float maxClanPoints;
        public static int serverId, configId, syncPort, maxNickSize, minNickSize, maxActiveClans, minRankVote, maxBattleXP, maxBattleGP, maxBattleMY, maxChannelPlayers, gamePort, authPort, minCreateGold, minCreateRank, DropItem;
        public static ulong LauncherKey;
        public static ClientLocale LocalDoGame;
        public static string BOT_Name;
        public static bool EventChat, DropBox;

        public static void Load()
        {
            try
            {
                ConfigFile configFile = new ConfigFile("config/Settings.ini");

                serverId = configFile.ReadInt32("Servico", -1);
                configId = configFile.ReadInt32("Config", 0);
                IP_Jogo = configFile.ReadString("Protocolo", "127.0.0.1");
                authPort = configFile.ReadInt32("Autenticacao", 39193);
                gamePort = configFile.ReadInt32("Sistema", 39194);
                syncPort = configFile.ReadInt32("Sincronizacao", 1909);
                LauncherKey = configFile.ReadUInt64("LancadorChave", 0);
                LocalDoGame = (ClientLocale)configFile.ReadInt32("GamePais", 0);
                debugMode = configFile.ReadBoolean("Depuracao", true);
                isTestMode = configFile.ReadBoolean("SomenteGM", true);
                ConfigGB.EncodeText = Encoding.GetEncoding(configFile.ReadInt32("EncodingPage", 0));
                AUTO_ACCOUNTS = configFile.ReadBoolean("AutoLogin", false);
                Outpost = configFile.ReadBoolean("Outpost", false);
                BOT_Name = configFile.ReadString("BOT", "BOT");
                EventChat = configFile.ReadBoolean("EventoChat", false);
                DropItem = configFile.ReadInt32("DropItem", 1301045000);
                DropBox = configFile.ReadBoolean("DropBox", false);
                MeioDeComunicacao = configFile.ReadString("MeioDeComunicacao", "SeuDiscordOuTs");
                winCashPerBattle = configFile.ReadBoolean("CashPorPartida", true);
                showCashReceiveWarn = configFile.ReadBoolean("MensagemCashRecebido", true);
                EnableClassicRules = configFile.ReadBoolean("EnableClassicRules", false);
                ShowInitialRoom = configFile.ReadBoolean("MensagemDeJogadoresNaSala", false);
                BlackFriday = configFile.ReadBoolean("blackfridaysale", false);
                ChannelPass = configFile.ReadString("SenhaDoCanal", "");
                EnableTimeReal = configFile.ReadBoolean("MensagemTempoReal", false);
                minNickSize = configFile.ReadInt32("MinimoParaCriaNick", 0);
                maxNickSize = configFile.ReadInt32("MaximoParaCriarNick", 0);
                udpType = (SERVER_UDP_STATE)configFile.ReadByte("TipodaUDP", 3);
                minRankVote = configFile.ReadInt32("PossivelRankParaInicialVoteKick", 0);
                maxClanPoints = configFile.ReadFloat("ClanPontos", 0);
                minCreateRank = configFile.ReadInt32("PatenteParaCriarClan", 15);
                minCreateGold = configFile.ReadInt32("ComprarClan", 7500);
                maxActiveClans = configFile.ReadInt32("QtsDeClanPossiveis", 0);
                maxChannelPlayers = configFile.ReadInt32("QtsDePlayersNoCanal", 100);
                maxBattleXP = configFile.ReadInt32("BatalhaExp", 1000);
                maxBattleGP = configFile.ReadInt32("BatalhaGold", 1000);
                maxBattleMY = configFile.ReadInt32("BatalhaCash", 1000);
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("Settings: " + ex.ToString());
                return;
            }
        }
    }
}