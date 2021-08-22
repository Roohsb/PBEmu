using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class LOBBY_CHATTING_PAK : SendPacket
    {
        private string sender, msg;
        private uint sessionId;
        private int nameColor;
        private bool GMColor;
        public LOBBY_CHATTING_PAK(Account player, string message, bool GMCmd = false)
        {
            if (!GMCmd)
            {
                nameColor = player.name_color;
                GMColor = player.UseChatGM();
            }
            else
                GMColor = true;
            sender = player.player_name;
            sessionId = player.GetSessionId();
            msg = message;
        }
        public LOBBY_CHATTING_PAK(string snd, uint session, int name_color, bool chatGm, string message)
        {
            sender = snd;
            sessionId = session;
            nameColor = name_color;
            GMColor = chatGm;
            msg = message;
        }
        public override void Write()
        {
            WriteH(3093);
            WriteD(sessionId);
            WriteC((byte)(sender.Length + 1));
            WriteS(sender, sender.Length + 1);
            WriteC((byte)nameColor);
            WriteC(GMColor);
            WriteH((ushort)(msg.Length + 1));
            WriteS(msg, msg.Length + 1);
        }
    }
}