using Core;
using Core.models.account.players;
using Core.server;
using Game.data.model;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_MISSION_ENTER_REC : ReceiveGamePacket
    {
        private int cardIdx, actualMission, cardFlags;
        public BASE_MISSION_ENTER_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            actualMission = ReadC();
            cardIdx = ReadC();
            cardFlags = ReadUH();
        }

        public override void Run()
        {
            try
            {
                if (_client == null)
                    return;
                Account p = _client._player;
                if (p == null)
                    return;
                PlayerMissions missions = p._mission;
                DBQuery query = new DBQuery();
                if (missions.GetCard(actualMission) != cardIdx)
                {
                    switch (actualMission)
                    {
                        case 0: missions.card1 = cardIdx; break;
                        case 1: missions.card2 = cardIdx; break;
                        case 2: missions.card3 = cardIdx; break;
                        case 3: missions.card4 = cardIdx; break;
                    }
                    query.AddQuery("card" + (actualMission + 1), cardIdx);
                }
                missions.selectedCard = cardFlags == ushort.MaxValue;
                if (missions.actualMission != actualMission)
                {
                    query.AddQuery("actual_mission", actualMission);
                    missions.actualMission = actualMission;
                }
                ComDiv.UpdateDB("player_missions", "owner_id", _client.player_id, query.GetTables(), query.GetValues());
            }
            catch (Exception ex)
            {
                Logger.Info("BASE_MISSION_ENTER_REC: " + ex.ToString());
            }
        }
    }
}