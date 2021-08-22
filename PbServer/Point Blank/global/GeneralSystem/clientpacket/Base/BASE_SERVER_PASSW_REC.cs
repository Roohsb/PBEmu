using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_SERVER_PASSW_REC : ReceiveGamePacket
    {
        private string pass;
        public BASE_SERVER_PASSW_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            pass = ReadS(ReadC());
        }

        public override void Run()
        {
            if (_client != null)
                _client.SendPacket(new BASE_SERVER_PASSW_PAK(pass != Settings.ChannelPass ? 0x80000000 : 0));
        }
    }
}