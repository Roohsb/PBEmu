using Core;
using Game.data.model;
using Game.data.utils;

namespace Game.data.chat
{
    public class End
    {
        public static string FinalizarPartida(Room room)
        {
            if (room != null)
            {
                if (room.IsPreparing())
                {
                    AllUtils.EndBattle(room);
                    return Translation.GetLabel("EndRoomSuccess");
                }
                else
                    return Translation.GetLabel("EndRoomFail1");
            }
            else
                return Translation.GetLabel("GeneralRoomInvalid");

        }
    }
}
