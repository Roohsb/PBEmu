using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CREATE_REQUIREMENTS_REC : ReceiveGamePacket
    {
        public CLAN_CREATE_REQUIREMENTS_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            try
            {
                if (_client != null)
                    _client.SendPacket(new CLAN_CREATE_REQUIREMENTS_PAK());
            }
            catch
            {
            }
        }
    }
}