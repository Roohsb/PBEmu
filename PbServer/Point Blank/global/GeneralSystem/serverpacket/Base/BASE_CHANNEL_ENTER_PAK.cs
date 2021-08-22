using Core.server;

namespace Game.global.serverpacket
{
    public class BASE_CHANNEL_ENTER_PAK : SendPacket
    {
        private uint _channelId;
        private string _announce;
        public BASE_CHANNEL_ENTER_PAK(int id, string announce)
        {
            _channelId = (uint)id;
            _announce = announce;
        }
        public BASE_CHANNEL_ENTER_PAK(uint erro)
        {
            _channelId = erro;
        }

        public override void Write()
        {
            WriteH(2574);
            WriteD(_channelId);
            if (!string.IsNullOrEmpty(_announce))
            {
                WriteH((ushort)_announce.Length);
                WriteS(_announce);
            }
        }
    }
}