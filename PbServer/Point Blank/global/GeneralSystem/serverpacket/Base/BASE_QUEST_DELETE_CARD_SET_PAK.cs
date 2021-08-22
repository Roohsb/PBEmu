using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BASE_QUEST_DELETE_CARD_SET_PAK : SendPacket
    {
        private uint erro;
        private Account p;
        public BASE_QUEST_DELETE_CARD_SET_PAK(uint erro, Account player)
        {
            this.erro = erro;
            this.p = player;
        }

        public override void Write()
        {
            WriteH(2608);
            WriteD(erro);
            if (erro == 0)
            {
                WriteC((byte)p._mission.actualMission);
                WriteC((byte)p._mission.card1);
                WriteC((byte)p._mission.card2);
                WriteC((byte)p._mission.card3);
                WriteC((byte)p._mission.card4);
                WriteB(ComDiv.getCardFlags(p._mission.mission1, p._mission.list1));
                WriteB(ComDiv.getCardFlags(p._mission.mission2, p._mission.list2));
                WriteB(ComDiv.getCardFlags(p._mission.mission3, p._mission.list3));
                WriteB(ComDiv.getCardFlags(p._mission.mission4, p._mission.list4));
                WriteC((byte)p._mission.mission1);
                WriteC((byte)p._mission.mission2);
                WriteC((byte)p._mission.mission3);
                WriteC((byte)p._mission.mission4);
            }
        }
    }
}