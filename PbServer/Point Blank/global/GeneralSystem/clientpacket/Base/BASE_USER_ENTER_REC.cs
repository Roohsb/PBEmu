using Core;
using Core.managers;
using Core.managers.events;
using Core.models.account.players;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_USER_ENTER_REC : ReceiveGamePacket
    {
        private object Daily;
        private string login;
        private byte[] LocalIP;
        private long pId;
        private uint erro;
        public Account p;
        private byte Host;
        public BASE_USER_ENTER_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            login = ReadS(ReadC());
            pId = ReadQ();
            Host = ReadC();
            LocalIP = ReadB(4);
        }
        public void CheckIntegridy()
        {
            Account account = AccountManager.GetAccount(pId, true);
            if (account != null && account._isOnline)
            {
                if (account.LocalIP != LocalIP)
                    erro = 0x80000000;
            }
        }

        public override void Run()
        {
            if (_client == null)
                return;
            try
            {
                if (LocalIP[0] == 0 || LocalIP[3] == 0)
                {
                    erro = 0x80000000;
                    SendDebug.SendInfo("[Aviso] LocalIP off: " + LocalIP[0] + "." + LocalIP[1] + "." + LocalIP[2] + "." + LocalIP[3]);
                }
                else if (_client._player != null)
                    erro = 0x80000000;
                else
                {
                    p = AccountManager.GetAccountDB(pId, 2, 0);
                    Daily = PlayerManager.GetPlayerDailyRecord(pId);
                    if (Daily == null)
                    {
                        PlayerManager.CreatePlayerDailyRecord(pId);
                    }
                    if (p != null && p.login == login && p._status.serverId == 0 && p.access >= 0 && erro == 0)
                    {
                        _client.player_id = p.player_id;
                        p._connection = _client;
                        p.GetAccountInfos(29);
                        p.LocalIP = LocalIP;
                        p.LoadInventory();
                        p.LoadMissionList();
                        p.LoadPlayerBonus();
                        EnableQuestMission(p);
                        p._inventory.LoadBasicItems();
                        p.SetPublicIP(_client.GetAddress());
                        p.Session = new PlayerSession
                        {
                            _sessionId = _client.SessionId,
                            _playerId = _client.player_id
                        };
                        DefautDropWeapon(p);
                        p.UpdateCacheInfo();
                        p._status.updateServer((byte)Settings.serverId);
                        _client._player = p;
                        p._pccafes = PlayerManager.PCCafe(pId);
                        p.EventChat = PlayerManager.PlayerChatEVENTS(pId);
                        p.ModelEventChat = PlayerManager.PremiacaoChatEVENTS();
                        p.dados = NextModel.Pais(_client.GetAddress());
                        ComDiv.UpdateDB("accounts", "lastip", p.PublicIP.ToString(), "player_id", p.player_id);
                        SendDebug.SendInfo
                            (
                               "o jogador saiu da autenticação, entrando no jogo [SessionID:" + p.GetSessionId() + "" + " PlayerID: " + pId + "]"
                            );
                        GameManager.RemoveBanned(_client);
                    }
                    else
                    {
                        erro = 0x80000000;
                    }
                }
                _client.SendPacket(new BASE_USER_ENTER_PAK(erro));
                if (erro > 0)
                    _client.Close(0, false);
            }
            catch (Exception ex)
            {
                Logger.Info("BASE_USER_ENTER_REC: " + ex.ToString());
                _client.Close(0, false);
            }
        }
        private void DefautDropWeapon(Account player)
        {
         //   CupomFlag DropWeapon = CupomEffectManagerJSON.GetCupomEffect(1200017000);
         //   if (DropWeapon != null && !player.effects.HasFlag(DropWeapon.EffectFlag))
            {
           //      player.effects |= DropWeapon.EffectFlag;
            }
        }
        private void EnableQuestMission(Account player)
        {
            PlayerEvent ev = player._event;
            if (ev == null)
                return;
            if (ev.LastQuestFinish == 0 && EventQuestSyncer.GetRunningEvent() != null)
                player._mission.mission4 = 13;
        }
    }
}