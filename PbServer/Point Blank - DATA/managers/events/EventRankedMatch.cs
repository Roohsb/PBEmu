using Core.DB_Battle;
using Core.server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Core.managers.events
{
    public class EventRankedMatch
    {
        private static List<EventLoginModel> _events = new List<EventLoginModel>();
        public static void GenerateList()
        {

        }


        public static void ReGenList()
        {
            _events.Clear();
            GenerateList();
        }

        public class EventRankedMatchh
        {

        }
    }
}
