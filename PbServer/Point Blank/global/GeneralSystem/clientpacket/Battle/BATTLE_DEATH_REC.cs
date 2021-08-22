using Core;
using Core.models.enums;
using Core.models.enums.missions;
using Core.models.enums.room;
using Core.models.room;
using Game.data.model;
using Game.data.sync.client_side;
using Game.data.utils;
using Game.global.serverpacket;
using Game.Progress;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_DEATH_REC : ReceiveGamePacket
    {
        private FragInfos kills = new FragInfos();
        private bool isSuicide;

        public BATTLE_DEATH_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            kills.killingType = (CharaKillType)ReadC();
            kills.killsCount = ReadC();
            kills.killerIdx = ReadC();
            kills.weapon = ReadD();
            kills.x = ReadT();
            kills.y = ReadT();
            kills.z = ReadT();
            kills.flag = ReadC();
            for (int i = 0; i < kills.killsCount; i++)
            {
                Frag frag = new Frag
                {
                    victimWeaponClass = ReadC()
                };
                frag.SetHitspotInfo(ReadC());
                ReadH();
                frag.flag = ReadC();
                frag.x = ReadT();
                frag.y = ReadT();
                frag.z = ReadT();
                kills.frags.Add(frag);
                if (frag.VictimSlot == kills.killerIdx)
                    isSuicide = true;
            }
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null)
                    return;
                Room room = player._room;
                if (room == null || room.round.Timer != null || room._state < RoomState.Battle)
                    return;
                bool isBotMode = room.IsBotMode();
                SLOT killer = room.GetSlot(kills.killerIdx);
                if (killer == null || !isBotMode && (killer.state < SLOT_STATE.BATTLE || killer._id != player._slotId))
                    return;
                if(room.room_type == 2)
                {

                }
                Net_Room_Death.RegistryFragInfos(room, killer, out int score, isBotMode, isSuicide, kills);
                if (isBotMode)
                {
                    killer.Score += killer.killsOnLife + room.IngameAiLevel + score;
                    if (killer.Score > ushort.MaxValue)
                    {
                        killer.Score = ushort.MaxValue;
                        SendDebug.SendInfo("[Player: " + player.player_name + "; Id: " + _client.player_id + "] chegou a pontuação máxima do modo BOT.");
                    }
                    kills.Score = killer.Score;
                }
                else
                {
                    killer.Score += score;
                    AllUtils.CompleteMission(room, player, killer, kills, MISSION_TYPE.NA, 0);
                    kills.Score = score;
                }
                using (BATTLE_DEATH_PAK packet = new BATTLE_DEATH_PAK(room, kills, killer, isBotMode))
                    room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                Net_Room_Death.EndBattleByDeath(room, killer, isBotMode, isSuicide);
            }
            catch (Exception ex)
            {
                Logger.Info("BATTLE_DEATH_REC: " + ex.ToString());
                _client.Close(0, false);
            }
        }
    }
}