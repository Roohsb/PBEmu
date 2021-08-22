using Core;
using Core.managers;
using Core.managers.events;
using Core.managers.server;
using Core.models.account;
using Core.models.account.clan;
using Core.models.account.players;
using Core.models.enums;
using Core.models.enums.room;
using Core.server;
using Core.xml;
using Game.data.chat;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;

namespace Game.global.Authentication
{
    public class BASE_USER_INFO_PAK : SendPacket
    {
        private Account c;
        private Clan clan;
        private uint _erro;
        private bool _xmas;
        private List<ItemsModel> charas = new List<ItemsModel>(), weapons = new List<ItemsModel>(), cupons = new List<ItemsModel>();
        public BASE_USER_INFO_PAK(Account p)
        {
            c = p;
            if (c != null && c._inventory._items.Count == 0)
            {
                clan = ClanManager.GetClanDB(c.clanId, 1);
                c.LoadInventory();
                c.LoadMissionList();
                c.DiscountPlayerItems();
                GetInventoryInfo();
                c.SetHideColorVip();
                c.SetHideGmColor();
            }
            else _erro = 0x80000000;
        }
        private void GetInventoryInfo()
        {
            lock (c._inventory._items)
            {
                for (int i = 0; i < c._inventory._items.Count; i++)
                {
                    ItemsModel item = c._inventory._items[i];
                    switch (item._category)
                    {
                        case 1: weapons.Add(item); break;
                        case 2: charas.Add(item); break;
                        case 3: cupons.Add(item); break;
                    }
                }
            }
        }
        public void AcessVipMASTER()
        {
            if (c.pc_cafe == PcCafe.VIP_MASTER)
            {
                if (c._inventory.getItem(LorenstudioSettings.WeaponIDVip6) == null)
                {
                    ItemsModel item = new ItemsModel(LorenstudioSettings.WeaponIDVip6, ComDiv.GetItemCategory(LorenstudioSettings.WeaponIDVip6), LorenstudioSettings.NameWeaponVIP6, 1, 86400);
                    PlayerManager.TryCreateItem(item, c._inventory, c.player_id);
                }
            }
        }
        public void AcessVipPREMIUM()
        {
            if(c.pc_cafe == PcCafe.VIP_PREMIUM)
            {
                if (c._inventory.getItem(LorenstudioSettings.WeaponIDVip5) == null)
                {
                    ItemsModel item = new ItemsModel(LorenstudioSettings.WeaponIDVip5, ComDiv.GetItemCategory(LorenstudioSettings.WeaponIDVip5), LorenstudioSettings.NameWeaponVIP5, 1, 86400);
                    PlayerManager.TryCreateItem(item, c._inventory, c.player_id);
                }
            }
        }

        public void AcessVipPLUS()
        {
            if (c.pc_cafe == PcCafe.VIP_PLUS)
            {
                if (c._inventory.getItem(LorenstudioSettings.WeaponIDVip2) == null)
                {
                    ItemsModel item = new ItemsModel(LorenstudioSettings.WeaponIDVip2, ComDiv.GetItemCategory(LorenstudioSettings.WeaponIDVip2), LorenstudioSettings.NameWeaponVIP2, 1, 86400);
                    PlayerManager.TryCreateItem(item, c._inventory, c.player_id);
                }
            }
        }

        public void AcessGame()
        {
            if (!c.IsGM())
                return;
            if (c._inventory.getItem(1103003016) == null || c._inventory.getItem(1103003010) == null)
            {
                ItemsModel Boina = new ItemsModel(1103003016, 2, "BoinaGeneral", 3, 86400),
                BoinaPBTN = new ItemsModel(1103003010, 2, "Login GM BoinaPBTN", 3, 86400);
                PlayerManager.TryCreateItem(Boina, c._inventory, c.player_id);
                PlayerManager.TryCreateItem(BoinaPBTN, c._inventory, c.player_id);
                SendDebug.SendInfo("[System] Item criado para o jogador que possui nível de acesso. usuario: " + c.player_name + " AcessLevel: " + c.access + "");
            }
            else
                SendDebug.SendInfo("[System] O item já foi criado para o jogador com nível de acesso. usuario: " + c.player_name + " AcessLevel: " + c.access + "");
        }

