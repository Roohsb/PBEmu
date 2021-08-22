using Core.managers;
using Core.models.enums.Clan;

namespace Core.models.account.clan
{
    public class Clan
    {
        public int _id, creationDate, partidas, vitorias, derrotas, autoridade, limite_rank, limite_idade, limite_idade2, _exp, _rank, _name_color, maxPlayers = 50;
        public string _name = "", _info = "", _news = "";
        public long owner_id;
        public uint _logo = 4294967295;
        public float _pontos = 1000;
        public ClanBestPlayers BestPlayers = new ClanBestPlayers();
        /// <summary>
        /// Gera o tipo de unidade do clã através da quantia de jogadores adquiridas pela Database.
        /// </summary>
        /// <returns></returns>
        public int GetClanUnit() => GetClanUnit(PlayerManager.GetClanPlayers(_id));
        /// <summary>
        /// Gerar o tipo de unidade do clã através da quantia de jogadores fornecida pelo aplicativo.
        /// </summary>
        /// <param name="count">Quantia de jogadores no clã.</param>
        /// <returns></returns>
        public int GetClanUnit(int count)
        {
            switch(count)
            {
                case int corpo when(corpo >= 250): return (int) Division.Corpo;
                case int divisao when (divisao >= 200): return (int)Division.Divisão;
                case int Brigada when (Brigada >= 150): return (int)Division.Brigada;
                case int Regimento when (Regimento >= 100): return (int)Division.Regimento;
                case int Batalhão when (Batalhão >= 50): return (int)Division.Batalhão;
                case int Companhia when (Companhia >= 30): return (int)Division.Companhia;
                case int Pelotão when (Pelotão >= 10): return (int)Division.Pelotão;
                default: return 0;
            }
        }
    }
}