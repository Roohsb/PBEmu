﻿using Core;
using Core.models.enums;
using Core.models.enums.match;
using Game.data.model;
using Game.data.xml;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_ACCEPT_BATTLE_REC : ReceiveGamePacket
    {
        private int id, serverInfo, type;
        private uint erro;
        public CLAN_WAR_ACCEPT_BATTLE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            ReadD();
            id = ReadH();
            serverInfo = ReadH();
            type = ReadC();
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null)
                    return;
                Match mt = player._match;
                int channelId = serverInfo - ((serverInfo / 10) * 10);
                Match mt2 = ChannelsXML.getChannel(channelId).GetMatch(id);
                if (mt != null && mt2 != null && player.matchSlot == mt._leader)
                {
                    if (type == 1)
                    {
                        if (mt.formação != mt2.formação)
                            erro = 2147487890;
                        else if (mt2.GetCountPlayers() != mt.formação || mt.GetCountPlayers() != mt.formação)
                            erro = 2147487889;
                        else if (mt2._state == MatchState.Play || mt._state == MatchState.Play)
                            erro = 2147487888;
                        else
                        {
                            mt._state = MatchState.Play;
                            Account pM = mt2.GetLeader();
                            if (pM != null && pM._match != null)
                            {
                                pM.SendPacket(new CLAN_WAR_ENEMY_INFO_PAK(mt));
                                pM.SendPacket(new CLAN_WAR_CREATED_ROOM_PAK(mt));
                                mt2._slots[pM.matchSlot].state = SlotMatchState.Ready;
                            }
                            mt2._state = MatchState.Play;
                        }
                    }
                    else
                    {
                        Account pM = mt2.GetLeader();
                        if (pM != null && pM._match != null)
                            pM.SendPacket(new CLAN_WAR_RECUSED_BATTLE_PAK(0x80001093));
                    }
                }
                else erro = 0x80001094;
                _client.SendPacket(new CLAN_WAR_ACCEPTED_BATTLE_PAK(erro));
            }
            catch (Exception ex)
            {
                Logger.Info("CLAN_WAR_ACCEPT_BATTLE_REC: " + ex.ToString());
            }
        }
    }
}