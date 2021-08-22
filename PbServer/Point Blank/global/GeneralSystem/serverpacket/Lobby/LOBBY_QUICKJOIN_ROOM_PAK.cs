using Core.server;

namespace Game.global.serverpacket
{
    public class LOBBY_QUICKJOIN_ROOM_PAK : SendPacket
    {
        public LOBBY_QUICKJOIN_ROOM_PAK()
        {
        }

        public override void Write()
        {
            WriteH(3078);
            WriteD(0x80000000);
        }
    }
}