using Core.server;

namespace Game.global.Authentication
{
    public class A_LOGIN_QUEUE_PAK : SendPacket
    {
        private int queue_pos, estimated_time;
        public A_LOGIN_QUEUE_PAK(int queue_pos, int estimated_time)
        {
            this.queue_pos = queue_pos;
            this.estimated_time = estimated_time;
        }

        public override void Write()
        {
            WriteH(2676);
            WriteD(queue_pos); //Posição na fila
            WriteD(estimated_time); //Tempo estimado para entrar (Segundos)
        }
    }
}