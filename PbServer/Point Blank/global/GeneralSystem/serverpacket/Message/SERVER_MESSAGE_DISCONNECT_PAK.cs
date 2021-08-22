using Core.server;
using System;

namespace Game.global.serverpacket
{
    public class SERVER_MESSAGE_DISCONNECT_PAK : SendPacket
    {
        private uint _erro;
        private bool type;
        public SERVER_MESSAGE_DISCONNECT_PAK(uint erro, bool HackUse)
        {
            _erro = erro;
            type = HackUse;
        }

        public override void Write()
        {
            WriteH(2062);
            WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
            WriteD(_erro);
            WriteD(type); //Se for igual a 1, novo writeD (Da DC no cliente, Programa ilegal)
            if (type)
                WriteD(0);
        }
    }
}