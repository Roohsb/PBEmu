using Game.data.model;

namespace Game.data.chat
{
    public class Damage
    {
        public static string SyncDAMAGE(Account player, Room room, ref bool normal)
        {
            try
            {
                normal = true;
                if (player.isLatency)
                    return "you cannot connect this system with the ping system turned on.";
                else if (room != null && !room.IsBotMode())
                {
                    player.damage = !player.damage;
                    SendDebug.SendInfo(player.player_name + "the player activated the damage system.");
                    return "the damage visualization was '" + string.Concat(player.damage == true ? "Enabled" : "Disabled") + "' successfully.";
                }
                else
                    return "You need to be playing in a room. BOT (N), NORMAL (S) MODE.";
            }
            catch
            {
                return "Error Grave!";
            }
        }
        public static string SyncContry(Account player, ref bool normal) { normal = true; return player.dados; }
        public static string SyncLocation(Account player, ref bool normal)
        {
            normal = true;
            if (player.isLatency || player.damage)
                return "you cannot turn this system on with the ping / damage system on.";
            player.location = !player.location;
            return "location view was '" + string.Concat(player.location == true ? "Enabled" : "Disabled") + "' successfully.";
        }
    }
}
