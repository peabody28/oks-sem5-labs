using lab1.Builders;
using lab1.Data;
using lab1.Extensions;
using System.IO.Ports;

namespace lab1
{
    public class Producer : Node
    {
        private readonly TokenRingPackageBuilder tokenRingPackageBuilder;

        public readonly bool isMonitor;

        public Producer(string serialPortName, NodeRoot nodeRoot, bool isMonitor, Parity parity = Parity.None) : base(serialPortName, nodeRoot, parity)
        {
            this.isMonitor = isMonitor;
            tokenRingPackageBuilder = new TokenRingPackageBuilder();
        }

        public override void Do()
        {
            base.Do();

            Task.Run(() => ProcessQueue());

            if (isMonitor)
                Task.Run(() => Monitoring());

            while(true)
            {
                Console.Write("input destination: ");
                var destination = Convert.ToInt32(Console.ReadLine());
                var data = Console.ReadLine();

                var packages = tokenRingPackageBuilder.Build(destination, data, serialPort.GetPortNumber(), null, null);

                foreach(var package in packages)
                {
                    nodeRoot.sendQueue.Enqueue(package);
                }   
            }
        }

        private void ProcessQueue()
        {
            while(true)
            {
                if(nodeRoot.sendQueue.Any())
                {
                    var package = nodeRoot.sendQueue.Dequeue() as TokenRingPackage;

                    //SimulateError(package);

                    var packageBytes = package.ToByteArray();

                    serialPort.Write(packageBytes, 0, packageBytes.Length);

                    if(!package.accessControl.isToken)
                        Console.WriteLine($"{packageBytes.Length} bytes sended");
                }
            }
        }

        private void Monitoring()
        {
            while (true)
            {
                var accessControl = new AccessControl();
                accessControl.isMonitor = isMonitor;
                accessControl.isToken = true;

                var tokenPackage = tokenRingPackageBuilder.Build(0, string.Empty, serialPortNumber, accessControl, null).First();

                nodeRoot.sendQueue.Enqueue(tokenPackage);

                Thread.Sleep(1000);
            }
        }

        private static void SimulateError(Package package)
        {
            var randomByteIndex = new Random().Next(0, package.data.Length - 1);
            var randomBitIndex = new Random().Next(0, 7);

            var randomByte = package.data[randomByteIndex];
            package.data[randomByteIndex] |= (byte)(1 << randomBitIndex);

            if((randomByte ^ package.data[randomByteIndex]) != 0)
                Console.WriteLine($"{randomByteIndex} data byte, {randomBitIndex} bit setted to 1");
        }
    }
}
