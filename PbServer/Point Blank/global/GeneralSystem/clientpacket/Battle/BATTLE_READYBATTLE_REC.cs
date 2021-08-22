using Core;
using Core.models.account.players;
using Core.models.enums;
using Core.models.enums.item;
using Core.models.room;
using Game.data.managers;
using Game.data.model;
using Game.data.sync;
using Game.data.utils;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;
using System.Net;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_READYBATTLE_REC : ReceiveGamePacket
    {
        private int erro;
        public BATTLE_READYBATTLE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            erro = ReadD();
        }
        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null)
                    return;
                Room room = player._room;
                if (room == null || room.GetLeader() == null || !room.GetChannel(out Channel ch) || !room.GetSlot(player._slotId, out SLOT slot))
                    return;
                if (slot._equip == null)
                {
                    _client.SendPacket(new BATTLE_READY_ERROR_PAK(0x800010AB));
                    return;
                }
                bool isBotMode = room.IsBotMode();
                if (erro == 1 && player.IsGM())
                    slot.specGM = true;
                else
                    slot.specGM = false;

                player.DebugPing = false;

                if (Settings.EnableClassicRules && ClassicModeCheck(player, room))
                    return;
                if (!BloqueioNoPVP(player, room))
                    return;
                room.ReadysPvp.Clear();
                Game_SyncNet.SendIPPrestart(player, new IPEndPoint(IPAddress.Parse(Settings.IP_Jogo), 1910));
                if (room._leader == player._slotId)
                {
                    if (room._state != RoomState.Ready && room._state != RoomState.CountDown)
                        return;
                    bool check = false;
                    Checkx2(room, ref check);
                    if (check)
                        return;
                    int TotalEnemys = 0, redPlayers = 0, bluePlayers = 0;
                    GetReadyPlayers(room, ref redPlayers, ref bluePlayers, ref TotalEnemys);
                    if (room.stage4v4 == 1 && (redPlayers >= 4 || bluePlayers >= 4))
                    {
                        _client.SendPacket(new BATTLE_4VS4_ERROR_PAK());
                        return;
                    }
                    if (ClanMatchCheck(room, ch._type, TotalEnemys))
                        return;
                    if (isBotMode || room.room_type == 10 || TotalEnemys > 0 && !isBotMode)
                    {
                        room.ChangeSlotState(slot, SLOT_STATE.READY, false);
                        if ((int)room._state != 1)
                            TryBalanceTeams(room, isBotMode);
                        if (room.ThisModeHaveCD())
                        {
                            if (room._state == 0)
                            {
                                room._state = RoomState.CountDown;
                                room.UpdateRoomInfo();
                                room.StartCountDown();
                            }
                            else if ((int)room._state == 1)
                            {
                                room.ChangeSlotState(room._leader, SLOT_STATE.NORMAL, false);
                                room.StopCountDown(CountDownEnum.StopByHost);
                            }
                        }
                        else
                            room.StartBattle(false);
                        room.UpdateSlotsInfo();
                    }
                    else if (TotalEnemys == 0 && !isBotMode)
                        _client.SendPacket(new BATTLE_READY_ERROR_PAK(0x80001009));
                }
                else if ((int)room._slots[room._leader].state >= 9)
                {
                    if (slot.state == SLOT_STATE.NORMAL)
                    {
                        if (room.autobalans == 1 && !isBotMode)
                            AllUtils.TryBalancePlayer(room, player, true, ref slot);
                        room.ChangeSlotState(slot, SLOT_STATE.LOAD, true);
                        slot.SetMissionsClone(player._mission);
                        _client.SendPacket(new BATTLE_READYBATTLE_PAK(room));
                        _client.SendPacket(new BATTLE_READY_ERROR_PAK((uint)slot.state));
                        using BATTLE_READYBATTLE2_PAK packet = new BATTLE_READYBATTLE2_PAK(slot, player._titles);
                        room.SendPacketToPlayers(packet, SLOT_STATE.READY, 1, slot._id);
                    }
                }
                else if ((int)slot.state == 7)
                {
                    room.ChangeSlotState(slot, SLOT_STATE.READY, true);
                    if ((int)room._state == 1)
                        TryBalanceTeams(room, isBotMode);
                }
                else if ((int)slot.state == 8)
                {
                    room.ChangeSlotState(slot, SLOT_STATE.NORMAL, false);
                    if ((int)room._state == 1 && room.GetPlayingPlayers(room._leader % 2 == 0 ? 1 : 0, SLOT_STATE.READY, 0) == 0)
                    {
                        room.ChangeSlotState(room._leader, SLOT_STATE.NORMAL, false);
                        room.StopCountDown(CountDownEnum.StopByPlayer);
                    }
                    room.UpdateSlotsInfo();
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("BATTLE_READYBATTLE_REC: " + ex.ToString());
            }
        }
        public void Checkx2(Room room, ref bool check)
        {
            if (room.name.ToLower().Contains("@camp") || room.name.ToLower().Contains(" @camp") || room.name.ToLower().Contains("@camp ") || room.name.ToLower().Contains("camp") || room.name.ToLower().Contains("@pvp "))
            {
                for (int i = 0; i < 16; i++)
                {
                    int idx = 0;
                    SLOT slot = room._slots[i];
                    Account p = AccountManager.GetAccount(slot._playerId, false);
                    if (p != null)
                    {
                        for (int weaponID = 0; weaponID < ClassicModeManager.itemscamp.Count; weaponID++)
                        {
                            int id = ClassicModeManager.itemscamp[weaponID];
                            if (ClassicModeManager.IsBlocked(id, p._equip._primary))
                                idx += 1;
                            if (ClassicModeManager.IsBlocked(id, p._equip._secondary))
                                idx += 1;
                            if (ClassicModeManager.IsBlocked(id, p._equip._melee))
                                idx += 1;
                            if (ClassicModeManager.IsBlocked(id, p._equip._grenade))
                                idx += 1;
                            if (ClassicModeManager.IsBlocked(id, p._equip._special))
                                idx += 1;
                            if (ClassicModeManager.IsBlocked(id, p._equip._beret))
                                idx += 1;
                            if (ClassicModeManager.IsBlocked(id, p._equip._red))
                                idx += 1;
                            if (ClassicModeManager.IsBlocked(id, p._equip._blue))
                                idx += 1;
                        }
                        if(idx != 0)
                        {

                            p.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK("Encontramos uma irregularidade no seu @camp!"));
                            if(room.GetPlayingPlayers(room._leader % 2 == 0 ? 1 : 0, SLOT_STATE.READY, 0) == 0)
                            {
                                room.ChangeSlotState(room._leader, SLOT_STATE.NORMAL, false);
                                room.StopCountDown(CountDownEnum.StopByPlayer);
                                check = true;
                            }
                            else
                            {
                                room.ChangeSlotState(slot, SLOT_STATE.NORMAL, true);
                            }
                            room.UpdateSlotsInfo();
                        }
                    }
                }
            }
        }
        private void GetReadyPlayers(Room room, ref int redPlayers, ref int bluePlayers, ref int TotalEnemys)
        {
            for (int i = 0; i < 16; i++)
            {
                SLOT slot = room._slots[i];
                if ((int)slot.state == 8)
                {
                    if (slot._team == 0)
                        redPlayers++;
                    else
                        bluePlayers++;
                }
            }
            if (room._leader % 2 == 0)
                TotalEnemys = bluePlayers;
            else
                TotalEnemys = redPlayers;
        }
        private bool ClanMatchCheck(Room room, int type, int TotalEnemys)
        {
            if (Settings.isTestMode || type != 4)
                return false;

            if (!AllUtils.Have2ClansToClanMatch(room))
            {
                _client.SendPacket(new BATTLE_READY_ERROR_PAK(0x80001071));
                return true;
            }
            if (TotalEnemys > 0 && !AllUtils.HavePlayersToClanMatch(room))
            {
                _client.SendPacket(new BATTLE_READY_ERROR_PAK(0x80001072));
                return true;
            }
            return false;
        }
        public static bool BloqueioNoPVP(Account p, Room room)
        {
            bool check;
            Account Lider = room.GetLeader();
            try
            {
                if(room.name.ToLower().Contains("@pvp "))
                {
                    int aposta = int.Parse(room.name.Split(' ')[1]);
                    if(aposta > 50000 || aposta < 500)
                    {
                        string Texto = "the bet must be greater than 500 or less than 50k so you will not be able to participate. ;ç";
                        if (!room.ReadysPvp.Contains(p.player_id))
                        {
                            SendErros(Lider, Texto);
                            room.ReadysPvp.Add(p.player_id);
                        }
                        SendErros(p, Texto);
                        check = false;
                    }
                    else if (p._rank <= 10)
                    {
                        SendErros(p, "Your rank is lower than the challenge! therefore, you will not be able to participate. ;ç");
                        check = false;
                    }
                    else if (p._money < aposta)
                    {
                        SendErros(p, "Your cash is not suitable for wagering! therefore, you will not be able to participate. ;ç");
                        check = false;
                    }
                }
                check = true;
            }
            catch
            {
                string Texto = "problem in creating the room name incompatible with the event.";
                if (!room.ReadysPvp.Contains(p.player_id))
                {
                    SendErros(Lider, Texto);
                    room.ReadysPvp.Add(p.player_id);
                }
                SendErros(p, Texto);
                check = false;
            }
            return check;
        }
        public static void SendErros(Account p, string texto)
        {
            p.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(texto));
        }
        private void TryBalanceTeams(Room room, bool isBotMode)
        {
            if (room.autobalans != 1 || isBotMode)
                return;
            int TeamIdx = AllUtils.GetBalanceTeamIdx(room, false, -1);
            if (TeamIdx == -1)
                return;
            int[] teamArray = TeamIdx == 1 ? room.RED_TEAM : room.BLUE_TEAM;
            SLOT LastSlot = null;
            for (int i = teamArray.Length - 1; i >= 0; i--)
            {
                SLOT slot = room._slots[teamArray[i]];
                if ((int)slot.state == 8 && room._leader != slot._id)
                {
                    LastSlot = slot;
                    break;
                }
            }
            if (LastSlot != null && room.GetPlayerBySlot(LastSlot, out Account player))
                AllUtils.TryBalancePlayer(room, player, false, ref LastSlot);
        }
    #region @Camp
    private bool ClassicModeCheck(Account p, Room room)
        {
            if (!room.name.ToLower().Contains("@camp") && !room.name.ToLower().Contains("camp") && !room.name.ToLower().Contains("@pvp "))
                return false;
            List<string> blocks = new List<string>();
            PlayerEquipedItems equip = p._equip;
            if (room.name.ToLower().Contains("@camp") || room.name.ToLower().Contains(" @camp") || room.name.ToLower().Contains("@camp ") || room.name.ToLower().Contains("camp") || room.name.ToLower().Contains("@pvp "))
            {
                for (int i = 0; i < ClassicModeManager.itemscamp.Count; i++)
                {
                    int id = ClassicModeManager.itemscamp[i];
                    if (ClassicModeManager.IsBlocked(id, equip._primary, ref blocks, Translation.GetLabel("ClassicCategory1")) ||
                    ClassicModeManager.IsBlocked(id, equip._secondary, ref blocks, Translation.GetLabel("ClassicCategory2")) ||
                    ClassicModeManager.IsBlocked(id, equip._melee, ref blocks, Translation.GetLabel("ClassicCategory3")) ||
                    ClassicModeManager.IsBlocked(id, equip._grenade, ref blocks, Translation.GetLabel("ClassicCategory4")) ||
                    ClassicModeManager.IsBlocked(id, equip._special, ref blocks, Translation.GetLabel("ClassicCategory5")) ||
                    ClassicModeManager.IsBlocked(id, equip._red, ref blocks, Translation.GetLabel("ClassicCategory6")) ||
                    ClassicModeManager.IsBlocked(id, equip._blue, ref blocks, Translation.GetLabel("ClassicCategory7")) ||
                    ClassicModeManager.IsBlocked(id, equip._helmet, ref blocks, Translation.GetLabel("ClassicCategory8")) ||
                    ClassicModeManager.IsBlocked(id, equip._dino, ref blocks, Translation.GetLabel("ClassicCategory9")) ||
                    ClassicModeManager.IsBlocked(id, equip._beret, ref blocks, Translation.GetLabel("ClassicCategory10")))
                        continue;
                }
            }
            if (blocks.Count > 0)
            {
                p.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(Translation.GetLabel("ClassicModeWarn", string.Join(", ", blocks.ToArray()))));
                return true;
            }
            return false;
        }
        private void CheckFlags(Account p, Room room)
        {
            List<string> effectdisable = new List<string>();
         PlayerInventory iv = p._inventory;
            if(iv != null)
            {
                if (room.name.ToLower().Contains("@camp") || room.name.ToLower().Contains(" @camp") || room.name.ToLower().Contains("@camp ") || room.name.ToLower().Contains("camp") || room.name.ToLower().Contains("@pvp "))
                {
                }
            } 
        }
        #endregion
    }
}