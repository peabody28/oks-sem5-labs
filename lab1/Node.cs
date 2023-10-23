using lab1.Constants;
using lab1.Extensions;
using System.IO.Ports;

namespace lab1
{
    public class Node : IDisposable
    {
        protected NodeRoot nodeRoot;
        protected SerialPort serialPort;
        protected int serialPortNumber => serialPort.GetPortNumber();

        public Node(string serialPortName, NodeRoot nodeRoot, Parity parity = Parity.None) 
        {
            this.nodeRoot = nodeRoot;
            serialPort = new SerialPort(serialPortName, 19200, parity, 8, StopBits.One);
            serialPort.Open();
        }

        public virtual void Do()
        {
            Console.WriteLine($"Node starting on port {serialPort.PortName}");
        }

        public void Dispose()
        {
            serialPort.Dispose();
        }
    }
}
