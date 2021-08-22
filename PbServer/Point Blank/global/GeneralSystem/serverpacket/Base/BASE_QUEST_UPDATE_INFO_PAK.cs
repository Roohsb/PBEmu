using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BASE_QUEST_UPDATE_INFO_PAK : SendPacket
    {
        private Account p;
        public BASE_QUEST_UPDATE_INFO_PAK(Account p)
        {
            this.p = p;
        }

        public override void Write()
        {
            WriteH(2604);
            if (p != null)
            {
                WriteQ(p.player_id);
                WriteD(p.brooch);
                WriteD(p.insignia);
                WriteD(p.medal);
                WriteD(p.blue_order);
            }
            else
                WriteB(new byte[24]);
        }
    }
}