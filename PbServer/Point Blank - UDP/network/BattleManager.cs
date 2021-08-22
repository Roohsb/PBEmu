using Battle.config;
using Battle.data.sync;
using Battle.data.sync.client_side;
using System;
using System.Net;
using System.Net.Sockets;

namespace Battle.network
{
    public class BattleManager
    {
        public static uint SIO_UDP_CONNRESET = 0x80000000 | 0x18000000 | 12;
        private static UdpClient udpClient;
        private static IPEndPoint localEP;
        public static void Init()
        {
            try
            {
                udpClient = new UdpClient();
                udpClient.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                localEP = new IPEndPoint(IPAddress.Parse(Config.hosIp), Config.hosPort);
                UdpState s = new UdpState(localEP, udpClient);

                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Bind(localEP);
                udpClient.BeginReceive(GerenciaRetorno, s);
                Logger.Carregar(" [System] Sistema de tráfego de pacotes lançado.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString() + "\r\nOcorreu um erro ao listar as conexões UDP!!");
            }
        }
        private static void Read(UdpState state)
        {
            udpClient.BeginReceive(new AsyncCallback(GerenciaRetorno), state);
        }
        private static void GerenciaRetorno(IAsyncResult ar)
        {
            ar.AsyncWaitHandle.WaitOne(7000);
            IPEndPoint recEP = new IPEndPoint(IPAddress.Any, Config.hosPort);

            UdpClient c = ((UdpState)ar.AsyncState).c;
            IPEndPoint e = ((UdpState)ar.AsyncState).e;
            byte[] buffer = c.EndReceive(ar, ref recEP);
            string IpV4 = recEP.Address.ToString();
            bool error = false;
            if (NextModel.ContemOffset(IpV4))
            {
                Read(new UdpState(e, c));
                return;
            }
            try
            {
                if (buffer.Length >= 22)
                    new BattleHandler(udpClient, IpV4, buffer, recEP, DateTime.Now);
                else
                {
                    Logger.Warning("[System] O tamanho do pacote é inferior a 22 > " + buffer.Length + " e o Endereço de Protocolo é: " + recEP.ToString() + "");
                    NextModel.AddOffet(IpV4);
                    error = true;
                }
            }
            catch
            {
                Logger.Warning("[System] Há um problema com este IP, ele foi bloqueado. " + IpV4);
                NextModel.AddOffet(IpV4);
                error = true;
            }
            if (error)
            {
                Battle_SyncNet.ExcptionPlayer(IpV4);
                GetPrestart.Bloqueando(IpV4);
            }
            Read(new UdpState(e, c));
        }
        public static void Send(byte[] data, IPEndPoint ip)
        {
            udpClient.Send(data, data.Length, ip);
        }
        private class UdpState : object
        {
            public UdpState(IPEndPoint e, UdpClient c) { this.e = e; this.c = c; }
            public IPEndPoint e;
            public UdpClient c;
        }
    }
}