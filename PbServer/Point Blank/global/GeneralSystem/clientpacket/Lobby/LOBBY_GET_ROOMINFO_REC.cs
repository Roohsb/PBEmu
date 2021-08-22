using Core;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class LOBBY_GET_ROOMINFO_REC : ReceiveGamePacket
    {
        private int roomId;
        public LOBBY_GET_ROOMINFO_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            roomId = ReadD();
        }

        public override void Run()
        {
            if (_client == null)
                return;
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Channel ch = p.GetChannel();
                if (ch != null)
                {
                    Room room = ch.GetRoom(roomId);
                    if (room != null && room.GetLeader(out Account leader))
                        _client.SendPacket(new LOBBY_GET_ROOMINFO_PAK(room, leader));
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[LOBBY_GET_ROOMINFO_REC] " + ex.ToString());
            }
        }
    }
}