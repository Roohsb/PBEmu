using Core;
using Core.server;
using System;
using System.Net;
using System.Net.Sockets;

namespace Game
{
    public class SendDebug
    {
        public static void SendConta(string name, long id, uint SessionId,string IP, string AcessLevel, string IsRealIP, string Pais)
        {
            try
            {
                using SendGPacket pk = new SendGPacket();
                pk.writeH(01);
                pk.writeQ(id);
                pk.writeD(SessionId);
                pk.writeC((byte)name.Length);
                pk.writeS(name, name.Length);
                pk.writeC((byte)IP.Length);
                pk.writeS(IP, IP.Length);
                pk.writeC((byte)IsRealIP.Length);
                pk.writeS(IsRealIP, IsRealIP.Length);
                pk.writeC((byte)AcessLevel.Length);
                pk.writeS(AcessLevel, AcessLevel.Length);
                pk.writeC((byte)Pais.Length);
                pk.writeS(Pais, Pais.Length);
                SendPacket(pk.mstream.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public static void SendInfo(string txt)
        {
            try
            {
                using SendGPacket pk = new SendGPacket();
                pk.writeH(02);
                pk.writeD((byte)txt.Length);
                pk.writeS(txt, txt.Length);
                SendPacket(pk.mstream.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public static void SendComandos(string txt)
        {
            try
            {
                using SendGPacket pk = new SendGPacket();
                pk.writeH(06);
                pk.writeD((byte)txt.Length);
                pk.writeS(txt, txt.Length);
                SendPacket(pk.mstream.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public static void SendChatLog(string txt)
        {
            try
            {
                using SendGPacket pk = new SendGPacket();
                pk.writeH(08);
                pk.writeD((byte)txt.Length);
                pk.writeS(txt, txt.Length);
                SendPacket(pk.mstream.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
        public static void SendFeed(string txt)
        {
            try
            {
                using SendGPacket pk = new SendGPacket();
                pk.writeH(04);
                pk.writeD((byte)txt.Length);
                pk.writeS(txt, txt.Length);
                SendPacket(pk.mstream.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public static void SendHack(string txt)
        {
            try
            {
                using SendGPacket pk = new SendGPacket();
                pk.writeH(05);
                pk.writeD((byte)txt.Length);
                pk.writeS(txt, txt.Length);
                SendPacket(pk.mstream.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public static void Clear()
        {
            using SendGPacket pk = new SendGPacket();
            pk.writeH(03);
            SendPacket(pk.mstream.ToArray());
        }
        public static void SendPacket(byte[] packet)
        {
            using UdpClient udp = new UdpClient();
            udp.Send(packet, packet.Length, new IPEndPoint(IPAddress.Parse(Settings.IP_Jogo), 250));
        }

        public static void SendFone(string fone)
        {
            try
            {
                using SendGPacket pk = new SendGPacket();
                pk.writeH(07);
                pk.writeD((byte)fone.Length);
                pk.writeS(fone, fone.Length);
                SendPacket(pk.mstream.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
    }
}

