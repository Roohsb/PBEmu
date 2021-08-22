using Core.server;

namespace Game.global.serverpacket
{
    public class AUTH_GOLD_REWARD_PAK : SendPacket
    {
        private int gp, _gpIncrease, type;
        public AUTH_GOLD_REWARD_PAK(int increase, int gold, int type)
        {
            _gpIncrease = increase;
            gp = gold;
            this.type = type;
        }

        public override void Write()
        {
            WriteH(561);
            WriteD(_gpIncrease);
            WriteD(gp);
            WriteD(type); //Faz aparecer STR_POPUP_GET_POINT
        }
    }
}