using Core;
using Game.data.model;
using Game.data.sync;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class LOBBY_JOIN_ROOM_REC : ReceiveGamePacket
    {
        private int roomId, type;
        private string password;
        public LOBBY_JOIN_ROOM_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            roomId = ReadD();
            password = ReadS(4);
            try { type = ReadC(); } catch { type = 255; }
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p != null && p.player_name.Length > 0 && p._room == null &&
                    p._match == null && p.GetChannel(out Channel ch))
                {
                    Room room = ch.GetRoom(roomId);
                    if (room != null && room.GetLeader(out Account leader))
                    {
                        if (room.room_type == 10)
                            _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0x8000107C)); //Tutorial
                        else if (room.password.Length > 0 && password != room.password && p._rank != 53 && !p.HaveGMLevel() && type != 1)
                            _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0x80001005));
                        else if (room.limit == 1 && (int)room._state >= 1 && !p.HaveGMLevel() || room.special == 5)
                            _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0x80001013)); //Entrada proibida com partida em andamento
                        else if (room.kickedPlayers.Contains(p.player_id) && !p.HaveGMLevel())
                            _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0x8000100C)); //Você foi expulso dessa sala.
                        else if (room.AddPlayer(p) >= 0)
                        {
                            if (p._room != null && p._slotId != -1)
                            {
                                p.ResetPages();
                                using (ROOM_GET_SLOTONEINFO_PAK packet = new ROOM_GET_SLOTONEINFO_PAK(p))
                                    room.SendPacketToPlayers(packet, p.player_id);
                                _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0, p, leader));
                                LoggerPVP(p, room);
                            }
                            else _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0x80001004));
                        }
                        else _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0x80001003)); //SLOTFULL
                    }
                    else _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0x80001004)); //INVALIDROOM
                }
                else _client.SendPacket(new LOBBY_JOIN_ROOM_PAK(0x80001004));
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[ROOM_JOIN_NORMAL_REC] " + ex.ToString());
            }
        }
        public void LoggerPVP(Account p, Room r)
        {
            if (r.name.ToLower().Contains("@pvp "))
                p.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK("Servidor informa: voce esta prestes a jogar um modo pvp, onde se apostam cash. tem certeza? "));
        }
    }
}