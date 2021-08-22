using Core.managers;
using Core.models.account;
using Core.models.account.players;
using Core.server;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class LOBBY_ENTER_REC : ReceiveGamePacket
    {
        public LOBBY_ENTER_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
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
                p.LastLobbyEnter = DateTime.Now;
                if (p.channelId >= 0)
                {
                    Channel ch = p.GetChannel();
                    if (ch != null)
                        ch.AddPlayer(p.Session);
                }
                Room room = p._room;
                if (room != null)
                {
                    if (p._slotId >= 0 && (int)room._state >= 2 && (int)room._slots[p._slotId].state >= 9)
                        goto JumpToPacket;
                    else
                        room.RemovePlayer(p, false);
                }
                AllUtils.SyncPlayerToFriends(p, false);
                AllUtils.SyncPlayerToClanMembers(p);
                AllUtils.GetXmasReward(p);
                JumpToPacket:
                _client.SendPacket(new LOBBY_ENTER_PAK());
                int contagemdex = p.CountLobbys += 1;
                string valor = p.player_name != "" ? "o jogador entrou no lobby: " + p.player_name + " " + contagemdex + "x (times) " : "New account entered the Lobby:" + p.login;
                SendDebug.SendInfo(valor);
                if (SetVip(p))
                {
                    SendDebug.SendInfo("VIP exhausted for the player. " + p.player_name);
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[LOBBY_ENTER_REC] " + ex.ToString());
            }
        }
        public bool SetVip(Account player)
        {
            if (!player.HaveVipTotal())
                return false;
            PlayerVip pvip = player._pccafes;
            if (pvip.data_inicio == 0 || pvip.data_fim == 0)
            {
                Console.WriteLine("-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-");
                Console.WriteLine(" O VIP do jogador está incorreto. então o jogador levou dc. verifique a conta dele.");
                Console.WriteLine($" -         NickName {player.player_name}, id {player.player_id}.");
                Console.WriteLine("-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-");
                player.SendPacket(new AUTH_ACCOUNT_KICK_PAK(0));
                player.Close(1000);
            }
            else if (PlayerVip.DateAtual() > pvip.data_fim)
            {
                if (PlayerManager.UpdateAccountVip(player.player_id, 0))
                    player.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(CreateMessage(player.player_id)));
                PlayerManager.UpdatePlayerVip(player.player_id, 0, 0);
                player.pc_cafe = 0;
                if (ComDiv.UpdateDB("accounts", "name_color", 0, "player_id", player.player_id))
                    player.name_color = 0;
                pvip.data_inicio = 0;
                pvip.data_fim = 0;
                return true;
            }
            else
                player.SetHideColorVip();
            player.SetHideGmColor();
            return false;
        }
        private Message CreateMessage(long owner)
        {
            Message msg = new Message(15)
            {
                sender_name = LorenstudioSettings.ProjectName,
                sender_id = 0,
                text = "Seu VIP expirou, entre em contato com a equipe e renove-o.",
                state = 1
            };
            if (!MessageManager.CreateMessage(owner, msg))
                return null;
            return msg;
        }
    }
}