using Core.models.account.clan;
using Core.server;
using Game.data.managers;

namespace Game.data.sync.client_side
{
    public class Net_Clan_Servers_Sync
    {
        public static void Load(ReceiveGPacket p)
        {
            int type = p.ReadC();
            int clanId = p.ReadD();
            Clan clanCache = ClanManager.GetClan(clanId);
            if (type == 0)
            {
                if (clanCache != null)
                    return;
                long ownerId = p.ReadQ();
                int date = p.ReadD();
                string name = p.ReadS(p.ReadC());
                string info = p.ReadS(p.ReadC());
                ClanManager.AddClan(new Clan { _id = clanId, _name = name, owner_id = ownerId, _logo = 0, _info = info, creationDate = date });
            }
            else //DNS 181.213.132.2 / 3
            {
                if (clanCache != null)
                    ClanManager.RemoveClan(clanCache);
            }
        }
    }
}