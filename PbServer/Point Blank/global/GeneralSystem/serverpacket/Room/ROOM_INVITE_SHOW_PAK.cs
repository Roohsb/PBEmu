using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class ROOM_INVITE_SHOW_PAK : SendPacket
    {
        private Account sender;
        private Room room;
        public ROOM_INVITE_SHOW_PAK(Account sender, Room room)
        {
            this.sender = sender;
            this.room = room;
        }

        public override void Write()
        {
            WriteH(2053);
            WriteS(sender.player_name, 33);
            WriteD(room._roomId);
            WriteQ(sender.player_id);
            WriteS(room.password, 4);
        }
    }
}