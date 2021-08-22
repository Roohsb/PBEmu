using Core;
using Core.server;
using System.Data.Sql;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BOX_MESSAGE_VIEW_REC : ReceiveGamePacket
    {
        private int msgsCount;
        private List<int> messages = new List<int>();
        public BOX_MESSAGE_VIEW_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            msgsCount = ReadC();
            for (int i = 0; i < msgsCount; i++)
                messages.Add(ReadD());
        }

        public override void Run()
        {
            try
            {
                if (_client == null || _client._player == null || msgsCount == 0)
                    return;
                ComDiv.UpdateDB("player_messages", "object_id", messages.ToArray(),"owner_id", _client.player_id, new string[] { "expire", "state" },long.Parse(DateTime.Now.AddDays(7).ToString("yyMMddHHmm")), 0);
            }
            catch (Exception ex)
            {
                Logger.Info("[BOX_MESSAGE_VIEW_REC] " + ex.ToString());
            }
        }
    }
}