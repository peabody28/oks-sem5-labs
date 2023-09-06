using System.Diagnostics;
using System.IO.Ports;

namespace lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Node node = null;

            if (args.Length == 0)
            {
                ParitySelectMessage();

                var parityName = Console.ReadLine();
                var parity = (Parity)Enum.Parse(typeof(Parity), parityName);

                var ports = GetFreeSerialPorts();

                node = new Producer(ports.Item1, parity);

                StartConsumer(ports.Item2);
            }
            else
                node = new Consumer(args[0]);

            node.Do();
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

            int i = 1;
            for(; i < serialPortNames.Length; i++)
            {
                var portName = serialPortNames[i];
                try
                {
                    using (var node = new Node(portName))
                    {
                        break;
                    }
                }
                catch
                {

                }
            }

            return (serialPortNames[i], serialPortNames[i + 1]);
        }

        private static void StartConsumer(string portName)
        {
            using (var p = new Process())
            {
                p.StartInfo.FileName = $"E:\\unik\\sem5\\oks\\Labs\\lab1\\bin\\Debug\\net7.0\\lab1.exe";
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.Arguments = portName;
                p.Start();
            }
        }
    }
}