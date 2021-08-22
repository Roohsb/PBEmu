using Core.DB_Battle;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Core.managers.events
{
    public class EventRankUpSyncer
    {
        private static List<EventUpModel> _events = new List<EventUpModel>();
        public static void GenerateList()
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SELECT * FROM events_rankup";
                    command.CommandType = CommandType.Text;
                    SqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        _events.Add(new EventUpModel
                        {
                            _startDate = (uint)data.GetInt64(0),
                            _endDate = (uint)data.GetInt64(1),
                            _percentXp = data.GetInt32(2),
                            _percentGp = data.GetInt32(3)
                        });
                    }
                    command.Dispose();
                    data.Close();
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
        public static EventUpModel GetRunningEvent()
        {
            try
            {
                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < _events.Count; i++)
                {
                    EventUpModel ev = _events[i];
                    if (ev._startDate <= date && date < ev._endDate)
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
    public class EventUpModel
    {
        public int _percentXp, _percentGp;
        public uint _startDate, _endDate;
    }
}