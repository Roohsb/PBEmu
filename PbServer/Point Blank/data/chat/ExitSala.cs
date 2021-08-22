using Core;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.data.chat
{
    public class ExitSala 
    {
        public static string Messagem = "Você foi kikado por usar (armas, personagem, itens) indevidamente.";
        public static string ExitLobby(Room room, string playernick)
        {
            try
            {
                if (room != null && room.IsPreparing())
                {
                    Account pR = AccountManager.GetAccount(playernick, 1, 0);
                    if (pR.access > 0)
                        return "Você não pode Kikar GM!";
                    if (pR != null && pR._isOnline)
                    {
                        room.RemovePlayer(pR, true, 0);
                        using (SERVER_MESSAGE_ANNOUNCE_PAK packet1 = new SERVER_MESSAGE_ANNOUNCE_PAK(Messagem))
                            pR.SendPacket(packet1);

                        return "O Jogador " + pR.player_name + " foi kikado!";
                    }
                    else
                        return "O Jogador que você quer kikar não existe, ou está offline!";
                }
                else
                    return "você precisa está em uma sala, ou a sala já em andamento!";
            }
            catch(Exception EX)
            {
                SendDebug.SendInfo(EX.ToString());
                return "";
            }
        }
    }
}
