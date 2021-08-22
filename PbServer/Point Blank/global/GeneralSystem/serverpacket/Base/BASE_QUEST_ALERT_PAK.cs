using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BASE_QUEST_ALERT_PAK : SendPacket
    {
        private Account p;
        private uint erro;
        private int type;
        public BASE_QUEST_ALERT_PAK(uint erro, int type, Account p)
        {
            this.erro = erro;
            this.type = type;
            this.p = p;
        }

        public override void Write()
        {
            WriteH(2602);
            WriteD(erro); //Sem efeito - 0 || 1 - Efeito || 273 - finish?
            WriteC((byte)type); //1 CardSetIdx?
            if ((erro & 1) == 1)
            {
                WriteD(p._exp);
                WriteD(p._gp);
                WriteD(p.brooch);
                WriteD(p.insignia);
                WriteD(p.medal);
                WriteD(p.blue_order);
                WriteD(p._rank);
            }
        }
    }
}