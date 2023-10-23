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

            NodeRoot nodeRoot = new NodeRoot();
            Node producer = new Producer(ports.Item1, nodeRoot, isMonitor == 1, parity);
            Node consumer = new Consumer(ports.Item2, nodeRoot);

            Task.Run(() => consumer.Do());
            Thread.Sleep(200);
            producer.Do();
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
                try
                {
                    using (var node = new Node(serialPortNames[i], null))
                    {
                        break;
                    }
                }
                catch
                {

                }
            }

            int j = i - 1;
            if (j < 0)
                j += 6;

            return (serialPortNames[i], serialPortNames[j]);
        }
    }
}