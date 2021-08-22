using Core.server;

namespace Game.global.serverpacket
{
    public class BATTLE_MISSION_BOMB_INSTALL_PAK : SendPacket
    {
        private int _slot;
        private float _x, _y, _z;
        private byte _zone;
        public BATTLE_MISSION_BOMB_INSTALL_PAK(int slot, byte zone, float x, float y, float z)
        {
            _zone = zone;
            _slot = slot;
            _x = x;
            _y = y;
            _z = z;
        }

        public override void Write()
        {
            WriteH(3357);
            WriteD(_slot);
            WriteC(_zone);
            WriteH(42);
            WriteT(_x);
            WriteT(_y);
            WriteT(_z);
        }
    }
}