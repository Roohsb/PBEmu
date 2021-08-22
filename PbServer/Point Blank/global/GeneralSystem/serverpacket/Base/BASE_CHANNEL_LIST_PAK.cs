using Core.server;
using Game.data.model;
using Game.data.xml;

namespace Game.global.serverpacket
{
    public class BASE_CHANNEL_LIST_PAK : SendPacket
    {
        public BASE_CHANNEL_LIST_PAK()
        {
        }

        public override void Write()
        {
            WriteH(2572);
            WriteD(ChannelsXML._channels.Count);
            WriteD(Settings.maxChannelPlayers);
            for (int i = 0; i < ChannelsXML._channels.Count; i++)
                WriteD(ChannelsXML._channels[i]._players.Count);
        }
    }
}