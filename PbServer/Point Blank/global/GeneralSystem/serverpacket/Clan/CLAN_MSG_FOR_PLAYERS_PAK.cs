using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_MSG_FOR_PLAYERS_PAK : SendPacket
    {
        private int playersCount;
        public CLAN_MSG_FOR_PLAYERS_PAK(int count)
        {
            playersCount = count;
        }

        public override void Write()
        {
            WriteH(1397);
            WriteD(playersCount);
        }
    }
}