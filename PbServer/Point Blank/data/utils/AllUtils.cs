﻿using Core;
using Core.DB_Battle;
using Core.managers;
using Core.managers.events;
using Core.models.account;
using Core.models.account.mission;
using Core.models.account.players;
using Core.models.enums;
using Core.models.enums.flags;
using Core.models.enums.friends;
using Core.models.enums.item;
using Core.models.enums.missions;
using Core.models.enums.room;
using Core.models.room;
using Core.server;
using Core.xml;
using Game.data.managers;
using Game.data.model;
using Game.data.sync;
using Game.data.sync.server_side;
using Game.global.serverpacket;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Game.data.utils
{
    public static class AllUtils
    {
        public static int GetKillScore(KillingMessage msg)
        {
            int score = 0;
            switch(msg)
            {
                case KillingMessage kill when (kill == KillingMessage.ChainSlugger || kill == KillingMessage.MassKill || kill == KillingMessage.PiercingShot):
                        score += 6;
                    break;
                case KillingMessage.ChainStopper:
                        score += 8; 
                    break;
                case KillingMessage.Headshot:
                        score += 10;
                    break;
                case KillingMessage.ChainHeadshot:
                        score += 14; 
                    break;
                case KillingMessage.ObjectDefense:
                        score += 7; 
                    break;
                case KillingMessage.Suicide:
                        score += 5; 
                    break;
            }            
            return score;
        }
        public static void CompleteMission(Room room, SLOT slot, FragInfos kills, MISSION_TYPE autoComplete, int moreInfo)
        {
            try
            {
                Account player = room.GetPlayerBySlot(slot);
                if (player == null)
                    return;
                MissionCompleteBase(room, player, slot, kills, autoComplete, moreInfo);
            }
            catch (Exception ex)
            {
                Logger.Error("[AllUtils.CompleteMission1] " + ex.ToString());
            }
        }
        public static void CompleteMission(Room room, SLOT slot, MISSION_TYPE autoComplete, int moreInfo)
        {
            try
            {
                Account player = room.GetPlayerBySlot(slot);
                if (player == null)
                    return;
                MissionCompleteBase(room, player, slot, autoComplete, moreInfo);
            }
            catch (Exception ex)
            {
                Logger.Error("[AllUtils.CompleteMission2] " + ex.ToString());
            }
        }
        public static void CompleteMission(Room room, Account player, SLOT slot, FragInfos kills, MISSION_TYPE autoComplete, int moreInfo)
        {
            MissionCompleteBase(room, player, slot, kills, autoComplete, moreInfo);
        }
        public static void CompleteMission(Room room, Account player, SLOT slot, MISSION_TYPE autoComplete, int moreInfo)
        {
            MissionCompleteBase(room, player, slot, autoComplete, moreInfo);
        }
        private static void MissionCompleteBase(Room room, Account pR, SLOT slot, FragInfos kills, MISSION_TYPE autoComplete, int moreInfo)
        {
            try
            {
                PlayerMissions missions = slot.Missions;
                if (missions == null)
                {
                    Logger.Error(DateTime.Now.ToString("HH:mm:ss") + ": Missions null1! by accountId: " + slot._playerId);
                    return;
                }
                int cmId = missions.GetCurrentMissionId(), cardId = missions.GetCurrentCard();
                if (cmId <= 0 || missions.selectedCard)
                    return;
                List<Card> cards = MissionCardXML.GetCards(cmId, cardId);
                if (cards.Count == 0)
                    return;
                KillingMessage km = kills.GetAllKillFlags();
                byte[] missionArray = missions.GetCurrentMissionList();

                ClassType weaponClass = ComDiv.GetIdClassType(kills.weapon);
                ClassType convertedClass = ConvertWeaponClass(weaponClass);
                int weaponId = ComDiv.GetIdStatics(kills.weapon, 4);
                ClassType moreClass = moreInfo > 0 ? ComDiv.GetIdClassType(moreInfo) : 0;
                ClassType moreConvClass = moreInfo > 0 ? ConvertWeaponClass(moreClass) : 0;
                int moreId = moreInfo > 0 ? ComDiv.GetIdStatics(moreInfo, 4) : 0;

                for (int i = 0; i < cards.Count; i++)
                {
                    Card card = cards[i];
                    int count = 0;
                    if (card._mapId == 0 || card._mapId == room.mapId)
                    {
                        if (kills.frags.Count > 0)
                        {
                            if (card._missionType == MISSION_TYPE.KILL ||
                                card._missionType == MISSION_TYPE.CHAINSTOPPER && km.HasFlag(KillingMessage.ChainStopper) ||
                                card._missionType == MISSION_TYPE.CHAINSLUGGER && km.HasFlag(KillingMessage.ChainSlugger) ||
                                card._missionType == MISSION_TYPE.CHAINKILLER && slot.killsOnLife >= 4 ||
                                card._missionType == MISSION_TYPE.TRIPLE_KILL && slot.killsOnLife == 3 ||
                                card._missionType == MISSION_TYPE.DOUBLE_KILL && slot.killsOnLife == 2 ||
                                card._missionType == MISSION_TYPE.HEADSHOT && (km.HasFlag(KillingMessage.Headshot) || km.HasFlag(KillingMessage.ChainHeadshot)) ||
                                card._missionType == MISSION_TYPE.CHAINHEADSHOT && km.HasFlag(KillingMessage.ChainHeadshot) ||
                                card._missionType == MISSION_TYPE.PIERCING && km.HasFlag(KillingMessage.PiercingShot) ||
                                card._missionType == MISSION_TYPE.MASS_KILL && km.HasFlag(KillingMessage.MassKill) ||
                                card._missionType == MISSION_TYPE.KILL_MAN && (room.room_type == 7 || room.room_type == 12) && (slot._team == 1 && room.rodada == 2 || slot._team == 0 && room.rodada == 1))
                                count = CheckPlayersClass1(card, weaponClass, convertedClass, weaponId, kills);
                            else if (card._missionType == MISSION_TYPE.KILL_WEAPONCLASS ||
                                card._missionType == MISSION_TYPE.DOUBLE_KILL_WEAPONCLASS && slot.killsOnLife == 2 ||
                                card._missionType == MISSION_TYPE.TRIPLE_KILL_WEAPONCLASS && slot.killsOnLife == 3)
                                count = CheckPlayersClass2(card, kills);
                        }
                        else if (card._missionType == MISSION_TYPE.DEATHBLOW && autoComplete == MISSION_TYPE.DEATHBLOW)
                            count = CheckPlayerClass(card, moreClass, moreConvClass, moreId);
                        else if (card._missionType == autoComplete)
                            count = 1;
                    }
                    if (count == 0)
                        continue;

                    int ArrayIdx = card._arrayIdx;
                    if (missionArray[ArrayIdx] + 1 > card._missionLimit)
                        continue;
                    slot.MissionsCompleted = true;
                    missionArray[ArrayIdx] += (byte)count;
                    if (missionArray[ArrayIdx] > card._missionLimit)
                        missionArray[ArrayIdx] = (byte)card._missionLimit;

                    int progress = missionArray[ArrayIdx];
                    pR.SendPacket(new BASE_QUEST_COMPLETE_PAK(progress, card));
                }
            }
            catch (Exception ex)
            { Logger.Error(ex.ToString()); }
        }
        private static void MissionCompleteBase(Room room, Account pR, SLOT slot, MISSION_TYPE autoComplete, int moreInfo)
        {
            try
            {
                PlayerMissions missions = slot.Missions;
                if (missions == null)
                {
                    Logger.Error(DateTime.Now.ToString("HH:mm:ss") + ": Missions null2! by accountId: " + slot._playerId);
                    return;
                }
                int cmId = missions.GetCurrentMissionId(), cardId = missions.GetCurrentCard();
                if (cmId <= 0 || missions.selectedCard)
                    return;
                List<Card> cards = MissionCardXML.GetCards(cmId, cardId);
                if (cards.Count == 0)
                    return;
                byte[] missionArray = missions.GetCurrentMissionList();

                ClassType moreClass = moreInfo > 0 ? ComDiv.GetIdClassType(moreInfo) : 0;
                ClassType moreConvClass = moreInfo > 0 ? ConvertWeaponClass(moreClass) : 0;
                int moreId = moreInfo > 0 ? ComDiv.GetIdStatics(moreInfo, 4) : 0;

                for (int i = 0; i < cards.Count; i++)
                {
                    Card card = cards[i];
                    int count = 0;
                    if (card._mapId == 0 || card._mapId == room.mapId)
                    {
                        if (card._missionType == MISSION_TYPE.DEATHBLOW && autoComplete == MISSION_TYPE.DEATHBLOW)
                            count = CheckPlayerClass(card, moreClass, moreConvClass, moreId);
                        else if (card._missionType == autoComplete)
                            count = 1;
                    }
                    if (count == 0)
                        continue;

                    int ArrayIdx = card._arrayIdx;
                    if (missionArray[ArrayIdx] + 1 > card._missionLimit)
                        continue;
                    slot.MissionsCompleted = true;
                    missionArray[ArrayIdx] += (byte)count;
                    if (missionArray[ArrayIdx] > card._missionLimit)
                        missionArray[ArrayIdx] = (byte)card._missionLimit;

                    pR.SendPacket(new BASE_QUEST_COMPLETE_PAK(missionArray[ArrayIdx], card));
                }
            }
            catch (Exception ex)
            { Logger.Error(ex.ToString()); }
        }
        private static int CheckPlayersClass1(Card card, ClassType weaponClass, ClassType convertedClass, int weaponId, FragInfos infos)
        {
            int count = 0;
            if ((card._weaponReqId == 0 || card._weaponReqId == weaponId) &&
                (card._weaponReq == ClassType.Unknown || card._weaponReq == weaponClass || card._weaponReq == convertedClass))
            {
                for (int i = 0; i < infos.frags.Count; i++)
                {
                    Frag f = infos.frags[i];
                    if (f.VictimSlot % 2 != infos.killerIdx % 2)
                        count++;
                }
            }
            return count;
        }
        private static int CheckPlayersClass2(Card card, FragInfos infos)
        {
            int count = 0;
            for (int i = 0; i < infos.frags.Count; i++)
            {
                Frag f = infos.frags[i];
                if (f.VictimSlot % 2 != infos.killerIdx % 2 &&
                    (card._weaponReq == ClassType.Unknown ||
                    card._weaponReq == (ClassType)f.victimWeaponClass ||
                    card._weaponReq == ConvertWeaponClass((ClassType)f.victimWeaponClass)))
                    count++;
            }
            return count;
        }
        private static int CheckPlayerClass(Card card, ClassType weaponClass, ClassType convertedClass, int weaponId, int killerId, Frag frag)
        {
            if ((card._weaponReqId == 0 || card._weaponReqId == weaponId) &&
                (card._weaponReq == ClassType.Unknown || card._weaponReq == weaponClass || card._weaponReq == convertedClass))
            {
                if (frag.VictimSlot % 2 != killerId % 2)
                    return 1;
            }
            return 0;
        }
        private static int CheckPlayerClass(Card card, ClassType weaponClass, ClassType convertedClass, int weaponId)
        {
            if ((card._weaponReqId == 0 || card._weaponReqId == weaponId) &&
                (card._weaponReq == ClassType.Unknown || card._weaponReq == weaponClass || card._weaponReq == convertedClass))
                return 1;
            return 0;
        }
        private static ClassType ConvertWeaponClass(ClassType weaponClass)
        {
            switch (weaponClass)
            {
                case ClassType.DualSMG: return ClassType.SMG;
                case ClassType.DualHandGun: return ClassType.HandGun;
                case ClassType @class when (@class == ClassType.DualKnife || @class == ClassType.Knuckle): return ClassType.Knife;
                case ClassType.DualShotgun: return ClassType.Shotgun;
                default:
                    break;
            }
            return weaponClass;
        }
        public static TeamResultType GetWinnerTeam(Room room)
        {
            if (room == null)
                return TeamResultType.TeamDraw;
            byte value = 0;
            if (room.room_type == 2 || room.room_type == 3 || room.room_type == 4 || room.room_type == 5)
            {
                if (room.blue_rounds == room.red_rounds) value = 2;
                else if (room.blue_rounds > room.red_rounds) value = 1;
                else if (room.blue_rounds < room.red_rounds) value = 0;
            }
            else if (room.room_type == 7)
            {
                if (room.blue_dino == room.red_dino) value = 2;
                else if (room.blue_dino > room.red_dino) value = 1;
                else if (room.blue_dino < room.red_dino) value = 0;
            }
            else
            {
                if (room._blueKills == room._redKills) value = 2;
                else if (room._blueKills > room._redKills) value = 1;
                else if (room._blueKills < room._redKills) value = 0;
            }
            return (TeamResultType)value;
        }
        public static TeamResultType GetWinnerTeam(Room room, int RedPlayers, int BluePlayers)
        {
            if (room == null)
                return TeamResultType.TeamDraw;
            byte value = 2;
            if (RedPlayers == 0) value = 1;
            else if (BluePlayers == 0) value = 0;
            return (TeamResultType)value;
        }
        public static void endMatchMission(Room room, Account player, SLOT slot, TeamResultType winnerTeam)
        {
            if (winnerTeam != TeamResultType.TeamDraw)
                CompleteMission(room, player, slot, slot._team == (int)winnerTeam ? MISSION_TYPE.WIN : MISSION_TYPE.DEFEAT, 0);
        }
        public static void updateMatchCount(bool WonTheMatch, Account p, int winnerTeam, DBQuery query)
        {
            if (winnerTeam == 2)
                query.AddQuery("fights_draw", ++p._statistic.fights_draw);
            else if (WonTheMatch)
                query.AddQuery("fights_win", ++p._statistic.fights_win);
            else
                query.AddQuery("fights_lost", ++p._statistic.fights_lost);
            query.AddQuery("fights", ++p._statistic.fights);
            query.AddQuery("totalfights_count", ++p._statistic.totalfights_count);
        }

        public static void UpdateDailyRecord(bool WonTheMatch, Account p, int winnerTeam, DBQuery query)
        {
            if (winnerTeam == 2)
            {
                query.AddQuery("draws", ++p.Daily.Draws);
            }
            else if (WonTheMatch)
            {
                query.AddQuery("wins", ++p.Daily.Wins);
            }
            else
            {
                query.AddQuery("loses", ++p.Daily.Loses);
            }
            query.AddQuery("total", ++p.Daily.Total);
        }
        public static void GenerateMissionAwards(Account player, DBQuery query)
        {
            PlayerMissions missions = player._mission;
            int activeM = missions.actualMission, missionId = missions.GetCurrentMissionId(), cardId = missions.GetCurrentCard();
            if (missionId <= 0 || missions.selectedCard)
                return;
            int CompletedLastCardCount = 0, allCompletedCount = 0;
            byte[] missionL = missions.GetCurrentMissionList();
            List<Card> cards = MissionCardXML.GetCards(missionId, -1);
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (missionL[card._arrayIdx] >= card._missionLimit)
                {
                    allCompletedCount++;
                    if (card._cardBasicId == cardId)
                        CompletedLastCardCount++;
                }
            }
            if (allCompletedCount >= 40)
            {
                int blueOrder = player.blue_order, brooch = player.brooch, medal = player.medal, insignia = player.insignia;
                CardAwards c = MissionCardXML.GetAward(missionId, cardId);
                if (c != null)
                {
                    player.brooch += c._brooch;
                    player.medal += c._medal;
                    player.insignia += c._insignia;
                    player._gp += c._gp;
                    player._exp += c._exp;
                }
                MisAwards m = MissionAwardsJSON.getAward(missionId);
                if (m != null)
                {
                    player.blue_order += m._blueOrder;
                    player._exp += m._exp;
                    player._gp += m._gp;
                }
                List<ItemsModel> items = MissionCardXML.GetMissionAwards(missionId);
                if (items.Count > 0)
                    player.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, player, items));
                player.SendPacket(new BASE_QUEST_ALERT_PAK(273, 4, player));
                if (player.brooch != brooch)
                    query.AddQuery("brooch", player.brooch);
                if (player.insignia != insignia)
                    query.AddQuery("insignia", player.insignia);
                if (player.medal != medal)
                    query.AddQuery("medal", player.medal);
                if (player.blue_order != blueOrder)
                    query.AddQuery("blue_order", player.blue_order);
                query.AddQuery("mission_id" + (activeM + 1), 0);
                ComDiv.UpdateDB("player_missions", "owner_id", player.player_id, new string[] { "card" + (activeM + 1), "mission" + (activeM + 1) }, 0, new byte[0]);
                switch (activeM)
                {
                    case 0:
                        {
                            missions.mission1 = 0;
                            missions.card1 = 0;
                            missions.list1 = new byte[40]; break;
                        }
                    case 1:
                        {
                            missions.mission1 = 0;
                            missions.card1 = 0;
                            missions.list1 = new byte[40]; break;
                        }
                    case 2:
                        {
                            missions.mission3 = 0;
                            missions.card3 = 0;
                            missions.list3 = new byte[40]; break;
                        }
                    case 3:
                        {
                            missions.mission4 = 0;
                            missions.card4 = 0;
                            missions.list4 = new byte[40]; 
                            if (player._event != null)
                            {
                                player._event.LastQuestFinish = 1;
                                ComDiv.UpdateDB("player_events", "last_quest_finish", 1, "player_id", player.player_id);
                            }
                            break;
                        }
                }
            }
            else if (CompletedLastCardCount == 4 && !missions.selectedCard)
            {
                CardAwards c = MissionCardXML.GetAward(missionId, cardId);
                if (c != null)
                {
                    int brooch = player.brooch, medal = player.medal, insignia = player.insignia;
                    player.brooch += c._brooch;
                    player.medal += c._medal;
                    player.insignia += c._insignia;
                    player._gp += c._gp;
                    player._exp += c._exp;
                    if (player.brooch != brooch)
                        query.AddQuery("brooch", player.brooch);
                    if (player.insignia != insignia)
                        query.AddQuery("insignia", player.insignia);
                    if (player.medal != medal)
                        query.AddQuery("medal", player.medal);
                }
                missions.selectedCard = true;
                player.SendPacket(new BASE_QUEST_ALERT_PAK(1, 1, player));
            }
        }
        /// <summary>
        /// Reseta as informações básicas do slot. Se o estado do slot for maior que READY, ele volta para NORMAL. É reiniciado o histórico de missões da partida.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="slot">Slot do jogador</param>
        /// <param name="updateInfo">Atualizar informações dos slots?</param>
        public static void ResetSlotInfo(Room room, SLOT slot, bool updateInfo)
        {
            if ((int)slot.state >= 9)
            {
                room.ChangeSlotState(slot, SLOT_STATE.NORMAL, updateInfo);
                slot.ResetSlot();
            }
        }
        public static void VotekickResult(Room room)
        {
            VoteKick votekick = room.votekick;
            if (votekick != null)
            {
                int Count = votekick.GetInGamePlayers();
                if (votekick.kikar > votekick.deixar && votekick.enemys > 0 && votekick.allies > 0 &&
                    votekick._votes.Count >= Count / 2)
                {
                    Account j = room.GetPlayerBySlot(votekick.victimIdx);
                    if (j != null)
                    {
                        j.SendPacket(new VOTEKICK_KICK_WARNING_PAK());
                        room.kickedPlayers.Add(j.player_id);
                        room.RemovePlayer(j, true, 2);
                    }
                }
                uint erro = 0;
                if (votekick.allies == 0) erro = 2147488001;
                else if (votekick.enemys == 0) erro = 2147488002;
                else if (votekick.deixar < votekick.kikar ||
                    votekick._votes.Count < Count / 2) erro = 2147488000;

                using (VOTEKICK_RESULT_PAK packet = new VOTEKICK_RESULT_PAK(erro, votekick))
                    room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                LogVotekickResult(room);
            }
            room.votekick = null;
        }
        public static void resetBattleInfo(Room room)
        {
            LogRoomResult(room);
            for (int  i = 0; i < 16; ++i)
            {
                SLOT slot = room._slots[i];
                if ((int)slot.state >= 9)
                {
                    slot.state = SLOT_STATE.NORMAL;
                    slot.ResetSlot();
                }
            }
            room.blockedClan = false;
            room.rodada = 1;
            room.spawnsCount = 0;
            room._redKills = 0;
            room._redDeaths = 0;
            room._blueKills = 0;
            room._blueDeaths = 0;
            room.red_dino = 0;
            room.blue_dino = 0;
            room.red_rounds = 0;
            room.blue_rounds = 0;
            room.BattleStart = new DateTime();
            room._timeRoom = 0;
            room.Bar1 = 0;
            room.Bar2 = 0;
            room.swapRound = false;
            room.IngameAiLevel = 0;
            room._state = RoomState.Ready;
            room.UpdateRoomInfo();
            room.votekick = null;
            room.UDPServer = null;
            room.isRoomMute = false;
            room.ReadysPvp.Clear();
            if (room.round.Timer != null)
                room.round.Timer = null;
            if (room.vote.Timer != null)
                room.vote.Timer = null;
            if (room.bomba.Timer != null)
                room.bomba.Timer = null;
            room.UpdateSlotsInfo();
        }
        /// <summary>
        /// Gera uma lista de jogadores como dinossauros. 
        /// <para>É possível indicar um novo T-Rex.</para>
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="forceNewTRex">Gerar um novo T-Rex a força</param>
        /// <param name="forceRexIdx">Indicar um slot para ser o T-Rex. (-2 = Aleatório)</param>
        /// <returns></returns>
        public static List<int> getDinossaurs(Room room, bool forceNewTRex, int forceRexIdx)
        {
            List<int> dinos = new List<int>();
            if (room.room_type == 7 || room.room_type == 12)
            {
                int teamIdx = room.rodada == 1 ? 0 : 1;
                int[] array = room.GetTeamArray(teamIdx);
                for (int i = 0; i < array.Length; i++)
                {
                    int slotIdx = array[i];
                    SLOT slot = room._slots[slotIdx];
                    if ((int)slot.state == 13 && !slot.specGM)
                        dinos.Add(slotIdx);
                }
                if ((room.TRex == -1 || (int)room._slots[room.TRex].state <= 12 || forceNewTRex) && dinos.Count > 1 && room.room_type == 7)
                {
                    SendDebug.SendInfo("[" + DateTime.Now.ToString("HH:mm") + "] trex: " + room.TRex);
                    if (forceRexIdx >= 0 && dinos.Contains(forceRexIdx))
                        room.TRex = forceRexIdx;
                    else if (forceRexIdx == -2)
                        room.TRex = dinos[new Random().Next(0, dinos.Count)];
                    SendDebug.SendInfo("[" + DateTime.Now.ToString("HH:mm") + "] forceRexIdx: " + forceRexIdx + "; force: " + forceNewTRex + "; teamIdx: " + teamIdx + "; trex: " + room.TRex);
                }
            }
            return dinos;
        }
        /// <summary>
        /// Checa se a partida tem jogadores suficientes para permanecer ativa.
        /// Não funciona em modo contra I.A.
        /// Funciona tanto para partida em andamento, quanto para o preparatório.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="isBotMode">É modo contra I.A?</param>
        public static void BattleEndPlayersCount(Room room, bool isBotMode)
        {
            if (room == null || isBotMode || !room.IsPreparing())
                return;
            int blue = 0, red = 0, blue2 = 0, red2 = 0;
            for (int i = 0; i < 16; i++)
            {
                SLOT slot = room._slots[i];
                if ((int)slot.state == 13)
                {
                    if (slot._team == 0) red++;
                    else blue++;
                }
                else if ((int)slot.state >= 9)
                {
                    if (slot._team == 0) red2++;
                    else blue2++;
                }
            }
            if ((red == 0 || blue == 0) && (int)room._state == 5 ||
                (red2 == 0 || blue2 == 0) && (int)room._state <= 4)
                EndBattle(room, isBotMode);
        }
        /// <summary>
        /// Calcula os resultados da partida e envia o placar a todos os jogadores com estado acima de READY.
        /// </summary>
        /// <param name="room">Sala</param>
        public static void EndBattle(Room room)
        {
            EndBattle(room, room.IsBotMode());
        }
        /// <summary>
        /// Calcula os resultados da partida e envia o placar a todos os jogadores com estado acima de READY.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="isBotMode">É modo contra I.A?</param>
        public static void EndBattle(Room room, bool isBotMode)
        {
            EndBattle(room, isBotMode, GetWinnerTeam(room));
        }
        /// <summary>
        /// Não calcula os resultados da partida e envia o placar a todos os jogadores com estado acima de READY.
        /// </summary>
        /// <param name="room">Sala</param>
        public static void EndBattleNoPoints(Room room)
        {
            List<Account> players = room.GetAllPlayers(SLOT_STATE.READY, 1);
            if (players.Count == 0)
                goto EndLabel;
            GetBattleResult(room, out ushort missionCompletes, out ushort inBattle, out byte[]  a1);
            bool isBotMode = room.IsBotMode();
            for (int i = 0; i < players.Count; i++)
            {
                Account pR = players[i];
                pR.SendPacket(new BATTLE_ENDBATTLE_PAK(pR, TeamResultType.TeamDraw, inBattle, missionCompletes, isBotMode, a1));
            }

        EndLabel:
            resetBattleInfo(room);
        }
        /// <summary>
        /// Calcula os resultados da partida e envia o placar a todos os jogadores com estado acima de READY.
        /// </summary>
        /// <param name="room">Room</param>
        /// <param name="isBotMode">É modo contra I.A?</param>
        /// <param name="winnerTeam">Time vencedor</param>
        public static void EndBattle(Room room, bool isBotMode, TeamResultType winnerTeam)
        {
            List<Account> players = room.GetAllPlayers(SLOT_STATE.READY, 1);
            if (players.Count == 0)
                goto EndLabel;

            room.CalculateResult(winnerTeam, isBotMode);
            GetBattleResult(room, out ushort missionCompletes, out ushort inBattle, out byte[] a1);
            for (int i = 0; i < players.Count; i++)
            {
                Account pR = players[i];
                pR.SendPacket(new BATTLE_ENDBATTLE_PAK(pR, winnerTeam, inBattle, missionCompletes, isBotMode, a1));
            }

        EndLabel:
            resetBattleInfo(room);
        }
        public static int percentage(int total, int percent) => total * percent / 100; 
        /// <summary>
        /// Analisa se a partida deve ser finalizada, ou reiniciada, seguindo alguns parâmetros. Haverá mensagem do motivo da vitória (Time morto). Haverá checagem para desativação da bomba.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="winner">Time vencedor</param>
        /// <param name="forceRestart">Reiniciar uma rodada, de maneira forçada, caso necessário</param>
        public static void BattleEndRound(Room room, int winner, bool forceRestart)
        {
            if(room != null)
            {
                int roundsByMask = room.GetRoundsByMask();
                if (room.red_rounds >= roundsByMask || room.blue_rounds >= roundsByMask)
                {
                    room.StopBomb();
                    using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(room, winner, RoundEndType.AllDeath))
                        room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                    EndBattle(room, room.IsBotMode(), (TeamResultType)winner);
                }
                else if (!room.C4_actived || forceRestart)
                {
                    room.StopBomb();
                    room.rodada++;
                    Game_SyncNet.SendUDPRoundSync(room);
                    using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(room, winner, RoundEndType.AllDeath))
                        room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                    room.RoundRestart();
                }
            }
        }
        /// <summary>
        /// Analisa se a partida deve ser finalizada, ou reiniciada. Haverá mensagem do motivo da vitória e é possível personalizá-la. Haverá checagem para desativação da bomba.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="winner">Time vencedor</param>
        /// <param name="motive">Motivo da vitória</param>
        public static void BattleEndRound(Room room, int winner, RoundEndType motive)
        {
            using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(room, winner, motive))
                room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
            room.StopBomb();
            int roundsByMask = room.GetRoundsByMask();
            if (room.red_rounds >= roundsByMask || room.blue_rounds >= roundsByMask)
                EndBattle(room, room.IsBotMode(), (TeamResultType)winner);
            else
            {
                room.rodada++;
                Game_SyncNet.SendUDPRoundSync(room);
                room.RoundRestart();
            }
        }
        public static int AddFriend(Account owner, Account friend, int state)
        {
            if (owner == null || friend == null)
                return -1;
            try
            {
                Friend f = owner.FriendSystem.GetFriend(friend.player_id);
                if (f == null)
                {
                    using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                    {
                        SqlCommand command = connection.CreateCommand();
                        connection.Open();
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@friend", friend.player_id);
                        command.Parameters.AddWithValue("@owner", owner.player_id);
                        command.Parameters.AddWithValue("@state", state);
                        command.CommandText = "INSERT INTO friends(friend_id,owner_id,state)VALUES(@friend,@owner,@state)";
                        command.ExecuteNonQuery();
                        command.Dispose();
                        connection.Dispose();
                        connection.Close();
                    }
                    lock (owner.FriendSystem._friends)
                    {
                        Friend friendM = new Friend(friend.player_id, friend._rank, friend.player_name, friend._isOnline, friend._status)
                        {
                            state = state,
                            removed = false
                        };
                        owner.FriendSystem._friends.Add(friendM);
                        SEND_FRIENDS_INFOS.Load(owner, friendM, 0);
                    }
                    return 0;
                }
                else
                {
                    if (f.removed)
                    {
                        f.removed = false;
                        PlayerManager.UpdateFriendBlock(owner.player_id, f);
                        SEND_FRIENDS_INFOS.Load(owner, f, 1);
                    }
                    else
                        SendDebug.SendInfo("Other.");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Logger.Info("[AllUtils.AddFriend] " + ex.ToString());
                return -1;
            }
        }
        public static void SyncPlayerToFriends(Account p, bool all)
        {
            try
            {
                if (p == null || p.FriendSystem._friends.Count == 0)
                    return;
                PlayerInfo info = new PlayerInfo(p.player_id, p._rank, p.player_name, p._isOnline, p._status);
                for (int i = 0; i < p.FriendSystem._friends.Count; i++)
                {
                    Friend friend = p.FriendSystem._friends[i];
                    if (all || friend.state == 0 && !friend.removed)
                    {
                        Account f1 = AccountManager.GetAccount(friend.player_id, 32);
                        if (f1 != null)
                        {
                            Friend f2 = f1.FriendSystem.GetFriend(p.player_id, out int idx);
                            if (f2 != null)
                            {
                                f2.player = info;
                                f1.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeState.Update, f2, idx), false);
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error updating the room for friends.");
            }
            
        }
        public static void SyncPlayerToClanMembers(Account player)
        {
            if (player == null || player.clanId <= 0)
                return;
            using CLAN_MEMBER_INFO_CHANGE_PAK packet = new CLAN_MEMBER_INFO_CHANGE_PAK(player);
            ClanManager.SendPacket(packet, player.clanId, player.player_id, true, true);
        }
        public static void UpdateSlotEquips(Account p)
        {
            Room room = p._room;
            if (room != null)
                UpdateSlotEquips(p, room);
        }
        public static void UpdateSlotEquips(Account p, Room room)
        {
            if (room.GetSlot(p._slotId, out SLOT slot))
                slot._equip = p._equip;
        }
        /// <summary>
        /// Identifica os jogadores que estão dentro da partida. Resultado no padrão Flag.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="onlyNoSpectators">Somente jogadores que não estão espectando a partida</param>
        /// <param name="missionSuccess">Somente jogadores que conseguiram completar no mínimo 1 missão</param>
        /// <returns></returns>
        public static ushort getSlotsFlag(Room room, bool onlyNoSpectators, bool missionSuccess)
        {
            if (room == null)
                return 0;
            int flags = 0;
            for (int i = 0; i < 16; i++)
            {
                SLOT slot = room._slots[i];
                if ((int)slot.state >= 9 && (missionSuccess && slot.MissionsCompleted || !missionSuccess && (!onlyNoSpectators || !slot.espectador)))
                    flags += slot._flag;
            }
            return (ushort)flags;
        }
        /// <summary>
        /// Gera uma array de bytes com o resultado da partida.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <param name="result1">Jogadores que completaram missões</param>
        /// <param name="result2">Jogadores na partida</param>
        /// <param name="data">Resultado</param>
        public static void GetBattleResult(Room room, out ushort result1, out ushort result2, out byte[] data)
        {
            result1 = 0;
            result2 = 0;
            data = new byte[144];
            if (room == null)
                return;
            using SendGPacket p1 = new SendGPacket();
            for (int i = 0; i < 16; i++)
            {
                SLOT slot = room._slots[i];
                if ((int)slot.state >= 9)
                {
                    ushort flag = (ushort)slot._flag;
                    if (slot.MissionsCompleted)
                        result1 += flag;
                    result2 += flag;
                }
                p1.writeH(0 + (i * 2), (ushort)slot.exp);
                p1.writeH(32 + (i * 2), (ushort)slot.gp);
                p1.writeH(64 + (i * 2), (ushort)slot.BonusXP);
                p1.writeH(96 + (i * 2), (ushort)slot.BonusGP);
                p1.writeC(128 + (i), (byte)slot.bonusFlags);
            }
            data = p1.mstream.ToArray();
        }
        public static void DiscountPlayerItems(SLOT slot, Account p)
        {
            uint data_atual = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
            bool loadCode = false, updEffect = false;
            List<ItemsModel> updateList = new List<ItemsModel>();
            List<object> removeList = new List<object>();
            PlayerBonus bonus = p._bonus;
            int bonuses = bonus != null ? bonus.bonuses : 0, 
                freepass = bonus != null ? bonus.freepass : 0;
            lock (p._inventory._items)
            {
                for (int i = 0; i < p._inventory._items.Count; i++)
                {
                    ItemsModel item = p._inventory._items[i];
                    if (item._equip == 1 && slot.armas_usadas.Contains(item._id) && !slot.specGM)
                    {
                        if (--item._count < 1)
                        {
                            removeList.Add(item._objId);
                            p._inventory._items.RemoveAt(i--);
                        }
                        else
                        {
                            updateList.Add(item);
                            ComDiv.UpdateDB("player_items", "count", (long)item._count, "object_id", item._objId, "owner_id", p.player_id);
                        }
                    }
                    else if (item._count <= data_atual & item._equip == 2)
                    {
                        if (item._category == 3 && ComDiv.GetIdStatics(item._id, 1) == 12)
                        {
                            if (bonus == null)
                                continue;
                            bool changed = bonus.RemoveBonuses(item._id);
                            if (!changed)
                            {
                                if (item._id == 1200014000)
                                {
                                    ComDiv.UpdateDB("player_bonus", "sightcolor", 4, "player_id", p.player_id);
                                    bonus.sightColor = 4;
                                    loadCode = true;
                                }
                                else if (item._id == 1200009000)
                                {
                                    ComDiv.UpdateDB("player_bonus", "fakerank", 55, "player_id", p.player_id);
                                    bonus.fakeRank = 55;
                                    loadCode = true;
                                }
                            }
                            else if (item._id == 1200006000)
                            {
                                ComDiv.UpdateDB("accounts", "name_color", 0, "player_id", p.player_id);
                                p.name_color = 0;
                                if (p._room != null)
                                    using (ROOM_GET_NICKNAME_PAK packet = new ROOM_GET_NICKNAME_PAK(slot._id, p.player_name, p.name_color))
                                        p._room.SendPacketToPlayers(packet);
                            }
                            CupomFlag cupom = CupomEffectManagerJSON.GetCupomEffect(item._id);
                            if (cupom != null && cupom.EffectFlag > 0 && p.effects.HasFlag(cupom.EffectFlag))
                            {
                                p.effects -= cupom.EffectFlag;
                                updEffect = true;
                            }
                        }
                        removeList.Add(item._objId);
                        p._inventory._items.RemoveAt(i--);
                    }
                }
            }
            if (bonus != null && (bonus.bonuses != bonuses || bonus.freepass != freepass))
                PlayerManager.UpdatePlayerBonus(p.player_id, bonus.bonuses, bonus.freepass);
            if (p.effects < 0) p.effects = 0;
            if (updEffect)
                PlayerManager.UpdateCupomEffects(p.player_id, p.effects);

            if (updateList.Count > 0)
                p.SendPacket(new INVENTORY_ITEM_CREATE_PAK(2, p, updateList));
            for (int i = 0; i < removeList.Count; i++)
            {
                long objId = (long)removeList[i];
                p.SendPacket(new INVENTORY_ITEM_EXCLUDE_PAK(1, objId));
            }
            ComDiv.DeleteDB("player_items", "object_id", removeList.ToArray(), "owner_id", p.player_id);
            if (loadCode)
                p.SendPacket(new BASE_USER_EFFECTS_PAK(0, p._bonus));
            int type = PlayerManager.CheckEquipedItems(p._equip, p._inventory._items);
            if (type > 0)
            {
                p.SendPacket(new INVENTORY_EQUIPED_ITEMS_PAK(p, type));
                slot._equip = p._equip;
            }
        }
        public static void TryBalancePlayer(Room room, Account player, bool inBattle, ref SLOT mySlot)
        {
            SLOT oldSlot = room.GetSlot(player._slotId);
            if (oldSlot == null)
                return;
            if (room._leader == player._slotId)
                return;
            if (player.IsGM())
                return;
            int PlayerTeamIdx = oldSlot._team;
            int TeamIdx = GetBalanceTeamIdx(room, inBattle, PlayerTeamIdx);
            if (PlayerTeamIdx == TeamIdx || TeamIdx == -1)
                return;
            SLOT newSlot = null;
            int[] teamArray = PlayerTeamIdx == 1 ? room.RED_TEAM : room.BLUE_TEAM;
            for (int i = 0; i < teamArray.Length; i++)
            {
                SLOT slot = room._slots[teamArray[i]];
                if ((int)slot.state != 1 && slot._playerId == 0)
                {
                    newSlot = slot;
                    break;
                }
            }
            if (newSlot == null)
                return;

            List<SLOT_CHANGE> changeList = new List<SLOT_CHANGE>();
            lock (room._slots)
                room.SwitchSlots(changeList, newSlot._id, oldSlot._id, false);
            if (changeList.Count > 0)
            {
                player._slotId = oldSlot._id;
                mySlot = oldSlot;
                using (ROOM_CHANGE_SLOTS_PAK packet = new ROOM_CHANGE_SLOTS_PAK(changeList, room._leader, 1))
                    room.SendPacketToPlayers(packet);
                room.UpdateSlotsInfo();
            }
        }
        public static int GetBalanceTeamIdx(Room room, bool inBattle, int PlayerTeamIdx)
        {
            int redPlayers = inBattle && PlayerTeamIdx == 0 ? 1 : 0,
                bluePlayers = inBattle && PlayerTeamIdx == 1 ? 1 : 0;
            for (int i = 0; i < 16; i++)
            {
                SLOT slot = room._slots[i];//state == 8
                if ((int)slot.state == 7 && !inBattle || (int)slot.state >= 9 && inBattle)
                    if (slot._team == 0) redPlayers++;
                    else bluePlayers++;
            }
            return redPlayers + 1 < bluePlayers ? 0 : bluePlayers + 1 < redPlayers + 1 ? 1 : -1;
        }
        public static int GetNewSlotId(int slotIdx) => slotIdx % 2 == 0 ? (slotIdx + 1) : (slotIdx - 1);
        public static void GetXmasReward(Account p)
        {
            EventXmasModel ev = EventXmasSyncer.GetRunningEvent();
            if (ev == null)
                return;
            PlayerEvent pev = p._event;
            uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
            if (pev != null && !(pev.LastXmasRewardDate > ev.startDate && pev.LastXmasRewardDate <= ev.endDate) && ComDiv.UpdateDB("player_events", "last_xmas_reward_date", (long)date, "player_id", p.player_id))
            {
                pev.LastXmasRewardDate = date;
                p.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, p, new ItemsModel(702001024, 1, "Alcaçuz (Evento)", 1, 100)));
            }
        }
        /// <summary>
        /// Tenta recomeçar/terminar a partida, de acordo com a quantidade de jogadores vivos. (Somente para os modos Destruição e Supressão)
        /// </summary>
        /// <param name="room">Sala</param>
        public static void BattleEndRoundPlayersCount(Room room)
        {
            if (room.round.Timer == null && (room.room_type == 2 || room.room_type == 4)) //Destruição e Supressão
            {
                room.GetPlayingPlayers(true, out int  redPlayers, out int bluePlayers, out int redDeaths, out int blueDeaths);
                if (redDeaths == redPlayers)
                {
                    if (!room.C4_actived)
                        room.blue_rounds++;
                    BattleEndRound(room, 1, false);
                }
                else if (blueDeaths == bluePlayers)
                {
                    room.red_rounds++;
                    BattleEndRound(room, 0, true);
                }
            }
        }
        public static void BattleEndKills(Room room)
        {
            BaseEndByKills(room, room.IsBotMode());
        }
        public static void BattleEndKills(Room room, bool isBotMode)
        {
            BaseEndByKills(room, isBotMode);
        }
        private static void BaseEndByKills(Room room, bool isBotMode)
        {
            int killsByMask = room.GetKillsByMask();
            if (room._redKills < killsByMask && room._blueKills < killsByMask)
                return;
            List<Account> players = room.GetAllPlayers(SLOT_STATE.READY, 1);
            if (players.Count == 0)
                goto EndLabel;
            TeamResultType winner = GetWinnerTeam(room);
            room.CalculateResult(winner, isBotMode);

            GetBattleResult(room, out ushort missionCompletes, out ushort inBattle, out byte[]  a1);
            
            using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(room, winner, RoundEndType.TimeOut))
            {
                byte[] data = packet.GetCompleteBytes("AllUtils.BaseEndByKills");
                for (int i = 0; i < players.Count; i++)
                {
                    Account pR = players[i];
                    SLOT slot = room.GetSlot(pR._slotId);
                    if (slot != null)
                    {
                        if (slot.state == SLOT_STATE.BATTLE)
                            pR.SendCompletePacket(data);
                        pR.SendPacket(new BATTLE_ENDBATTLE_PAK(pR, winner, inBattle, missionCompletes, isBotMode, a1));
                    }
                }
            }
        EndLabel:
            resetBattleInfo(room);
        }
        public static bool CheckClanMatchRestrict(Room room)
        {
            if (room._channelType == 4)
            {
                SortedList<int, ClanModel> clans = GetClanListMatchPlayers(room);
                for (int i = 0; i < clans.Values.Count; i++)
                {
                    ClanModel cm = clans.Values[i];
                    if (cm.RedPlayers >= 1 && cm.BluePlayers >= 1)
                    {
                        room.blockedClan = true;
                        SendDebug.SendInfo("XP canceled in clanfronto [Room: " + room._roomId + "; Canal: " + room._channelId + "; Clã: " + cm.clanId + "]");
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Checa se só tem 2 clãs na partida de clãs.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <returns></returns>
        public static bool Have2ClansToClanMatch(Room room) => (GetClanListMatchPlayers(room).Count == 2);
        /// <summary>
        /// Checa se os 2 times possuem 4 jogadores de mesmos clãs.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <returns></returns>
        public static bool HavePlayersToClanMatch(Room room)
        {
            SortedList<int, ClanModel> clans = GetClanListMatchPlayers(room);
            bool teamRed = false, teamBlue = false;
            for (int i = 0; i < clans.Values.Count; i++)
            {
                ClanModel clan = clans.Values[i];
                if (clan.RedPlayers >= 4)
                    teamRed = true;
                else if (clan.BluePlayers >= 4)
                    teamBlue = true;
            }
            return (teamRed && teamBlue);
        }
        /// <summary>
        /// Gera uma lista de TODOS os clãs que estão na sala, sem especificar qualquer SLOT_STATE.
        /// </summary>
        /// <param name="room">Sala</param>
        /// <returns></returns>
        private static SortedList<int, ClanModel> GetClanListMatchPlayers(Room room)
        {
            SortedList<int, ClanModel> clans = new SortedList<int, ClanModel>();
            List<Account> list = room.GetAllPlayers();
            for (int i = 0; i < list.Count; i++)
            {
                Account pR = list[i];
                if (pR.clanId == 0)
                    continue;
                if (clans.TryGetValue(pR.clanId, out ClanModel model) && model != null)
                {
                    if (pR._slotId % 2 == 0)
                        model.RedPlayers++;
                    else
                        model.BluePlayers++;
                }
                else
                {
                    model = new ClanModel
                    {
                        clanId = pR.clanId
                    };
                    if (pR._slotId % 2 == 0)
                        model.RedPlayers++;
                    else
                        model.BluePlayers++;
                    clans.Add(pR.clanId, model);
                }
            }
            return clans;
        }
        public static void PlayTimeEvent(long playedTime, Account p, PlayTimeModel ptEvent, bool isBotMode)
        {
            Room room = p._room;
            PlayerEvent pev = p._event;
            if (room == null || isBotMode || pev == null)
                return;
            long value = pev.LastPlaytimeValue, finish = pev.LastPlaytimeFinish, date2 = pev.LastPlaytimeDate;
            if (pev.LastPlaytimeDate < ptEvent._startDate)
            {
                pev.LastPlaytimeFinish = 0;
                pev.LastPlaytimeValue = 0;
            }
            if (pev.LastPlaytimeFinish == 0)
            {
                pev.LastPlaytimeValue += playedTime;
                if (pev.LastPlaytimeValue >= ptEvent._time)
                    pev.LastPlaytimeFinish = 1;
                pev.LastPlaytimeDate = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                if (pev.LastPlaytimeValue >= ptEvent._time)
                    p.SendPacket(new BATTLE_PLAYED_TIME_PAK(0, ptEvent));
                else
                    p.SendPacket(new BATTLE_PLAYED_TIME_PAK(1, new PlayTimeModel { _time = ptEvent._time - pev.LastPlaytimeValue }));
            }
            else if (pev.LastPlaytimeFinish == 1)
                p.SendPacket(new BATTLE_PLAYED_TIME_PAK(0, ptEvent));
            if (pev.LastPlaytimeValue != value || pev.LastPlaytimeFinish != finish || pev.LastPlaytimeDate != date2)
                EventPlayTimeSyncer.ResetPlayerEvent(p.player_id, pev);
        }
        public static void LogRoomBattleStart(Room room)
        {
            SendDebug.SendInfo("[Room] Tech id: " + room.UniqueRoomId);
            SendDebug.SendInfo("[Room] Room id: " + string.Format("{0:0##}", room._roomId + 1));
            SendDebug.SendInfo("[Room] Channel id: " + string.Format("{0:0##}", room._channelId + 1));
            SendDebug.SendInfo("[Room] Room name: '" + room.name + "'");
            SendDebug.SendInfo("[Room] Room type: " + (RoomType)room.room_type);
            SendDebug.SendInfo("[Room] Room special: " + (RoomSpecial)room.special);
            SendDebug.SendInfo("[Room] Room weapons: " + (RoomWeaponsFlag)room.weaponsFlag);
            SendDebug.SendInfo("[Room] Map name: '" + room._mapName + "'");
            SendDebug.SendInfo("[Room] Actual players count: " + room.GetPlayingPlayers(2, true));
            SendDebug.SendInfo("[Room] Started battle.");
        }
        public static void LogVotekickStart(Room room, Account p, SLOT slot)
        {
            VoteKick vote = room.votekick;
            if (vote == null)
                {
                return;
            }
            SendDebug.SendInfo("[Room] Votekick has been started by " + slot._id + " (Id: " + (p.player_id) + "; Nick: " + (p.player_name) + "), (To: " + vote.victimIdx + "; Motive: " + (VoteKickMotive)vote.motive + ") Date: " + room.StartDate + " info: " +room.UniqueRoomId);
        }
        public static void LogVotekickResult(Room room)
        {
            VoteKick vote = room.votekick;
            if (vote == null)
                {
                return;
            }
            SendDebug.SendInfo("[Room] Votekick result: (Allies: " + vote.allies + "; Enemys: " + vote.enemys + "); Total: (" + vote.kikar + "/" + vote.deixar + ") Date: " + room.StartDate + " info: " + room.UniqueRoomId);
        }
        public static void LogRoomRoundRestart(Room room)
        {
            SendDebug.SendInfo("[Room] Round " + room.rodada + " has been started. Date: " + room.StartDate + " info: " +room.UniqueRoomId);
        }
        public static void LogRoomResult(Room room)
        {
            uint info = room.UniqueRoomId;
            SendDebug.SendInfo("[Room] ;------------------;");
            SendDebug.SendInfo("[Room] Battle is finished.");
            SendDebug.SendInfo("[Room] Tech id: " + info);
            SendDebug.SendInfo("[Room] Used spawn count: " + room.spawnsCount);
            SendDebug.SendInfo("[Room] End Players count: " + room.GetPlayingPlayers(2, true));
            SendDebug.SendInfo("[Room] ;------------------;");
            SendDebug.SendInfo("[Room] Room result info.");
            for (int i = 0; i < 16; i++)
            {
                SLOT slot = room._slots[i];
                if (slot.state == SLOT_STATE.BATTLE && slot._playerId > 0)
                    SendDebug.SendInfo("[Room] Player (Id: " + slot._playerId + "; Slot: " + i + ") Exp: " + slot.exp + "; Gp: " + slot.gp + "; KD: " + slot.allKills + "/" + slot.allDeaths + "; Score: " + slot.Score);
            }
        }
    }
}