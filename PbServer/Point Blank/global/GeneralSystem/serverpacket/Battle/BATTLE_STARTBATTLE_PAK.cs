using Core.models.enums.missions;
using Core.models.room;
using Core.server;
using Game.data.model;
using Game.data.utils;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class BATTLE_STARTBATTLE_PAK : SendPacket
    {
        private Room room;
        private SLOT slot;
        private int isBattle, type;
        private List<int> dinos;
        public BATTLE_STARTBATTLE_PAK(SLOT slot, Account pR, List<int> dinos, bool isBotMode, bool type)
        {
            this.slot = slot;
            room = pR._room;
            this.type = type ? 0 : 1;
            this.dinos = dinos;
            if (room != null)
            {
                isBattle = 1;
                if (!isBotMode)
                    AllUtils.CompleteMission(room, pR, slot, type ? MISSION_TYPE.STAGE_ENTER : MISSION_TYPE.STAGE_INTERCEPT, 0);
            }
        }
        public BATTLE_STARTBATTLE_PAK()
        {
        }
        public override void Write()
        {
            WriteH(3334);
            WriteD(isBattle);
            if (isBattle == 1)
            {
                WriteD(slot._id);
                WriteC((byte)type);
                WriteH(AllUtils.getSlotsFlag(room, false, false));
                if (room.room_type == 2 || room.room_type == 3 || room.room_type == 4 || room.room_type == 5) //2-Destruição || 3-Sabotagem || 4-Supressão || 5-Defesa
                {
                    WriteH((ushort)room.red_rounds);
                    WriteH((ushort)room.blue_rounds);
                    if (room.room_type != 3 && room.room_type != 5)
                        WriteH(AllUtils.getSlotsFlag(room, true, false));
                    else
                    {
                        WriteH((ushort)room.Bar1);
                        WriteH((ushort)room.Bar2);
                        for (int i = 0; i < 16; i++)
                            WriteH(room._slots[i].damageBar1);
                        if (room.room_type == 5)
                            for (int i = 0; i < 16; i++)
                                WriteH(room._slots[i].damageBar2);
                    }
                }
                else if (room.room_type == 7 || room.room_type == 12)
                {
                    WriteH((ushort)(room.room_type == 12 ? room._redKills : room.red_dino));
                    WriteH((ushort)(room.room_type == 12 ? room._blueKills : room.blue_dino));
                    WriteC((byte)room.rodada);
                    WriteH(AllUtils.getSlotsFlag(room, false, false)); //usa primeira lógica de slots EF AF (eu entrando 16 pessoas)
                    int TRex = dinos.Count == 1 || room.room_type == 12 ? 255 : room.TRex;
                    WriteC((byte)TRex); //T-Rex || 255 (não tem t-rex)
                    for (int i1 = 0; i1 < dinos.Count; i1++)
                    {
                        int slotId = dinos[i1];
                        if (slotId != room.TRex && room.room_type == 7 || room.room_type == 12)
                            WriteC((byte)slotId);
                    }

                    int falta = 8 - dinos.Count - (TRex == 255 ? 1 : 0);
                    for (int i = 0; i < falta; i++)
                        WriteC(255);
                    WriteC(255);
                    WriteC(255);
                    WriteC(37); //89
                }
            }
        }
    }
}