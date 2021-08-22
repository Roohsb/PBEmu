using System;

namespace Core.models.account.players
{
   public class PlayerVip
    {
        public uint data_inicio;
        public uint data_fim;
        public static uint DateAtual() => uint.Parse(DateTime.Now.ToString("yyyyMMdd"));
    }
}
