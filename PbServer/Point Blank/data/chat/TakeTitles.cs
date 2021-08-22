using Core;
using Core.managers;
using Core.models.account.players;
using Core.models.account.title;
using Core.server;
using Core.xml;
using Game.data.model;
using Game.global.serverpacket;
using System.Collections.Generic;

namespace Game.data.chat
{
    public static class TakeTitles
    {
        public static string GetAllTitles(Account p)
        {
            if (p._titles.ownerId == 0)
            {
                TitleManager.GetInstance().CreateTitleDB(p.player_id);
                p._titles = new PlayerTitles { ownerId = p.player_id };
            }
            PlayerTitles titles = p._titles;
            int count = 0;
            for (int i = 1; i <= 44; i++)
            {
                TitleQ title = TitlesXML.getTitle(i, true);
                if (title != null && !titles.Contains(title._flag))
                {
                    count++;
                    titles.Add(title._flag);
                    if (titles.Slots < title._slot)
                        titles.Slots = title._slot;
                    List<ItemsModel> items = TitleAwardsXML.getAwards(i);
                    if (items.Count > 0)
                        p.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, p, items));
                }
            }
            if (count > 0)
            {
                ComDiv.UpdateDB("player_titles", "titleslots", titles.Slots, "owner_id", p.player_id);
                TitleManager.GetInstance().UpdateTitlesFlags(p.player_id, titles.Flags);
                p.SendPacket(new BASE_2626_PAK(p));
            }
            return Translation.GetLabel("TitleAcquisiton", count);
        }
    }
}