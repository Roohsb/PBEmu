using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_GIVEUPBATTLE_PAK : SendPacket
    {
        private Room _r;
        private int _oldLeader;
        public BATTLE_GIVEUPBATTLE_PAK(Room room, int oldLeader)
        {
            _r = room;
            _oldLeader = oldLeader;
        }

        public override void Write()
        {
            WriteH(3347);
            WriteD(_r._leader);
            for (int i = 0; i < 16; i++)
            {
                if (_oldLeader == i)
                    WriteB(new byte[13]);
                else
                {
                    Account p = _r.GetPlayerBySlot(i);
                    if (p != null)
                    {
                        WriteIP(p.PublicIP);
                        WriteH(29890);
                        WriteB(p.LocalIP);
                        WriteH(29890);
                        WriteC(0);
                    }
                    else WriteB(new byte[13]);
                }
            }
        }
    }
}