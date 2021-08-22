using Core.server;

namespace Game.global.Authentication
{
    public class SERVER_MESSAGE_EVENT_QUEST_PAK : SendPacket
    {
        public override void Write()
        {
            WriteH(2061);
        }
    }
}
