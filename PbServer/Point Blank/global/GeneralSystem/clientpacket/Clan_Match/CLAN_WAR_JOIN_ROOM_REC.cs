﻿using Core;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_JOIN_ROOM_REC : ReceiveGamePacket
    {
        private int match, channel, unk;
        public CLAN_WAR_JOIN_ROOM_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            match = ReadD();
            unk = ReadH();
            channel = ReadH();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null || p.clanId == 0 || p._match == null)
                    return;
                Account leader;
                Channel ch;
                if (p != null && p.player_name.Length > 0 && p._room == null && p.GetChannel(out ch))
                {
                    Room room = ch.GetRoom(match);
                    if (room != null && room.GetLeader(out leader))
                    {
                        if (room.password.Length > 0 && !p.HaveGMLevel())
                            _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487749));
                        else if (room.limit == 1 && (int)room._state >= 1)
                            _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487763)); //Entrada proibida com partida em andamento
                        else if (room.kickedPlayers.Contains(p.player_id))
                            _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487756)); //Você foi expulso dessa sala.
                        else if (room.AddPlayer(p, unk) >= 0)
                        {
                            using (ROOM_GET_SLOTONEINFO_PAK packet = new ROOM_GET_SLOTONEINFO_PAK(p))
                                room.SendPacketToPlayers(packet, p.player_id);
                            _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0, p, leader));
                        }
                        else _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487747));
                    }
                    else _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487748));
                }
                else _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(2147487748));
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[CLAN_WAR_JOIN_ROOM_REC] " + ex.ToString());
            }
        }
    }
}