using Core;
using Core.models.enums;
using Core.models.room;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class VOTEKICK_UPDATE_REC : ReceiveGamePacket
    {
        private byte type;
        public VOTEKICK_UPDATE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            type = ReadC();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                Room r = p?._room;
                if (r == null || r._state != RoomState.Battle || r.vote.Timer == null ||
                    r.votekick == null || !r.GetSlot(p._slotId, out SLOT slot) || slot.state != SLOT_STATE.BATTLE)
                    return;
                VoteKick vote = r.votekick;
                if (vote._votes.Contains(p._slotId))
                {
                    _client.SendPacket(new VOTEKICK_UPDATE_RESULT_PAK(0x800010F1));
                    return;
                }
                lock (vote._votes)
                {
                    vote._votes.Add(slot._id);
                }
                if (type == 0)
                {
                    vote.kikar++;
                    if (slot._team == vote.victimIdx % 2)
                        vote.allies++;
                    else
                        vote.enemys++;
                }
                else vote.deixar++;
                if (vote._votes.Count >= vote.GetInGamePlayers())
                {
                    r.vote.Timer = null;
                    AllUtils.VotekickResult(r);
                }
                else
                {
                    using VOTEKICK_UPDATE_PAK packet = new VOTEKICK_UPDATE_PAK(vote);
                    r.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Info("VOTEKICK_UPDATE_REC: " + ex.ToString());
            }
        }
    }
}