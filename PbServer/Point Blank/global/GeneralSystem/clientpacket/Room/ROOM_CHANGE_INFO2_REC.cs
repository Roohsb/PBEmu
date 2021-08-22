using Core;
using Core.models.enums;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class ROOM_CHANGE_INFO2_REC : ReceiveGamePacket
    {
        private string leader;
        public ROOM_CHANGE_INFO2_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            try
            {
                Account player = _client._player;
                Room room = player?._room;
                if (room != null && room._leader == player._slotId && room._state == RoomState.Ready)
                {
                    leader = ReadS(33);
                    room.killtime = ReadD();
                    room.limit = ReadC();
                    room.seeConf = ReadC();
                    room.autobalans = ReadH();
                    using ROOM_CHANGE_INFO_PAK packet = new ROOM_CHANGE_INFO_PAK(room, leader);
                    room.SendPacketToPlayers(packet);
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }

        public override void Run()
        {
        }
    }
}