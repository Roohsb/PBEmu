using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BASE_QUEST_GET_INFO_PAK : SendPacket
    {
        private Account player;
        public BASE_QUEST_GET_INFO_PAK(Account player)
        {
            this.player = player;
        }

        public override void Write()
        {
            WriteH(2596);
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