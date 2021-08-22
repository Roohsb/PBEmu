using Core.server;

namespace Game.global.serverpacket
{
    public class BATTLE_HACK_DETECTED_PAK : SendPacket
    {
        private int slotId;
        public BATTLE_HACK_DETECTED_PAK(int slot)
        {
            slotId = slot;
        }

        public override void Write()
        {
            WriteH(3413);
            WriteC((byte)slotId); //Slot do hacker FUNCIONA PORRA NENHUMA
            WriteC(1); //?
            WriteD(1); //?
        }
    }
}