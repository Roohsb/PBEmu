using Core;
using Core.managers;
using Core.models.account.players;
using Core.models.account.title;
using Core.server;
using Core.xml;
using Game.data.model;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_TITLE_GET_REC : ReceiveGamePacket
    {
        private int titleIdx;
        private uint erro;
        public BASE_TITLE_GET_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            titleIdx = ReadC();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null || titleIdx >= 45)
                    return;
                if (p._titles.ownerId == 0)
                {
                    TitleManager.GetInstance().CreateTitleDB(p.player_id);
                    p._titles = new PlayerTitles { ownerId = p.player_id };
                }
                TitleQ t1 = TitlesXML.getTitle(titleIdx);
                if (t1 != null)
                {
                    TitlesXML.get2Titles(t1._req1, t1._req2, out TitleQ tr1, out TitleQ tr2, false);
                    if ((t1._req1 == 0 || tr1 != null) &&
                        (t1._req2 == 0 || tr2 != null) &&
                        p._rank >= t1._rank &&
                        p.brooch >= t1._brooch &&
                        p.medal >= t1._medals &&
                        p.blue_order >= t1._blueOrder &&
                        p.insignia >= t1._insignia &&
                        !p._titles.Contains(t1._flag) &&
                        p._titles.Contains(tr1._flag) &&
                        p._titles.Contains(tr2._flag))
                    {
                        p.brooch -= t1._brooch;
                        p.medal -= t1._medals;
                        p.blue_order -= t1._blueOrder;
                        p.insignia -= t1._insignia;
                        long flags = p._titles.Add(t1._flag);
                        TitleManager.GetInstance().UpdateTitlesFlags(p.player_id, flags);
                        List<ItemsModel> items = TitleAwardsXML.getAwards(titleIdx);
                        if (items.Count > 0)
                            _client.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, p, items));
                        _client.SendPacket(new BASE_QUEST_UPDATE_INFO_PAK(p));
                        TitleManager.GetInstance().UpdateRequi(p.player_id, p.medal, p.insignia, p.blue_order, p.brooch);
                        if (p._titles.Slots < t1._slot)
                        {
                            p._titles.Slots = t1._slot;
                            ComDiv.UpdateDB("player_titles", "titleslots", p._titles.Slots, "owner_id", p.player_id);
                        }
                    }
                    else erro = 0x80001083;
                }
                else erro = 0x80001083;
                _client.SendPacket(new BASE_TITLE_GET_PAK(erro, p._titles.Slots));
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[BASE_TITLE_GET_REC] " + ex.ToString());
            }
        }
    }
}