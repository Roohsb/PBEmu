using Core;
using Core.models.room;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.data.chat
{
    public static class GetRoomInfo
    {
        public static string GetSlotStats(string str, Account player, Room room)
        {
            int slotIdx = (int.Parse(str.Substring(5)) - 1);
            string infos = "information:";
            if (room != null)
            {
                SLOT slot = room.GetSlot(slotIdx);
                if (slot != null)
                {
                    infos += "\nIndex: " + slot._id;
                    infos += "\nTeam: " + slot._team;
                    infos += "\nFlag: " + slot._flag;
                    infos += "\nAccountId: " + slot._playerId;
                    infos += "\nState: " + slot.state;
                    infos += "\nMissions: " + ((slot.Missions != null) ? "Valid" : "Null");
                    player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(infos));
                    return "Slot logs successfully generated. [Server]";
                }
                else
                    return "Invalid slot. [Server]";
            }
            else
                return "Invalid room. [Server]";
        }
    }
}