﻿using Core;
using Core.models.room;
using Game.data.model;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class LOBBY_QUICKJOIN_ROOM_REC : ReceiveGamePacket
    {
        private List<Room> salas = new List<Room>();
        public LOBBY_QUICKJOIN_ROOM_REC(GameClient client, byte[] data)
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
                if (p == null)
                    return;
                if (p != null && p.player_name.Length > 0 && p._room == null &&
                    p._match == null && p.GetChannel(out Channel ch))
                {
                    lock (ch._rooms)
                    {
                        for (int ridx = 0; ridx < ch._rooms.Count; ridx++)
                        {
                            Room room = ch._rooms[ridx];
                            if (room.room_type == 10)
                                continue;
                            if (room.password.Length == 0 && room.limit == 0 && room.special != 5 &&
                                (!room.kickedPlayers.Contains(p.player_id) || p.HaveGMLevel()))
                            {
                                for (int sidx = 0; sidx < 16; sidx++)
                                {
                                    SLOT slot = room._slots[sidx];
                                    if (slot._playerId == 0 && (int)slot.state == 0)
                                    {
                                        salas.Add(room);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (salas.Count == 0)
                    _client.SendPacket(new LOBBY_QUICKJOIN_ROOM_PAK());
                else
                    getRandomRoom(p);
                salas = null;
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[ROOM_JOIN_QUICK_REC] " + ex.ToString());
            }
        }

        private void getRandomRoom(Account player)
        {
            if (player != null)
            {
                Room room = salas[new Random().Next(salas.Count)];
                if (room != null && room.GetLeader(out Account leader) && room.AddPlayer(player) >= 0)
                {
                    player.ResetPages();
                    using (ROOM_GET_SLOTONEINFO_PAK packet = new ROOM_GET_SLOTONEINFO_PAK(player))
                        room.SendPacketToPlayers(packet, player.player_id);
                    _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0, player, leader));
                }
                else _client.SendPacket(new LOBBY_QUICKJOIN_ROOM_PAK());
            }
            else _client.SendPacket(new LOBBY_QUICKJOIN_ROOM_PAK());
        }
    }
}