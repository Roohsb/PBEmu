using Core.server;

namespace Game.global.serverpacket
{
    public class AUTH_WEB_CASH_PAK : SendPacket
    {
        private int erro, gold, cash;
        public AUTH_WEB_CASH_PAK(int erro, int gold = 0, int cash = 0)
        {
            this.erro = erro;
            this.gold = gold;
            this.cash = cash;
            if (erro > 0)
            {
                this.gold = 0;
                this.cash = 0;
            }
            else
            {
                if (gold > 9999999)
                    this.gold = 9999999;
                if (cash > 9999999)
                    this.cash = 9999999;
            }
        }

        public override void Write()
        {
            WriteH(545);
            WriteD(0);
            WriteD(gold);
            WriteD(cash);
        }
    }
}