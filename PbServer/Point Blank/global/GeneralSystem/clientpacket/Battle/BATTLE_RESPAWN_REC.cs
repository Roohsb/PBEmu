using Core;
using Core.managers;
using Core.models.account.players;
using Core.models.enums;
using Core.models.enums.flags;
using Core.models.enums.item;
using Core.models.room;
using Game.data.managers;
using Game.data.model;
using Game.data.sync;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_RESPAWN_REC : ReceiveGamePacket
    {
        private PlayerEquipedItems equip;
        private int WeaponsFlag;
        private Account p;
        public BATTLE_RESPAWN_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            equip = new PlayerEquipedItems
            {
                _primary = ReadD(),
                _secondary = ReadD(),
                _melee = ReadD(),
                _grenade = ReadD(),
                _special = ReadD(),
                unk = ReadD(),
                _red = ReadD(),
                _blue = ReadD(),
                _helmet = ReadD(),
                _beret = ReadD(),
                _dino = ReadD()
            };
            WeaponsFlag = ReadC();
        }

        public override void Run()
        {
            try
            {
                p = _client._player;
                if (p == null)
                    return;
                Room r = p._room;
                if (r != null && r._state == RoomState.Battle)
                {
                    SLOT slot = r.GetSlot(p._slotId);
                    if (slot != null && slot.state == SLOT_STATE.BATTLE)
                    {
                        if (slot._deathState.HasFlag(DeadEnum.isDead) || slot._deathState.HasFlag(DeadEnum.useChat))
                            slot._deathState = DeadEnum.isAlive;
                        PlayerManager.CheckEquipedItems(equip, p._inventory._items, true);
                        ClassicModeCheck(r, equip);
                        slot._equip = equip;
                        if ((WeaponsFlag & 8) > 0)
                            InsertItem(equip._primary, slot);
                        if ((WeaponsFlag & 4) > 0)
                            InsertItem(equip._secondary, slot);
                        if ((WeaponsFlag & 2) > 0)
                            InsertItem(equip._melee, slot);
                        if ((WeaponsFlag & 1) > 0)
                            InsertItem(equip._grenade, slot);
                        InsertItem(equip._special, slot);
                        if (slot._team == 0)
                            InsertItem(equip._red, slot);
                        else
                            InsertItem(equip._blue, slot);
                        InsertItem(equip._helmet, slot);
                        InsertItem(equip._beret, slot);
                        InsertItem(equip._dino, slot);
                   
                        using (BATTLE_RESPAWN_PAK packet = new BATTLE_RESPAWN_PAK(r, slot))
                            r.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                        if (slot.firstRespawn)
                        {
                            slot.firstRespawn = false;
                            Game_SyncNet.SendUDPPlayerSync(r, slot, 0);
                        }
                        else
                            Game_SyncNet.SendUDPPlayerSync(r, slot, 2);
                        if (r.weaponsFlag != WeaponsFlag)
                            SendDebug.SendInfo("Room: " + r.weaponsFlag + "; Player: " + WeaponsFlag);
                    }
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[BATTLE_RESPAWN_REC] " + ex.ToString());
            }
        }
        private void ClassicModeCheck(Room room, PlayerEquipedItems equip)
        {
            int countdeblock = 0;
            if (room.name.ToLower().Contains("@camp") || room.name.ToLower().Contains(" @camp") || room.name.ToLower().Contains("@camp ") || room.name.ToLower().Contains("camp") || room.name.ToLower().Contains("@pvp "))
            {
                for (int i = 0; i < ClassicModeManager.itemscamp.Count; i++)
                {
                    int id = ClassicModeManager.itemscamp[i];
                    if (ClassicModeManager.IsBlocked(id, equip._primary))
                    {
                        countdeblock += 1;
                        equip._primary = RandomWeaponsClear();
                    }
                    if (ClassicModeManager.IsBlocked(id, equip._secondary))
                    {
                        countdeblock += 1;
                        equip._secondary = (int)Item_defaut.secundaria;
                    }
                    if (ClassicModeManager.IsBlocked(id, equip._melee))
                    {
                        countdeblock += 1;
                        equip._melee = (int)Item_defaut.faca;
                    }
                    if (ClassicModeManager.IsBlocked(id, equip._grenade))
                    {
                        countdeblock += 1;
                        equip._grenade = (int)Item_defaut.granada;
                    }
                    if (ClassicModeManager.IsBlocked(id, equip._special))
                    {
                        countdeblock += 1;
                        equip._special = (int)Item_defaut.smoke;
                    }
                    if (ClassicModeManager.IsBlocked(id, equip._beret))
                    {
                        countdeblock += 1;
                        equip._beret = 0;
                    }
                    if (ClassicModeManager.IsBlocked(id, equip._red))
                    {
                        countdeblock += 1;
                        equip._red = (int)Item_defaut._red;
                    }
                    if (ClassicModeManager.IsBlocked(id, equip._blue))
                    {
                        countdeblock += 1;
                        equip._blue = (int)Item_defaut._blue;
                    }
                }
                if (countdeblock != 0)
                {
                    string str = p.player_name + " your weapon is not allowed on @Camp.";
                    using SERVER_MESSAGE_ANNOUNCE_PAK inGame = new SERVER_MESSAGE_ANNOUNCE_PAK(str);
                    p.SendPacket(inGame);
                }
            }
        }
        private void InsertItem(int id, SLOT slot)
        {
            lock (slot.armas_usadas)
            {
                if (!slot.armas_usadas.Contains(id))
                    slot.armas_usadas.Add(id);
            }
        }
        public int RandomWeaponsClear()
        {
            int valor = 0;
            switch (new Random().Next(1, 4))
            {
                case 1: valor = (int)Item_defaut.Mode_Primaria_K1; break;
                case 2: valor = (int)Item_defaut.Mode_Primaria_K2; break;
                case 3: valor = (int)Item_defaut.Mode_Primaria_SSG; break;
                case 4: valor = (int)Item_defaut.Mode_Primaria_870MCS; break;
            }
            return valor;
        }
    }
}