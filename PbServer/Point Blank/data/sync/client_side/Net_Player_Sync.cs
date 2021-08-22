using Core.models.enums;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.data.xml;
using Game.global.serverpacket;

namespace Game.data.sync.client_side
{
    public static class Net_Player_Sync
    {
        public static void Load(ReceiveGPacket p)
        {
            long playerId = p.ReadQ();
            int type = p.ReadC();
            int rank = p.ReadC();
            int gold = p.ReadD();
            int cash = p.ReadD();
            Account player = AccountManager.GetAccount(playerId, true);
            if (player == null)
                return;

            if (type == 0)
            {
                player._rank = rank;
                player._gp = gold;
                player._money = cash;
            }
        }
        public static void Location(ReceiveGPacket p)
        {
            byte slot = p.ReadC();
            int canal = p.ReadH();
            int RoomID = p.ReadH();
            float x = p.ReadT();
            float y = p.ReadT();
            string texto = "Camera X: " + x + ", Y: " + y + "";
            Channel ch = ChannelsXML.getChannel(canal);
            if (ch == null)
                return;
            Room room = ch.GetRoom(RoomID);
            if (room != null && room._state == RoomState.Battle)
            {
                Account player = room.GetPlayerBySlot(slot);
                if (player != null)
                {
                    if (player.location)
                        player.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, player.GetSessionId(), 0, true, texto));
                }
            }
        }
        public static void ExcptionIP(ReceiveGPacket p)
        {
            string ip = p.ReadS(p.ReadC());
            foreach (var client in GameManager._socketList.Values)
            {
                Account account = client._player;
                if (account != null && account._isOnline && account.PublicIP.ToString() == ip)
                {
                    account.SendPacket(new AUTH_ACCOUNT_KICK_PAK(0));
                    string str = account.player_name; //Diga não a referenciar ao objeto!
                    account.Close(1000);
                    if (ComDiv.UpdateDB("accounts", "access_level", -1, "player_id", account.player_id))
                    {
                        SendDebug.SendInfo("Jogador foi desconectado por violação. -> " + str);
                    }
                }
            }
        }
    }
}