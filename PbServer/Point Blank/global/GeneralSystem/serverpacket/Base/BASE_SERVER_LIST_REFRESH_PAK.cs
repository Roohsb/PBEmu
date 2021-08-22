using Core.models.servers;
using Core.server;
using Core.xml;

namespace Game.global.serverpacket
{
    public class BASE_SERVER_LIST_REFRESH_PAK : SendPacket
    {
        public BASE_SERVER_LIST_REFRESH_PAK()
        {
        }

        public override void Write()
        {
            WriteH(2643);
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
        }
    }
}