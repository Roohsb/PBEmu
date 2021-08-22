using Core.server;

namespace Game.global.serverpacket
{
    public class AUTH_ACCOUNT_KICK_PAK : SendPacket
    {
        private int _type;
        public AUTH_ACCOUNT_KICK_PAK(int type)
        {
            _type = type;
        }

        public override void Write()
        {
            WriteH(513);
            WriteC((byte)_type); //0 (O jogo será finalizado por solicitação do servidor) || 1 (Conexão simultanêa) || 2 (Jogo finalizado em instantes [GM]) || 3
        }
    }
}