using Core.models.enums;
using Core.models.room;
using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_DEATH_PAK : SendPacket
    {
        private Room room;
        private FragInfos kills;
        private SLOT killer;
        private bool isBotMode;
        public BATTLE_DEATH_PAK(Room r, FragInfos kills, SLOT killer, bool isBotMode)
        {
            room = r;
            this.kills = kills;
            this.killer = killer;
            this.isBotMode = isBotMode;
        }

        public override void Write()
        {
            WriteH(3355);
            WriteC((byte)kills.killingType);
            WriteC(kills.killsCount);
            WriteC(kills.killerIdx);
            WriteD(kills.weapon);
            WriteT(kills.x);
            WriteT(kills.y);
            WriteT(kills.z);
            WriteC(kills.flag);
            for (int i = 0; i < kills.frags.Count; i++)
            {
                Frag frag = kills.frags[i];
                WriteC(frag.victimWeaponClass);
                WriteC(frag.hitspotInfo);
                WriteH((short)frag.killFlag);
                WriteC(frag.flag);
                WriteT(frag.x);
                WriteT(frag.y);
                WriteT(frag.z);
            }
            WriteH((short)room._redKills);
            WriteH((short)room._redDeaths);
            WriteH((short)room._blueKills);
            WriteH((short)room._blueDeaths);
            for (int i = 0; i < 16; i++)
            {
                SLOT slot = room._slots[i];
                WriteH((short)slot.allKills);
                WriteH((short)slot.allDeaths);
            }
            switch(killer.killsOnLife)
            {
                case 2: WriteC(1); break;
                case 3: WriteC(2); break;
                case int kill when (kill > 3):
                        WriteC(3); break;
                default:
                        WriteC(0); break;
            }
            WriteH((ushort)kills.Score);
            if ((RoomType)room.room_type == RoomType.Boss)
            {
                WriteH((ushort)room.red_dino);
                WriteH((ushort)room.blue_dino);
            }
        }
    }
}