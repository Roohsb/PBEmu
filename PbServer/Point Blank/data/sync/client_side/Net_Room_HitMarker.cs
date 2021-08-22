using Core;
using Core.managers;
using Core.models.account.players;
using Core.models.enums;
using Core.models.shop;
using Core.server;
using Game.data.model;
using Game.data.xml;
using Game.global.serverpacket;
using System;

namespace Game.data.sync.client_side
{
    public static class Net_Room_HitMarker
    {
        public static void Load(ReceiveGPacket p)
        {
            int roomId = p.ReadH();
            int channelId = p.ReadH();
            byte killerIdx = p.ReadC();
            byte deathtype = p.ReadC();
            byte hitEnum = p.ReadC();
            int damage = p.ReadH();
            int life = p.ReadD();
            if (p.GetBuffer().Length > 23)
                SendDebug.SendInfo("[Invalid MARKER: " + BitConverter.ToString(p.GetBuffer()) + "]");
            Channel ch = ChannelsXML.getChannel(channelId);
            if (ch == null)
                return;
            Room room = ch.GetRoom(roomId);
            if (room != null && room._state == RoomState.Battle)
            {
                Account player = room.GetPlayerBySlot(killerIdx);

                if (player != null)
                {
                    if (!player.damage)
                        return;
                    string warn = "";
                    if (deathtype == 10)
                        warn = Translation.GetLabel("LifeRestored", damage);
                    switch (hitEnum)
                    {
                        case 0: warn = "Damage Applied " + damage + "  / '" + life + " HP'   -->  CHEST."; break;
                        case 1: warn = "Damage Applied " + damage + "  / '" + life + " HP'   -->  HEAD."; break;
                        case 2: warn = Translation.GetLabel("HitMarker3"); break;
                        case 3: warn = Translation.GetLabel("HitMarker4"); break;
                        case 4: warn = "Player you are applying damage to has infinite health."; break;
                    }
                    player.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, player.GetSessionId(), 0, true, warn));
                }
            }
        }
    }
}