using Core;
using Core.managers;
using Core.models.account.players;
using Core.server;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class INVENTORY_ITEM_EXCLUDE_REC : ReceiveGamePacket
    {
        private long objId;
        private uint erro = 1;
        public INVENTORY_ITEM_EXCLUDE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            objId = ReadQ();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                ItemsModel item = p._inventory.getItem(objId);
                PlayerBonus bonus = p._bonus;
                if (item == null)
                    erro = 0x80000000;
                else if (ComDiv.GetIdStatics(item._id, 1) == 12)
                {
                    if (bonus == null)
                    {
                        _client.SendPacket(new INVENTORY_ITEM_EXCLUDE_PAK(0x80000000));
                        return;
                    }
                    if (!bonus.RemoveBonuses(item._id))
                    {
                        switch (item._id)
                        {
                            case 1200014000:
                                {
                                    if (ComDiv.UpdateDB("player_bonus", "sightcolor", 4, "player_id", p.player_id))
                                    {
                                        bonus.sightColor = 4;
                                        _client.SendPacket(new BASE_USER_EFFECTS_PAK(0, bonus));
                                    }
                                    else erro = 0x80000000;
                                    break;
                                }
                            case 1200010000:
                                {
                                    if (bonus.fakeNick.Length == 0)
                                        erro = 0x80000000;
                                    else
                                    {
                                        if (ComDiv.UpdateDB("accounts", "player_name", bonus.fakeNick, "player_id", p.player_id) &&
                                            ComDiv.UpdateDB("player_bonus", "fakenick", "", "player_id", p.player_id))
                                        {
                                            p.player_name = bonus.fakeNick;
                                            bonus.fakeNick = "";
                                            _client.SendPacket(new BASE_USER_EFFECTS_PAK(0, bonus));
                                            _client.SendPacket(new AUTH_CHANGE_NICKNAME_PAK(p.player_name));
                                        }
                                        else erro = 0x80000000;
                                    }
                                    break;
                                }
                            case 1200009000:
                                {
                                    if (ComDiv.UpdateDB("player_bonus", "fakerank", 55, "player_id", p.player_id))
                                    {
                                        bonus.fakeRank = 55;
                                        _client.SendPacket(new BASE_USER_EFFECTS_PAK(0, bonus));
                                    }
                                    else erro = 0x80000000;
                                    break;
                                }
                            case 1200006000:
                                {
                                    if (ComDiv.UpdateDB("accounts", "name_color", 0, "player_id", p.player_id))
                                    {
                                        p.name_color = 0;
                                        _client.SendPacket(new BASE_2612_PAK(p));
                                        Room room = p._room;
                                        if (room != null)
                                            using (ROOM_GET_NICKNAME_PAK packet = new ROOM_GET_NICKNAME_PAK(p._slotId, p.player_name, p.name_color))
                                                room.SendPacketToPlayers(packet);
                                    }
                                    else erro = 0x80000000;
                                    break;
                                }
                            default:
                                {
                                    PlayerManager.UpdatePlayerBonus(p.player_id, bonus.bonuses, bonus.freepass); break;
                                }
                        }
                    }
                    CupomFlag cupom = CupomEffectManagerJSON.GetCupomEffect(item._id);
                    if (cupom != null && cupom.EffectFlag > 0 && p.effects.HasFlag(cupom.EffectFlag))
                    {
                        p.effects -= cupom.EffectFlag;
                        PlayerManager.UpdateCupomEffects(p.player_id, p.effects);
                    }
                }
                if (erro == 1 && item != null)
                {
                    if (PlayerManager.DeleteItem(item._objId, p.player_id))
                        p._inventory.RemoveItem(item);
                    else
                        erro = 0x80000000;
                }
                _client.SendPacket(new INVENTORY_ITEM_EXCLUDE_PAK(erro, objId));
            }
            catch (Exception ex)
            {
                Logger.Info("[INVENTORY_ITEM_EXCLUDE_REC] " + ex.ToString());
                _client.SendPacket(new INVENTORY_ITEM_EXCLUDE_PAK(0x80000000));
            }
        }
    }
}