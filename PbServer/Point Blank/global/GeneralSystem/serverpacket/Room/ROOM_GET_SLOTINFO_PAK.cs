using Core;
using Core.models.account.clan;
using Core.models.enums.flags;
using Core.models.room;
using Core.server;
using Game.data.managers;
using Game.data.model;
using System;

namespace Game.global.serverpacket
{
    public class ROOM_GET_SLOTINFO_PAK : SendPacket
    {
        private Room room;
        private bool check = false;
        private CupomEffects cupom;
        public ROOM_GET_SLOTINFO_PAK(Room r)
        {
            room = r;
        }
        public ROOM_GET_SLOTINFO_PAK(Room r, bool check, CupomEffects cupom)
        {
            room = r;
            this.check = check;
            this.cupom = cupom;
        }

        public override void Write()
        {
            try
            {
                if (room == null)
                    return;
                WriteH(3861);
                if (room.GetLeader() == null)
                    room.SetNewLeader(-1, 0, room._leader, false);
                if (room.GetLeader() != null)
                {
                    WriteD(room._leader);
                    for (int i = 0; i < 16; i++)
                    {
                        SLOT slot = room._slots[i];
                        Account pR = room.GetPlayerBySlot(slot);
                        if (pR != null)
                        {
                            Clan clan = ClanManager.GetClan(pR.clanId);
                            WriteC((byte)slot.state);
                            WriteC((byte)pR.GetRank());
                            WriteD(clan._id);
                            WriteD(pR.clanAccess);
                            WriteC((byte)clan._rank);
                            WriteD(clan._logo);
                            WriteC((byte)pR.pc_cafe >= 5 ? (byte)2 : (byte)pR.pc_cafe);
                            WriteC((byte)pR.tourneyLevel);
                            CupomEffects effects = room.CupomEffectsCheck(pR);
                            WriteD((uint)effects);
                            WriteS(clan._name, 17);
                            WriteD(0);
                            WriteC(31);
                        }
                        else
                        {
                            WriteC((byte)slot.state);
                            WriteB(new byte[10]);
                            WriteD(4294967295);
                            WriteB(new byte[28]);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("[ROOM_GET_SLOTINFO_PAK] " + ex.ToString());
            }
        }
    }
}