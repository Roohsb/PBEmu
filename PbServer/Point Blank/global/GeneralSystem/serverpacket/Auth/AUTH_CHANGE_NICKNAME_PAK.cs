using Core.server;

namespace Game.global.serverpacket
{
    public class AUTH_CHANGE_NICKNAME_PAK : SendPacket
    {
        private string name;
        public AUTH_CHANGE_NICKNAME_PAK(string name)
        {
            this.name = name;
        }

        public override void Write()
        {
            WriteH(300);
            WriteC((byte)name.Length);
            WriteS(name, name.Length);
        }
    }
}