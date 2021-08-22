using Core.DB_Battle;
using Core.server;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Core.managers.events
{
    public class EventLoginSyncer
    {
        private static List<EventLoginModel> _events = new List<EventLoginModel>();
        public static void GenerateList()
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SELECT * FROM events_login";
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            EventLoginModel ev = new EventLoginModel
                            {
                                startDate = (uint)data.GetInt64(0),
                                endDate = (uint)data.GetInt64(1),
                                _rewardId = data.GetInt32(2),
                                _count = data.GetInt32(3)
                            };
                            ev._category = ComDiv.GetItemCategory(ev._rewardId);
                            if (ev._rewardId < 100000000)
                            {
                                Logger.Error("[EventLogin] Evento com premiação incorreta! [Id: " + ev._rewardId + "]");
                            }
                            else
                                _events.Add(ev);
                        }
                        if(data != null)
                            data.Close();
                    }
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public static void ReGenList()
        {
            _events.Clear();
            GenerateList();
        }
        public static EventLoginModel GetRunningEvent()
        {
            try
            {
                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < _events.Count; i++)
                {                    
                    EventLoginModel ev = _events[i];
                    if (ev.startDate <= date && date < ev.endDate)
                        return ev;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return null;
        }
    }
    public class EventLoginModel
    {
        public int _rewardId, _category, _count;
        public uint startDate, endDate;
    }
}