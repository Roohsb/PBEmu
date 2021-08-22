using Core;
using Core.managers;
using Core.managers.events;
using Core.models.account;
using Core.models.account.players;
using Core.models.account.title;
using Core.models.enums;
using Core.models.enums.flags;
using Core.server;
using Game.data.managers;
using Game.data.sync;
using Game.data.xml;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace Game.data.model
{
    public class Account
    {
        public byte[] LocalIP = new byte[4];
        public bool _isOnline, HideGMcolor, AntiKickGM, LoadedShop, DebugPing, _myConfigsLoaded;
        public string player_name = "", password, login, dados, textocache = "";
        public long player_id, ban_obj_id;
        public uint LastRankUpDate, LastLoginDate;
        public IPAddress PublicIP;
        public CupomEffects effects;
        public PlayerSession Session;
        //public PhysicalAddress MacAddress;
        public int LastRoomPage, LastPlayerPage,
            tourneyLevel, channelId = -1, clanAccess, clanDate, _exp, _gp, clanId, _money, brooch, insignia, medal, blue_order, _slotId = -1, name_color, _rank, matchSlot = -1;
        public PlayerEquipedItems _equip = new PlayerEquipedItems();
        public PlayerInventory _inventory = new PlayerInventory();
        public PlayerConfig _config;
        public GameClient _connection;
        public Room _room;
        public PlayerBonus _bonus;
        public Match _match;
        public AccessLevel access;
        public PlayerDailyRecord Daily = new PlayerDailyRecord();
        public PcCafe pc_cafe;
        public PlayerVip _pccafes = new PlayerVip();
        public PlayerMissions _mission = new PlayerMissions();
        public PlayerStats _statistic = new PlayerStats();
        public FriendSystem FriendSystem = new FriendSystem();
        public PlayerTitles _titles = new PlayerTitles();
        public AccountStatus _status = new AccountStatus();
        public List<Account> _clanPlayers = new List<Account>();
        public Ping ms = new Ping();
        public PlayerEvent _event;
        public DateTime LastSlotChange, LastLobbyEnter, LastPingDebug;
        public bool
            fistShoplobby = false,
            BoxLogin = false,
            damage = false,
            location = false,
            quiz = false,
            isChatBanned = false,
            _lifeInfinity = false,
            FeedBack = false, 
            isLatency = false,
            EventChat = false,
            usedMegaFone = false;
        public int
            CountLobbys = 0,
            chancesQuiz = 0,
            limitesofCommands = 0,
            isChatMinute = 0,
            AntiCheatDamage = 0;
        public DateTime isChatDate, IsChatDateFinish;
        public Event_Chat ModelEventChat;
        public Account()
        {
            LastSlotChange = DateTime.Now;
            LastLobbyEnter = DateTime.Now;
        }
        /// <summary>
        /// Eventos, Títulos, Missões, Inventário, Status, Lista de amigos (Clean), Sessão, ClanMatch, Sala, Config, Connection,
        /// </summary>
        public void SimpleClear()
        {
            _titles = new PlayerTitles();
            _mission = new PlayerMissions();
            _inventory = new PlayerInventory();
            _status = new AccountStatus();
            Daily = new PlayerDailyRecord();
            FriendSystem.CleanList();
            Session = null;
            _event = null;
            _match = null;
            _room = null;
            _config = null;
            _connection = null;
        }
        public void SetPublicIP(IPAddress address)
        {
            if (address == null)
                PublicIP = new IPAddress(new byte[4]);
            PublicIP = address;
        }
        public void SetPublicIP(string address)
        {
            PublicIP = IPAddress.Parse(address);
        }
        public Channel GetChannel() => ChannelsXML.getChannel(channelId);
        public void ResetPages()
        {
            LastRoomPage = 0;
            LastPlayerPage = 0;
        }
        public bool GetChannel(out Channel channel)
        {
            channel = ChannelsXML.getChannel(channelId);
            return channel != null;
        }
        public void SetOnlineStatus(bool online)
        {
            if (_isOnline != online && ComDiv.UpdateDB("accounts", "online", online, "player_id", player_id))
                _isOnline = online;
        }
        public void UpdateCacheInfo()
        {
            if (player_id == 0)
                return;
            lock (AccountManager._contas)
            {
                AccountManager._contas[player_id] = this;
            }
        }
        public bool ComparePassword(string pass) => pass == password;
        public int GetRank() => _bonus == null || _bonus.fakeRank == 55 ? _rank : _bonus.fakeRank;
        public void Close(int time, bool kicked = false)
        {
            if (_connection != null)
                _connection.Close(time, kicked);
        }
        /// <summary>
        /// Envia pacotes para o jogador. Caso ele não esteja conectado, os dados não serão enviados.
        /// </summary>
        /// <param name="data">Dados</param>
        public void SendPacket(SendPacket sp)
        {
            if (_connection != null)
                _connection.SendPacket(sp);
        }
        public void SendPacket(SendPacket sp, bool OnlyInServer)
        {
            if (_connection != null)
                _connection.SendPacket(sp);
            else if (!OnlyInServer && (_status.serverId != 255 && _status.serverId != Settings.serverId))
                Game_SyncNet.SendBytes(player_id, sp, _status.serverId);
        }
        /// <summary>
        /// Envia pacotes para o jogador. Caso ele não esteja conectado, os dados não serão enviados.
        /// </summary>
        /// <param name="data">Dados</param>
        public void SendPacket(byte[] data)
        {
            if (_connection != null)
                _connection.SendPacket(data);
        }
        /// <summary>
        /// Envia pacotes para o jogador. Caso ele não esteja conectado, os dados não serão enviados.
        /// </summary>
        /// <param name="data">Dados</param>
        public void SendCompletePacket(byte[] data)
        {
            if (_connection != null)
                _connection.SendCompletePacket(data);
        }
        public void SendCompletePacket(byte[] data, bool OnlyInServer)
        {
            if (_connection != null)
                _connection.SendCompletePacket(data);
            else if (!OnlyInServer && (_status.serverId != 255 && _status.serverId != Settings.serverId))
                Game_SyncNet.SendCompleteBytes(player_id, data, _status.serverId);
        }
        public void LoadInventory()
        {
            lock (_inventory._items)
            {
                _inventory._items.AddRange(PlayerManager.GetInventoryItems(player_id));
            }
        }
        public void LoadMissionList()
        {
            PlayerMissions m = MissionManager.getInstance().getMission(player_id, _mission.mission1, _mission.mission2, _mission.mission3, _mission.mission4);
            if (m == null)
                MissionManager.getInstance().addMissionDB(player_id);
            else
                _mission = m;
        }
        public void LoadPlayerBonus()
        {
            PlayerBonus bonus = PlayerManager.GetPlayerBonusDB(player_id);
            if (bonus.ownerId == 0)
            {
                PlayerManager.CreatePlayerBonusDB(player_id);
                bonus.ownerId = player_id;
            }
            _bonus = bonus;
        }
        public uint GetSessionId() => Session != null ? Session._sessionId : 0;
        public void SetPlayerId(long id)
        {
            player_id = id;
            GetAccountInfos(35);
        }
        public void SetPlayerId(long id, int LoadType)
        {
            player_id = id;
            GetAccountInfos(LoadType);
        }
        public void SetPlayerId()
        {
            _titles = new PlayerTitles();
            _bonus = new PlayerBonus();
            GetAccountInfos(8);
        }
        /// <summary>
        /// Gera informações adicionais a conta\n0 = Nada\n1 = Títulos\n2 = Bônus\n4 = Amigos (Completo)\n8 = Eventos\n16 = Configurações\n32 = Amigos (Básico)
        /// </summary>
        /// <param name="LoadType">Detalhes</param>
        public void GetAccountInfos(int LoadType)
        {
            try
            {
                if (LoadType > 0 && player_id > 0)
                {
                    if ((LoadType & 1) == 1)
                        _titles = TitleManager.GetInstance().GetTitleDB(player_id);
                         Daily = PlayerManager.GetPlayerDailyRecord(player_id);
                    if (Daily == null)
                    {
                        PlayerManager.CreatePlayerDailyRecord(player_id);
                    }
                    if ((LoadType & 2) == 2)
                        _bonus = PlayerManager.GetPlayerBonusDB(player_id);
                    if ((LoadType & 4) == 4)
                    {
                        List<Friend> fs = PlayerManager.GetFriendList(player_id);
                        if (fs.Count > 0)
                        {
                            FriendSystem._friends = fs;
                            for (int idx = 0; idx < FriendSystem._friends.Count; ++idx)
                                CorrectionFriend(FriendSystem._friends[idx], idx);
                            AccountManager.GetFriendlyAccounts(FriendSystem);
                        }
                    }
                    if ((LoadType & 8) == 8)
                        _event = PlayerManager.GetPlayerEventDB(player_id);
                    if (_event == null)
                    {
                        PlayerManager.AddEventDB(player_id);
                        _event = new PlayerEvent();
                    }
                        if ((LoadType & 16) == 16)
                        _config = PlayerManager.GetConfigDB(player_id);
                    if ((LoadType & 32) == 32)
                    {
                        List<Friend> fs = PlayerManager.GetFriendList(player_id);
                        if (fs.Count > 0)
                        {
                            for (int idx = 0; idx < FriendSystem._friends.Count; ++idx)
                                CorrectionFriend(FriendSystem._friends[idx], idx);
                            FriendSystem._friends = fs;
                        }
                    }
                }
            }
            catch (Exception)
            {
                SendDebug.SendInfo("Error em Remover Dados. " + player_name);
            }
        }
        public void CorrectionFriend(Friend exist, int idx)
        {
            try
            {
                Account account = AccountManager.GetAccount(exist.player_id, true);
                if (account == null && exist != null && exist.state == 1)
                {
                   bool Fr_1 = PlayerManager.DeleteFriend(exist.player_id, player_id);
                   bool Fr_2 = FriendSystem.RemoveFriend(FriendSystem._friends[idx]);
                }
            }
            catch
            {
                Console.WriteLine("Error ao remover jogador dos amigos.");
            }
        }
        public void DiscountPlayerItems()
        {

            bool updEffect = false;
            uint data_atual = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
            List<object> removedItems = new List<object>();
            int bonuses = _bonus != null ? _bonus.bonuses : 0, freepass = _bonus != null ? _bonus.freepass : 0;
            lock (_inventory._items)
            {
                for (int i = 0; i < _inventory._items.Count; i++)
                {
                    ItemsModel item = _inventory._items[i];
                    if (item._count <= data_atual & item._equip == 2)
                    {
                        if (item._category == 3)
                        {
                            if (_bonus == null)
                                continue;
                            if (!_bonus.RemoveBonuses(item._id))
                            {
                                switch (item._id)
                                {
                                    case 1200014000:
                                        {
                                            ComDiv.UpdateDB("player_bonus", "sightcolor", 4, "player_id", player_id);
                                            _bonus.sightColor = 4;
                                            break;
                                        }
                                    case 1200006000:
                                        {
                                            ComDiv.UpdateDB("accounts", "name_color", 0, "player_id", player_id);
                                            name_color = 0;
                                            break;
                                        }
                                    case 1200009000:
                                        {
                                            ComDiv.UpdateDB("player_bonus", "fakerank", 55, "player_id", player_id);
                                            _bonus.fakeRank = 55;
                                            break;
                                        }
                                    case 1200010000:
                                        {
                                            if (_bonus.fakeNick.Length > 0)
                                            {
                                                ComDiv.UpdateDB("player_bonus", "fakenick", "", "player_id", player_id);
                                                ComDiv.UpdateDB("accounts", "player_name", _bonus.fakeNick, "player_id", player_id);
                                                player_name = _bonus.fakeNick;
                                                _bonus.fakeNick = "";
                                            }
                                            break;
                                        }
                                }
                            }
                            CupomFlag cupom = CupomEffectManagerJSON.GetCupomEffect(item._id);
                            if (cupom != null && cupom.EffectFlag > 0 && effects.HasFlag(cupom.EffectFlag))
                            {
                                effects -= cupom.EffectFlag;
                                updEffect = true;
                            }
                        }
                        removedItems.Add(item._objId);
                        _inventory._items.RemoveAt(i--);
                    }
                    else if (item._count == 0)
                    {
                        removedItems.Add(item._objId);
                        _inventory._items.RemoveAt(i--);
                    }
                }
                ComDiv.DeleteDB("player_items", "object_id", removedItems.ToArray(), "owner_id", player_id);
            }

            if (_bonus != null && (_bonus.bonuses != bonuses || _bonus.freepass != freepass))
                PlayerManager.UpdatePlayerBonus(player_id, _bonus.bonuses, _bonus.freepass);
            if (effects < 0) effects = 0;
            if (updEffect)
                PlayerManager.UpdateCupomEffects(player_id, effects);
            _inventory.LoadBasicItems();
            int type = PlayerManager.CheckEquipedItems(_equip, _inventory._items);
            if (type > 0)
            {
                DBQuery query = new DBQuery();
                if ((type & 2) == 2) PlayerManager.UpdateWeapons(_equip, query);
                if ((type & 1) == 1) PlayerManager.UpdateChars(_equip, query);
                ComDiv.UpdateDB("accounts", "player_id", player_id, query.GetTables(), query.GetValues());
            }
        }

        public void SetHideGmColor()
        {
            if(HaveGMLevel())
            {
                name_color = LorenstudioSettings.NameColorFree;
            }
        }
        /// <summary>
        /// Retorna TRUE se rank == 53 ou 54 ou HaveGMLevel.
        /// </summary>
        /// <returns></returns>
        public bool UseChatGM() => !HideGMcolor && (_rank == 53 || _rank == 54);
        public bool IsGM() => _rank == 53 || _rank == 54 ||  HaveGMLevel();
        public bool HaveGMLevel() => (int)access > 2;
        public bool HaveAcessLevel() => access > 0;
        public bool HaveVip4() => pc_cafe == PcCafe.VIP_MASTER;
        public bool HaveVipTotal() => pc_cafe > 0;
        public void SetHideColorVip()
        {
            if (IsGM())
                return;
            switch (pc_cafe)
            {
                case PcCafe.VIP_BASIC:
                    name_color = LorenstudioSettings.NameColorVip1;
                    break;
                case PcCafe.VIP_MASTER:
                    name_color = LorenstudioSettings.NameColorVip2;
                    break;
                case PcCafe.VIP_PLUS:
                    name_color = LorenstudioSettings.NameColorVip5;
                    break;
                case PcCafe.VIP_PREMIUM:
                    name_color = LorenstudioSettings.NameColorVip6;
                    break;
            }
        }
    }
}