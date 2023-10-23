using lab1.Data;

namespace lab1
{
    public class NodeRoot
    {
        public Queue<Package> sendQueue;

        public NodeRoot()
        {
            sendQueue = new Queue<Package>();
        }
    }
}
