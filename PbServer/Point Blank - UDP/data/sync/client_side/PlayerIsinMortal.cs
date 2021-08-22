using Battle.data.models;
using Battle.network;

namespace Battle.data.sync.client_side
{
    public class PlayerIsHPOriMortal
    {
        public static void LoadingRespawn(ReceivePacket p)
        {
            uint UniqueRoomId = p.readUD();
            int gen2 = p.readD();
            byte ativar = p.readC();
            int slotId = p.readC();
            Room room = RoomsManager.GetRoom(UniqueRoomId, gen2);
            Player player = room.GetPlayer(slotId, false);
            if (player == null && room == null)
                return;
            switch (ativar)
            {
                case 0:
                    player.Immortal = false;
                    Logger.Warning($"Player desativou o modo HP Inifinito. {player.isNick}");
                    break;
                case 1:
                    player.Immortal = true;
                    player._life = int.MaxValue;
                    player.ResetLife();
                    Logger.Warning($"Player Ativou o modo HP Inifinito. {player.isNick}");
                    break;
            }
        }
        public static void LoadingHPRespwan(ReceivePacket p)
        {
            uint UniqueRoomId = p.readUD();
            int gen2 = p.readD();
            int HP = p.readD();
            int slotId = p.readC();
            Room room = RoomsManager.GetRoom(UniqueRoomId, gen2);
            Player player = room.GetPlayer(slotId, false);
            if (player == null && room == null && HP < 100)
                return;
            player._LifeAdut = true;
            player._maxLife = HP;
            player.ResetLife();
            Logger.Warning("HP higher than the limit were published HP:[" + player._maxLife + "] / [" + player._life + "]");
        }
    }
}
