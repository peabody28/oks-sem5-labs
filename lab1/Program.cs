using System.IO.Ports;

namespace lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ParitySelectMessage();
            var parityName = Console.ReadLine();
            var parity = (Parity)Enum.Parse(typeof(Parity), parityName);

            var ports = GetFreeSerialPorts();

            Console.WriteLine("Is monitor? (1/0)");
            var isMonitor = Convert.ToInt32(Console.ReadLine());

            var node = new Node(ports.Item1, ports.Item2, isMonitor == 1, parity);

            node.Produce();
        }

        private static void ParitySelectMessage()
        {
            var parities = Enum.GetNames<Parity>();
            Console.WriteLine("Select parity form:");
            foreach (var parity in parities)
            {
                Console.WriteLine( " - " + parity);
            }
        }

        private static (string, string) GetFreeSerialPorts()
        {
            var serialPortNames = SerialPort.GetPortNames();

            int i = 0;
            for (; i < serialPortNames.Length; i += 2)
            {
                SerialPort serialPort = null;
                try
                {
                    serialPort = new SerialPort(serialPortNames[i], 19200, Parity.None, 8, StopBits.One);
                    serialPort.Open();
                    break;
                }
                catch
                {

                }
                finally
                {
                    serialPort.Dispose();
                }
            }

            int j = i - 1;
            if (j < 0)
                j += 6;

            return (serialPortNames[i], serialPortNames[j]);
        }
    }
}