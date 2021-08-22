
using Core.server;

namespace Core.models.account.players
{
    public class PlayerMissions
    {
        public byte[] list1 = new byte[40], list2 = new byte[40], list3 = new byte[40], list4 = new byte[40];
        public int actualMission, card1, card2, card3, card4, mission1, mission2, mission3, mission4;
        public bool selectedCard;
        public PlayerMissions DeepCopy() => (PlayerMissions)MemberwiseClone();
        /// <summary>
        /// Retorna a progressão do baralho atual.
        /// </summary>
        /// <returns></returns>
        public byte[] GetCurrentMissionList()
        {
            switch (actualMission)
            {
                case 0: return list1;
                case 1: return list2;
                case 2: return list3;
                default: return list4;
            }
        }
        /// <summary>
        /// Retorna o número do cartão selecionado, do baralho atual.
        /// </summary>
        /// <returns></returns>
        public int GetCurrentCard() => GetCard(actualMission);
        public int GetCard(int index)
        {
            switch (index)
            {
                case 0: return card1;
                case 1: return card2;
                case 2: return card3;
                default: return card4;
            }
        }
        public int GetCurrentMissionId()
        {
            switch (actualMission)
            {
                case 0: return mission1;
                case 1: return mission2;
                case 2: return mission3;
                default: return mission4;
            }
        }
        public void UpdateSelectedCard()
        {
            if (ushort.MaxValue == ComDiv.getCardFlags(GetCurrentMissionId(), GetCurrentCard(), GetCurrentMissionList()))
                selectedCard = true;
        }
    }
}