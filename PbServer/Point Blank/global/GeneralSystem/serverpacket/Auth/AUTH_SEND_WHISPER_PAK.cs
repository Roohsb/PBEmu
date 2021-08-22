using Core.server;

namespace Game.global.serverpacket
{
    public class AUTH_SEND_WHISPER_PAK : SendPacket
    {
        private string name, msg;
        private uint erro;
        private int type, bantime;
        public AUTH_SEND_WHISPER_PAK(string name, string msg, uint erro)
        {
            this.name = name;
            this.msg = msg;
            this.erro = erro;
        }
        public AUTH_SEND_WHISPER_PAK(int type, int bantime)
        {
            this.type = type;
            this.bantime = bantime;
        }
        public override void Write()
        {
            WriteH(291);
            WriteC((byte)type);
            if (type == 0)
            {
                WriteD(erro);
                WriteS(name, 33);
                if (erro == 0)
                {
                    WriteH((ushort)(msg.Length + 1));
                    WriteS(msg, msg.Length + 1);
                }
            }
            else
                WriteD(bantime);
        }
    }
}