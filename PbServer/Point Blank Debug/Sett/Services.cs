using Core.server;
using Point_Blank_Debug.core;
using System;

namespace Point_Blank_Debug.Sett
{
    public class Services
    {
        public static readonly object sync = new object();
        public static void Contas(ReceiveGPacket G)
        {
            lock (sync)
            {
                long pID = G.ReadQ();
                uint SessionId = G.ReadUD();
                string name = G.ReadS(G.ReadC());
                string IP = G.ReadS(G.ReadC());
                string IsRealIP = G.ReadS(G.ReadC());
                string AcessLevel = G.ReadS(G.ReadC());
                string strCountry = G.ReadS(G.ReadC());
                Loggers.Green("-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-");
                Loggers.Yellow("O jogador criou uma conta nova: \n");
                Loggers.Yellow("Identidade: " + pID);
                Loggers.Yellow("Sessão: " + SessionId);
                Loggers.Yellow("Login: " + name);
                Loggers.Yellow("IP: " + IP);
                Loggers.Yellow("Hosts is: " + IsRealIP);
                Loggers.Yellow("Acesso: " + AcessLevel);
                Loggers.Yellow("Data de Criação: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
                Loggers.Yellow("localizacao: " + strCountry);
            }
        }
        public static void ErrosContas(ReceiveGPacket G)
        {
            lock (sync)
            {
                Loggers.Green("-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-");
                Loggers.Yellow(G.ReadS(G.ReadD()));
            }
        }
        public static void FeedBacks(ReceiveGPacket G)
        {
            lock (sync)
                Loggers.FeedBack(G.ReadS(G.ReadD()));
        }
        public static void Hacks(ReceiveGPacket G)
        {
            lock (sync)
                Loggers.Hack(G.ReadS(G.ReadD()));
        }
        public static void Comandos(ReceiveGPacket G)
        {
            lock (sync)
                Loggers.Comandos(G.ReadS(G.ReadD()));
        }

        public static void ChatLog(ReceiveGPacket G)
        {
            lock (sync)
                Loggers.ChatLog(G.ReadS(G.ReadD()));
        }

        internal static void MegaFone(ReceiveGPacket G)
        {
            lock (sync)
                Loggers.MegaFone(G.ReadS(G.ReadD()));
        }

        public static void Clear(ReceiveGPacket p)
        {
            lock (sync)
                Console.Clear();
        }
    }
}
