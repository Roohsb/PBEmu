using Core.DB_Battle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Core.managers.events
{
    public class EventMapSyncer
    {
        private static List<EventMapModel> _events = new List<EventMapModel>();
        public static void GenerateList()
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SELECT * FROM events_mapbonus";
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            _events.Add(new EventMapModel
                            {
                                _startDate = (uint)data.GetInt64(0),
                                _endDate = (uint)data.GetInt64(1),
                                _mapId = data.GetInt32(2),
                                _stageType = data.GetInt32(3),
                                _percentXp = data.GetInt32(4),
                                _percentGp = data.GetInt32(5),
                            });
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
        public static EventMapModel GetRunningEvent()
        {
            try
            {
                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < _events.Count; i++)
                {
                    EventMapModel ev = _events[i];
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
        public static bool EventIsValid(EventMapModel ev, int map, int stageType) =>
            ev != null && (ev._mapId == map || ev._stageType == stageType);
    }
    public class EventMapModel
    {
        public int _mapId, _percentXp, _percentGp, _stageType;
        public uint _startDate, _endDate;
    }
}