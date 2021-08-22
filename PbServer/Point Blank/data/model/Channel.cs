﻿using Core;
using Core.server;
using Game.data.managers;
using Game.data.sync;
using System;
using System.Collections.Generic;

namespace Game.data.model
{
    public class Channel
    {
        public int _id, _type, serverId;
        public string _announce = "";
        public List<PlayerSession> _players = new List<PlayerSession>();
        public List<Room> _rooms = new List<Room>();
        public List<Match> _matchs = new List<Match>();
        private DateTime LastRoomsSync = DateTime.Now;
        public PlayerSession GetPlayer(uint session)
        {
            lock (_players)
            {
                try
                {
                    for (int i = 0; i < _players.Count; i++)
                    {
                        PlayerSession inf = _players[i];
                        if (inf._sessionId == session)
                            return inf;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
                return null;
            }
        }
        public PlayerSession GetPlayer(uint session, out int idx)
        {
            idx = -1;
            lock (_players)
            {
                try
                {
                    for (int i = 0; i < _players.Count; i++)
                    {
                        PlayerSession inf = _players[i];
                        if (inf._sessionId == session)
                        {
                            idx = i;
                            return inf;
                        }
                    }
                }
                catch (Exception ex)
                {
                    SendDebug.SendInfo(ex.ToString());
                }
                return null;
            }
        }
        public bool AddPlayer(PlayerSession pS)
        {
            lock (_players)
            {
                if (!_players.Contains(pS))
                {
                    _players.Add(pS);
                    Game_SyncNet.UpdateGSCount(serverId);
                    return true;
                }
                return false;
            }
        }
        public void RemoveMatch(int matchId)
        {
            try
            {
                lock (_matchs)
                {
                    for (int i = 0; i < _matchs.Count; ++i)
                        if (matchId == _matchs[i]._matchId)
                        {
                            _matchs.RemoveAt(i);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo(ex.ToString());
            }
        }
        public void AddMatch(Match match)
        {
            lock (_matchs)
            {
                if (!_matchs.Contains(match))
                    _matchs.Add(match);
            }
        }
        /// <summary>
        /// Adiciona uma sala no canal. Proteção Thread-Safety.
        /// </summary>
        /// <param name="room">Sala</param>
        public void AddRoom(Room room)
        {
            lock (_rooms)
            {
                _rooms.Add(room);
            }
        }
        /// <summary>
        /// Limpa as salas que a quantidade de jogadores for 0. Proteção Thread-Safety.
        /// </summary>
        public void RemoveEmptyRooms()
        {
            try
            {
                lock (_rooms)
                {
                    if ((DateTime.Now - LastRoomsSync).TotalSeconds >= 2)
                    {
                        LastRoomsSync = DateTime.Now;
                        for (int i = 0; i < _rooms.Count; ++i)
                        {
                            Room r = _rooms[i];
                            if (r.GetAllPlayers().Count < 1)
                                _rooms.RemoveAt(i--);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[Channel.RemoveEmptyRooms] " + ex.ToString());
            }
        }
        public Match GetMatch(int id)
        {
            lock (_matchs)
            {
                try
                {
                    for (int i = 0; i < _matchs.Count; i++)
                    {
                        Match mt = _matchs[i];
                        if (mt._matchId == id)
                            return mt;
                    }
                }
                catch (Exception ex)
                {
                    SendDebug.SendInfo(ex.ToString());
                }
                return null;
            }
        }
        public Match GetMatch(int id, int clan)
        {
            lock (_matchs)
            {
                try
                {
                    for (int i = 0; i < _matchs.Count; i++)
                    {
                        Match mt = _matchs[i];
                        if (mt.friendId == id && mt.clan._id == clan)
                            return mt;
                    }
                }
                catch (Exception ex)
                {
                    SendDebug.SendInfo(ex.ToString());
                }
                return null;
            }
        }
        /// <summary>
        /// Procura uma sala no canal. (Proteção Thread-Safety)
        /// </summary>
        /// <param name="id">Id da sala</param>
        /// <returns></returns>
        public Room GetRoom(int id)
        {
            lock (_rooms)
            {
                try
                {
                    for (int i = 0; i < _rooms.Count; i++)
                    {
                        Room room = _rooms[i];
                        if (room._roomId == id)
                            return room;
                    }
                }
                catch (Exception ex)
                {
                    SendDebug.SendInfo(ex.ToString());
                }
                return null;
            }
        }
        /// <summary>
        /// Gera uma lista de contas que não estão em uma sala, que possuem um apelido, sem utilizar a Database. Proteção Thread-Safety.
        /// </summary>
        /// <returns></returns>
        public List<Account> GetWaitPlayers()
        {
            List<Account> list = new List<Account>();
            lock (_players)
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    Account player = AccountManager.GetAccount(_players[i]._playerId, true);
                    if (player != null && player._room == null && player.player_name != "")
                        list.Add(player);
                }
            }
            return list;
        }
        public void SendPacketToWaitPlayers(SendPacket packet)
        {
            List<Account> players = GetWaitPlayers();
            if (players.Count == 0)
                return;
            byte[] data = packet.GetCompleteBytes("Channel.SendPacketToWaitPlayers");
            for (int i = 0; i < players.Count; i++)
                players[i].SendCompletePacket(data);
        }
        public bool RemovePlayer(Account p)
        {
            try
            {
                p.channelId = -1;
                if (p.Session != null)
                {
                    lock (_players)
                        if (_players.Remove(p.Session))
                        {
                            Game_SyncNet.UpdateGSCount(serverId);
                            return true;
                        }
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo(ex.ToString());
            }
            return false;
        }
    }
}