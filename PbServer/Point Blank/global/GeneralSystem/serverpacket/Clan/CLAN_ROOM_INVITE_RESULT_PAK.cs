using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_ROOM_INVITE_RESULT_PAK : SendPacket
    {
        private long _pId;
        public CLAN_ROOM_INVITE_RESULT_PAK(long pId)
        {
            _pId = pId;
        }

        public override void Write()
        {
            WriteH(1383);
            WriteQ(_pId);
        }
    }
}