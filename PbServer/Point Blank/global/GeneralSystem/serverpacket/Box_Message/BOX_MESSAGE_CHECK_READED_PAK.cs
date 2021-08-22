using Core.server;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class BOX_MESSAGE_CHECK_READED_PAK : SendPacket
    {
        private List<int> msgs;
        public BOX_MESSAGE_CHECK_READED_PAK(List<int> msgs)
        {
            this.msgs = msgs;
        }

        public override void Write()
        {
            WriteH(423);
            WriteC((byte)msgs.Count);
            for (int i = 0; i < msgs.Count; i++)
                WriteD(msgs[i]);
        }
    }
}