using Core.models.servers;
using Core.server;
using Core.xml;
using Game.data.xml;
using System.Net;

namespace Game.global.serverpacket
{
    public class BASE_SERVER_LIST_PAK : SendPacket
    {
        private readonly IPAddress IP;
        private readonly uint SessaoID;
        private readonly ushort Semente;
        public int type;
        public BASE_SERVER_LIST_PAK(GameClient gc, int type)
        {
            SessaoID = gc.SessionId;
            IP = gc.GetAddress();
            Semente = gc.SessionSeed;
            this.type = type;
        }

        public override void Write()
        {
            WriteH(2049);
            WriteD(SessaoID);
            WriteIP(IP);
            WriteH(29890);
            WriteH(Semente);
            for (int i = 0; i < 10; i++)
                WriteC((byte)ChannelsXML._channels[i]._type);
            WriteC(1);
            WriteD(ServersXML._servers.Count);
            for (int i = 0; i < ServersXML._servers.Count; i++)
            {
                GameServerModel server = ServersXML._servers[i];
                WriteD(server._state);
                WriteIP(server.Connection.Address);
                WriteH(server._port);
                WriteC((byte)server._type);
                WriteH((ushort)server._maxPlayers);
                WriteD(server._LastCount);
            }
            WriteH(1); //Quantidade de algo
            WriteH(300);
            WriteD(200);
            WriteD(100);
            WriteC(1); //Events
            WriteD(type); //Type
            WriteD(100); //Exp
            WriteD(150); //Point
        }
    }
}