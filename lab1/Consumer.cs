using System.IO.Ports;
using System.Text;

namespace lab1
{
    public class Consumer : Node
    {
        public Consumer(string serialPortName) : base(serialPortName)
        {
            serialPort.DataReceived += new SerialDataReceivedEventHandler(OutputData);
        }

        public override void Do()
        {
            base.Do();
            while(true)
                ;
        }

        private void OutputData(object sender, SerialDataReceivedEventArgs e)
        {
            var buffer = new byte[1024];
            serialPort.Read(buffer, 0, 1024);

            var valueBuffer = buffer.TakeWhile(b => b != 0).ToArray();

            var data = Encoding.ASCII.GetString(valueBuffer);

            Console.Write(data);
        }
    }
}
