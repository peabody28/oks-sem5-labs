using lab1.Builders;
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
            var buffer = new byte[11];
            
            serialPort.Read(buffer, 0, 11);

            var package = PackageBuilder.Parse(buffer);

            var data = Encoding.ASCII.GetString(package.data);

            Console.WriteLine(data);
        }
    }
}
