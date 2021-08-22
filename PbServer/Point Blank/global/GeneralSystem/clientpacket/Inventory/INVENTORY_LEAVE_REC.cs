using Core;
using Core.managers;
using Core.models.account.players;
using Core.models.enums;
using Core.server;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class INVENTORY_LEAVE_REC : ReceiveGamePacket
    {
        private int erro, type;
        private PlayerEquipedItems data;
        public INVENTORY_LEAVE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            type = ReadD();
        }

        public override void Run()
        {
            try
            {
                if (_client == null)
                    return;
                Account p = _client._player;
                if (p == null)
                    return;
                data = new PlayerEquipedItems();
                DBQuery query = new DBQuery();
                if ((type & 1) == 1)
                    LoadCharaData(p, query);
                if ((type & 2) == 2)
                    LoadWeaponsData(p, query);
                if (ComDiv.UpdateDB("accounts", "player_id", p.player_id, query.GetTables(), query.GetValues()))
                {
                    UpdateChara(p);
                    UpdateWeapons(p);
                }
                query = null;
                Room room = p._room;
                if (room != null)
                {
                    if (type > 0)
                        AllUtils.UpdateSlotEquips(p, room);
                    room.ChangeSlotState(p._slotId, SLOT_STATE.NORMAL, true);
                }
                _client.SendPacket(new INVENTORY_LEAVE_PAK(erro, (erro > 0 ? 3 : 0)));
            }
            catch (Exception ex)
            {
                Logger.Info("INVENTORY_LEAVE_REC: " + ex.ToString());
            }
        }
        private void LoadWeaponsData(Account p, DBQuery query)
        {
            data._primary = ReadD();
            data._secondary = ReadD();
            data._melee = ReadD();
            data._grenade = ReadD();
            data._special = ReadD();
            if (p != null)
                PlayerManager.UpdateWeapons(data, p._equip, query);
            else erro = -1;
        }
        private void UpdateWeapons(Account p)
        {
            if ((type & 2) == 2)
            {
                p._equip._primary = data._primary;
                p._equip._secondary = data._secondary;
                p._equip._melee = data._melee;
                p._equip._grenade = data._grenade;
                p._equip._special = data._special;
            }
        }
        private void UpdateChara(Account p)
        {
            if ((type & 1) == 1)
            {
                p._equip._red = data._red;
                p._equip._blue = data._blue;
                p._equip._helmet = data._helmet;
                p._equip._beret = data._beret;
                p._equip._dino = data._dino;
            }
        }
        private void LoadCharaData(Account p, DBQuery query)
        {
            data._red = ReadD();
            data._blue = ReadD();
            data._helmet = ReadD();
            data._beret = ReadD();
            data._dino = ReadD();
            if (p != null)
                PlayerManager.UpdateChars(data, p._equip, query);
            else erro = -1;
        }
    }
}