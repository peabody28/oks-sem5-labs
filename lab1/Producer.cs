using System.IO.Ports;
using System.Text;

namespace lab1
{
    public class Producer : Node
    {
        public Producer(string serialPortName, Parity parity = Parity.None) : base(serialPortName, parity)
        {
        }

        public override void Do()
        {
            base.Do();
            while(true)
            {
                var data = Console.ReadLine();
                
                var bytes = Encoding.ASCII.GetBytes(data);

                var valueBytes = bytes.Append((byte)0).ToArray();

                serialPort.Write(valueBytes, 0, valueBytes.Length);

                Console.WriteLine($"{valueBytes.Length} bytes sended");
            }
        }
    }
}
