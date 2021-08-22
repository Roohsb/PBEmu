using Core;
using Game.data.model;

namespace Game.data.chat
{
    public class ShopBlackFriday
    {
        public static string MercadoNegro()
        {
            if (!Settings.BlackFriday)
            {
                Settings.BlackFriday = true;
                foreach (System.Collections.Generic.KeyValuePair<uint, GameClient> client in GameManager._socketList)
                {
                    GameClient game = client.Value;
                    if (game != null && game._player != null && game.player_id > 0)
                    {
                        RefillShop.InstantRefill(game._player, true);
                    }
                }
                return "[Sistema] o sistema black friday foi ativado. ";
            }
            else if (Settings.BlackFriday)
            {
                Settings.BlackFriday = false;
                foreach (System.Collections.Generic.KeyValuePair<uint, GameClient> client in GameManager._socketList)
                {
                    GameClient game = client.Value;
                    if (game != null && game._player != null && game.player_id > 0)
                    {
                        RefillShop.InstantRefill(game._player, false);
                    }
                }
                return "[Sistema] o sistema black friday foi desativado. ";
            }
            return "BlackFriday null";
        }
    }
}
