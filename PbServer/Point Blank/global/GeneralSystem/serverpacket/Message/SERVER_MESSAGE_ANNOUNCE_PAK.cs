using Core;
using Core.models.enums;
using Core.server;

namespace Game.global.serverpacket
{
    public class SERVER_MESSAGE_ANNOUNCE_PAK : SendPacket
    {
        private string _message;
        private int type;
        /// <summary>
        /// Envia uma mensagem global ao jogador. (GM)
        /// </summary>
        /// <param name="msg"></param>
        public SERVER_MESSAGE_ANNOUNCE_PAK(string msg)
        {
            _message = msg;
            type = 2;
            if (msg.Length >= 1024)
                Logger.Error("[GM] Mensagem com tamanho maior a 1024 enviada!!");
        }
        public SERVER_MESSAGE_ANNOUNCE_PAK (string msg, int type)
        {
            this.type = type;
            if (msg.Length >= 1024)
                Logger.Error("[GM] Mensagem com tamanho maior a 1024 enviada!!");
        }
        public override void Write()
        {
            WriteH(2055);
            WriteD(type); //Tipo da notícia [NOTICE_TYPE_NORMAL - 1 || NOTICE_TYPE_EMERGENCY - 2]
            WriteH((ushort)_message.Length);
            WriteS(_message);
        }
    }
}