using Core;
using Core.models.room;
using Core.xml;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class LOBBY_CREATE_ROOM_REC : ReceiveGamePacket
    {
        private uint erro;
        private Room room;
        private Account p;
        public LOBBY_CREATE_ROOM_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            p = _client._player;
            Channel channel = p?.GetChannel();
            try
            {
                if (p == null || channel == null || p.player_name.Length == 0 || p._room != null || p._match != null)
                {
                    erro = 0x80000000;
                    return;
                }
                lock (channel._rooms)
                    for (int i = 0; i < 100; i++)
                    {
                        if (channel.GetRoom(i) == null)
                        {
                            room = new Room(i, channel);
                            ReadD();
                            room.name = ReadS(23);
                            room.mapId = ReadH();
                            room.stage4v4 = ReadC();
                            room.room_type = ReadC();
                            if (room.room_type == 0)
                                break;
                            ReadB(2);
                            room.InitSlotCount(ReadC());
                            ReadC();
                            room.weaponsFlag = ReadC();
                            room.random_map = ReadC();
                            room.special = ReadC();
                            bool isBotMode = room.IsBotMode();
                            if (isBotMode && room._channelType == 4)
                            {
                                erro = 0x8000107D;
                                return;
                            }
                            ReadS(33);
                            room.killtime = ReadC();
                            ReadB(3);
                            room.limit = ReadC();
                            room.seeConf = ReadC();
                            room.autobalans = ReadH();
                            if (channel._type == 4)
                            {
                                room.limit = 1;
                                room.autobalans = 0;
                            }
                            room.password = ReadS(4);
                            if (isBotMode)
                            {
                                room.aiCount = ReadC();
                                room.aiLevel = ReadC();
                            }
                            room.AddPlayer(p);
                            p.ResetPages();
                            channel.AddRoom(room);
                            return;
                        }
                    }
            }
            catch (Exception ex)
            {
                Logger.Error("[ROOM_CREATE_REC] " + ex.ToString());
            }
            erro = 0x80000000;
        }
        public override void Run()
        {
            _client.SendPacket(new LOBBY_CREATE_ROOM_PAK(erro, room, p));
        }
    }
}