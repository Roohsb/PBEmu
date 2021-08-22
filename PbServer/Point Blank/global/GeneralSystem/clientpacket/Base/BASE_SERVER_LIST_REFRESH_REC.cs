using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_SERVER_LIST_REFRESH_REC : ReceiveGamePacket
    {
        public BASE_SERVER_LIST_REFRESH_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            if (_client != null)
                _client.SendPacket(new BASE_SERVER_LIST_REFRESH_PAK());
        }
    }
}