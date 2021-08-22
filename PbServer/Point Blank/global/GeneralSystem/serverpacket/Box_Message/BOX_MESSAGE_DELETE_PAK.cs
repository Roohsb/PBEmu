using Core.server;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class BOX_MESSAGE_DELETE_PAK : SendPacket
    {
        private uint _erro;
        private List<object> _objs;
        public BOX_MESSAGE_DELETE_PAK(uint erro, List<object> objs)
        {
            _erro = erro;
            _objs = objs;
        }

        public override void Write()
        {
            WriteH(425);
            WriteD(_erro);
            WriteC((byte)_objs.Count);
            for (int i = 0; i < _objs.Count; i++)
                WriteD((int)_objs[i]);
        }
    }
}