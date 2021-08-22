using Battle.config;
using Battle.data.models;
using Battle.data.sync.client_side;
using Battle.network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Battle.data.sync
{
    public class Battle_SyncNet
    {
        public static uint SIO_UDP_CONNRESET = 0x80000000 | 0x18000000 | 12;
        private static UdpClient udp;
        public static void Start()
        {
            try
            {
                udp = new UdpClient(Config.syncPort);
                udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                new Thread(Read).Start();
            }
            catch (Exception e)
            {
                Logger.Warning(e.ToString());
            }
        }
        public static void Read()
        {
            udp.BeginReceive(new AsyncCallback(Recv), null);
        }
        private static void Recv(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            byte[] received = udp.EndReceive(res, ref RemoteIpEndPoint);
            new Thread(Read).Start();
            if (received.Length >= 6)
                LoadPacket(received);
        }
        private static void LoadPacket(byte[] buffer)
        {
            ReceivePacket p = new ReceivePacket(buffer);
            short opcode = p.readH();
            int pass = p.readD();
            if (pass != 02031999)
                return;
            switch (opcode)
            {
                case 10: 
                    RespawnSync.Load(p); 
                    break;
                case 20: 
                    RemovePlayerSync.Load(p);
                    break;
                case 30:
                    uint UniqueRoomId = p.readUD();
                    int gen2 = p.readD();
                    int round = p.readC();
                    Room room = RoomsManager.GetRoom(UniqueRoomId, gen2);
                    if (room != null)
                        room._serverRound = round;
                    break;
                case 40: 
                    PlayerIsHPOriMortal.LoadingRespawn(p);
                    break;
                case 50:
                    PlayerIsHPOriMortal.LoadingHPRespwan(p);
                    break;
                case 60: 
                    GetPrestart.Conection(p);
                    break;
                case 70:
                    GetPrestart.Remove();
                    break;
                default:
                    {
                        Logger.Warning("Valor incorreto de pacote " + opcode); break;
                    }
            }

        }
        public static void SendPortalPass(Room room, Player pl, int portalIdx)
        {
            if (room.stageType != 7)
                return;
            pl._life = pl._maxLife;
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (SendPacket p = new SendPacket())
            {
                p.WriteH(10);
                p.WriteD(02031999);
                p.WriteH((short)room._roomId);
                p.WriteH((short)room._channelId);
                p.WriteC((byte)pl._slot);
                p.WriteC((byte)portalIdx);
                SendData(room, s, p.mstream.ToArray());
            }
        }
        public static void SendDeathSync(Room room, Player killer, int objId, int weaponId, List<DeathServerData> deaths)
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (SendPacket p = new SendPacket())
            {
                p.WriteH(30);
                p.WriteD(02031999);
                p.WriteH((short)room._roomId);
                p.WriteH((short)room._channelId);
                p.WriteC((byte)killer._slot);
                p.WriteC((byte)objId);
                p.WriteD(weaponId);
                p.WriteTVector(killer.Position);
                p.WriteC((byte)deaths.Count);
                for (int i = 0; i < deaths.Count; i++)
                {
                    DeathServerData ob = deaths[i];
                    p.WriteC((byte)ob._player.WeaponClass);
                    p.WriteC((byte)(((int)ob._deathType * 16) + ob._player._slot));
                    p.WriteTVector(ob._player.Position);
                    p.WriteC(0);
                }
                SendData(room, s, p.mstream.ToArray());
            }
        }
        public static void SendBombSync(Room room, Player pl, int type, int bombArea)
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (SendPacket p = new SendPacket())
            {
                p.WriteH(20);
                p.WriteD(02031999);
                p.WriteH((short)room._roomId);
                p.WriteH((short)room._channelId);
                p.WriteC((byte)type);
                p.WriteC((byte)pl._slot);
                if (type == 0)
                {
                    p.WriteC((byte)bombArea);
                    p.WriteTVector(pl.Position);
                    room.BombPosition = pl.Position;
                }
                SendData(room, s, p.mstream.ToArray());
            }
        }

        public static void SendHitMarkerSync(Room room, Player pl, int deathType, int hitEnum, int damage, int life, int WeaponID)
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (SendPacket p = new SendPacket())
            {
                p.WriteH(40);
                p.WriteD(02031999);
                p.WriteH((short)room._roomId);
                p.WriteH((short)room._channelId);
                p.WriteC((byte)pl._slot);
                p.WriteC((byte)deathType);
                p.WriteC((byte)hitEnum);
                p.WriteH((short)damage);
                p.WriteD(life);
                p.WriteD(WeaponID);
                SendData(room, s, p.mstream.ToArray());
            }
        }
        public static void ExcptionPlayer(string IP)
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (SendPacket p = new SendPacket())
            {
                p.WriteH(220);
                p.WriteD(02031999);
                p.WriteC((byte)IP.Length);
                p.WriteS(IP, IP.Length);
                s.SendTo(p.mstream.ToArray(), new IPEndPoint(IPAddress.Parse(Config.serverIp), 1909));
            }
        }
        public static void SendSabotageSync(Room room, Player pl, int damage, int ultraSYNC)
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (SendPacket p = new SendPacket())
            {
                p.WriteH(50);
                p.WriteD(02031999);
                p.WriteH((short)room._roomId);
                p.WriteH((short)room._channelId);
                p.WriteC((byte)pl._slot);
                p.WriteH((ushort)room._bar1);
                p.WriteH((ushort)room._bar2);
                p.WriteC((byte)ultraSYNC); //barnumber (1 = primeiro/2 = segundo)
                p.WriteH((ushort)damage);
                SendData(room, s, p.mstream.ToArray());
            }
        }
        public static void SendLocalSync(Room room, Player pl, float x, float y)
        {
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (SendPacket p = new SendPacket())
            {
                p.WriteH(240);
                p.WriteD(02031999);
                p.WriteC((byte)pl._slot);
                p.WriteH((short)room._channelId);
                p.WriteH((short)room._roomId);

                p.WriteT(x);
                p.WriteT(y);
                SendData(room, s, p.mstream.ToArray());
            }
        }
        private static void SendData(Room room, Socket socket, byte[] data)
        {
            socket.SendTo(data, room.gs.Connection);
        }
    }
}