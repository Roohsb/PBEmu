using Core.server;
using Core.models.account.clan;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_REPLACE_NOTICE_REC : ReceiveGamePacket
    {
        private string clan_news;
        private uint erro;
        public CLAN_REPLACE_NOTICE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            clan_news = ReadS(ReadC());
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p != null)
                {
                    Clan c = ClanManager.GetClan(p.clanId);
                    if (c._id > 0 && c._news != clan_news && (c.owner_id == _client.player_id || p.clanAccess >= 1 && p.clanAccess <= 2))
                    {
                        if (ComDiv.UpdateDB("clan_data", "clan_news", clan_news, "clan_id", c._id))
                            c._news = clan_news;
                        else
                            erro = 2147487859;
                    }
                    else erro = 2147487835;
                }
                else erro = 2147487835;
            }
            catch
            {
                erro = 2147487859;
            }
            _client.SendPacket(new CLAN_REPLACE_NOTICE_PAK(erro));
        }
    }
}