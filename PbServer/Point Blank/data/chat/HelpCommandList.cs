using System;
using Core;
using Core.models.room;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.data.chat
{
    public static class HelpCommandList
    {
        /// <summary>
        /// Acesso 3.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static string GetList1(Account player)
        {
            if ((int)player.access >= 3)
            {
                if (InGame(player))
                    return Translation.GetLabel("InGameBlock");
                string comandos = Translation.GetLabel("HelpListTitle3");
                comandos += "\n\n" + Translation.GetLabel("NickHistoryByID");
                comandos += "\n" + Translation.GetLabel("IDHistoryByNick");
                comandos += "\n" + Translation.GetLabel("KickPlayer");
                comandos += "\n" + Translation.GetLabel("EnableDisableGMColor");
                comandos += "\n" + Translation.GetLabel("AntiKickActive");
                comandos += "\n" + Translation.GetLabel("RoomUnlock");
                comandos += "\n" + Translation.GetLabel("AFKCounter");
                comandos += "\n" + Translation.GetLabel("AFKKick");
                comandos += "\n" + Translation.GetLabel("PlayersCountInServer");
                comandos += "\n" + Translation.GetLabel("PlayersCountInServer2");
                comandos += "\n" + "!alls - (Ver todos os jogadores online por nick)";
                comandos += "\n" + "! atenção playerid msg - (enviar anúncio MSG para um jogador)";
                using SERVER_MESSAGE_ANNOUNCE_PAK data = new SERVER_MESSAGE_ANNOUNCE_PAK(comandos);
                player.SendPacket(data);
                return Translation.GetLabel("HelpListList3");
            }
            else return Translation.GetLabel("HelpListNoLevel");
        }
        public static string GetList2(Account player)
        {
            if ((int)player.access >= 4)
            {
                if (InGame(player))
                    return Translation.GetLabel("InGameBlock");
                string comandos = Translation.GetLabel("HelpListTitle4");
                comandos += "\n\n" + Translation.GetLabel("MsgToAllServer");
                comandos += "\n" + Translation.GetLabel("MsgToAllRoom");
               // comandos += "\n" + Translation.GetLabel("ChangeMapId");
                //comandos += "\n" + Translation.GetLabel("ChangeRoomTime");           
                comandos += "\n" + Translation.GetLabel("Give10Cash");
                comandos += "\n" + Translation.GetLabel("Give10Gold");
                comandos += "\n" + "!gp3 (playerid) (gold) - de gold ao jogador.";
                comandos += "\n" + "!cp3 (playerid) (cash) - de cash ao jogador.";
                comandos += "\n" + Translation.GetLabel("KickAll");
                comandos += "\n" + Translation.GetLabel("SendGift");
                comandos += "\n" + Translation.GetLabel("GoodsFound");
                comandos += "\n" + Translation.GetLabel("SimpleBanNormal");
                comandos += "\n" + Translation.GetLabel("AdvancedBanNormal");
                comandos += "\n" + Translation.GetLabel("UnbanNormal");
                comandos += "\n" + Translation.GetLabel("GetPlayersByIP");
                comandos += "\n" + Translation.GetLabel("BanReason");
                comandos += "\n" + Translation.GetLabel("GetPlayerInfos");
                comandos += "\n" + Translation.GetLabel("OpenRoomSlot");
                comandos += "\n" + Translation.GetLabel("OpenRandomRoomSlot");
                comandos += "\n" + Translation.GetLabel("OpenAllClosedRoomSlots");
                comandos += "\n" + Translation.GetLabel("TakeTitles");
                comandos += "\n" + "boxI (PlayerID) (msg) - (To Send BOX to player.)";
                comandos += "\n" + "boxA (msg) - (To Send BOX to everyone.)";
               // comandos += "\n" + "stcolor (id) (color: 1 to 10) - (send color to the player.)";

                using SERVER_MESSAGE_ANNOUNCE_PAK data = new SERVER_MESSAGE_ANNOUNCE_PAK(comandos);
                player.SendPacket(data);
                return Translation.GetLabel("HelpListList4");
            }
            else return Translation.GetLabel("HelpListNoLevel");
        }
        public static string GetList3(Account player)
        {
            if ((int)player.access >= 5)
            {
                if (InGame(player))
                    return Translation.GetLabel("InGameBlock");
                string comandos = Translation.GetLabel("HelpListTitle5");
                comandos += "\n\n" + Translation.GetLabel("ChangeRank");
                comandos += "\n" + "mudar nick: !changenick nick";
                comandos += "\n" + Translation.GetLabel("GetBanInfo");
                comandos += "\n" + Translation.GetLabel("UnbanEtern");
                comandos += "\n" + Translation.GetLabel("CreateItemPt1");
                comandos += "\n" + Translation.GetLabel("CreateItemPt2");
                comandos += "\n" + Translation.GetLabel("CreateGoldItem"); 
                comandos += "\n" + Translation.GetLabel("V2ReloadShop");
                comandos += "\n" + Translation.GetLabel("ChangeAnnounce");
                comandos += "\n" + Translation.GetLabel("SetCashD");
                comandos += "\n" + Translation.GetLabel("SetGoldD");
                comandos += "\n" + Translation.GetLabel("SetVip");
                comandos += "\n" + Translation.GetLabel("SetAcess");
                comandos += "\n" + "encontrar id dentro da sala: !sloti slot";
                comandos += "\n" + "!taket (player_id) - (dar títulos aos jogadores)";
                comandos += "\n" + "!exitR (Nick) - (kikar da sala sem votekickk)";
                using SERVER_MESSAGE_ANNOUNCE_PAK data = new SERVER_MESSAGE_ANNOUNCE_PAK(comandos);
                player.SendPacket(data);
                return Translation.GetLabel("HelpListList5");
            }
            else return Translation.GetLabel("HelpListNoLevel");
        }     
        private static bool InGame(Account player)
        {
            Room room = player._room;
            if (room != null && room.GetSlot(player._slotId, out SLOT slot) && (int)slot.state >= 9)
                return true;
            return false;
        }
        public static string GetList4(Account player)
        {
            if ((int)player.access >= 6)
            {
                if (InGame(player))
                    return Translation.GetLabel("InGameBlock");
                string comandos = "Nível 7 Novos Comandos Abaixo: ";
                comandos += "\n\n" + Translation.GetLabel("ChangeRoomType");
                comandos += "\n" + Translation.GetLabel("ChangeRoomSpecial");
                comandos += "\n" + Translation.GetLabel("ChangeRoomWeapons");
                comandos += "\n" + Translation.GetLabel("ChangeUDP");
                comandos += "\n" + Translation.GetLabel("EnableMissions");
                comandos += "\n" + "!ciday1 (playerID) (itemID) (day) - arma para jogador";
                comandos += "\n" + "!ciday2 (itemID) (day)  - armas para todos";
                comandos += "\n" + "!ciday3 (playerID) (itemID) (count) - arma para jogador";
                comandos += "\n" + "!ciday4 (itemID) (count)  - armas para todos";
                comandos += "\n" + "!blackmarket (Ative a loja Black Friday)";
                comandos += "\n" + "!setquiz t - (evento matemático t (true) f (false))";
                comandos += "\n" + "!chat (playerid) (minutes) - (deixa o jogador com o chat bloqueado)";
                comandos += "\n" + "!chatN (nick) (minutes) - (deixa o jogador com o chat bloqueado)";
                comandos += "\n" + "!mute (agora você pode mutar o quarto)";
                comandos += "\n" + "!hpinfinity - (HP fica infinito, Normal[OK] Bot [!]))";
          //      comandos += "\n" + ".hp (100 à 999) - (HP é adicionado, Normal[OK] Bot [!]))";
               // comandos += "\n" + "!BanIP (playerid) - (Banir a conexão completa do jogador com o servidor). ";
                //comandos += "\n" + "!uBanIP (playerid) - (restabelecer a conexão do jogador com o servidor).";
                //comandos += "\n" + "!BanMC (playerid) - (banir um jogador pelo mac).";
                using SERVER_MESSAGE_ANNOUNCE_PAK data = new SERVER_MESSAGE_ANNOUNCE_PAK(comandos);
                player.SendPacket(data);
                return Translation.GetLabel("Novos comandos carregados.");
            }
            else return Translation.GetLabel("HelpListNoLevel");
        }

        public static string GetListVIP(Account player)
        {
            if (InGame(player))
                return Translation.GetLabel("InGameBlock");
            string comandos = " VIP PANEL " + LorenstudioSettings.ProjectName;
            comandos += "\n" + Translation.GetLabel("TakeTitles");
            comandos += "\n" + Translation.GetLabel("PlayersCountInServer");
            comandos += "\n" + Translation.GetLabel("IDHistoryByNick");
            using SERVER_MESSAGE_ANNOUNCE_PAK data = new SERVER_MESSAGE_ANNOUNCE_PAK(comandos);
            player.SendPacket(data);
            return Translation.GetLabel("Painel VIP carregado.");
        }
    }
}