        private void CheckGameEvents(EventVisitModel evVisit)
        {
            long dateNow = long.Parse(DateTime.Now.ToString("yyMMddHHmm"));
            PlayerEvent pev = c._event;
            if (pev != null)
            {
                QuestModel evQuest = EventQuestSyncer.GetRunningEvent();
                if (evQuest != null)
                {
                    long date = pev.LastQuestDate, finish = pev.LastQuestFinish;
                    if (pev.LastQuestDate < evQuest.startDate)
                    {
                        pev.LastQuestDate = 0;
                        pev.LastQuestFinish = 0;
                        c.SendPacket(new SERVER_MESSAGE_EVENT_QUEST_PAK());
                    }
                    if (pev.LastQuestFinish == 0)
                    {
                        c._mission.mission4 = 13; //MissionId
                        if (pev.LastQuestDate == 0)
                            pev.LastQuestDate = (uint)dateNow;
                    }
                    if (pev.LastQuestDate != date || pev.LastQuestFinish != finish)
                        EventQuestSyncer.ResetPlayerEvent(c.player_id, pev);
                }
                EventLoginModel evLogin = EventLoginSyncer.GetRunningEvent();
                if (evLogin != null && !(evLogin.startDate < pev.LastLoginDate && pev.LastLoginDate < evLogin.endDate))
                {
                    ItemsModel item = new ItemsModel(evLogin._rewardId, evLogin._category, "Login event", 1, (uint)evLogin._count);
                    PlayerManager.TryCreateItem(item, c._inventory, c.player_id);
                    Message msg = new Message(15)
                    {
                        sender_name = LorenstudioSettings.ProjectName,
                        sender_id = 0,
                        text = "Você recebeu um presente, Verifique seu inventario.",
                        state = 1
                    };
                    Logger.EventSystem("O Usuario:" + c.player_id + " || " + "Recebeu o item:" + " " + evLogin._rewardId + " " + ("Com sucesso!!"));
                    c.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(msg), false);
                    MessageManager.CreateMessage(c.player_id, msg);
                    c.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0));
                    switch (item._category)
                    {
                        case 1: weapons.Add(item); break;
                        case 2: charas.Add(item); break;
                        case 3: cupons.Add(item); break;
                    }
                    ComDiv.UpdateDB("player_events", "last_login_date", dateNow, "player_id", c.player_id);
                }
                if (evVisit != null && pev.LastVisitEventId != evVisit.id)
                {
                    pev.LastVisitEventId = evVisit.id;
                    pev.LastVisitSequence1 = 0;
                    pev.LastVisitSequence2 = 0;
                    pev.NextVisitDate = 0;
                    EventVisitSyncer.ResetPlayerEvent(c.player_id, evVisit.id);
                }
                EventXmasModel evXmas = EventXmasSyncer.GetRunningEvent();
                if (evXmas != null)
                {
                    if (pev.LastXmasRewardDate < evXmas.startDate)
                    {
                        pev.LastXmasRewardDate = 0;
                        ComDiv.UpdateDB("player_events", "last_xmas_reward_date", 0, "player_id", c.player_id);
                    }
                    if (!(pev.LastXmasRewardDate > evXmas.startDate && pev.LastXmasRewardDate <= evXmas.endDate))
                        _xmas = true;
                }
            }
            ComDiv.UpdateDB("accounts", "last_login", dateNow, "player_id", c.player_id);
            AcessGame();
            AcessVipMASTER();
            AcessVipPREMIUM();
             AcessVipPLUS();
        }
        public override void Write()
        {
            WriteH(2566);
            WriteD(_erro);
            if (_erro > 0 && _erro != 0)
                return;
            ServerConfig cfg = LoginManager.Config;
            EventVisitModel evVisit = EventVisitSyncer.GetRunningEvent();
            WriteC(42); //formart result
            WriteS(c.player_name, 33);
            WriteD(c._exp);
            WriteD(c._rank);
            WriteD(c._rank);
            WriteD(c._gp);
            WriteD(c._money);
            WriteD(clan._id);
            WriteD(c.clanAccess);
            WriteQ(0);
            bool isLvl5or6 = (int)c.pc_cafe == 5 || (int)c.pc_cafe == 6; //tem que ser 6 ou 5 de certeza
            WriteC((byte)(isLvl5or6 ? 2 : (byte)c.pc_cafe)); //se for 5 ou 6 é 2 se nao retorna pccafe normal
            WriteC((byte)c.tourneyLevel);
            WriteC((byte)c.name_color);
            WriteS(clan._name, 17);
            WriteC((byte)clan._rank);
            WriteC((byte)clan.GetClanUnit());
            WriteD(clan._logo);
            WriteC((byte)clan._name_color);
            WriteD(10000);
            WriteC(0);
            WriteD(0);
            WriteD(c.LastRankUpDate); //109 BYTES
            WriteD(c._statistic.fights);
            WriteD(c._statistic.fights_win);
            WriteD(c._statistic.fights_lost);
            WriteD(c._statistic.fights_draw);
            WriteD(c._statistic.kills_count);
            WriteD(c._statistic.headshots_count);
            WriteD(c._statistic.deaths_count);
            WriteD(c._statistic.totalfights_count);
            WriteD(c._statistic.totalkills_count);
            WriteD(c._statistic.escapes);
            WriteD(c._statistic.fights);
            WriteD(c._statistic.fights_win);
            WriteD(c._statistic.fights_lost);
            WriteD(c._statistic.fights_draw);
            WriteD(c._statistic.kills_count);
            WriteD(c._statistic.headshots_count);
            WriteD(c._statistic.deaths_count);
            WriteD(c._statistic.totalfights_count);
            WriteD(c._statistic.totalkills_count);
            WriteD(c._statistic.escapes);
            WriteD(c._equip._red);
            WriteD(c._equip._blue);
            WriteD(c._equip._helmet);
            WriteD(c._equip._beret);
            WriteD(c._equip._dino);
            WriteD(c._equip._primary);
            WriteD(c._equip._secondary);
            WriteD(c._equip._melee);
            WriteD(c._equip._grenade);
            WriteD(c._equip._special);
            WriteH(0);
            WriteD(c._bonus.fakeRank);
            WriteD(c._bonus.fakeRank);
            WriteS(c._bonus.fakeNick, 33);
            WriteH((short)c._bonus.sightColor);
            WriteC(31);
            CheckGameEvents(evVisit);
            if (cfg.ClientVersion == "1.15.36" || cfg.ClientVersion == "1.15.37")
                WriteTopicoVersao37();
            WriteC(Settings.Outpost);
            WriteD(c.brooch);
            WriteD(c.insignia);
            WriteD(c.medal);
            WriteD(c.blue_order);
            WriteC((byte)c._mission.actualMission);
            WriteC((byte)c._mission.card1);
            WriteC((byte)c._mission.card2);
            WriteC((byte)c._mission.card3);
            WriteC((byte)c._mission.card4);
            WriteB(ComDiv.getCardFlags(c._mission.mission1, c._mission.list1));
            WriteB(ComDiv.getCardFlags(c._mission.mission2, c._mission.list2));
            WriteB(ComDiv.getCardFlags(c._mission.mission3, c._mission.list3));
            WriteB(ComDiv.getCardFlags(c._mission.mission4, c._mission.list4));
            WriteC((byte)c._mission.mission1);
            WriteC((byte)c._mission.mission2);
            WriteC((byte)c._mission.mission3);
            WriteC((byte)c._mission.mission4);
            WriteB(c._mission.list1);
            WriteB(c._mission.list2);
            WriteB(c._mission.list3);
            WriteB(c._mission.list4);
            WriteQ(c._titles.Flags);
            WriteC((byte)c._titles.Equiped1);
            WriteC((byte)c._titles.Equiped2);
            WriteC((byte)c._titles.Equiped3);
            WriteD(c._titles.Slots);
            WriteD((int)ConfigMaps.Desafio);
            WriteD((int)ConfigMaps.MataMata);
            WriteD((int)ConfigMaps.Destruicao);
            WriteD((int)ConfigMaps.Sabotage);
            WriteD((int)ConfigMaps.Supressao);
            WriteD((int)ConfigMaps.Defender);
            WriteD((int)ConfigMaps.Desafio);
            WriteD((int)ConfigMaps.Dinossauro);
            WriteD((int)ConfigMaps.Sniper);
            WriteD((int)ConfigMaps.Shotgun);
            WriteD((int)ConfigMaps.TiroNaCabeca);
            WriteD((int)ConfigMaps.Soco);
            WriteD((int)ConfigMaps.CrossCounter);
            WriteD((int)ConfigMaps.Caos);
            if (cfg.ClientVersion.EndsWith("38")
             || cfg.ClientVersion.EndsWith("39") ||
                cfg.ClientVersion.EndsWith("41") ||
                cfg.ClientVersion.EndsWith("42"))
                WriteD((int)ConfigMaps.TheifMode);
            //TERMINAR CONVERSÃO PARA XML
            WriteC((byte)MapsXML.ModeList.Count); //124 maps ver 42

            WriteC(4); //(Flag pages | 4 bytes)
            WriteD(MapsXML.maps1);
            WriteD(MapsXML.maps2);
            WriteD(MapsXML.maps3);
            WriteD(MapsXML.maps4);
            for (int index = 0; index < MapsXML.ModeList.Count; index++)
                WriteH(MapsXML.ModeList[index]);
            WriteB(MapsXML.TagList.ToArray());

            WriteC(cfg.missions);
            WriteD(MissionJSON._missionPage1);
            WriteD(50);
            WriteD(75);
            WriteC(1);
            WriteH(20);
            WriteB(new byte[20] { 0x70, 0x0C, 0x94, 0x2D, 0x48, 0x08, 0xDD, 0x1E, 0xB0, 0xAB, 0x1A, 0x00, 0x99, 0x7B, 0x42, 0x00, 0x70, 0x0C, 0x94, 0x2D });
            WriteD(c.IsGM() || c.HaveAcessLevel() || c.HaveVip4()); //observer
            WriteD(_xmas);
            WriteC(1);

            WriteVisitEvent(evVisit);

            if (cfg.ClientVersion.EndsWith("39") ||
                cfg.ClientVersion.EndsWith("41") ||
                cfg.ClientVersion.EndsWith("42"))
                WriteB(new byte[9]);

            WriteD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
            WriteS("10.120.1.44", 256);
            WriteD(8085);
            WriteC(cfg.GiftSystem);

            WriteH(1);
            WriteC(0);
            WriteC(1);

            WriteC(7); // contagen de bytees para baixo
            WriteC((byte)ITEM_TAG.NEW);
            //NextModel.SomeMais1 += 1;
            //Console.WriteLine("Teste: " + NextModel.SomeMais1);
            //WriteC((byte)NextModel.SomeMais1); //montagem da loja
            WriteC(1); //montagem da loja

            // 3 Tudo de cabeça pra baixo menos o VIP
            WriteC((byte)ITEM_TAG.HOT);
            WriteC((byte)ITEM_TAG.SALE);
            WriteC((byte)ITEM_TAG.EVENT);
            WriteC((byte)ITEM_TAG.VIP);
            WriteC((byte)ITEM_TAG.SIX); //nao é tag eu que sou doenter
        }
        private void WriteVisitEvent(EventVisitModel ev)
        {
            PlayerEvent pev = c._event;
            if (ev != null && (pev.LastVisitSequence1 < ev.checks && pev.NextVisitDate <= int.Parse(DateTime.Now.ToString("yyMMdd")) || pev.LastVisitSequence2 < ev.checks && pev.LastVisitSequence2 != pev.LastVisitSequence1))
            {
                WriteD(ev.id);
                WriteC((byte)pev.LastVisitSequence1);
                WriteC((byte)pev.LastVisitSequence2);
                WriteH(0);
                WriteD(ev.startDate); //12
                WriteS(ev.title, 60);
                WriteC(2);
                WriteC((byte)ev.checks);
                WriteH(0);
                WriteD(ev.id);
                WriteD(ev.startDate);
                WriteD(ev.endDate);
                for (int i = 0; i < 7; i++)
                {
                    VisitBox box = ev.box[i];
                    WriteD(box.RewardCount);
                    WriteD(box.reward1.good_id);
                    WriteD(box.reward2.good_id);
                }
            }
            else
                WriteB(new byte[172]);
        }

   

        public void WriteTopicoVersao37() //se conta isso pq na v37 nao se usa pacote de inventario separado tipo  a v42
        {
            int charascount = 0, weaponscount = 0, cuponscount = 0;
            WriteC(1);
            WriteD(charas.Count);
            WriteD(weapons.Count);
            WriteD(cupons.Count);
            WriteD(0);
            do
            {
                ItemsModel item = charas[charascount];
                WriteQ(item._objId);
                WriteD(item._id);
                WriteC((byte)item._equip);
                WriteD(item._count);
            }
            while (++charascount < charas.Count);
            do
            {
                ItemsModel item = weapons[weaponscount];
                WriteQ(item._objId);
                WriteD(item._id);
                WriteC((byte)item._equip);
                WriteD(item._count);
            }
            while (++weaponscount < weapons.Count);
            do
            {
                ItemsModel item = cupons[cuponscount];
                WriteQ(item._objId);
                WriteD(item._id);
                WriteC((byte)item._equip);
                WriteD(item._count);
            }
            while (++cuponscount < cupons.Count);
        }
    }
}