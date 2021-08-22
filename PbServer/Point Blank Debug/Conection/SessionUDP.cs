using Core.server;
using Game;
using Point_Blank_Debug.core;
using Point_Blank_Debug.Sett;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Point_Blank_Debug.Conection
{
    public class SessionUDP
    {
        public static UdpClient udp;
        public static uint SIO_UDP_CONNRESET = 0x80000000 | 0x18000000 | 12;
        public static byte[] received = new byte[0];
        public static bool Start()
        {
            try
            {
                udp = new UdpClient(250);
                udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                new Thread(Read).Start();
                return true;
            }
            catch(Exception ex)
            {
                Loggers.Red(ex.ToString());
                return false;
            }
        }

        private static void Read()
        {
            udp.BeginReceive(new AsyncCallback(Recv), udp);
        }

        private static void Recv(IAsyncResult ar)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            if (ar.IsCompleted)
                received = udp.EndReceive(ar, ref RemoteIpEndPoint);
            Thread.Sleep(5);
            new Thread(Read).Start();
            if (received.Length >= 2)
                LoadPacket(received);
        }

        private static void LoadPacket(byte[] received)
        {
            try
            {
                ReceiveGPacket p = new ReceiveGPacket(received);
                short opcode = p.ReadH();
                switch (opcode)
                {
                    case 01: Services.Contas(p); break;
                    case 02: Services.ErrosContas(p); break;
                    case 03: Services.Clear(p); break;
                    case 04: Services.FeedBacks(p); break;
                    case 05: Services.Hacks(p); break;
                    case 06: Services.Comandos(p); break;
                    case 07: Services.MegaFone(p); break;
                    case 08: Services.ChatLog(p); break;
                }
            }
            catch(Exception ex)
            {
                Loggers.Red(ex.ToString());
            }

        }
    }
}
