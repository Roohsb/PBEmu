using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_CHANNEL_LIST_REC : ReceiveGamePacket
    {
        public BASE_CHANNEL_LIST_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            if (_client._player == null)
                return;
            _client.SendPacket(new BASE_CHANNEL_LIST_PAK());
        }
    }
}