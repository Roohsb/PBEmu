using Core;
using Core.managers;
using Core.models.account.players;
using Core.server;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_QUEST_DELETE_CARD_SET_REC : ReceiveGamePacket
    {
        private uint erro;
        private int missionIdx;
        public BASE_QUEST_DELETE_CARD_SET_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            missionIdx = ReadC();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                PlayerMissions missions = p._mission;
                bool failed = false;
                if (missionIdx >= 3 || missionIdx == 0 && missions.mission1 == 0 || missionIdx == 1 && missions.mission2 == 0 ||
                    missionIdx == 2 && missions.mission3 == 0)
                    failed = true;
                if (!failed && PlayerManager.UpdateMissionId(p.player_id, 0, missionIdx) &&
                    ComDiv.UpdateDB("player_missions", "owner_id", p.player_id, new string[]
                    {
                        "card" + (missionIdx + 1),
                        "mission" + (missionIdx + 1)
                    }, 0, new byte[0]))
                {
                    switch (missionIdx)
                    {
                        case 0:
                            {
                                missions.mission1 = 0;
                                missions.card1 = 0;
                                missions.list1 = new byte[40];
                                break;
                            }
                        case 1:
                            {
                                missions.mission2 = 0;
                                missions.card2 = 0;
                                missions.list2 = new byte[40];
                                break;
                            }
                        case 2:
                            {
                                missions.mission3 = 0;
                                missions.card3 = 0;
                                missions.list3 = new byte[40];
                                break;
                            }
                    }
                }
                else erro = 0x80001050;
                _client.SendPacket(new BASE_QUEST_DELETE_CARD_SET_PAK(erro, p));
            }
            catch (Exception ex)
            {
                Logger.Info("BASE_QUEST_DELETE_CARD_SET_REC: " + ex.ToString());
            }
        }
    }
}