using Core;
using Core.DB_Battle;
using Core.models.account;
using Core.models.account.players;
using Core.models.enums;
using Core.models.enums.flags;
using Core.server;
using Game.data.model;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;

namespace Game.data.managers
{
    public class AccountManager
    {
        private static readonly SortedList<string, DateTime> LastCreation = new SortedList<string, DateTime>();
        public static SortedList<long, Account> _contas = new SortedList<long, Account>();
        public static void AddAccount(Account acc)
        {
            lock (_contas)
            {
                if (!_contas.ContainsKey(acc.player_id))
                    _contas.Add(acc.player_id, acc);
            }
        }
        public static List<string> GetAccountsByIP(string ip)
        {
            List<string> accs = new List<string>();
            try
            {
                using SqlConnection connection = ServerLoadDB.GetInstance().Conn();
                SqlCommand command = connection.CreateCommand();
                connection.Open();
                command.Parameters.AddWithValue("@ip", ip);
                command.CommandText = "SELECT player_name FROM accounts WHERE lastip=@ip";
                command.CommandType = CommandType.Text;
                SqlDataReader data = command.ExecuteReader();
                {
                    while (data.Read())
                    {
                        accs.Add(data.GetString(0));
                    }
                    if (data != null)
                        data.Close();
                }
                command.Dispose();
                connection.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("Ocorreu um problema ao carregar a conta pelo IP!\r\n" + ex.ToString());
            }
            return accs;
        }
        /// <summary>
        /// Procura na Database uma conta. Também feita a procura dos Títulos, Amigos e Bônus.
        /// </summary>
        /// <param name="valor">Valor para procura</param>
        /// <param name="type">Tipo de procura\n0 = Login\n1 = Apelido\n2 = Id</param>
        /// <returns></returns>
        public static Account GetAccountDB(object valor, int type) => GetAccountDB(valor, type, 35);
        /// <summary>
        /// Procura na Database uma conta. É possível escolher se será feita a procura dos Títulos, Amigos, Bônus, Eventos, Configurações.
        /// </summary>
        /// <param name="valor">Valor para procura</param>
        /// <param name="type">Tipo de procura\n0 = Login\n1 = Apelido\n2 = Id</param>
        /// <param name="searchDBFlag">Detalhes de procura (DB;Flag)\n0 = Nada\n1 = Títulos\n2 = Bônus\n4 = Amigos (Completo)\n8 = Eventos\n16 = Configurações\n32 = Amigos (Básico)</param>
        /// <returns></returns>
        public static Account GetAccountDB(object valor, int type, int searchFlag)
        {
            if (type == 2 && (long)valor == 0 || (type == 0 || type == 1) && (string)valor == "")
                return null;
            Account conta = null;
            try
            {
                using SqlConnection connection = ServerLoadDB.GetInstance().Conn();
                SqlCommand command = connection.CreateCommand();
                connection.Open();
                command.Parameters.AddWithValue("@value", valor);
                command.CommandText = "SELECT * FROM accounts WHERE " + (type == 0 ? "login" : type == 1 ? "player_name" : "player_id") + "=@value";
                command.CommandType = CommandType.Text;
                SqlDataReader data = command.ExecuteReader();
                while (data.Read())
                {
                    conta = new Account
                    {
                        login = data.GetString(0),
                        password = data.GetString(1)
                    };
                    conta.SetPlayerId(data.GetInt64(2), searchFlag);
                    conta.player_name = data.GetString(3);
                    conta.name_color = data.GetInt32(4);
                    conta.clanId = data.GetInt32(5);
                    conta._rank = data.GetInt32(6);
                    conta._gp = data.GetInt32(7);
                    conta._exp = data.GetInt32(8);
                    conta.pc_cafe = (PcCafe)data.GetInt32(9);
                    conta._statistic.fights = data.GetInt32(10);
                    conta._statistic.fights_win = data.GetInt32(11);
                    conta._statistic.fights_lost = data.GetInt32(12);
                    conta._statistic.kills_count = data.GetInt32(13);
                    conta._statistic.deaths_count = data.GetInt32(14);
                    conta._statistic.headshots_count = data.GetInt32(15);
                    conta._statistic.escapes = data.GetInt32(16);
                    conta.access = (AccessLevel)data.GetInt32(17);
                    conta.SetPublicIP(data.GetString(18));
                    conta.LastRankUpDate = (uint)data.GetInt64(20);
                    conta._money = data.GetInt32(21);
                    conta._isOnline = data.GetBoolean(22);
                    conta._equip._primary = data.GetInt32(23);
                    conta._equip._secondary = data.GetInt32(24);
                    conta._equip._melee = data.GetInt32(25);
                    conta._equip._grenade = data.GetInt32(26);
                    conta._equip._special = data.GetInt32(27);
                    conta._equip._red = data.GetInt32(28);
                    conta._equip._blue = data.GetInt32(29);
                    conta._equip._helmet = data.GetInt32(30);
                    conta._equip._dino = data.GetInt32(31);
                    conta._equip._beret = data.GetInt32(32);
                    conta.brooch = data.GetInt32(33);
                    conta.insignia = data.GetInt32(34);
                    conta.medal = data.GetInt32(35);
                    conta.blue_order = data.GetInt32(36);
                    conta._mission.mission1 = data.GetInt32(37);
                    conta.clanAccess = data.GetInt32(38);
                    conta.clanDate = data.GetInt32(39);
                    conta.effects = (CupomEffects)data.GetInt64(40);
                    conta._statistic.fights_draw = data.GetInt32(41);
                    conta._mission.mission2 = data.GetInt32(42);
                    conta._mission.mission3 = data.GetInt32(43);
                    conta._statistic.totalkills_count = data.GetInt32(44);
                    conta._statistic.totalfights_count = data.GetInt32(45);
                    conta._status.SetData((uint)data.GetInt64(46), conta.player_id);
                    conta.LastLoginDate = (uint)data.GetInt64(47);
                    conta._statistic.ClanGames = data.GetInt32(48);
                    conta._statistic.ClanWins = data.GetInt32(49);
                    // conta.MacAddress = (PhysicalAddress)data.GetValue(50);
                    conta.ban_obj_id = data.GetInt64(51);
                    AddAccount(conta);
                }
                command.Dispose();
                data.Close();
                connection.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("Ocorreu um problema ao carregar as contas!\r\n" + ex.ToString());
            }
            return conta;
        }
        public static void GetFriendlyAccounts(FriendSystem system)
        {
            if (system == null || system._friends.Count == 0)
                return;
            try
            {
                using SqlConnection connection = ServerLoadDB.GetInstance().Conn();
                SqlCommand command = connection.CreateCommand();
                connection.Open();
                string loaded = "";
                List<string> parameters = new List<string>();
                for (int idx = 0; idx < system._friends.Count; idx++)
                {
                    Friend friend = system._friends[idx];
                    string param = "@valor" + idx;
                    command.Parameters.AddWithValue(param, friend.player_id);
                    parameters.Add(param);
                }
                loaded = string.Join(",", parameters.ToArray());
                command.CommandText = "SELECT player_name,player_id,rank,online,status FROM accounts WHERE player_id in (" + loaded + ") ORDER BY player_id";
                SqlDataReader data = command.ExecuteReader();
                {
                    while (data.Read())
                    {
                        Friend friend = system.GetFriend(data.GetInt64(1));
                        if (friend != null)
                        {
                            friend.player.player_name = data.GetString(0);
                            friend.player._rank = data.GetInt32(2);
                            friend.player._isOnline = data.GetBoolean(3);
                            friend.player._status.SetData((uint)data.GetInt64(4), friend.player_id);
                        }
                    }
                    if (data != null)
                        data.Close();
                }
                command.Dispose();
                connection.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("Ocorreu um problema ao carregar (FriendlyAccounts)!\r\n" + ex.ToString());
            }
        }
        /// <summary>
        /// Procura no cache uma conta pelo Id. Caso não encontrada, é feita uma procura na Database.
        /// </summary>
        /// <param name="id">Id da conta</param>
        /// <param name="searchFlag">Detalhes de procura (DB;Flag)\n0 = Nada\n1 = Títulos\n2 = Bônus\n4 = Amigos (Completo)\n8 = Eventos\n16 = Configurações\n32 = Amigos (Básico)</param>
        /// <returns></returns>
        public static Account GetAccount(long id, int searchFlag)
        {
            if (id == 0)
                return null;
            try
            {
                return _contas.TryGetValue(id, out Account p) ? p : GetAccountDB(id, 2, searchFlag);
            }
            catch { return null; }
        }
        /// <summary>
        /// Procura no cache uma conta pelo Id.
        /// <para>É possível escolher se, caso não encontrada, procurar a conta na Database.</para>
        /// <para>Flag padrão de busca: 35.</para>
        /// </summary>
        /// <param name="id">Id da conta</param>
        /// <param name="noUseDB">Não usar Database?</param>
        /// <returns></returns>
        public static Account GetAccount(long id, bool noUseDB)
        {
            if (id == 0)
                return null;
            try
            {
                return _contas.TryGetValue(id, out Account p) ? p : (noUseDB ? null : GetAccountDB(id, 2));
            }
            catch { return null; }
        }
        /// <summary>
        /// Pesquisa no Cache do servidor uma conta, caso não encontrada, é feita uma pesquisa na Database.
        /// </summary>
        /// <param name="text">Texto</param>
        /// <param name="type">Tipo de procura. 0 = Login; 1 = Apelido</param>
        /// <param name="searchFlag">Detalhes de procura (DB;Flag)\n0 = Nada\n1 = Títulos\n2 = Bônus\n4 = Amigos (Completo)\n8 = Eventos\n16 = Configurações\n32 = Amigos (Básico)</param>
        /// <returns></returns>
        public static Account GetAccount(string text, int type, int searchFlag)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            IList<Account> lis = _contas.Values;
            for (int i = 0; i < lis.Count; i++)
            {
                Account p = lis[i];
                if (p != null && (type == 1 && p.player_name == text && p.player_name.Length > 0 || type == 0 && string.Compare(p.login, text) == 0))
                    return p;
            }
            return GetAccountDB(text, type, searchFlag);
        }
        public static bool UpdatePlayerName(string name, long playerId) =>
            ComDiv.UpdateDB("accounts", "player_name", name, "player_id", playerId);
        public static bool CreateAccount(out Account p, string login, string password, string endPoint, DateTime time)
        {
            try
            {
                if (LastCreation.TryGetValue(endPoint, out time) && (DateTime.Now - time).TotalMinutes < 5 && LastCreation.ContainsKey(endPoint))
                {
                    p = null;
                    return false;
                }
                if (!LastCreation.ContainsKey(endPoint))
                    LastCreation.Add(endPoint, time);
                else
                    LastCreation[endPoint] = DateTime.Now;

                using SqlConnection connection = ServerLoadDB.GetInstance().Conn();
                SqlCommand command = connection.CreateCommand();
                connection.Open();
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@pass", password);
                command.CommandText = "INSERT INTO accounts (login, password) VALUES (@login,@pass)";
                command.ExecuteNonQuery();
                command.CommandText = "SELECT * FROM accounts WHERE login=@login";

                SqlDataReader data = command.ExecuteReader();
                {
                    Account acc = new Account();
                    while (data.Read())
                    {
                        acc.login = login;
                        acc.password = password;
                        acc.player_id = data.GetInt64(2);
                        acc.SetPlayerId();
                        acc.player_name = data.GetString(3);
                        acc.name_color = data.GetInt32(4);
                        acc.clanId = data.GetInt32(5);
                        acc._rank = data.GetInt32(6);
                        acc._gp = data.GetInt32(7);
                        acc._exp = data.GetInt32(8);
                        acc.pc_cafe = (PcCafe)data.GetInt32(9);
                        acc._statistic.fights = data.GetInt32(10);
                        acc._statistic.fights_win = data.GetInt32(11);
                        acc._statistic.fights_lost = data.GetInt32(12);
                        acc._statistic.kills_count = data.GetInt32(13);
                        acc._statistic.deaths_count = data.GetInt32(14);
                        acc._statistic.headshots_count = data.GetInt32(15);
                        acc._statistic.escapes = data.GetInt32(16);
                        acc.access = (AccessLevel)data.GetInt32(17);
                        acc.LastRankUpDate = (uint)data.GetInt64(20);
                        acc._money = data.GetInt32(21);
                        acc._isOnline = data.GetBoolean(22);
                        acc._equip._primary = data.GetInt32(23);
                        acc._equip._secondary = data.GetInt32(24);
                        acc._equip._melee = data.GetInt32(25);
                        acc._equip._grenade = data.GetInt32(26);
                        acc._equip._special = data.GetInt32(27);
                        acc._equip._red = data.GetInt32(28);
                        acc._equip._blue = data.GetInt32(29);
                        acc._equip._helmet = data.GetInt32(30);
                        acc._equip._dino = data.GetInt32(31);
                        acc._equip._beret = data.GetInt32(32);
                        acc.brooch = data.GetInt32(33);
                        acc.insignia = data.GetInt32(34);
                        acc.medal = data.GetInt32(35);
                        acc.blue_order = data.GetInt32(36);
                        acc._mission.mission1 = data.GetInt32(37);
                        acc.clanAccess = data.GetInt32(38);
                        acc.effects = (CupomEffects)data.GetInt64(40);
                        acc._statistic.fights_draw = data.GetInt32(41);
                        acc._mission.mission2 = data.GetInt32(42);
                        acc._mission.mission3 = data.GetInt32(43);
                        acc._statistic.totalkills_count = data.GetInt32(44);
                        acc._statistic.totalfights_count = data.GetInt32(45);
                        acc._status.SetData((uint)data.GetInt64(46), acc.player_id);
                        //   acc.MacAddress = (PhysicalAddress)data.GetValue(50);
                        acc.ban_obj_id = data.GetInt64(51);
                    }
                    p = acc;
                    AddAccount(acc);
                    if (data != null)
                        data.Close();
                }
                command.Dispose();
                connection.Dispose();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[AccountManager.CreateAccount] " + ex.ToString());
                p = null;
                return false;
            }
        }
    }
}