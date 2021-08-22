using Core;
using Core.DB_Battle;
using Core.models.enums;
using Core.models.enums.errors;
using Core.models.room;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using Game.Progress;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_TIMERSYNC_REC : ReceiveGamePacket
    {
        private float unk0;
        private uint TimeRemaining;
        private int Ping, unk5, Latency, Round;
        public BATTLE_TIMERSYNC_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            TimeRemaining = ReadUD();
            unk0 = ReadT(); //BanMotive1
            Round = ReadC(); //Round da partida (Não o round atual do jogador)
            Ping = ReadC(); //ping            
            unk5 = ReadC(); //BanMotive2
            Latency = ReadH(); //Latência?
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Room room = p._room;
                if (room == null)
                    return;
                bool isBotMode = room.IsBotMode();
                SLOT slot = room.GetSlot(p._slotId);
                if (slot == null || slot.state != SLOT_STATE.BATTLE)
                    return;
                if (unk0 != 1 || unk5 != 0)
                {
                    SendDebug.SendHack("Hacker: [" + unk0 + "; " + unk5 + " (" + (HackType)unk5 + ")] Player: " + p.player_name + "; Id: " + p.player_id);
                    p.AntiCheatDamage += 1;
                    UpdateBanSystem();
                    BanInsert();
                    p.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2));
                    p.Close(1000);
                    if (p.AntiCheatDamage > 10)
                    {
                        SendDebug.SendHack("-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-");
                        SendDebug.SendHack("~ Servidor Informa: monitorou e achou um comportamento muito estranho. Decidimos dar dc no player.");
                        SendDebug.SendHack("~ Dados: Name [" + p.player_name + "], ID [" + p.player_id + "], Login [" + p.login + "], IP [" + _client.GetIPAddress() + "].");
                        SendDebug.SendHack("~ Data: [" + DateTime.Now.ToString() + "].");
                        SendDebug.SendHack("-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-");
                        UpdateBanSystem();
                        BanInsert();
                        p.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2));
                        p.Close(1000);
                        return;

                    }
                }

                room._timeRoom = TimeRemaining;
                SyncPlayerPings(p, room, slot, isBotMode);
                if ((TimeRemaining > 0x80000000) && !room.swapRound && CompareRounds(room, Round) && (int)room._state == 5)
                    EndRound(room, isBotMode);
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[BATTLE_TIMERSYNC_REC] " + ex.ToString());
            }
        }

        public void UpdateBanSystem()
        {
            Account p = _client._player;
            if (p == null)
                return;

            using SqlConnection connection = ServerLoadDB.GetInstance().Conn();
            {
                SqlCommand command = connection.CreateCommand();
                connection.Open();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@playerid", p.player_id);
                command.Parameters.AddWithValue("@ipPlayer", _client.GetIPAddress());
                command.CommandText = "UPDATE accounts SET access_level = '-1' WHERE player_id=@playerid";
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Dispose();
                connection.Close();
            }
        }

        public void BanInsert()
        {
            Account p = _client._player;
            if (p == null)
                return;
            using SqlConnection connection = ServerLoadDB.GetInstance().Conn();
            {
                SqlCommand command = connection.CreateCommand();
                connection.Open();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@playerid", p.player_id);
                command.Parameters.AddWithValue("@playerlogin", p.login);
                command.Parameters.AddWithValue("@playername", p.player_name);
                command.Parameters.AddWithValue("@ipPlayer", _client.GetIPAddress());
                command.CommandText = @"INSERT INTO auto_ban (player_id, login, player_name, type, time, ip, hack_type) VALUES (@playerid, @playerlogin, @playername, '0', '9999', @ipPlayer, 'BotSystem');";
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Dispose();
                connection.Close();
            }
        }
        private void SyncPlayerPings(Account p, Room room, SLOT slot, bool isBotMode)
        {
            if (isBotMode)
                return;
            bool aceito = false;
            PingReply ms = null;
            try
            {

                ms = p.ms.Send("facebook.com");
                aceito = ms.Status == IPStatus.Success;
            }
            catch { }
            try
            {

                slot.latency = Latency;
                slot.ping = Ping;


                if (p.isLatency && (DateTime.Now - p.LastPingDebug).TotalSeconds >= 3 && aceito)
                {
                    p.LastPingDebug = DateTime.Now;
                    using LOBBY_CHATTING_PAK pPing = new LOBBY_CHATTING_PAK("Latency to player", p.GetSessionId(), 0, true, "ms: (" + string.Concat(ms.RoundtripTime) + "), ping: (" + slot.ping + ")");
                    p.SendPacket(pPing);
                }

                double secs = (DateTime.Now - room.LastPingSync).TotalSeconds;

                if (secs < 7)
                    return;

                using BATTLE_SENDPING_PAK packet = new BATTLE_SENDPING_PAK();
                room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);

                room.LastPingSync = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erros Pings: " + ex.ToString());
            }
        }
        private bool CompareRounds(Room room, int externValue)
        {
            if (room.room_type == (int)RoomType.Boss || room.room_type == (int)RoomType.Cross_Counter)
                return (room.rodada == externValue);
            else
                return (room.rodada == externValue + 1);
        }
        private void EndRound(Room room, bool isBotMode)
        {
            try
            {
                room.swapRound = true;
                if (room.room_type == 7 || room.room_type == 12)
                {
                    if (room.rodada == 1)
                    {
                        room.rodada = 2;
                        for (int i = 0; i < 16; i++)
                        {
                            SLOT slot = room._slots[i];
                            if (slot.state == SLOT_STATE.BATTLE)
                            {
                                slot.killsOnLife = 0;
                                slot.lastKillState = 0;
                                slot.repeatLastState = false;
                            }
                        }
                        List<int> dinos = AllUtils.getDinossaurs(room, true, -2);
                        using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(room, 2, RoundEndType.TimeOut))
                        using (BATTLE_ROUND_RESTART_PAK packet2 = new BATTLE_ROUND_RESTART_PAK(room, dinos, isBotMode))
                            room.SendPacketToPlayers(packet, packet2, SLOT_STATE.BATTLE, 0);

                        room.round.StartJob(5250, (callbackState) =>
                        {
                            if (room._state == RoomState.Battle)
                            {
                                room.BattleStart = DateTime.Now.AddSeconds(5);
                                using BATTLE_TIMERSYNC_PAK packet = new BATTLE_TIMERSYNC_PAK(room);
                                room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                            }
                            room.swapRound = false;
                            lock (callbackState)
                            { room.round.Timer = null; }
                        });
                    }
                    else if (room.rodada == 2)
                        AllUtils.EndBattle(room, isBotMode);
                }
                else if (room.ThisModeHaveRounds())
                {
                    int winner = 1;
                    if (room.room_type != 3)
                        room.blue_rounds++;
                    else
                    {
                        if (room.Bar1 > room.Bar2)
                        {
                            room.red_rounds++;
                            winner = 0;
                        }
                        else if (room.Bar1 < room.Bar2)
                            room.blue_rounds++;
                        else
                            winner = 2;
                    }
                    AllUtils.BattleEndRound(room, winner, RoundEndType.TimeOut);
                }
                else
                {
                    List<Account> players = room.GetAllPlayers(SLOT_STATE.READY, 1);
                    if (players.Count == 0)
                        goto EndLabel;
                    TeamResultType winnerTeam = AllUtils.GetWinnerTeam(room);
                    room.CalculateResult(winnerTeam, isBotMode);
                    using (BATTLE_ROUND_WINNER_PAK packet = new BATTLE_ROUND_WINNER_PAK(room, winnerTeam, RoundEndType.TimeOut))
                    {
                        AllUtils.GetBattleResult(room, out ushort missionCompletes, out ushort inBattle, out byte[] a1);
                        byte[] data = packet.GetCompleteBytes("BATTLE_TIMERSYNC_REC");
                        for (int i = 0; i < players.Count; i++)
                        {
                            Account pR = players[i];
                            if (room._slots[pR._slotId].state == SLOT_STATE.BATTLE)
                                pR.SendCompletePacket(data);
                            pR.SendPacket(new BATTLE_ENDBATTLE_PAK(pR, winnerTeam, inBattle, missionCompletes, isBotMode, a1));
                        }
                    }
                    EndLabel:
                    AllUtils.resetBattleInfo(room);
                }
            }
            catch (Exception ex)
            {
                if (room != null)
                    Logger.Error("[!] Crash no BATTLE_TIMERSYNC_REC, Sala: " + room._roomId + ";" + room._channelId + ";" + room.room_type);
                Logger.Error("[BATTLE_TIMERSYNC_REC2] " + ex.ToString());
            }
        }
    }
}