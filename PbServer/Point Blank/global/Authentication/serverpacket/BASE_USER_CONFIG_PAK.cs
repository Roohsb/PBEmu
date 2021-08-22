using Core.models.account.players;
using Core.server;

namespace Game.global.Authentication
{
    public class BASE_USER_CONFIG_PAK : SendPacket
    {
        private int error;
        private PlayerConfig c;
        private bool isValid;
        public BASE_USER_CONFIG_PAK(int error, PlayerConfig config)
        {
            this.error = error;
            c = config;
            isValid = (c != null);
        }
        public BASE_USER_CONFIG_PAK(int error)
        {
            this.error = error;
        }
        public override void Write()
        {
            WriteH(2568);
            WriteD(error);
            if (error < 0)
                return;
            WriteC(!isValid); //1= Default | 0 = Customizable
            if (isValid)
            {
                WriteH((short)c.blood);
                WriteC((byte)c.sight);
                WriteC((byte)c.hand);
                WriteD(c.config);
                WriteD(c.audio_enable);
                WriteH(0);
                WriteC((byte)c.audio1);
                WriteC((byte)c.audio2);
                WriteC((byte)c.fov);
                WriteC(0);
                WriteC((byte)c.sensibilidade);
                WriteC((byte)c.mouse_invertido);
                WriteH(0);
                WriteC((byte)c.msgConvite);
                WriteC((byte)c.chatSussurro);
                WriteD(c.macro);
                WriteB(new byte[] { 0, 57, 248, 16, 0 });
                WriteB(c.keys);
                WriteC((byte)(c.macro_1.Length + 1));
                WriteS(c.macro_1, c.macro_1.Length + 1);
                WriteC((byte)(c.macro_2.Length + 1));
                WriteS(c.macro_2, c.macro_2.Length + 1);
                WriteC((byte)(c.macro_3.Length + 1));
                WriteS(c.macro_3, c.macro_3.Length + 1);
                WriteC((byte)(c.macro_4.Length + 1));
                WriteS(c.macro_4, c.macro_4.Length + 1);
                WriteC((byte)(c.macro_5.Length + 1));
                WriteS(c.macro_5, c.macro_5.Length + 1);
            }
        }
    }
}