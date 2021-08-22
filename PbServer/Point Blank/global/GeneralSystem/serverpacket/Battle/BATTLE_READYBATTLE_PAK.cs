﻿using Core.models.enums;
using Core.models.room;
using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_READYBATTLE_PAK : SendPacket
    {
        private readonly int PlayersCount;
        private Room room;
        private readonly byte[] Data;
        public BATTLE_READYBATTLE_PAK(Room r)
        {
            room = r;
            using SendGPacket pk = new SendGPacket();
            for (int i = 0; i < 16; i++)
            {
                SLOT slot = room._slots[i];
                if ((int)slot.state >= 8 && slot._equip != null)
                {
                    Account player = room.GetPlayerBySlot(slot);
                    if (player != null && player._slotId == i)
                    {
                        WriteSlotInfo(slot, player, LoginManager.Config.ClientVersion, pk);
                        PlayersCount++;
                    }
                }
            }
            Data = pk.mstream.ToArray();
        }
        private void WriteSlotInfo(SLOT s, Account p, string client, SendGPacket pk)
        {
            pk.writeC((byte)s._id);
            pk.writeD(s._equip._red);
            pk.writeD(s._equip._blue);
            pk.writeD(s._equip._helmet);
            pk.writeD(s._equip._beret);
            pk.writeD(s._equip._dino);
            pk.writeD(s._equip._primary);
            pk.writeD(s._equip._secondary);
            pk.writeD(s._equip._melee);
            pk.writeD(s._equip._grenade);
            pk.writeD(s._equip._special);
            pk.writeD(0); 
            if (p != null)
            {
                pk.writeC((byte)p._titles.Equiped1);
                pk.writeC((byte)p._titles.Equiped2);
                pk.writeC((byte)p._titles.Equiped3);
            }
            else
                pk.writeB(new byte[3]);
            if (client == "1.15.42")
                pk.writeD(0);
        }
        public override void Write()
        {
            WriteH(3426);
            WriteH((short)room.mapId);
            WriteC(room.stage4v4);
            WriteC(room.room_type);
            WriteC((byte)PlayersCount);
            WriteB(Data);
        }
    }
}