using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BASE_QUEST_BUY_CARD_SET_PAK : SendPacket
    {
        private Account player;
        private uint _erro;
        public BASE_QUEST_BUY_CARD_SET_PAK(uint erro, Account p)
        {
            _erro = erro;
            player = p;
        }

        public override void Write()
        {
            WriteH(2606);
            WriteD(_erro);
            if (_erro == 0)
            {
                WriteD(player._gp);
                WriteC((byte)player._mission.actualMission);
                WriteC((byte)player._mission.actualMission);
                WriteC((byte)player._mission.card1);
                WriteC((byte)player._mission.card2);
                WriteC((byte)player._mission.card3);
                WriteC((byte)player._mission.card4);
                WriteB(ComDiv.getCardFlags(player._mission.mission1, player._mission.list1));
                WriteB(ComDiv.getCardFlags(player._mission.mission2, player._mission.list2));
                WriteB(ComDiv.getCardFlags(player._mission.mission3, player._mission.list3));
                WriteB(ComDiv.getCardFlags(player._mission.mission4, player._mission.list4));
                WriteC((byte)player._mission.mission1);
                WriteC((byte)player._mission.mission2);
                WriteC((byte)player._mission.mission3);
                WriteC((byte)player._mission.mission4);
            }
        }
    }
}