using Core.server;

namespace Game.global.serverpacket
{
    public class VOTEKICK_KICK_WARNING_PAK : SendPacket
    {
        public VOTEKICK_KICK_WARNING_PAK()
        {
        }

        public override void Write()
        {
            WriteH(3409);
        }
    }
}