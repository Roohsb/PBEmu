using Core.DB_Battle;
using Core.models.account;
using Core.models.account.players;
using Core.models.enums.friends;
using Core.models.enums.item;
using Core.xml;
using System.Data.SqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Core.server
{
    public static class ComDiv
    {
        public static DateTime GetDate()
        {
            try
            {
                using (WebResponse response = WebRequest.Create("http://www.google.com").GetResponse())
                    return DateTime.ParseExact(response.Headers["date"],
                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }
        public static int GetItemCategory(int id)
        {
            switch (GetIdStatics(id, 1))
            {
                case int id1 when (id1 >= 1 && id1 <= 9): return 1;
                case int id2 when (id2 >= 10 && id2 <= 11): return 2;
                case int id3 when (id3 >= 12 && id3 <= 19): return 3;
                default:
                    {
                        return 0;
                    }
            }
        }
        public static string ArrayToString(byte[] buffer, int length)
        {
            string str = "";
            try
            {
                str = ConfigGB.EncodeText.GetString(buffer, 0, length);
                int idx = str.IndexOf(char.MinValue);
                if (idx != -1)
                    str = str.Substring(0, idx);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return str;
        }
        public static bool UpdateDB(string TABELA, string COLUNA, object VALOR)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                   SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@valor", VALOR);
                    command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Info("[AllUtils.UpdateDB1] " + ex.ToString());
                return false;
            }
        }
        public static bool UpdateDB(string TABELA, string req1, object valorReq1, string[] COLUNAS, params object[] VALORES)
        {
            if (COLUNAS.Length > 0 && VALORES.Length > 0 && COLUNAS.Length != VALORES.Length)
            {
                Logger.Error("[UpdateDB2] Wrong values: " + String.Join(",", COLUNAS) + "/" + String.Join(",", VALORES));
                return false;
            }
            else if (COLUNAS.Length == 0 || VALORES.Length == 0)
                return false;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    string loaded = "";
                    List<string> parameters = new List<string>();
                    for (int i = 0; i < VALORES.Length; i++)
                    {
                        object obj = VALORES[i];
                        string column = COLUNAS[i];
                        string param = "@valor" + i;
                        command.Parameters.AddWithValue(param, obj);
                        parameters.Add(column + "=" + param);
                    }
                    loaded = string.Join(",", parameters.ToArray());
                    command.Parameters.AddWithValue("@req1", valorReq1);
                    command.CommandText = "UPDATE " + TABELA + " SET " + loaded + " WHERE " + req1 + "=@req1";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Info("[AllUtils.UpdateDB2] " + ex.ToString());
                return false;
            }
        }
        public static bool UpdateDB(string TABELA, string COLUNA, object VALOR, string req1, object valorReq1)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@valor", VALOR);
                    command.Parameters.AddWithValue("@req1", valorReq1);
                    command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor WHERE " + req1 + "=@req1";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Info("[AllUtils.UpdateDB3] " + ex.ToString());
                return false;
            }
        }
        public static bool UpdateDB(string TABELA, string req1, object valorReq1, string req2, object valorReq2, string[] COLUNAS, params object[] VALORES)
        {
            if (COLUNAS.Length > 0 && VALORES.Length > 0 && COLUNAS.Length != VALORES.Length)
            {
                Logger.Error("[updateDB4] Wrong values: " + String.Join(",", COLUNAS) + "/" + String.Join(",", VALORES));
                return false;
            }
            else if (COLUNAS.Length == 0 || VALORES.Length == 0)
                return false;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    string loaded = "";
                    List<string> parameters = new List<string>();
                    for (int i = 0; i < VALORES.Length; i++)
                    {
                        object obj = VALORES[i];
                        string column = COLUNAS[i];
                        string param = "@valor" + i;
                        command.Parameters.AddWithValue(param, obj);
                        parameters.Add(column + "=" + param);
                    }
                    loaded = string.Join(",", parameters.ToArray());
                    if (req1 != null)
                        command.Parameters.AddWithValue("@req1", valorReq1);
                    if (req2 != null)
                        command.Parameters.AddWithValue("@req2", valorReq2);
                    if (req1 != null && req2 == null)
                        command.CommandText = "UPDATE " + TABELA + " SET " + loaded + " WHERE " + req1 + "=@req1";
                    else if (req2 != null && req1 == null)
                        command.CommandText = "UPDATE " + TABELA + " SET " + loaded + " WHERE " + req2 + "=@req2";
                    else if (req2 != null && req1 != null)
                        command.CommandText = "UPDATE " + TABELA + " SET " + loaded + " WHERE " + req1 + "=@req1 AND " + req2 + "=@req2";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Info("[AllUtils.updateDB4] " + ex.ToString());
                return false;
            }
        }
        public static bool UpdateDB(string TABELA, string req1, int[] valorReq1, string req2, object valorReq2, string[] COLUNAS, params object[] VALORES)
        {
            if (COLUNAS.Length > 0 && VALORES.Length > 0 && COLUNAS.Length != VALORES.Length)
            {
                Logger.Error("[UpdateDB5] Wrong values: " + string.Join(",", COLUNAS) + "/" + string.Join(",", VALORES));
                return false;
            }
            else if (COLUNAS.Length == 0 || VALORES.Length == 0)
                return false;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    string loaded = "";
                   
                    List<string> parameters = new List<string>();
                    for (int i = 0; i < VALORES.Length; i++)
                    {
                        object obj = VALORES[i];
                        string column = COLUNAS[i];
                        string param = "@valor" + i;
                        command.Parameters.AddWithValue(param, obj);
                        parameters.Add(column + "=" + param);
                    }
                    loaded = string.Join(",", parameters.ToArray());

                    if (req1 != null)
                        command.Parameters.AddWithValue("@req1", valorReq1);
                    if (req2 != null)
                        command.Parameters.AddWithValue("@req2", valorReq2);
                    if (req1 != null && req2 == null)
                        command.CommandText = "UPDATE " + TABELA + " SET " + loaded + " WHERE " + req1 + "=@req1";
                    else if (req2 != null && req1 == null)
                        command.CommandText = "UPDATE " + TABELA + " SET " + loaded + " WHERE " + req2 + "=@req2";
                    else if (req2 != null && req1 != null)
                        command.CommandText = "UPDATE " + TABELA + " SET " + loaded + " WHERE " + req1 + "=@req1 AND " + req2 + "=@req2";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                //Logger.Info("[AllUtils.UpdateDB5] " + ex.ToString());
                return false;
            }
        }
        public static bool UpdateDB(string TABELA, string COLUNA, object VALOR, string req1, object valorReq1, string req2, object valorReq2)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@valor", VALOR);
                    if (req1 != null)
                        command.Parameters.AddWithValue("@req1", valorReq1);
                    if (req2 != null)
                        command.Parameters.AddWithValue("@req2", valorReq2);
                    if (req1 != null && req2 == null)
                        command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor WHERE " + req1 + "=@req1";
                    else if (req2 != null && req1 == null)
                        command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor WHERE " + req2 + "=@req2";
                    else if (req2 != null && req1 != null)
                        command.CommandText = "UPDATE " + TABELA + " SET " + COLUNA + "=@valor WHERE " + req1 + "=@req1 AND " + req2 + "=@req2";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Info("[AllUtils.UpdateDB6] " + ex.ToString());
                return false;
            }
        }
        public static bool DeleteDB(string TABELA, string req1, object valorReq1)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@req1", valorReq1);
                    command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req1 + "=@req1";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return false;
            }
        }
        public static bool DeleteDB(string TABELA, string req1, object valorReq1, string req2, object valorReq2)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;
                    if (req1 != null)
                        command.Parameters.AddWithValue("@req1", valorReq1);
                    if (req2 != null)
                        command.Parameters.AddWithValue("@req2", valorReq2);
                    if (req1 != null && req2 == null)
                        command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req1 + "=@req1";
                    else if (req2 != null && req1 == null)
                        command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req2 + "=@req2";
                    else if (req2 != null && req1 != null)
                        command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req1 + "=@req1 AND " + req2 + "=@req2";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return false;
            }
        }
        public static bool DeleteDB(string TABELA, string req1, object[] valorReq1, string req2, object valorReq2)
        {
            if (valorReq1.Length == 0)
                return false;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;

                    string loaded = "";
                    List<string> parameters = new List<string>();
                    for (int i = 0; i < valorReq1.Length; i++)
                    {
                        object obj = valorReq1[i];
                        string param = "@valor" + i;
                        command.Parameters.AddWithValue(param, obj);
                        parameters.Add(param);
                    }
                    loaded = string.Join(",", parameters.ToArray());
                    command.Parameters.AddWithValue("@req2", valorReq2);
                    command.CommandText = "DELETE FROM " + TABELA + " WHERE " + req1 + " in (" + loaded + ") AND " + req2 + "=@req2";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return false;
            }
        }
        public static byte[] Decrypt(byte[] data, int shift)
        {
            //essa decrypt serve pra tudo no pb ate pra mudar files pesquisa por bitrotate

            byte[] newBuffer = new byte[data.Length];
            Array.Copy(data, 0, newBuffer, 0, newBuffer.Length);
            byte lastElement = newBuffer[newBuffer.Length - 1];
            for (int i = newBuffer.Length - 1; i > 0; --i)
                newBuffer[i] = (byte)(((newBuffer[i - 1] & 255) << (8 - shift)) | ((newBuffer[i] & 255) >> shift));
            newBuffer[0] = (byte)((lastElement << (8 - shift)) | ((newBuffer[0] & 255) >> shift));
            return newBuffer;
        }
        public static ushort getCardFlags(int missionId, int cardIdx, byte[] arrayList)
        {
            if (missionId == 0)
                return 0;
            int result = 0;
            List<Card> list = MissionCardXML.GetCards(missionId, cardIdx);
            for (int i = 0; i < list.Count; i++)
            {
                Card card = list[i];
                if (arrayList[card._arrayIdx] >= card._missionLimit)
                    result |= card._flag;
            }
            return (ushort)result;
        }
        public static byte[] getCardFlags(int missionId, byte[] arrayList)
        {
            if (missionId == 0)
                return new byte[20];
            List<Card> list = MissionCardXML.GetCards(missionId);
            if (list.Count == 0)
                return new byte[20];
            using (SendGPacket p = new SendGPacket(20))
            {
                int result = 0;
                for (int i = 0; i < 10; i++)
                {
                    List<Card> list2 = MissionCardXML.GetCards(list, i);
                    for (int i2 = 0; i2 < list2.Count; i2++)
                    {
                        Card card = list2[i2];
                        if (arrayList[card._arrayIdx] >= card._missionLimit)
                            result |= card._flag;
                    }
                    p.writeH((ushort)result);
                    result = 0;
                }
                return p.mstream.ToArray();
            }
        }
        public static uint GetPlayerStatus(AccountStatus status, bool isOnline)
        {
            GetPlayerLocation(status, isOnline, out FriendState state, out int roomId, out int channelId, out int serverId);
            return GetPlayerStatus(roomId, channelId, serverId, (int)state);
        }
        public static uint GetPlayerStatus(int roomId, int channelId, int serverId, int stateId)
        {
            int p1 = ((stateId & 0xFF) << 28),
                 p2 = ((serverId & 0xFF) << 20),
                 p3 = ((channelId & 0xFF) << 12),
                 p4 = (roomId & 0xFFF);
            return (uint)(p1 | p2 | p3 | p4);
        }
        public static ulong GetPlayerStatus(int clanFId, int roomId, int channelId, int serverId, int stateId)
        {
            long p1 = ((clanFId & 0xFFFFFFFF) << 32),
                p2 = ((stateId & 0xFF) << 28),
                p3 = ((serverId & 0xFF) << 20),
                p4 = ((channelId & 0xFF) << 12),
                p5 = (roomId & 0xFFF);
            return (ulong)(p1 | p2 | p3 | p4 | p5);
        }
        public static ulong GetClanStatus(AccountStatus status, bool isOnline)
        {
            GetPlayerLocation(status, isOnline, out FriendState state, out int roomId, out int channelId, out int serverId, out int clanFId);
            return GetPlayerStatus(clanFId, roomId, channelId, serverId, (int)state);
        }
        public static ulong GetClanStatus(FriendState state) =>
             GetPlayerStatus(0, 0, 0, 0, (int)state);
        public static uint GetFriendStatus(Friend f)
        {
            PlayerInfo Info = f.player;
            if (Info == null)
                return 0;
            FriendState state = 0;
            int serverId = 0, channelId = 0, roomId = 0;
            if (f.removed)
                state = FriendState.Offline;
            else if (f.state > 0)
                state = (FriendState)f.state;
            else
                GetPlayerLocation(Info._status, Info._isOnline, out state, out roomId, out channelId, out serverId);
            return GetPlayerStatus(roomId, channelId, serverId, (int)state);
        }
        public static uint GetFriendStatus(Friend f, FriendState stateN)
        {
            PlayerInfo Info = f.player;
            if (Info == null)
                return 0;
            FriendState state = stateN;
            int serverId = 0, channelId = 0, roomId = 0;
            if (f.removed)
                state = FriendState.Offline;
            else if (f.state > 0)
                state = (FriendState)f.state;
            else if (stateN == 0)
                GetPlayerLocation(Info._status, Info._isOnline, out state, out roomId, out channelId, out serverId);
            return GetPlayerStatus(roomId, channelId, serverId, (int)state);
        }
        public static void GetPlayerLocation(AccountStatus status, bool isOnline, out FriendState state, out int roomId, out int channelId, out int serverId)
        {
            roomId = 0;
            channelId = 0;
            serverId = 0;
            if (isOnline)
            {
                if (status.roomId != 255)
                {
                    roomId = status.roomId;
                    channelId = status.channelId;
                    state = FriendState.Room;
                }
                else if (status.roomId == 255 && status.channelId != 255)
                {
                    channelId = status.channelId;
                    state = FriendState.Lobby;
                }
                else if (status.roomId == 255 && status.channelId == 255)
                    state = FriendState.Online;
                else state = FriendState.Offline;
                if (status.serverId != 255)
                    serverId = status.serverId;
            }
            else state = FriendState.Offline;
        }
        public static void GetPlayerLocation(AccountStatus status, bool isOnline, out FriendState state, out int roomId, out int channelId, out int serverId, out int clanFId)
        {
            roomId = 0;
            channelId = 0;
            serverId = 0;
            clanFId = 0;
            if (isOnline)
            {
                if (status.roomId != 255)
                {
                    roomId = status.roomId;
                    channelId = status.channelId;
                    state = FriendState.Room;
                }
                else if ((status.clanFId != 255 || status.roomId == 255) && status.channelId != 255)
                {
                    channelId = status.channelId;
                    state = FriendState.Lobby;
                }
                else if (status.roomId == 255 && status.channelId == 255)
                    state = FriendState.Online;
                else state = FriendState.Offline;
                if (status.serverId != 255)
                    serverId = status.serverId;
                if (status.clanFId != 255)
                    clanFId = status.clanFId + 1;
            }
            else state = FriendState.Offline;
        }
        public static string Gen5(string text)
        {
            using (var hmacMD5 = new HMACMD5(Encoding.UTF8.GetBytes("/x!a@r-$r%an¨.&e&+f*f(f(a)")))
            {
                byte[] data = hmacMD5.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));
                return sBuilder.ToString();
            }
        }
        /// <summary>
        /// Gera Informações do Id de um item.
        /// </summary>
        /// <param name="id">Id do item</param>
        /// <param name="type">Tipo de Informação. 1 = Inicio (ITEM_CLASS); 2 = Usage; 3 = Meio (ClassType); 4 = Final (Number)</param>
        /// <returns></returns>
        public static int GetIdStatics(int id, int type)
        {
            switch (type)
            {
                case 1: return id / 100000000; //primeiros valores - classtype
                case 2: return (id % 100000000) / 1000000; //usage
                case 3: return (id % 1000000) / 1000; //valores do meio - type
                case 4: return id % 1000; //ultimos 3 valores - number
                default: return 0;
            }
        }
        /// <summary>
        /// Retorna a classe de um equipamento.
        /// </summary>
        /// <param name="id">Id do item</param>
        /// <returns></returns>
        public static ClassType GetIdClassType(int id) => (ClassType)((id % 1000000) / 1000);
        public static int CreateItemId(int class1, int usage, int classtype, int number) =>
             (class1 * 100000000) + (usage * 1000000) + (classtype * 1000) + number;
    }
}