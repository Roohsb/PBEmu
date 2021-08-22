using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_PRESTARTBATTLE_PAK : SendPacket
    {
        private Account player, leader;
        private Room room;
        private bool isPreparing, LoadHits;
        private uint UniqueRoomId;
        private int gen2;
        public BATTLE_PRESTARTBATTLE_PAK(Account p, Account l, bool more, int gen2)
        {
            player = p;
            leader = l;
            LoadHits = more;
            this.gen2 = gen2;
            room = p._room;
            if (room != null)
            {
                isPreparing = room.IsPreparing();
                UniqueRoomId = room.UniqueRoomId;
            }
        }
        public BATTLE_PRESTARTBATTLE_PAK()
        {
        }
        public override void Write()
        {
            WriteH(3349);
            WriteD(isPreparing);
            if (!isPreparing)
                return;

            WriteD(player._slotId);
            WriteC((byte)Settings.udpType);
            WriteIP(leader.PublicIP);
            WriteH(29890);
            WriteB(leader.LocalIP);
            WriteH(29890);
            WriteC(0);
            WriteIP(player.PublicIP);
            WriteH(29890);
            WriteB(player.LocalIP);
            WriteH(29890);
            WriteC(0);
            WriteIP(room.UDPServer.Connection.Address);
            WriteH((ushort)room.UDPServer.Port);
            WriteD(UniqueRoomId);
            WriteD(gen2);
            if (LoadHits)
            {
                WriteB(new byte[35] //hitparts
				{
					0x20, 0x15, 0x16, 0x17, 
                    0x18, 0x19, 0x11, 0x1B, 
                    0x1C, 0x1D, 0x1A, 0x1F, 
                    0x09, 0x21, 0x0E, 0x1E, 
                    0x01, 0x02, 0x03, 0x04, 
                    0x05, 0x06, 0x07, 0x08, 
                    0x14, 0x0A, 0x0B, 0x0C, 
                    0x0D, 0x22, 0x0F, 0x10, 
                    0x00, 0x12, 0x13
				});
            }
        }
    }
}