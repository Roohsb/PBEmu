using Battle.data;
using Battle.data.models;
using System;
using System.Collections.Generic;

namespace Battle.network
{
    public class RoomsManager
    {
        private static readonly List<Room> list = new List<Room>();
        public static int GetGenV(int gen, int type)
        {
            switch(type)
            {
                case 1:  return gen >> 4;
                case 2:  return gen & 15;
                default: return 0;
            }
        }
        public static Room CreateOrGetRoom(uint UniqueRoomId, int gen2)
        {
            lock (list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Room room = list[i];
                    if (room.UniqueRoomId == UniqueRoomId)
                        return room;
                }
                int serverId = AllUtils.GetRoomInfo(UniqueRoomId, 2),
                    channelId = AllUtils.GetRoomInfo(UniqueRoomId, 1),
                    roomId = AllUtils.GetRoomInfo(UniqueRoomId, 0);
                Room roomNew = new Room(serverId)
                {
                    UniqueRoomId = UniqueRoomId,
                    _genId2 = gen2,
                    _roomId = roomId,
                    _channelId = channelId,
                    _mapId = GetGenV(gen2, 1),
                    stageType = GetGenV(gen2, 2)
                };
                list.Add(roomNew);
                return roomNew;
            }
        }
        public static Room GetRoom(uint UniqueRoomId)
        {
            lock (list)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    Room r = list[i];
                    if (r != null && r.UniqueRoomId == UniqueRoomId)
                        return r;
                }
                return null;
            }
        }
        public static Room GetRoom(uint UniqueRoomId, int gen2)
        {
            lock (list)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    Room r = list[i];
                    if (r != null && r.UniqueRoomId == UniqueRoomId && r._genId2 == gen2)
                        return r;
                }
                return null;
            }
        }
        public static bool GetRoom(uint UniqueRoomId, out Room room)
        {
            room = null;
            lock (list)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    Room r = list[i];
                    if (r != null && r.UniqueRoomId == UniqueRoomId)
                    {
                        room = r;
                        return true;
                    }
                }
            }
            return false;
        }
        public static void RemoveRoom(uint UniqueRoomId)
        {
            try
            {
                lock (list)
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (list[i].UniqueRoomId == UniqueRoomId)
                        {
                            list.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning(ex.ToString());
            }
        }
    }
}