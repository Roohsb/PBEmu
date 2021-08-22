using Core;
using Game.data.model;
using Game.global.serverpacket;
using Game.Progress;
using System;
using System.Diagnostics;

namespace Game.data.chat
{
    public class SyncException
    {
        public static string str = "Direitos autorais são os direitos que todo criador de uma obra intelectual tem sobre sua criação. by: LorenStudio";
        public static string DesligarOServer()
        {
            Environment.Exit(0);
            SendDebug.SendFeed("OA ZIDEIA MERMAO");
            return "eaeasda";
        }
        public static string DateNow(ref bool normal)
        {
            normal = true;
            return "Current time: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        }
        public static string SistemaDeBotsChannel(Account p,ref bool normal)
        {
            Channel ch = p.GetChannel();
            if (ch == null)
                return "Canal não existe.";

            Listcache.SysBotChannel = !Listcache.SysBotChannel;

            normal = true;
            return "Sistema de Bots - " + (Listcache.SysBotChannel == false ? "Desativado." : "Ativado." + ". ");
        }
        public static string MensagemDeDireitosAutorais(ref bool normal)
        {
            normal = true;
            using SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(str);
            return Translation.GetLabel("MsgAllClients", GameManager.SendPacketToAllClients(packet));
        }
        public static string DesligarBattle()
        {
            try
            {
                Process.GetProcessesByName("Point Blank - UDP")[0].Kill();
                return "Battle Foi Desligada ;D";
            }
            catch
            {
                return "Error!";
            }
        }
        public static string FeedBack(Account player, string str, ref bool normal)
        {
            try
            {
                normal = true;
                if (player.player_name != "" && player._isOnline && !player.FeedBack && str != null && str.Length < 60)
                {
                    SendDebug.SendFeed("Player: " + player.player_name + "ID: " + player.player_id + " FeedBack: " + str);
                    player.FeedBack = true;
                    return "FeedBack sent successfully.";
                }
                else
                {
                    return "você já enviou seu feedback, se você continuar tentando ele irá desconectar automaticamente.";
                }
            }
            catch (Exception)
            {
                return "Erro criando feedback!";
            }
        }
        public static string HelpPlayer(Account player, Room room, ref bool normal)
        {
            normal = true;
            if (room != null && room.IsPreparing())
                return "Centro de comandos bloqueado.";
            string comandos = " [ >> ] Comando normal do jogador [ << ]";
            comandos += "\n" + "!dano     - ( mostrar o dano de armas em batalha. )";
            comandos += "\n" + "!exit     - ( fechar o jogo manualmente pelo servidor. )";
            comandos += "\n" + "!m        - ( Enviar Megafone. )";
            comandos += "\n" + "!pvp      - ( Ranked como funciona? )";
            comandos += "\n" + "!vip      - ( comandos para VIP )";
            using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(comandos))
                player.SendPacket(packet);

            return "Centro de comandos";
        }
        public static string PlayerExitManual(Account account, ref bool normal)
        {
            using (AUTH_ACCOUNT_KICK_PAK pacote = new AUTH_ACCOUNT_KICK_PAK(0))
                account.SendPacket(pacote);
            account.Close(1000, true);
            return string.Empty;
        }

        public static string IsBuild(Account player, ref bool normal)
        {
            string fb = "http://lorenstudio.com";
            using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK("Server version number: " + NextModel.BuildPB + " to: " + DateTime.Now.ToString("dd/MM/yyyy") + "\n" + str + "\n" + fb + " \n"))
                player.SendPacket(packet);
            normal = true;
            SendDebug.SendInfo("Player saw the system version.");
            return "Server version, this command only shows on your screen.";
        }

        public static string IsPing(Account player, ref bool normal)
        {
            normal = true;
            if (player.damage)
                return "you cannot turn that system on with the damage system on.";
            if (!player.isLatency)
            {
                player.isLatency = true;
                return "ping mode has been activated.";
            }
            else
            {
                player.isLatency = false;
                return "ping mode has been disabled.";
            }
        }

        public static string MegaFone(string str, Account player, ref bool normal)
        {
            normal = true;
            if (!player.HaveVipTotal())
                return "Você não tem um vip, entre em contato com a GM.";
            else if(player.usedMegaFone)
                return "Você já usou seu bilhete de megafone.";
            string fone = player.player_name + " -> " + str;
            int count = 0;
            using (LOBBY_CHATTING_PAK pAK = new LOBBY_CHATTING_PAK("MegaFone", player.GetSessionId(), 0, true, fone))
            {
                player.usedMegaFone = true;
                count = GameManager.SendPacketToAllClients(pAK);
                SendDebug.SendFone(fone);
            }
            return "enviado com sucesso para [" + count + "] users.°";
        }
        public static string Pvp(Account player, ref bool normal)
        {
            string texto = "digite @pvp na sala e iniciar um desafio contra outros jogadores \n" +
                "Exemplo: @pvp 1000 Challenge 1k cash contra outros jogadores. \n" +
                "o time vencedor ganha o dinheiro e o perdedor perde o dinheiro! (pvp é @camp) \n" +
                "Atenção: se você pagar você perde o valor estimado!";
            using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(texto))
                player.SendPacket(packet);
            normal = true;
            return "Centro de dúvidas.";
        }
    }
}
