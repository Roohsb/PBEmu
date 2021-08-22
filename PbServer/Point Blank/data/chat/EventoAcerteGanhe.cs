using Core;
using Game.global.serverpacket;
using System;

namespace Game.data.chat
{
    public class EventoAcerteGanhe
    {
  
        public static int total = 0;
        public static string SetToQuiz(string str)
        {
            try
            {
                
                switch (str.ToLower())
                {
                    case "t":
                        {
                            GameManager.EventsReloadsTrue();
                            string contas = Contas();
                            using SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK($"[ >> ] This is a mathematical game [ << ] \n" +
                                $"How much is it {contas} ?");
                            return $"[Quiz system] Quiz Ativado, for {GameManager.SendPacketToAllClients(packet)} online players. answer: [" + total + "]";
                        }
                    case "f":
                        {
                            GameManager.EventsReloadsFalse();
                            return "[Quiz system] Quiz disabled.";
                        }
                    default:
                        return "[Quiz system] T to activate, F to disable.";
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return "[[Quiz system] Error found";
            }
        }
        public static string Contas()
        {
            Random r = new Random();
            string str = "";
            lock(new object())
            {
                switch (r.Next(1, 3))
                {
                    case 1:
                        {
                            int numero1 = r.Next(500);
                            int numero2 = r.Next(500);
                            total = (numero1 + numero2);
                            str = $"{numero1} + {numero2}     'sum'";
                            break;
                        }
                    case 2:
                        {
                            int numero1 = r.Next(50);
                            int numero2 = r.Next(50);
                            total = (numero1 * numero2);
                            str = $"{numero1} x {numero2}    'multiplication";
                            break;
                        }
                    case 3:
                        {
                            int numero1 = r.Next(50);
                            int numero2 = r.Next(50);
                            total = (numero1 / numero2);
                            str = $"{numero1} / {numero2}      'Division'";
                            break;
                        }
                }
            }
            return str;
        }
    }
}
