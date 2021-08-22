using Core;
using Core.models.enums;
using Core.models.enums.battle;
using Core.models.room;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using Game.Progress;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class VOTEKICK_START_REC : ReceiveGamePacket
    {
        private int slotIdx;
        private uint erro;
        private KickMotive motive;
        public VOTEKICK_START_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            slotIdx = ReadC();
            motive = (KickMotive)ReadC();
            //motive 0=NoManner|1=IllegalProgram|2=Abuse|3=ETC
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                Room room = p?._room;
                if (room == null || room._state != RoomState.Battle || p._slotId == slotIdx)
                    return;
                SLOT slot = room.GetSlot(p._slotId);
                if (slot != null && slot.state == SLOT_STATE.BATTLE && room._slots[slotIdx].state == SLOT_STATE.BATTLE)
                {
                    bool integridy = false;
                    room.GetPlayingPlayers(true, out int redPlayers, out int bluePlayers);
                    if (p._rank < Settings.minRankVote && !p.HaveGMLevel())
                    {
                        if (p.HaveVipTotal())
                        {
                            integridy = true;
                            goto sobrevip;
                        }
                        erro = 0x800010E4;
                    }
                    else if (room.vote.Timer != null) erro = 0x800010E0;
                    else if (slot.NextVoteDate > DateTime.Now) 
                        erro = 0x800010E1;
                    _client.SendPacket(new VOTEKICK_CHECK_PAK(erro));
                    if (erro > 0)
                        return;
                    sobrevip:
                    if(Listcache.VipStartVote.Contains(p.player_id))
                    {
                        string tik = "Você usou seu ticket para fazer uma votação.";
                        p.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, p.GetSessionId(), 0, true, tik));
                        return;
                    }
                    else
                    {
                        if (!p.HaveGMLevel() && p.HaveVipTotal() && !Listcache.VipStartVote.Contains(p.player_id) && integridy)
                            Listcache.VipStartVote.Add(p.player_id);
                        slot.NextVoteDate = DateTime.Now.AddMinutes(1);
                        room.votekick = new VoteKick(slot._id, slotIdx)
                        {
                            motive = motive
                        };
                        ChargeVoteKickArray(room);
                        using (VOTEKICK_START_PAK packet = new VOTEKICK_START_PAK(room.votekick))
                            room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0, p._slotId, slotIdx);
                        AllUtils.LogVotekickStart(room, p, slot);
                        room.StartVote();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info("VOTEKICK_START_REC: " + ex.ToString());
            }
        }
        /// <summary>
        /// Configura a array com os jogadores em partida.
        /// </summary>
        /// <param name="room"></param>
        private void ChargeVoteKickArray(Room room)
        {
            int jogadores = 0;
            do
            {
                room.votekick.TotalArray[jogadores] = (room._slots[jogadores].state == SLOT_STATE.BATTLE);
            }
            while (++jogadores < 16);
        }
    }
}