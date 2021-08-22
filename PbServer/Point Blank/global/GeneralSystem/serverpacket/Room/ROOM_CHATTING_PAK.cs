using Core.server;

namespace Game.global.serverpacket
{
    public class ROOM_CHATTING_PAK : SendPacket
    {
        private string msg;
        private int type, slotId;
        private bool GMColor;
        public ROOM_CHATTING_PAK(int chat_type, int slotId, bool GM, string message)
        {
            type = chat_type;
            this.slotId = slotId;
            GMColor = GM;
            msg = message;
        }
        public override void Write()
        {
            WriteH(3851);
            WriteH((short)type);
            WriteD(slotId);
            WriteC(GMColor);
            WriteD(msg.Length + 1);
            WriteS(msg, msg.Length + 1);
        }
    }
}