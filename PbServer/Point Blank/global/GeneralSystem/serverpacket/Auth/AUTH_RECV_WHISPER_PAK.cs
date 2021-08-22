using Core.server;

namespace Game.global.serverpacket
{
    public class AUTH_RECV_WHISPER_PAK : SendPacket
    {
        private string _sender, _msg;
        private bool chatGM;
        public AUTH_RECV_WHISPER_PAK(string sender, string msg, bool chatGM)
        {
            _sender = sender;
            _msg = msg;
            this.chatGM = chatGM;
        }

        public override void Write()
        {
            WriteH(294);
            WriteS(_sender, 33);
            WriteC(chatGM);
            WriteH((ushort)(_msg.Length + 1));
            WriteS(_msg, _msg.Length + 1);
        }
    }
}