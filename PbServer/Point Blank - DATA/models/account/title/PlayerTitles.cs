
namespace Core.models.account.title
{
    public class PlayerTitles
    {
        public int Equiped1, Equiped2, Equiped3, Slots = 1;
        public long ownerId, Flags;
        /// <summary>
        /// Adiciona novo título a lista de flags.
        /// </summary>
        /// <param name="Flag">Valor da flag</param>
        /// <returns></returns>
        public long Add(long flag)
        {
            Flags |= flag; 
            return Flags;
        }
        public bool Contains(long flag) => (Flags & flag) == flag || flag == 0;

        public void SetEquip(int index, int value)
        {
            switch (index)
            {
                case 0: Equiped1 = value; break;
                case 1: Equiped2 = value; break;
                case 2: Equiped3 = value; break;
            }               
        }
        public int GetEquip(int index)
        {
            switch (index)
            {

                case 0: return Equiped1;
                case 1: return Equiped2;
                case 2: return Equiped3;
                default: return 0;
            }
        }
    }
}