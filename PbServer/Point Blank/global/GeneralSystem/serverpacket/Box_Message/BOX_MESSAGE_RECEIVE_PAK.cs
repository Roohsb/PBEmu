using Core.models.account;
using Core.server;

namespace Game.global.serverpacket
{
    public class BOX_MESSAGE_RECEIVE_PAK : SendPacket
    {
        private Message msg;
        public BOX_MESSAGE_RECEIVE_PAK(Message msg)
        {
            this.msg = msg;
        }

        public override void Write()
        {
            WriteH(427);
            WriteD(msg.object_id);
            WriteQ(msg.sender_id);
            WriteC((byte)msg.type);
            WriteC((byte)msg.state);
            WriteC((byte)msg.DaysRemaining);
            WriteD(msg.clanId);
            WriteC((byte)(msg.sender_name.Length + 1));
            WriteC((byte)(msg.type == 5 || msg.type == 4 && (int)msg.cB != 0 ? 0 : (msg.text.Length + 1)));
            WriteS(msg.sender_name, msg.sender_name.Length + 1);
            if (msg.type == 5 || msg.type == 4)
            {
                if ((int)msg.cB >= 4 && (int)msg.cB <= 6)
                {
                    WriteC((byte)(msg.text.Length + 1));
                    WriteC((byte)msg.cB);
                    WriteS(msg.text, msg.text.Length + 1);
                }
                else if ((int)msg.cB == 0)
                    WriteS(msg.text, msg.text.Length + 1);
                else
                {
                    WriteC(2);
                    WriteH((short)msg.cB);
                }
            }
            else
                WriteS(msg.text, msg.text.Length + 1);
        }
    }
}