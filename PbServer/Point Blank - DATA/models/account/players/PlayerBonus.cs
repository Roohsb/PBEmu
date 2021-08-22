namespace Core.models.account.players
{
    public class PlayerBonus
    {
        public int bonuses, sightColor = 4, freepass, fakeRank = 55;
        public string fakeNick = "";
        public long ownerId;
        public bool RemoveBonuses(int itemId)
        {
            int Dbonuses = bonuses, Dfreepass = freepass;
            switch (itemId)
            {
                case 1200001000: Decrease(1); break; //Exp 10%
                case 1200002000: Decrease(2); break; //Exp 30%
                case 1200003000: Decrease(4); break; //Exp 50%
                case 1200037000: Decrease(8); break; //Exp 100%
                case 1200004000: Decrease(32); break;//Gold 30%
                case 1200119000: Decrease(64); break;//Gold 50%
                case 1200038000: Decrease(128); break; //Gold 100%
                case 200011000: freepass = 0; break;
            }
            return (bonuses != Dbonuses || freepass != Dfreepass);
        }
        public bool AddBonuses(int itemId)
        {
            int Dbonuses = bonuses, Dfreepass = freepass;
            switch (itemId)
            {
                case 1200001000: Increase(1); break; //Exp 10%
                case 1200002000: Increase(2); break; //Exp 30%
                case 1200003000: Increase(4); break; //Exp 50%
                case 1200037000: Increase(8); break; //Exp 100%
                case 1200004000: Increase(32); break;//Gold 30%
                case 1200119000: Increase(64); break;//Gold 50%
                case 1200038000: Increase(128); break; //Gold 100%
                case 200011000: freepass = 1; break;
            }
            return (bonuses != Dbonuses || freepass != Dfreepass);          
        }
        private void Decrease(int value)
        {
            bonuses &= ~value;
        }
        private void Increase(int value)
        {
            bonuses |= value;
        }
    }
}