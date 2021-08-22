using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_CHATTING_PAK : SendPacket
    {
        private string text;
        private Account p;
        private int type, bantime;
        public CLAN_CHATTING_PAK(string text, Account player)
        {
            this.text = text;
            p = player;
        }
        public CLAN_CHATTING_PAK(int type, int bantime)
        {
            this.type = type;
            this.bantime = bantime;
        }
        public override void Write()
        {
            WriteH(1359);
            WriteC((byte)type);
            if (type == 0)
            {
                WriteC((byte)(p.player_name.Length + 1));
                WriteS(p.player_name, p.player_name.Length + 1);
                WriteC(p.UseChatGM());
                WriteC((byte)(text.Length + 1));
                WriteS(text, text.Length + 1);
            }
            else
                WriteD(bantime);
            /*
             * 1=STR_MESSAGE_BLOCK_ING
             * 2=STR_MESSAGE_BLOCK_START
             */
        }
    }
}