using Core;
using Core.models.account.players;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;
using System.Linq;

namespace Game.data.chat
{
    public static class CreateItem
    {
        public static string CreateItemYourself(string str, Account player)
        {
            int id = int.Parse(str.Substring(3));
            if (id < 100000000)
                return Translation.GetLabel("CreateItemWrongID");
            else if (player != null)
            {
                int category = ComDiv.GetItemCategory(id);
                player.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, player, new ItemsModel(id, category, "Command item", category == 3 ? 1 : 3, 1)));
                player.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0));
                return Translation.GetLabel("CreateItemSuccess");
            }
            else
                return Translation.GetLabel("CreateItemFail");
        }
        public static string CreateItemByNick(string str, Account player)
        {
            string txt = str.Substring(str.IndexOf(" ") + 1);
            string[] split = txt.Split(' ');
            string nick = split[0];
            int item_id = Convert.ToInt32(split[1]);
            if (item_id < 100000000)
                return Translation.GetLabel("CreateItemWrongID");
            else
            {
                Account playerO = AccountManager.GetAccount(nick, 1, 0);
                if (playerO == null)
                    return Translation.GetLabel("CreateItemFail");
                if (playerO.player_id == player.player_id)
                    return Translation.GetLabel("CreateItemUseOtherCMD");
                else
                {
                    int category = ComDiv.GetItemCategory(item_id);
                    playerO.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, playerO, new ItemsModel(item_id, category, "Command item", category == 3 ? 1 : 3, 1)));
                    playerO.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0));
                    return Translation.GetLabel("CreateItemSuccess");
                }
            }
        }
        public static string CreateItemById(string str, Account player)
        {
            string txt = str.Substring(str.IndexOf(" ") + 1);
            string[] split = txt.Split(' ');
            int item_id = Convert.ToInt32(split[1]);
            long player_id = Convert.ToInt64(split[0]);
            if (item_id < 100000000)
                return Translation.GetLabel("CreateItemWrongID");
            else
            {
                Account playerO = AccountManager.GetAccount(player_id, 0);
                if (playerO != null)
                {
                    if (playerO.player_id == player.player_id)
                        return Translation.GetLabel("CreateItemUseOtherCMD");
                    else
                    {
                        int category = ComDiv.GetItemCategory(item_id);
                        playerO.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, playerO, new ItemsModel(item_id, category, "Command item", category == 3 ? 1 : 3, 1)), false);
                        playerO.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0), false);
                        return Translation.GetLabel("CreateItemSuccess");
                    }
                }
                else
                    return Translation.GetLabel("CreateItemFail");
            }
        }

        public static string CreateGoldCupom(string str)
        {
            string txt = str.Substring(str.IndexOf(" ") + 1);
            string[] split = txt.Split(' ');
            int gold = Convert.ToInt32(split[1]);
            long player_id = Convert.ToInt64(split[0]);
            if (gold.ToString().EndsWith("00"))
            {
                if (gold < 100 || gold > 99999999)
                    return Translation.GetLabel("CreateSItemWrongID");
                Account playerO = AccountManager.GetAccount(player_id, 0);
                if (playerO != null)
                {
                    int cuponId = ComDiv.CreateItemId(15, (gold / 1000000), (gold % 1000) / 100, (gold % 1000000) / 1000);
                    playerO.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, playerO, new ItemsModel(cuponId, 3, "Gold CMD item", 1, 1)), false);
                    playerO.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0), false);
                    return Translation.GetLabel("CreateSItemSuccess", gold);
                }
                else
                    return Translation.GetLabel("CreateItemFail");
            }
            else return Translation.GetLabel("CreateSItemFail");
        }
        public static string CreateItemDias(string str, Account player)
        {
            try
            {
                string txt = str.Substring(str.IndexOf(" ") + 1);
                string[] split = txt.Split(' ');
                long playerID = long.Parse(split[0]);
                int item_id = Convert.ToInt32(split[1]);
                uint DataFixa = Convert.ToUInt32(split[2]);
                if (DataFixa < 1 || DataFixa > 365)
                    return "days cannot be longer than 1 year, and less than 1 day.";
                if (item_id < 100000000)
                    return Translation.GetLabel("CreateItemWrongID");
                else
                {
                    Account playerO = AccountManager.GetAccount(playerID, true);
                    if (playerO == null)
                        return Translation.GetLabel("CreateItemFail");
                    else
                    {
                        playerO.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, playerO, new ItemsModel(item_id, ComDiv.GetItemCategory(item_id), "item dias", 1, Calculo(DataFixa))), false);
                        playerO.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0), false);
                        return Translation.GetLabel("CreateItemSuccess");
                    }
                }
            }
            catch
            {
                return "Error occurred when adding the weapon to the player.";
            }
        }
        public static string CreateItemdiasParaTodos(string str)
        {
            try
            {
                int index = 0;
                string txt = str.Substring(str.IndexOf(" ") + 1);
                string[] split = txt.Split(' ');
                int item_id = Convert.ToInt32(split[0]);
                uint DataFixa = Convert.ToUInt32(split[1]);
                if (DataFixa < 1 || DataFixa > 365)
                    return "os dias não podem ser maiores que 1 ano e menores que 1 dia.";
                if (item_id < 100000000)
                    return Translation.GetLabel("CreateItemWrongID");
                else
                {
                    foreach (Account playerO in from client in GameManager._socketList.Values
                                                let playerO = client._player
                                                where playerO != null && playerO._isOnline
                                                select playerO)
                    {
                        playerO.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, playerO, new ItemsModel(item_id, ComDiv.GetItemCategory(item_id), "item dias", 1, Calculo(DataFixa))));
                        playerO.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0));
                        index += 1;
                    }
                    if (index > 0)
                        return "o item foi enviado para '" + index + "' Players.";
                    else
                        return "o item não foi enviado a ninguém! (tente novamente)";
                }
            }
            catch
            {
                return "Ocorreu um erro ao adicionar a arma ao jogador.";
            }
        }
        public static string CreateItemUnidade(string str, Account player)
        {
            try
            {
                string txt = str.Substring(str.IndexOf(" ") + 1);
                string[] split = txt.Split(' ');
                long playerID = long.Parse(split[0]);
                int item_id = Convert.ToInt32(split[1]);
                uint unidade = Convert.ToUInt32(split[2]);
                if (unidade < 1 || unidade > 500)
                    return "unit cannot be greater than 500, and less than 1.";
                if (item_id < 100000000)
                    return Translation.GetLabel("CreateItemWrongID");
                else
                {
                    Account playerO = AccountManager.GetAccount(playerID, true);
                    if (playerO == null)
                        return Translation.GetLabel("CreateItemFail");
                    else
                    {
                        playerO.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, playerO, new ItemsModel(item_id, ComDiv.GetItemCategory(item_id), "item unidade", 1, unidade)), false);
                        playerO.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0), false);
                        return Translation.GetLabel("CreateItemSuccess");
                    }
                }
            }
            catch
            {
                return "Error occurred when adding the weapon to the player.";
            }
        }
        public static string CreateItemUnidadesParaTodos(string str)
        {
            try
            {
                int index = 0;
                string txt = str.Substring(str.IndexOf(" ") + 1);
                string[] split = txt.Split(' ');
                int item_id = Convert.ToInt32(split[0]);
                uint unidade = Convert.ToUInt32(split[1]);
                if (unidade < 1 || unidade > 500)
                    return "unit cannot be greater than 500 and less than 1.";
                if (item_id < 100000000)
                    return Translation.GetLabel("CreateItemWrongID");
                else
                {
                    foreach (Account playerO in from client in GameManager._socketList.Values
                                                let playerO = client._player
                                                where playerO != null && playerO._isOnline
                                                select playerO)
                    {
                        playerO.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, playerO, new ItemsModel(item_id, ComDiv.GetItemCategory(item_id), "item unidade", 1, unidade)));
                        playerO.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0));
                        index += 1;
                    }
                    if (index > 0)
                        return "item was sent to '" + index + "' Players.";
                    else
                        return "item was not sent to anyone! (try again)";
                }
            }
            catch
            {
                return "Error occurred when adding the weapon to the player.";
            }
        }
        public static uint Calculo(uint dias)
        {
            uint valor = dias * 86400;
            return valor;
        }
    }
}