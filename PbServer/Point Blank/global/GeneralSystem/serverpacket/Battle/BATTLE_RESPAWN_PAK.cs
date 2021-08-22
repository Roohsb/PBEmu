using Core.models.room;
using Core.server;
using Game.data.model;
using Game.data.utils;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class BATTLE_RESPAWN_PAK : SendPacket
    {
        private SLOT slot;
        private Room room;
        public BATTLE_RESPAWN_PAK(Room r, SLOT slot)
        {
            this.slot = slot; 
            this.room = r;
        }

        public override void Write()
        {
            WriteH(3338);
            WriteD(slot._id);
            WriteD(room.spawnsCount++); //total number of all players' respawns
            WriteD(++slot.spawnsCount); //total number of current player's respawns
            WriteD(slot._equip._primary);
            WriteD(slot._equip._secondary);
            WriteD(slot._equip._melee);
            WriteD(slot._equip._grenade);
            WriteD(slot._equip._special);
            WriteD(0);
            WriteB(new byte[6] { 100, 100, 100, 100, 100, 1 }); //Durabilidade das armas
            WriteD(slot._equip._red);
            WriteD(slot._equip._blue);
            WriteD(slot._equip._helmet);
            WriteD(slot._equip._beret);
            WriteD(slot._equip._dino);
            if (room.room_type == 7 || room.room_type == 12)
            {
                List<int> pL = AllUtils.getDinossaurs(room, false, slot._id);
                int TRex = pL.Count == 1 || room.room_type == 12 ? 255 : room.TRex;
                WriteC((byte)TRex);
                for (int index = 0; index < pL.Count; index++)
                {
                    int slotId = pL[index];
                    if (slotId != room.TRex && room.room_type == 7 || room.room_type == 12)
                        WriteC((byte)slotId);
                }

                int falta = 8 - pL.Count - (TRex == 255 ? 1 : 0);
                for (int i = 0; i < falta; i++)
                    WriteC(255);
                WriteC(255);
                WriteC(255);
            }
        }
    }
}