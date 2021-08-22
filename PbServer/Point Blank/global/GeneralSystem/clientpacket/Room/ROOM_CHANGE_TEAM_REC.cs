using Core.models.enums;
using Core.models.room;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Game.global.clientpacket
{
    public class ROOM_CHANGE_TEAM_REC : ReceiveGamePacket
    {
        private List<SLOT_CHANGE> changeList = new List<SLOT_CHANGE>();
        public ROOM_CHANGE_TEAM_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                Room r = p?._room;
                if (r != null && r._leader == p._slotId && r._state == RoomState.Ready && !r.changingSlots)
                {
                    Monitor.Enter(r._slots);
                    r.changingSlots = true;
                    for (int i = 0; i < r.RED_TEAM.Length; i++)
                    {
                        int slotIdx = r.RED_TEAM[i];
                        int NewId = (slotIdx + 1);
                        if (slotIdx == r._leader)
                            r._leader = NewId;
                        else if (NewId == r._leader)
                            r._leader = slotIdx;
                        r.SwitchSlots(changeList, NewId, slotIdx, true);
                    }
                    if (changeList.Count > 0)
                    {
                        using ROOM_CHANGE_SLOTS_PAK packet = new ROOM_CHANGE_SLOTS_PAK(changeList, r._leader, 2);
                        List<Account> list = r.GetAllPlayers();
                        byte[] data = packet.GetCompleteBytes("ROOM_CHANGE_TEAM_REC");
                        for (int i = 0; i < list.Count; i++)
                        {
                            Account ac = list[i];
                            ac._slotId = AllUtils.GetNewSlotId(ac._slotId);
                            SendDebug.SendInfo("[ROOM_CHANGE_TEAM_REC] Jogador '" + ac.player_id + "' '" + ac.player_name + "'; NewSlot: " + ac._slotId);
                            ac.SendCompletePacket(data);
                        }
                    }
                    r.changingSlots = false;
                    Monitor.Exit(r._slots);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ROOM_CHANGE_TEAM_REC: " + ex.ToString());
            }
        }
    }
}