using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_DINO_PLACAR_PAK : SendPacket
    {
        private Room _r;
        public BATTLE_DINO_PLACAR_PAK(Room r)
        {
            _r = r;
        }

        public override void Write()
        {
            WriteH(3389);
            WriteH((ushort)_r.red_dino);
            WriteH((ushort)_r.blue_dino);
        }
    }
}