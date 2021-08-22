using Core.server;

namespace Game.global.Authentication
{
    public class BASE_EXIT_URL_PAK : SendPacket
    {
        private string link;
        public BASE_EXIT_URL_PAK(string link)
        {
            this.link = link;
        }

        public override void Write()
        {
            WriteH(2694);
            WriteC(1);
            WriteD(1);
            WriteD((int)Settings.LocalDoGame);
            WriteS(link, 256);
        }
    }
}