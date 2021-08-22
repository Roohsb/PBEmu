using Core.DB_Battle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Core.managers.events
{
    public class EventXmasSyncer
    {
        private static List<EventXmasModel> _events = new List<EventXmasModel>();
        public static void GenerateList()
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SELECT * FROM events_xmas";
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader data = command.ExecuteReader())// so o srv fodase
                    {
                        while (data.Read())
                        {
                            _events.Add(new EventXmasModel
                            {
                                startDate = (uint)data.GetInt64(0),
                                endDate = (uint)data.GetInt64(1)
                            });
                        }
                        if (data != null)
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
        public static EventXmasModel GetRunningEvent()
        {
            try
            {
                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < _events.Count; i++)
                {
                    EventXmasModel ev = _events[i];
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
    public class EventXmasModel
    {
        public uint startDate, endDate;
    }
}