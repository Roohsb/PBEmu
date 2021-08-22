using Core.models.room;
using Game.data.model;
using Game.data.sync;

namespace Game.data.chat
{
   public class LifeInfinity
    {
        public static string Enable(Account pl, Room room, SLOT slot)
        {
            string IsTexto = "Error";

            pl._lifeInfinity = !pl._lifeInfinity;


            if (slot != null)
            {
                Game_SyncNet.SendHPMortalIndividual(room, slot, pl._lifeInfinity);
                IsTexto = pl._lifeInfinity == false ? "Infinite life has been disabled." : "Infinite life has been activated.";
            }
            return IsTexto;
        }
        public static string HP(Account Player, Room room, SLOT slot, string txt)
        {
            try
            {
                return "Sistema desativado";
                //int HP = int.Parse(txt);
                //if (slot != null && HP >= 100 && HP <= 999)
                //{
                //    Game_SyncNet.SendHPIndividual(room, slot, HP);
                //    return "HP Adicionado com sucesso [" + string.Concat(HP) + "].";
                //}
                //else
                //    return "Erro ao Adicionar o HP [" + string.Concat(HP) + "].";
            }
            catch
            {
                return "Error Gravissimo. (tente novamente)";
            }
        }
    }
}
