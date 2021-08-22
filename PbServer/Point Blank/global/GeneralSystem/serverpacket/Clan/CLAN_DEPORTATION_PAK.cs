using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_DEPORTATION_PAK : SendPacket
    {
        private uint result;
        public CLAN_DEPORTATION_PAK(uint result)
        {
            this.result = result;
        }

        public override void Write()
        {
            WriteH(1335);
            WriteD(result);
        }
    }
}