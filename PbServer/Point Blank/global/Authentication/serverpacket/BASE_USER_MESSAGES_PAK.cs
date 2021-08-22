using Core.models.account;
using Core.server;
using System.Collections.Generic;

namespace Game.global.Authentication
{
    public class BASE_USER_MESSAGES_PAK : SendPacket
    {
        private int pageIdx;
        private List<Message> msgs;
        public BASE_USER_MESSAGES_PAK(int pageIdx, List<Message> msgs)
        {
            this.pageIdx = pageIdx;
            this.msgs = new List<Message>();
            int count = 0;
            for (int i = pageIdx * 25; i < msgs.Count; i++)
            {
                this.msgs.Add(msgs[i]);
                if (++count == 25)
                    break;
            }
        }

        public override void Write()
        {
            WriteH(421);
            WriteC((byte)pageIdx); //PageIdx
            WriteC((byte)msgs.Count); //Max=25
            for (int i = 0; i < msgs.Count; i++)
            {
                Message msg = msgs[i];
                WriteD(msg.object_id);
                WriteQ(msg.sender_id);
                WriteC((byte)msg.type);
                WriteC((byte)msg.state);
                WriteC((byte)msg.DaysRemaining);
                WriteD(msg.clanId);
            }
            for (int i = 0; i < msgs.Count; i++)
            {
                Message msg = msgs[i];
                WriteC((byte)(msg.sender_name.Length + 1));
                WriteC((byte)(msg.type == 5 || msg.type == 4 && (int)msg.cB != 0 ? 0 : (msg.text.Length + 1)));
                WriteS(msg.sender_name, msg.sender_name.Length + 1);
                switch(msg.type)
                {
                    case int Tipos when (Tipos == 4 || Tipos == 5):
                        {
                            if ((int)msg.cB >= 4 && (int)msg.cB <= 6)
                            {
                                WriteC((byte)(msg.text.Length + 1));
                                WriteC((byte)msg.cB);
                                WriteS(msg.text, msg.text.Length);
                            }
                            else if (msg.cB == 0)
                                WriteS(msg.text, msg.text.Length + 1);
                            else
                            {
                                WriteC(2);
                                WriteH((short)msg.cB);
                            }
                            break;
                        }
                    default:
                        {
                            WriteS(msg.text, msg.text.Length + 1);
                            break;
                        }
                }
            }
        }
    }
}