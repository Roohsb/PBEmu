using Core.models.room;
using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_RECORD_PAK : SendPacket
    {
        private Room _r;
        public BATTLE_RECORD_PAK(Room r)
        {
            _r = r;
        }

        public override void Write()
        {
            WriteH(3363);
            WriteH((ushort)_r._redKills);
            WriteH((ushort)_r._redDeaths);
            WriteH((ushort)_r._blueKills);
            WriteH((ushort)_r._blueDeaths);
            for (int i = 0; i < 16; i++)
            {
                SLOT slot = _r._slots[i];
                WriteH((ushort)slot.allKills);
                WriteH((ushort)slot.allDeaths);
            }
        }
    }
}