using Core.DB_Battle;
using Core.models.account.players;
using Core.server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Core.managers.events
{
    public class EventQuestSyncer
    {
        private static List<QuestModel> _events = new List<QuestModel>();
        public static void GenerateList()
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SELECT * FROM events_quest";
                    command.CommandType = CommandType.Text;
                    SqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        _events.Add(new QuestModel
                        {
                            startDate = (uint)data.GetInt64(0),
                            endDate = (uint)data.GetInt64(1)
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
        public static QuestModel GetRunningEvent()
        {
            try
            {
                uint date = uint.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                for (int i = 0; i < _events.Count; i++)
                {
                    QuestModel ev = _events[i];
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
        /// <summary>
        /// Reseta e atualiza as informações do jogador na DB.
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="pE"></param>
        public static void ResetPlayerEvent(long pId, PlayerEvent pE)
        {
            if (pId == 0)
                return;
            ComDiv.UpdateDB("player_events", "player_id", pId, new string[] { "last_quest_date", "last_quest_finish" }, (long)pE.LastQuestDate, pE.LastQuestFinish);
        }
    }
    public class QuestModel
    {
        public uint startDate, endDate;
    }
}