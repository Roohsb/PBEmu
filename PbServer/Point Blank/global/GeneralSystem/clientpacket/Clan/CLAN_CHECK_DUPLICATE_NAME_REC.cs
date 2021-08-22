using Game.data.managers;
using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CHECK_DUPLICATE_NAME_REC : ReceiveGamePacket
    {
        private string clanName;
        public CLAN_CHECK_DUPLICATE_NAME_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            clanName = ReadS(ReadC());
        }

        public override void Run()
        {
            if (_client == null || _client._player == null)
                return;
            try
            {
                _client.SendPacket(new CLAN_CHECK_DUPLICATE_NAME_PAK(!ClanManager.IsClanNameExist(clanName) ? 0 : 0x80000000));
            }
            catch
            {
            }
        }
    }
}