using Core;
using Core.models.enums;
using Game.data.model;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;
namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_SENDPING_REC : ReceiveGamePacket
    {
        private byte[] slots;
        private int ReadyPlayersCount;
        public BATTLE_SENDPING_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            slots = ReadB(16);
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null)
                    return;
                Room room = player._room;
                if (room != null && room._slots[player._slotId].state >= SLOT_STATE.BATTLE_READY)
                {
                    if ((int)room._state == 5)
                        room._ping = slots[room._leader];
                    using (BATTLE_SENDPING_PAK packet = new BATTLE_SENDPING_PAK())
                    {
                        List<Account> players = room.GetAllPlayers(SLOT_STATE.READY, 1);
                        if (players.Count == 0)
                            return;
                        byte[] data = packet.GetCompleteBytes("BATTLE_SENDPING_REC");
                        for (int i = 0; i < players.Count; i++)
                        {
                            Account pR = players[i];
                            if ((int)room._slots[pR._slotId].state >= 12)
                                pR.SendCompletePacket(data);
                            else
                                ++ReadyPlayersCount;
                        }
                    }
                    if (ReadyPlayersCount == 0)
                        room.SpawnReadyPlayers();
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[BATTLE_SENDPING_REC] " + ex.ToString());
            }
        }
    }
}