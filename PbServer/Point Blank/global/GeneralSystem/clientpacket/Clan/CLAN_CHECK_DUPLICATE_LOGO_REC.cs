using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CHECK_DUPLICATE_LOGO_REC : ReceiveGamePacket
    {
        private uint logo, erro;
        public CLAN_CHECK_DUPLICATE_LOGO_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            logo = ReadUD();
        }

        public override void Run()
        {
            Account p = _client._player;
            if (p == null || ClanManager.GetClan(p.clanId)._logo == logo ||
                ClanManager.IsClanLogoExist(logo))
                erro = 0x80000000;
            _client.SendPacket(new CLAN_CHECK_DUPLICATE_MARK_PAK(erro));
        }
    }
}