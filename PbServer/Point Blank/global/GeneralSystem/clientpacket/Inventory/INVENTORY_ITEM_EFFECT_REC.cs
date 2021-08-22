using Core;
using Core.managers;
using Core.models.account.clan;
using Core.models.account.players;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class INVENTORY_ITEM_EFFECT_REC : ReceiveGamePacket
    {
        private long objId;
        private uint objetivo, erro = 1;
        private byte[] info;
        private string txt;
        public INVENTORY_ITEM_EFFECT_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            objId = ReadQ();
            info = ReadB(ReadC());
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                ItemsModel item = p?._inventory.getItem(objId);
                if (item != null && item._id > 1300000000)
                {
                    int cuponId = ComDiv.CreateItemId(12, ComDiv.GetIdStatics(item._id, 2), ComDiv.GetIdStatics(item._id, 3), 0);
                    uint cuponDays = uint.Parse(DateTime.Now.AddDays(ComDiv.GetIdStatics(item._id, 4)).ToString("yyMMddHHmm"));
                    switch (cuponId)
                    {
                        case int id when (id == 1201047000 || id == 1201051000 || id == 1200010000):
                            {
                                txt = ComDiv.ArrayToString(info, info.Length);
                                break;
                            }
                        case int id2 when (id2 == 1201052000 || id2 == 1200005000):
                            {
                                objetivo = BitConverter.ToUInt32(info, 0);
                                break;
                            }
                        default:
                            {
                                if (info.Length > 0)
                                    objetivo = info[0];
                                break;
                            }
                    }
                    CreateCuponEffects(cuponId, cuponDays, p);
                }
                else erro = 0x80000000;
                _client.SendPacket(new INVENTORY_ITEM_EQUIP_PAK(erro, item, p));
            }
            catch (Exception ex)
            {
                Logger.Info("INVENTORY_ITEM_EFFECT_REC: " + ex.ToString());
                _client.SendPacket(new INVENTORY_ITEM_EQUIP_PAK(0x80000000));
            }
        }
        /// <summary>
        /// Gera efeitos dos cupons na Database.
        /// </summary>
        /// <param name="cuponId">Id do cupom</param>
        /// <param name="cuponDays">Dias do cupom</param>
        /// <param name="p">Jogador</param>
        private void CreateCuponEffects(int cupomId, uint cuponDays, Account p)
        {
            switch (cupomId)
            {
                case 1201051000:
                    {
                        if (string.IsNullOrEmpty(txt) || txt.Length > 16)
                            erro = 0x80000000;
                        else
                        {
                            Clan c = ClanManager.GetClan(p.clanId);
                            if (c._id > 0 && c.owner_id == _client.player_id)
                            {
                                if (!ClanManager.IsClanNameExist(txt) && ComDiv.UpdateDB("clan_data", "clan_name", txt, "clan_id", p.clanId))
                                {
                                    c._name = txt;
                                    using CLAN_CHANGE_NAME_PAK packet = new CLAN_CHANGE_NAME_PAK(txt);
                                    ClanManager.SendPacket(packet, p.clanId, -1, true, true);
                                }
                                else erro = 0x80000000;
                            }
                            else erro = 0x80000000;
                        }
                        break;
                    }
                case 1201052000:
                    {
                        Clan clan = ClanManager.GetClan(p.clanId);
                        if (clan._id > 0 && clan.owner_id == _client.player_id && !ClanManager.IsClanLogoExist(objetivo) &&
                            PlayerManager.UpdateClanLogo(p.clanId, objetivo))
                        {
                            clan._logo = objetivo;
                            using CLAN_CHANGE_LOGO_PAK packet = new CLAN_CHANGE_LOGO_PAK(objetivo);
                            ClanManager.SendPacket(packet, p.clanId, -1, true, true);
                        }
                        else erro = 0x80000000;
                        break;
                    }
                case 1201047000:
                    {
                        if (string.IsNullOrEmpty(txt) || txt.Length < Settings.minNickSize || txt.Length > Settings.maxNickSize ||
                  p._inventory.getItem(1200010000) != null)
                            erro = 0x80000000;
                        else if (!PlayerManager.IsPlayerNameExist(txt))
                        {
                            if (ComDiv.UpdateDB("accounts", "player_name", txt, "player_id", p.player_id))
                            {
                                NickHistoryManager.CreateHistory(p.player_id, p.player_name, txt, "Change nick");
                                p.player_name = txt;
                                if (p._room != null)
                                    using (ROOM_GET_NICKNAME_PAK packet = new ROOM_GET_NICKNAME_PAK(p._slotId, p.player_name, p.name_color))
                                        p._room.SendPacketToPlayers(packet);
                                _client.SendPacket(new AUTH_CHANGE_NICKNAME_PAK(p.player_name));
                                if (p.clanId > 0)
                                {
                                    using CLAN_MEMBER_INFO_UPDATE_PAK packet = new CLAN_MEMBER_INFO_UPDATE_PAK(p);
                                    ClanManager.SendPacket(packet, p.clanId, -1, true, true);
                                }
                                AllUtils.SyncPlayerToFriends(p, true);
                            }
                            else erro = 0x80000000;
                        }
                        else erro = 2147483923;
                        break;
                    }
                case 1200006000:
                    {
                        if (ComDiv.UpdateDB("accounts", "name_color", (int)objetivo, "player_id", p.player_id))
                        {
                            p.name_color = (int)objetivo;
                            _client.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, p, new ItemsModel(cupomId, 3, "NameColor [Active]", 2, cuponDays, 0)));
                            _client.SendPacket(new BASE_2612_PAK(p));
                            if (p._room != null)
                                using (ROOM_GET_NICKNAME_PAK packet = new ROOM_GET_NICKNAME_PAK(p._slotId, p.player_name, p.name_color))
                                    p._room.SendPacketToPlayers(packet);
                        }
                        else erro = 0x80000000;
                        break;
                    }
                case 1200009000:
                    {
                        if ((int)objetivo >= 51 || (int)objetivo < p._rank - 10 || (int)objetivo > p._rank + 10)
                            erro = 0x80000000;
                        else if (ComDiv.UpdateDB("player_bonus", "fakerank", (int)objetivo, "player_id", p.player_id))
                        {
                            p._bonus.fakeRank = (int)objetivo;
                            _client.SendPacket(new BASE_USER_EFFECTS_PAK(info.Length, p._bonus));
                            _client.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, p, new ItemsModel(cupomId, 3, "Patente falsa [Active]", 2, cuponDays, 0)));
                            if (p._room != null)
                                p._room.UpdateSlotsInfo();
                        }
                        else erro = 0x80000000;
                        break;
                    }
                case 1200010000:
                    {
                        if (string.IsNullOrEmpty(txt) || txt.Length < Settings.minNickSize || txt.Length > Settings.maxNickSize)
                            erro = 0x80000000;
                        else if (ComDiv.UpdateDB("player_bonus", "fakenick", p.player_name, "player_id", p.player_id) &&
                            ComDiv.UpdateDB("accounts", "player_name", txt, "player_id", p.player_id))
                        {
                            p._bonus.fakeNick = p.player_name;
                            p.player_name = txt;
                            _client.SendPacket(new BASE_USER_EFFECTS_PAK(info.Length, p._bonus));
                            _client.SendPacket(new AUTH_CHANGE_NICKNAME_PAK(p.player_name));
                            _client.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, p, new ItemsModel(cupomId, 3, "FakeNick [Active]", 2, cuponDays, 0)));
                            if (p._room != null)
                                p._room.UpdateSlotsInfo();
                        }
                        else erro = 0x80000000;
                        break;
                    }
                case 1200014000:
                    {
                        if (ComDiv.UpdateDB("player_bonus", "sightcolor", (int)objetivo, "player_id", p.player_id))
                        {
                            p._bonus.sightColor = (int)objetivo;
                            _client.SendPacket(new BASE_USER_EFFECTS_PAK(info.Length, p._bonus));
                            _client.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, p, new ItemsModel(cupomId, 3, "Cor da mira [Active]", 2, cuponDays, 0)));
                        }
                        else erro = 0x80000000;
                        break;
                    }
                case 1200005000:
                    {
                        Clan c = ClanManager.GetClan(p.clanId);
                        if (c._id > 0 && c.owner_id == _client.player_id && ComDiv.UpdateDB("clan_data", "color", (int)objetivo, "clan_id", c._id))
                        {
                            c._name_color = (int)objetivo;
                            _client.SendPacket(new CLAN_CHANGE_NAME_COLOR_PAK((byte)c._name_color));
                        }
                        else erro = 0x80000000;
                        break;
                    }
                case 1201085000:
                    {
                        if (p._room != null)
                        {
                            Account pR = p._room.GetPlayerBySlot((int)objetivo);
                            if (pR != null)
                                _client.SendPacket(new ROOM_INSPECTPLAYER_PAK(pR));
                            else erro = 0x80000000;
                        }
                        else erro = 0x80000000;
                        break;
                    }
                default:
                    {
                        Logger.Error("[ITEM_EFFECT] Efeito do cupom não encontrado! Id: " + cupomId);
                        erro = 0x80000000;
                        break;
                    }
            }
        }
    }
}