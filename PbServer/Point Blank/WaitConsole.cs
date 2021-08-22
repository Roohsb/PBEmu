using Game.data.managers;
using Game.Progress;
using System;
using System.Threading.Tasks;

namespace Game
{
    public static class WaitConsole
    {
        public static async void UpdateRAM()
        {

            do
            {
                try
                {
                    int Sockets_G = GameManager._socketList.Count;
                    int Captured_A = LoginManager._lIstClient.Count;
                    int Captured_G = GameManager._lIstClient.Count;
                    string texto = "Socket[G]:'" + Sockets_G + "' (BLOCK) [Auth]: '" + Captured_A + "' | [Game]: '" + Captured_G + "' - [" + (GC.GetTotalMemory(true) / 1024) + " KB]  Status: '" + Listcache.Salas + "' Rooms, & '" + Listcache.pvps + "' pvp.";
                    Console.Title = texto;
                    await Task.Delay(1000);
                }
                catch
                {
                    Console.Title = "Contate o Desenvolvedor!";
                    break;
                }
            }
            while (true);
        }
    }
}